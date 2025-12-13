using BunnyBroker.Workers;
using Microsoft.AspNetCore.SignalR;

namespace BunnyBroker;

public class BunnyHub(BunnySenderQueue senderQueue, BunnyRegisterQueue registerQueue, BunnyLogQueue logQueue, BunnyProcessor bunnyProcessor, ILogger<BunnyHub> logger) : Hub<IBunnyObserver> {
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
		bunnyProcessor.Remove(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
	}

	public async Task SendBunnyMessageAsync(Contracts.BunnyMessage bunny) {
		logger.LogInformation($"Bunny sent: {bunny.Id}");
		await senderQueue.SendAsync(bunny);
    }

	public async Task StartObserving(string bunnyType, string uniqueHandlerName) {
		logger.LogInformation($"Observing: {uniqueHandlerName} - {bunnyType}");
        await registerQueue.SendAsync(new Entities.BunnyTypeRegistry {
			BunnyHandlerType = uniqueHandlerName,
			BunnyType = bunnyType
        });
		bunnyProcessor.Add(bunnyType, Context.ConnectionId, uniqueHandlerName);
        await Groups.AddToGroupAsync(Context.ConnectionId, bunnyType);
	}

	public async Task BunnyProcessed(Guid bunnyId, string uniqueHandlerName, bool sucess) {
		logger.LogInformation($"Bunny processed: {uniqueHandlerName} - {bunnyId} = {sucess}");
        await logQueue.SendAsync(new Entities.BunnyLog {
			BunnyHandlerType = uniqueHandlerName,
			BunnyMessageId = bunnyId,
			ProcessedAt = DateTime.UtcNow,
			Sucess = true,
			ErrorMessage = null
		});
    }

	public async Task BunnyFailed(Guid bunnyId, string uniqueHandlerName, Exception ex) {
		logger.LogInformation($"Bunny failed: {uniqueHandlerName} - {bunnyId} = {ex.Message}");
        await logQueue.SendAsync(new Entities.BunnyLog
		{
			BunnyHandlerType = uniqueHandlerName,
			BunnyMessageId = bunnyId,
			ProcessedAt = DateTime.UtcNow,
			Sucess = false,
			ErrorMessage = ex.Message
        });
    }
}
