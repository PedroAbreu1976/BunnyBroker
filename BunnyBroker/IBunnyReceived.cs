using BunnyBroker.Contracts;


namespace BunnyBroker
{
    public interface IBunnyReceived
    {
        Task OnBunnyReceivedAsync(BunnyMessage bunny);
    }
}
