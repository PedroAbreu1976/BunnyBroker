using BunnySlinger.Broker.Contracts;


namespace BunnySlinger.Broker
{
    public interface IBunnyReceived
    {
        Task OnBunnyReceivedAsync(BunnyMessage bunny);
    }
}
