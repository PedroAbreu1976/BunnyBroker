using BunnyBroker;
using BunnyBroker.Contracts;
using BunnyBroker.Extensions;
using BunnyBroker.Repository;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

using Scalar.AspNetCore;


var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddBunnyBroker();
builder.Services.AddDbContext<BunnyDbContext>(options =>
{
	options.UseInMemoryDatabase("MyDatabase");
	options.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
});

var app = builder.Build();

app.StartBunnyBroker();

app.MapOpenApi();
app.MapScalarApiReference();


app.Run();
