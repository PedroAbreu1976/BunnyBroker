using BunnyBroker.Contracts;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;

namespace BunnyBroker.Client;

public class BunnyChannel : IAsyncDisposable {
	public event Func<Exception?, Task>? StatusChanged;

    private readonly HubConnection _hubConnection;
	private readonly Dictionary<Type, List<(string Name, Func<object, Task<bool>> Handler)>> _dicHandlers = new();

    public BunnyChannel(BunnyBrokerOptions? options = null) {
	    if (options is null) {
			options = new BunnyBrokerOptions();
        }
		_hubConnection = new HubConnectionBuilder()
			.WithUrl(
				$"{options.Url.TrimEnd('/')}/bunny-hub", 
                HttpTransportType.WebSockets, 
				(o)=> {
					o.Credentials = new System.Net.NetworkCredential(options.User, options.Password);
					o.UseDefaultCredentials = false;
                    if (options != null) {
						o.AccessTokenProvider = options.ConnectionOptions.AccessTokenProvider;
						o.Headers = options.ConnectionOptions.Headers ?? o.Headers;
						o.ApplicationMaxBufferSize = options.ConnectionOptions.ApplicationMaxBufferSize;
						o.TransportMaxBufferSize = options.ConnectionOptions.TransportMaxBufferSize;
						o.ClientCertificates = options.ConnectionOptions.ClientCertificates;
						o.CloseTimeout = options.ConnectionOptions.CloseTimeout;
						o.Cookies = options.ConnectionOptions.Cookies;
						//o.Credentials = options.ConnectionOptions.Credentials;
						o.DefaultTransferFormat = options.ConnectionOptions.DefaultTransferFormat;
						o.HttpMessageHandlerFactory = options.ConnectionOptions.HttpMessageHandlerFactory;
						o.Proxy = options.ConnectionOptions.Proxy;
						o.SkipNegotiation = options.ConnectionOptions.SkipNegotiation;
						o.Transports = options.ConnectionOptions.Transports;
						//o.Url = options.ConnectionOptions.Url;
						o.WebSocketFactory = options.ConnectionOptions.WebSocketFactory;
						o.WebSocketConfiguration = options.ConnectionOptions.WebSocketConfiguration;
						//o.UseDefaultCredentials = options.ConnectionOptions.UseDefaultCredentials;
					}
				})
			.WithAutomaticReconnect()
			.WithStatefulReconnect()
			.Build();


		_hubConnection.On<BunnyMessage>("OnBunnyReceivedAsync", async (bunny) => {
			var content = bunny.ToBunny(_dicHandlers.Keys);
            if (_dicHandlers.TryGetValue(content!.GetType(), out var handlers)) {
				foreach (var handler in handlers) {
					try {
						var result = await handler.Handler(content!);
						await BunnyProcessed(bunny.Id, handler.Name, result);
					}catch(Exception ex) {
						await BunnyFailed(bunny.Id, handler.Name, ex);
                    }
                }
			}
		});

        _hubConnection.Closed += HubConnection_StatusChanged;
        _hubConnection.Reconnecting += HubConnection_StatusChanged;
		_hubConnection.Reconnected += _ => HubConnection_StatusChanged(null);
    }

    public BunnyChannelState State => (BunnyChannelState)_hubConnection.State;
    public string? ConnectionId => _hubConnection.ConnectionId;

    private async Task HubConnection_StatusChanged(Exception? ex)
    {
	    if (StatusChanged != null) {
		    await StatusChanged.Invoke(ex);
	    }
    }

	public Task StartAsync(CancellationToken ct = default) => _hubConnection.StartAsync(ct);

	public Task StopAsync(CancellationToken ct = default) => _hubConnection.StopAsync(ct);

	public Task AddObserver<TBunny>(string uniqueHandlerName, Func<TBunny, Task<bool>> onBunnyReceived, CancellationToken ct = default) {
		
		var type = typeof(TBunny);
        if (!_dicHandlers.TryGetValue(type, out var items))
		{
			items = new();
			_dicHandlers.Add(type, items);
        }
		items.Add((uniqueHandlerName, obj => onBunnyReceived((TBunny)obj)));

		return _hubConnection.SendAsync("StartObserving", typeof(TBunny).FullName, uniqueHandlerName, ct); 
    }

    public async Task SendBunnyAsync(object bunny, Guid? id = null, CancellationToken ct = default) {
	    var message = new BunnyMessage
	    {
		    Id = id ?? Guid.NewGuid(),
		    Body = System.Text.Json.JsonSerializer.Serialize(bunny),
		    CreatedAt = DateTime.UtcNow,
		    BunnyType = bunny.GetType().FullName!,
	    };
	    await WaitForConnection(ct);
        await _hubConnection.SendAsync("SendBunnyMessageAsync", message, ct);
    }

    private async Task BunnyProcessed(Guid bunnyId, string uniqueHandlerName, bool success, CancellationToken ct = default) {
	    await WaitForConnection(ct);
	    await _hubConnection.SendAsync("BunnyProcessed", bunnyId, uniqueHandlerName, success, ct);
    }

    private async Task BunnyFailed(Guid bunnyId, string uniqueHandlerName, Exception ex, CancellationToken ct = default)
    {
	    await WaitForConnection(ct);
	    await _hubConnection.SendAsync("BunnyFailed", bunnyId, uniqueHandlerName, ex, ct);
    }

    private async Task WaitForConnection(CancellationToken ct = default) {
	    if (State != BunnyChannelState.Connected)
	    {
		    int waitTime = 100;
		    while (State != BunnyChannelState.Connected)
		    {
			    if (ct.IsCancellationRequested)
			    {
				    throw new OperationCanceledException(
					    "The operation was cancelled before the BunnyChannel could send the message.", ct);
			    }

			    await Task.Delay(waitTime, ct);
			    waitTime += 100;
		    }
	    }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or
    /// resetting unmanaged resources asynchronously.</summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync() {
		await _hubConnection.DisposeAsync();
		StatusChanged = null;
		_dicHandlers.Clear();
    }
}