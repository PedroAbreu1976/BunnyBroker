using BunnySlinger.Broker.Contracts;

using Microsoft.AspNetCore.SignalR;

namespace BunnySlinger.Broker;

public class BunnyHub(BunnyInMemoryQueue queue) : Hub<IBunnyReceived> {
	/// <summary>
	/// Called when a new connection is established with the hub.
	/// </summary>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous connect.</returns>
	public override Task OnConnectedAsync() {
		return base.OnConnectedAsync();
	}

	/// <summary>Called when a connection with the hub is terminated.</summary>
	/// <returns>A <see cref="T:System.Threading.Tasks.Task" /> that represents the asynchronous disconnect.</returns>
	public override Task OnDisconnectedAsync(Exception? exception) {
		return base.OnDisconnectedAsync(exception);
	}

	public async Task SendBunnyMessageAsync(BunnyMessage bunny, CancellationToken ct = default) {
		await queue.SendAsync(bunny, ct);
    }
}
