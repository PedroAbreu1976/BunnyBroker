using System.ComponentModel.DataAnnotations;

namespace BunnyBroker.Contracts;

public class BunnyTypeRegistryItem
{
	public string BunnyType { get; set; }

	[Key]
	public string BunnyHandlerType { get; set; }
}

