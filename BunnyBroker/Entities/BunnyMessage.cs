using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;


namespace BunnyBroker.Entities;

[Index(nameof(CreatedAt), AllDescending = true, IsUnique = false)]
public class BunnyMessage {
	[Key]
	public required Guid Id { get; set; }

	public required string Body { get; set; }
	public required DateTime CreatedAt { get; set; }
	public DateTime SavedAt { get; set; }
    public required string BunnyType { get; set; }

	public List<BunnyLog> BunnyLogs { get; set; }
}
