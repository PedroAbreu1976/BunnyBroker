using BunnyBroker.Contracts;


namespace BunnyBroker
{
    public interface IBunnyObserver
    {
        Task OnBunnyReceivedAsync(BunnyMessageProcessRequest bunny);
    }
}
