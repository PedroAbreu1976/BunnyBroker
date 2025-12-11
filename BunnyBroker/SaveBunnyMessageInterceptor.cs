using BunnyBroker.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BunnyBroker;

public class SaveBunnyMessageInterceptor : SaveChangesInterceptor<BunnyDbContext> {


	public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(BunnyDbContext context, InterceptionResult<int> result, CancellationToken cancellationToken = default) {
		var entries = context.ChangeTracker
			.Entries<BunnyMessage>()
			.Where(x => x.State == EntityState.Added);
		if (entries.Any())
		{
			var bunnyTypes = entries.Select(x => x.Entity.BunnyType);
			var bunnyTypeRegistries = await context.BunnyTypeRegistries
				.Where(x => bunnyTypes.Contains(x.BunnyType))
				.AsNoTracking()
				.ToListAsync(cancellationToken);
			foreach (var entry in entries)
			{
				entry.Entity.SavedAt = DateTime.UtcNow;
			}
		}
		return result; 
    }
}
