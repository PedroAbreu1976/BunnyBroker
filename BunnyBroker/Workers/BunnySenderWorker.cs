using BunnyBroker.Extensions;
using BunnyBroker.Repository;
using Microsoft.AspNetCore.SignalR;

namespace BunnyBroker.Workers;

public class BunnySenderWorker(BunnySenderQueue queue, IServiceScopeFactory scopeFactory, IHubContext<BunnyHub, IBunnyObserver> hubContext, BunnyProcessor bunnyProcessor, ILogger<BunnySenderWorker> logger) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken ct) {
		while (await queue.WaitForBunnyAsync(ct))
		{
			var bunny = await queue.CatchAsync(ct);
			using (var scope = scopeFactory.CreateScope())
			{
				var repository = scope.ServiceProvider.GetRequiredService<IBunnyMessageRepository>();
				await repository.AddAsync(bunny.MapToEntity());
			}
			//await hubContext.Clients.Groups(bunny.BunnyType).OnBunnyReceivedAsync(bunny);#
			await bunnyProcessor.Process(bunny);
        }
    }
}

