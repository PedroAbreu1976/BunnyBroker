using System.Threading.Channels;

namespace BunnyBroker.Workers
{
    public abstract class WorkerChannel<T>(IServiceScopeFactory scopeFactory, int capacity = 1000) : BackgroundService
    {
	    private readonly Channel<T> _queue =
		    Channel.CreateBounded<T>(new BoundedChannelOptions(capacity)
		    {
			    FullMode = BoundedChannelFullMode.Wait,
			    SingleReader = false,
			    SingleWriter = false,
			    AllowSynchronousContinuations = false
		    });

	    private ChannelReader<T> Reader => _queue.Reader;
	    public ChannelWriter<T> Writer => _queue.Writer;

        protected sealed override async Task ExecuteAsync(CancellationToken ct)
	    {
		    while (await WaitAsync(ct))
		    {
			    var message = await CatchAsync(ct);
			    using var scope = scopeFactory.CreateScope();
			    await HandleMessageAsync(message, scope.ServiceProvider, ct);
		    }
        }

		protected abstract Task HandleMessageAsync(T message, IServiceProvider services,  CancellationToken ct = default);

        private ValueTask<bool> WaitAsync(CancellationToken ct = default) => _queue.Reader.WaitToReadAsync(ct);

	    private ValueTask SendAsync(T message, CancellationToken ct = default) => _queue.Writer.WriteAsync(message, ct);

	    private ValueTask<T> CatchAsync(CancellationToken ct = default) => _queue.Reader.ReadAsync(ct);
    }
}
