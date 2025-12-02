using Microsoft.AspNetCore.SignalR;
using System.Threading.Channels;

using BunnyBroker.Contracts;


namespace BunnyBroker;

public class BunnyInMemoryQueue(IHubContext<BunnyHub, IBunnyReceived> context, IBunnyBrokerRepository repository)
{
	private readonly Channel<BunnyMessage> _queue =
		Channel.CreateBounded<BunnyMessage>(new BoundedChannelOptions(100)
		{
			FullMode = BoundedChannelFullMode.Wait,
			SingleReader = false,
			SingleWriter = false,
			AllowSynchronousContinuations = false
		});

	public ChannelReader<BunnyMessage> Reader => _queue.Reader;
	public ChannelWriter<BunnyMessage> Writer => _queue.Writer;

	public ValueTask<bool> WaitForBunnyAsync(CancellationToken ct = default) {
		return _queue.Reader.WaitToReadAsync(ct);
	}

	public async ValueTask SendAsync(BunnyMessage message, CancellationToken ct = default) {
		await _queue.Writer.WriteAsync(message, ct);
		await repository.AddAsync(message, ct);
	}

	public ValueTask<BunnyMessage> CatchAsync(CancellationToken ct = default) {
		return _queue.Reader.ReadAsync(ct);
    }

	public async Task<bool> OnBunnyDispatchedAsync(BunnyMessage bunny, CancellationToken ct = default) {
		await context.Clients.All.OnBunnyReceivedAsync(bunny);
		await repository.SetProcessedAsync(bunny, ct);
        return true;
	}
}

