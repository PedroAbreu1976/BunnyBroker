using BunnySlinger.Broker.Contracts;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Http.Connections.Client;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;

namespace BunnySlinger.Broker.Client;

public class BunnyChannel : IAsyncDisposable {
	public event Func<Exception?, Task>? StatusChanged;
	public event Func<BunnyMessage, CancellationToken, Task>? BunnyReceived;

    private readonly HubConnection _hubConnection;

    public BunnyChannel(IOptions<BunnyBrokerOptions> options) {
		_hubConnection = new HubConnectionBuilder()
			.WithUrl(
				options.Value.Url, 
				HttpTransportType.WebSockets, 
				(o)=> {
					o.AccessTokenProvider = options.Value.ConnectionOptions.AccessTokenProvider;
					o.Headers = options.Value.ConnectionOptions.Headers ?? o.Headers;
					o.ApplicationMaxBufferSize = options.Value.ConnectionOptions.ApplicationMaxBufferSize;
					o.TransportMaxBufferSize = options.Value.ConnectionOptions.TransportMaxBufferSize;
					o.ClientCertificates = options.Value.ConnectionOptions.ClientCertificates;
					o.CloseTimeout = options.Value.ConnectionOptions.CloseTimeout;
					o.Cookies = options.Value.ConnectionOptions.Cookies;
					o.Credentials = options.Value.ConnectionOptions.Credentials;
					o.DefaultTransferFormat = options.Value.ConnectionOptions.DefaultTransferFormat;
					o.HttpMessageHandlerFactory = options.Value.ConnectionOptions.HttpMessageHandlerFactory;
					o.Proxy = options.Value.ConnectionOptions.Proxy;
					o.SkipNegotiation = options.Value.ConnectionOptions.SkipNegotiation;
					o.Transports = options.Value.ConnectionOptions.Transports;
					o.Url = options.Value.ConnectionOptions.Url;
					o.WebSocketFactory = options.Value.ConnectionOptions.WebSocketFactory;
					o.WebSocketConfiguration = options.Value.ConnectionOptions.WebSocketConfiguration;
					o.UseDefaultCredentials = options.Value.ConnectionOptions.UseDefaultCredentials;
                })
			.WithAutomaticReconnect()
			.WithStatefulReconnect()
			.Build();

		HttpConnectionOptions a = new HttpConnectionOptions {
		};

        _hubConnection.On<BunnyMessage, CancellationToken>("OnBunnyReceivedAsync", async (bunny, ct) =>
		{
			if (BunnyReceived != null) {
				await BunnyReceived.Invoke(bunny, ct);
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

    public Task SendBunnyAsync(BunnyMessage bunny, CancellationToken ct = default) {
		if (State != BunnyChannelState.Connected) {
			int waitTime = 100;
			while (State != BunnyChannelState.Connected) {
				if (ct.IsCancellationRequested) {
					throw new OperationCanceledException("The operation was cancelled before the BunnyChannel could send the message.", ct);
                }
				Task.Delay(waitTime, ct);
				waitTime += 100;
            }
		}

		return _hubConnection.SendAsync("SendBunnyMessageAsync", bunny, ct);
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