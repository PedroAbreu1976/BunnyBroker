using Microsoft.AspNetCore.SignalR;
using System.Threading.Channels;
using BunnyBroker.Entities;

namespace BunnyBroker.Workers;

public class BunnyLogQueue(IHubContext<BunnyHub, IBunnyReceived> context)
{
	private readonly Channel<BunnyLog> _queue =
		Channel.CreateBounded<BunnyLog>(new BoundedChannelOptions(1000)
		{
			FullMode = BoundedChannelFullMode.Wait,
			SingleReader = false,
			SingleWriter = false,
			AllowSynchronousContinuations = false
		});

	public ChannelReader<BunnyLog> Reader => _queue.Reader;
	public ChannelWriter<BunnyLog> Writer => _queue.Writer;

	public ValueTask<bool> WaitAsync(CancellationToken ct = default) {
		return _queue.Reader.WaitToReadAsync(ct);
	}

	public async ValueTask SendAsync(BunnyLog message, CancellationToken ct = default) {
		await _queue.Writer.WriteAsync(message, ct);
	}

	public ValueTask<BunnyLog> CatchAsync(CancellationToken ct = default) {
		return _queue.Reader.ReadAsync(ct);
    }
}

