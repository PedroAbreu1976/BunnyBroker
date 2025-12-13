namespace BunnyBroker.Contracts
{
    public class BunnyMessageProcessRequest {

        public BunnyMessage Message { get; set; }
	    public IEnumerable<BunnyObserverInfo> ExcludedObservers { get; set; } =
		    Enumerable.Empty<BunnyObserverInfo>();
    }

    public class BunnyObserverInfo
    {
        public required string ConnectionId { get; set; }
        public required string HandlerName { get; set; }
    }
}
