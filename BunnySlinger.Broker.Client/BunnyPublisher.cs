using BunnySlinger.Broker.Contracts;


namespace BunnySlinger.Broker.Client;

internal class BunnyPublisher(BunnyChannel channel) {
    public Task PublishAsync(BunnyMessage bunnyMessage, CancellationToken cancellationToken = default) =>
        channel.SendBunnyAsync(bunnyMessage, cancellationToken);
}
