using Microsoft.AspNetCore.SignalR;
using System.Threading.Channels;

using BunnyBroker.Contracts;


namespace BunnyBroker.Workers;

public class BunnySenderQueue()
{
	private readonly Channel<BunnyMessage> _queue =
		Channel.CreateBounded<BunnyMessage>(new BoundedChannelOptions(1000)
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
	}

	public ValueTask<BunnyMessage> CatchAsync(CancellationToken ct = default) {
		return _queue.Reader.ReadAsync(ct);
    }
}

