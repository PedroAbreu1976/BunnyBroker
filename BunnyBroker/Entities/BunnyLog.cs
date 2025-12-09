using Microsoft.EntityFrameworkCore;

namespace BunnyBroker.Entities;

[PrimaryKey(nameof(BunnyMessageId), nameof(BunnyHandlerType))]
public class BunnyLog {
	public Guid BunnyMessageId { get; set; }
	public string BunnyHandlerType { get; set; }
	public DateTime? ProcessedAt { get; set; }
	public bool Sucess { get; set; }
    public string? ErrorMessage { get; set; }

	public BunnyMessage BunnyMessage { get; set; }
	public BunnyTypeRegistry BunnyTypeRegistry { get; set; }
}
