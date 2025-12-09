namespace BunnyBroker.Contracts;

public class BunnyLogItem {
	public Guid BunnyMessageId { get; set; }
    public string BunnyHandlerType { get; set; }
	public DateTime? ProcessedAt { get; set; }
	public bool Sucess { get; set; }
	public string? ErrorMessage { get; set; }
}
