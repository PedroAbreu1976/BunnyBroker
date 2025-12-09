using System.ComponentModel.DataAnnotations;

namespace BunnyBroker.Entities;

public class BunnyTypeRegistry
{
	public string BunnyType { get; set; }

	[Key]
	public string BunnyHandlerType { get; set; }

	public List<BunnyLog> BunnyLogs { get; set; }
}

