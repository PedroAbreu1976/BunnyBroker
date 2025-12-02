using BunnyBroker.Contracts;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;

namespace BunnyBroker.Client;

public class BunnyChannel : IAsyncDisposable {
	public event Func<Exception?, Task>? StatusChanged;
	public event Func<object, Guid, Task>? BunnyReceived;

    private readonly HubConnection _hubConnection;

    public BunnyChannel(BunnyBrokerOptions? options = null) {
	    if (options is null) {
			options = new BunnyBrokerOptions();
        }
		_hubConnection = new HubConnectionBuilder()
			.WithUrl(
				$"{options.Url.TrimEnd('/')}/bunny-hub", 
                HttpTransportType.WebSockets, 
				(o)=> {
					if (options != null) {
						o.AccessTokenProvider = options.ConnectionOptions.AccessTokenProvider;
						o.Headers = options.ConnectionOptions.Headers ?? o.Headers;
						o.ApplicationMaxBufferSize = options.ConnectionOptions.ApplicationMaxBufferSize;
						o.TransportMaxBufferSize = options.ConnectionOptions.TransportMaxBufferSize;
						o.ClientCertificates = options.ConnectionOptions.ClientCertificates;
						o.CloseTimeout = options.ConnectionOptions.CloseTimeout;
						o.Cookies = options.ConnectionOptions.Cookies;
						o.Credentials = options.ConnectionOptions.Credentials;
						o.DefaultTransferFormat = options.ConnectionOptions.DefaultTransferFormat;
						o.HttpMessageHandlerFactory = options.ConnectionOptions.HttpMessageHandlerFactory;
						o.Proxy = options.ConnectionOptions.Proxy;
						o.SkipNegotiation = options.ConnectionOptions.SkipNegotiation;
						o.Transports = options.ConnectionOptions.Transports;
						//o.Url = options.ConnectionOptions.Url;
						o.WebSocketFactory = options.ConnectionOptions.WebSocketFactory;
						o.WebSocketConfiguration = options.ConnectionOptions.WebSocketConfiguration;
						o.UseDefaultCredentials = options.ConnectionOptions.UseDefaultCredentials;
					}
				})
			.WithAutomaticReconnect()
			.WithStatefulReconnect()
			.Build();

		HttpConnectionOptions a = new HttpConnectionOptions {
		};

        _hubConnection.On<BunnyMessage>("OnBunnyReceivedAsync", async (bunny) =>
		{
			if (BunnyReceived != null) {
                await BunnyReceived.Invoke(bunny.ToBunny()!, bunny.Id);
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

	public Task StartAsync() => _hubConnection.StartAsync();

	public Task StopAsync() => _hubConnection.StopAsync();

    private Task SendBunnyAsync(BunnyMessage bunny, CancellationToken ct = default) {
	    if (State != BunnyChannelState.Connected) {
		    int waitTime = 100;
		    while (State != BunnyChannelState.Connected) {
			    if (ct.IsCancellationRequested) {
				    throw new OperationCanceledException(
					    "The operation was cancelled before the BunnyChannel could send the message.", ct);
			    }

			    Task.Delay(waitTime, ct);
			    waitTime += 100;
		    }
	    }

	    return _hubConnection.SendAsync("SendBunnyMessageAsync", bunny, ct);
    }

    public Task SendBunnyAsync(object bunny, Guid? id = null, CancellationToken ct = default) {
	    return SendBunnyAsync(
		    new BunnyMessage {
			    Id = id ?? Guid.NewGuid(),
			    Body = System.Text.Json.JsonSerializer.Serialize(bunny),
			    CreatedAt = DateTime.UtcNow,
			    BunnyType = bunny.GetType().FullName!,
			    IsProcessed = false
		    }, ct);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or
    /// resetting unmanaged resources asynchronously.</summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync() {
		await _hubConnection.DisposeAsync();
		StatusChanged = null;
		BunnyReceived = null;

    }
}