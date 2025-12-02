using System.Text.Json;
using BunnyBroker.Client;

await Task.Delay(2000);

await using var channel = new BunnyChannel(new BunnyBrokerOptions{ Url = "https://localhost:7110" });
channel.StatusChanged += async (ex) =>
{
    if (ex != null)
    {
        Console.WriteLine($"Connection lost: {ex.Message}");
    }
    else
    {
        Console.WriteLine("Connection reestablished.");
    }
    await Task.CompletedTask;
};

channel.BunnyReceived += async (bunny, id) =>
{
    Console.WriteLine($"[{id}]: {JsonSerializer.Serialize(bunny)}");
    await Task.CompletedTask;
};



await channel.StartAsync();

Console.WriteLine("Enter messages to send (empty line to quit):");

var message = Console.ReadLine();
while (!string.IsNullOrWhiteSpace(message)) {
	await channel.SendBunnyAsync(message);
	message = Console.ReadLine();
}
