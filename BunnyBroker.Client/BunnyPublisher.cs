using BunnyBroker.Contracts;


namespace BunnyBroker.Client;

internal class BunnyPublisher(BunnyChannel channel) {
    public Task PublishAsync(BunnyMessage bunnyMessage, CancellationToken cancellationToken = default) =>
        channel.SendBunnyAsync(bunnyMessage, cancellationToken);
}
