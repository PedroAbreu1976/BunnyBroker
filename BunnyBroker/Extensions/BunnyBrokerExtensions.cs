using System.Collections.Concurrent;
using BunnyBroker.Repository;
using BunnyBroker.Workers;


namespace BunnyBroker.Extensions;

public static class BunnyBrokerExtensions {
    public static IServiceCollection AddBunnyBroker(this IServiceCollection services) {
	    services.AddScoped<IBunnyLogRepository, BunnyLogRepository>();
        services.AddScoped<IBunnyTypeRegistryRepository, BunnyTypeRegistryRepository>();
        services.AddScoped<IBunnyMessageRepository, BunnyMessageRepository>();
        services.AddSingleton<BunnySenderQueue>();
        services.AddSingleton<BunnyRegisterQueue>();
        services.AddSingleton<BunnyLogQueue>();
        services.AddSignalR();
        services.AddHostedService<BunnySenderWorker>();
        services.AddHostedService<BunnyRegisterWorker>();
        services.AddHostedService<BunnyLogWorker>();

        return services;
    }

    public static IEndpointRouteBuilder StartBunnyBroker(this IEndpointRouteBuilder app) {
	    app.MapHub<BunnyHub>("bunny-hub");
	    app.AddEndpoints();

        return app;
    }

    internal static TValue AddOrUpdate<TKey, TValue>(
	    this ConcurrentDictionary<TKey, TValue> dic, TKey key, TValue value) {
	    return dic.AddOrUpdate(key, x => value, (x, y) => value);
    }
}

