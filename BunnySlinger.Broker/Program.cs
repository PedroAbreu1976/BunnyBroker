//using BunnySlinger.Broker;
//using BunnySlinger.Broker.Contracts;


//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddSingleton<BunnyInMemoryQueue>();
//builder.Services.AddHostedService<Worker>();
//builder.Services.AddSignalR();
//builder.Services.AddSingleton<IBunnyBrokerRepository, InMemoryBunnyBrokerRepository>();

//var app = builder.Build();

//app.MapHub<BunnyHub>("bunny-hub");

//app.MapGet("/", async (IBunnyBrokerRepository repository, CancellationToken ct) =>
//{
//    var result = await repository.GetAllAsync(ct);
//    return Results.Ok(result);
//}).Produces<IEnumerable<BunnyMessage>>();

//app.Run();
