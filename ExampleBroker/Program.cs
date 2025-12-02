using BunnyBroker;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddBunnyBroker();

var app = builder.Build();

app.StartBunnyBroker();


app.Run();
