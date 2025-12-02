using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace BunnySlinger.Broker;

public class Worker(BunnyInMemoryQueue queue, ILogger<Worker> logger) : BackgroundService
{
	protected override async Task ExecuteAsync(CancellationToken ct) {
		while (await queue.WaitForBunnyAsync(ct))
		{
			var bunny = await queue.CatchAsync(ct);
			await queue.OnBunnyDispatchedAsync(bunny, ct);
		}
    }
}

