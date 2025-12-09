using System.Text.Json;
using BunnyBroker.Contracts;

namespace BunnyBroker.Client;

public static class BunnyMessageExtensions {
	public static TBunny ToBunny<TBunny>(this BunnyMessage message) {
		var result = message.ToBunny();
		if (result is not null && result is not TBunny) {
			throw new InvalidCastException(
				$"Cannot cast BunnyMessage of type {message.BunnyType} to {typeof(TBunny).FullName}");
		}

		return (TBunny)result!;
	}

	public static object? ToBunny(this BunnyMessage message) {
		return JsonSerializer.Deserialize(message.Body, Type.GetType(message.BunnyType)!);
	}

	internal static object? ToBunny(this BunnyMessage message, IEnumerable<Type> types) {
		var type = types.First(x => x.FullName == message.BunnyType);
        return JsonSerializer.Deserialize(message.Body, type);
	}
}
