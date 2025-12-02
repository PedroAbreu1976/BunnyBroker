using BunnySlinger.Broker;
using BunnySlinger.Broker.Contracts;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<BunnyInMemoryQueue>();
builder.Services.AddHostedService<Worker>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<IBunnyBrokerRepository, InMemoryBunnyBrokerRepository>();

var app = builder.Build();

app.MapHub<BunnyHub>("bunny-hub");
// Configure the HTTP request pipeline.
//app.MapPost("/", async (BunnyMessage message, BunnyInMemoryQueue queue, CancellationToken ct) =>
//{
//	await queue.SendAsync(message, ct);
//	return Results.NoContent();
//});

app.MapGet("/", async (IBunnyBrokerRepository repository, CancellationToken ct) => {
    var result = await repository.GetAllAsync(ct);
    return Results.Ok(result); 
}).Produces<IEnumerable<BunnyMessage>>();

app.Run();
