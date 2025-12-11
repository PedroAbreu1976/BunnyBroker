using BunnyBroker.Extensions;
using BunnyBroker.Repository;

namespace BunnyBroker.Workers;

public class BunnySenderWorker(BunnySenderQueue queue, IServiceScopeFactory scopeFactory, ILogger<BunnySenderWorker> logger) : BackgroundService
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

            await queue.OnBunnyDispatchedAsync(bunny, ct);
            
        }
    }
}

