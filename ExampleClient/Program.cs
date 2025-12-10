using System.Text.Json;
using BunnyBroker.Client;

await Task.Delay(2000);

await using var channel = new BunnyChannel(new BunnyBrokerOptions { Url = "https://localhost:7110", User = "admin", Password = "admin"});
await channel.StartAsync();

await channel.AddObserver<MessageLenght>("1", msg =>
{
    Console.WriteLine($"Lenght: {JsonSerializer.Serialize(msg)}");
    return Task.FromResult(true);
});
await channel.AddObserver<MessageContent>("2", msg =>
{
    Console.WriteLine($"Content: {JsonSerializer.Serialize(msg)}");
    return Task.FromResult(true);
});

await channel.AddObserver<MessageContent>("3", msg => {
	Console.WriteLine($"Message: {msg.Content}");
	return Task.FromResult(true);
});

var message = Console.ReadLine();
while (!string.IsNullOrWhiteSpace(message))
{
    await channel.SendBunnyAsync(new MessageContent {Content = message});
    await channel.SendBunnyAsync(new MessageLenght { Length = message.Length });
    message = Console.ReadLine();
}

public class MessageLenght {
    public int Length { get; set; }
}

public class MessageContent {
    public required string Content { get; set; }
}
