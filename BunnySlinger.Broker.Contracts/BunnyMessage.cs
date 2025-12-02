namespace BunnySlinger.Broker.Contracts
{
    public class BunnyMessage
    {
        public required Guid Id { get; set; }
        public required string Body { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required string BunnyType { get; set; }
        public bool IsProcessed { get; set; } = false;
    }
}
