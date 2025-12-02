using BunnySlinger.Broker.Client;
using BunnySlinger.Broker.Contracts;


await Task.Delay(2000);

var channel = new BunnyChannel(new BunnyBrokerOptions{ Url = "https://localhost:7110/bunny-hub" });
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

channel.BunnyReceived += async (bunny, ct) =>
{
    Console.WriteLine($"Received Bunny: {bunny.Body}");
    await Task.CompletedTask;
};



await channel.StartAsync();

await channel.SendBunnyAsync(new BunnyMessage {
	Id = Guid.NewGuid(),
	Body = "TEST BUNNY MESSAGE",
	BunnyType = typeof(string).FullName,
    CreatedAt = DateTime.UtcNow
});

Console.WriteLine("Press any key to exit...");
Console.ReadKey();
