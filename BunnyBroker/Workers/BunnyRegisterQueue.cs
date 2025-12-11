using Microsoft.AspNetCore.SignalR;
using System.Threading.Channels;
using BunnyBroker.Entities;

namespace BunnyBroker.Workers;

public class BunnyRegisterQueue(IHubContext<BunnyHub, IBunnyObserver> context)
{
	private readonly Channel<BunnyTypeRegistry> _queue =
		Channel.CreateBounded<BunnyTypeRegistry>(new BoundedChannelOptions(1000)
		{
			FullMode = BoundedChannelFullMode.Wait,
			SingleReader = false,
			SingleWriter = false,
			AllowSynchronousContinuations = false
		});

	public ChannelReader<BunnyTypeRegistry> Reader => _queue.Reader;
	public ChannelWriter<BunnyTypeRegistry> Writer => _queue.Writer;

	public ValueTask<bool> WaitAsync(CancellationToken ct = default) {
		return _queue.Reader.WaitToReadAsync(ct);
	}

	public async ValueTask SendAsync(BunnyTypeRegistry message, CancellationToken ct = default) {
		await _queue.Writer.WriteAsync(message, ct);
	}

	public ValueTask<BunnyTypeRegistry> CatchAsync(CancellationToken ct = default) {
		return _queue.Reader.ReadAsync(ct);
    }
}

