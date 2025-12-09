using BunnyBroker.Contracts;
using BunnyBroker.Repository;

using Microsoft.AspNetCore.Mvc;


namespace BunnyBroker.Extensions;

internal static class ApiExtensions {
	public static IEndpointRouteBuilder AddEndpoints(this IEndpointRouteBuilder app) {

		app.MapGet(
		  "/bunnies", async ([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, IBunnyMessageRepository repository, CancellationToken ct = default) =>
		  {
			  var result = await repository.GetAllAsync(startDate, endDate, ct);
			  return Results.Ok(result.Select(x=>x.MapToContract()));
		  })
		 .WithName("GetBunnies")
		 .Produces<IEnumerable<BunnyMessageItem>>();

		app.MapGet(
				"/bunnies/{id:guid}", async (Guid id, IBunnyMessageRepository repository, CancellationToken ct = default) =>
				{
					var result = await repository.GetByIdAsync(id, ct);
					return Results.Ok(result.MapToContractStatus());
				})
			.WithName("GetBunny")
			.Produces<BunnyMessage>();

        app.MapGet("/registries", async (IBunnyTypeRegistryRepository repository, CancellationToken ct = default) =>
			{
				var result = await repository.GetAllAsync(ct);
				return Results.Ok(result.Select(x=>x.MapToContract()));
			})
			.WithName("GetBunnyHandlers")
			.Produces<IEnumerable<BunnyTypeRegistryItem>>();

        app.MapGet(
		        "/logs", async ([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, IBunnyLogRepository repository, CancellationToken ct = default) =>
		        {
			        var result = await repository.GetAllAsync(startDate, endDate, ct);
			        return Results.Ok(result.Select(x => x.MapToContract()));
		        })
	        .WithName("GetLogs")
	        .Produces<IEnumerable<BunnyLogItem>>();



        return app;
    }
}
