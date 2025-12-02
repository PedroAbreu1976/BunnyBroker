using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace BunnyBroker.Contracts;

public class BunnyTypeRegistry
{
	public string BunnyType { get; set; }

	[Key]
	public string BunnyHandlerType { get; set; }
}

