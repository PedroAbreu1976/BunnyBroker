namespace BunnyBroker.Contracts;

public class BunnyMessageInfo : BunnyMessage {
	public IEnumerable<LogInfo> Logs { get; set; }
}

public class LogInfo {
	public required string BunnyHandlerType { get; set; }
	public DateTime? ProcessedAt { get; set; }
	public required bool Sucess { get; set; }
	public string? ErrorMessage { get; set; }
}