namespace BunnyBroker.Contracts;

public class BunnyMessageItem {
	public required Guid Id { get; set; }
	public required DateTime CreatedAt { get; set; }
	public required string BunnyType { get; set; }
}
