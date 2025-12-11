using BunnyBroker.Contracts;


namespace BunnyBroker
{
    public interface IBunnyObserver
    {
        Task OnBunnyReceivedAsync(BunnyMessageItem bunny);
    }
}
