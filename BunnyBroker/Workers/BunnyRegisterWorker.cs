using BunnyBroker.Extensions;
using BunnyBroker.Repository;

namespace BunnyBroker.Workers;

public class BunnyRegisterWorker(BunnyRegisterQueue queue, IServiceScopeFactory scopeFactory, ILogger<BunnyRegisterWorker> logger) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken ct) {
		while (await queue.WaitAsync(ct))
		{
			var entity = await queue.CatchAsync(ct);
			using (var scope = scopeFactory.CreateScope()) {
				var repository = scope.ServiceProvider.GetRequiredService<IBunnyTypeRegistryRepository>();
				await repository.AddAsync(entity);
            }
		}
    }
}

