using BunnyBroker.Contracts;

using Microsoft.AspNetCore.SignalR;

namespace BunnyBroker;

public class BunnyHub(BunnyInMemoryQueue queue, ILogger<BunnyHub> logger) : Hub<IBunnyReceived> {
	/// <summary>
	/// Called when a new connection is established with the hub.
	/// </summary>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous connect.</returns>
	public override Task OnConnectedAsync() {
		logger.LogInformation("Client connected: {ConnectionId}", Context.ConnectionId);
        return base.OnConnectedAsync();
	}

	/// <summary>Called when a connection with the hub is terminated.</summary>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous disconnect.</returns>
	public override Task OnDisconnectedAsync(Exception? exception) {
		logger.LogInformation("Client disconnected: {ConnectionId}", Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
	}

	public async Task SendBunnyMessageAsync(BunnyMessage bunny) {
		await queue.SendAsync(bunny);
    }
}
