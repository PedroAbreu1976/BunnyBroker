namespace BunnySlinger.Broker;

public static class BunnyBrokerExtensions {
    public static IServiceCollection AddBunnyBroker(this IServiceCollection services) {
        services.AddSingleton<IBunnyBrokerRepository, InMemoryBunnyBrokerRepository>();
        services.AddSingleton<BunnyInMemoryQueue>();
        services.AddSignalR();
        services.AddHostedService<Worker>();

        return services;
    }

    public static IEndpointRouteBuilder StartBunnyBroker(this IEndpointRouteBuilder app) {
	    app.MapHub<BunnyHub>("bunny-hub");

	    return app;
    }
}

