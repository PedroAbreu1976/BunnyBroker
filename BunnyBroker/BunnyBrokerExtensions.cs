using System.Collections.Concurrent;
using BunnyBroker.Contracts;

namespace BunnyBroker;

public static class BunnyBrokerExtensions {
    public static IServiceCollection AddBunnyBroker(this IServiceCollection services) {
        services.AddSingleton<IBunnyBrokerRepository, InMemoryBunnyBrokerRepository>();
        services.AddSingleton<IBunnyTypeRegistryRepository, InMemoryBunnyTypeRegistryRepository>();
        services.AddSingleton<BunnyInMemoryQueue>();
        services.AddSignalR();
        services.AddHostedService<Worker>();

        return services;
    }

    public static IEndpointRouteBuilder StartBunnyBroker(this IEndpointRouteBuilder app) {
	    app.MapHub<BunnyHub>("bunny-hub");

	    app.MapGet(
			    "/bunnies", async (IBunnyBrokerRepository repository, CancellationToken ct = default) => {
				    var result = await repository.GetAllAsync(ct);
				    return Results.Ok(result);
                })
		    .WithName("GetBunnies")
		    .Produces<BunnyMessage>();

        app.MapPost("/bunnies/registry", async (BunnyTypeRegistry handler, IBunnyTypeRegistryRepository repository, CancellationToken ct = default) => {
		        await repository.AddAsync(handler, ct);
                return Results.Created($"/bunnies/registry/{handler.BunnyHandlerType}", handler);
            })
            .WithName("AddBunnyHandler")
            .Accepts<BunnyTypeRegistry>("application/json")
            .Produces<BunnyTypeRegistry>(StatusCodes.Status201Created);

        app.MapGet("/bunnies/registry", async (IBunnyTypeRegistryRepository repository, CancellationToken ct = default) =>
	        {
				var result = await repository.GetAllAsync(ct);
		        return Results.Ok(result);

	        })
	        .WithName("GetBunnyHandlers")
	        .Produces<IEnumerable<BunnyTypeRegistry>>();

        app.MapGet("/bunnies/registry/{bunnyType}", async (string bunnyType, IBunnyTypeRegistryRepository repository, CancellationToken ct = default) =>
	        {
		        var result = await repository.GetByBunnyTypeAsync(bunnyType, ct);
		        return Results.Ok(result);

	        })
	        .WithName("GetBunnyHandlersByBunnyType")
	        .Produces<IEnumerable<BunnyTypeRegistry>>();

        app.MapDelete("/bunnies/registry/{bunnyHandlerType}", async (string bunnyHandlerType, IBunnyTypeRegistryRepository repository, CancellationToken ct = default) =>
            {
                var result = await repository.RemoveAsync(bunnyHandlerType, ct);
                return Results.Ok(result);
            })
            .WithName("RemoveBunnyHandler")
            .Produces<BunnyTypeRegistry>()
            .Produces(StatusCodes.Status404NotFound);

        return app;
    }

    internal static TValue AddOrUpdate<TKey, TValue>(
	    this ConcurrentDictionary<TKey, TValue> dic, TKey key, TValue value) {
	    return dic.AddOrUpdate(key, x => value, (x, y) => value);
    }
}

