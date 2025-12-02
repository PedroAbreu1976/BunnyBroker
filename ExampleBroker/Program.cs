using BunnyBroker;
using BunnyBroker.Contracts;
using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddBunnyBroker();

var app = builder.Build();

app.StartBunnyBroker();

app.MapOpenApi();
app.MapScalarApiReference();


app.Run();
