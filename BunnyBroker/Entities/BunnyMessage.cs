using System.ComponentModel.DataAnnotations;

namespace BunnyBroker.Entities;
public class BunnyMessage {
	[Key]
	public required Guid Id { get; set; }

	public required string Body { get; set; }
	public required DateTime CreatedAt { get; set; }
	public required string BunnyType { get; set; }

	public List<BunnyLog> BunnyLogs { get; set; }
}
