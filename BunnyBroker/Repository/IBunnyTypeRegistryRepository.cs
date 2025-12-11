using BunnyBroker.Entities;

using Microsoft.EntityFrameworkCore;


namespace BunnyBroker.Repository
{
    public interface IBunnyTypeRegistryRepository
    {
        Task<IEnumerable<BunnyTypeRegistry>> GetAllAsync(CancellationToken ct = default);
        Task<BunnyTypeRegistry> GetByHandlerTypeAsync(string handlerType, CancellationToken ct = default);
        Task<IEnumerable<BunnyTypeRegistry>> GetByBunnyTypeAsync(string bunnyType, CancellationToken ct = default);
        Task AddAsync(BunnyTypeRegistry typeRegistry, CancellationToken ct = default);
    }

    public class BunnyTypeRegistryRepository(BunnyDbContext context) : IBunnyTypeRegistryRepository {

	    public async Task<IEnumerable<BunnyTypeRegistry>> GetAllAsync(CancellationToken ct = default) {
		    return await context.BunnyTypeRegistries.ToListAsync(ct);
        }
	    public async Task<BunnyTypeRegistry> GetByHandlerTypeAsync(string handlerType, CancellationToken ct = default) {
		    return await context.BunnyTypeRegistries.FindAsync(handlerType, ct) ?? throw new KeyNotFoundException($"BunnyTypeRegistry with HandlerType {handlerType} not found.");
        }
	    public async Task<IEnumerable<BunnyTypeRegistry>> GetByBunnyTypeAsync(string bunnyType, CancellationToken ct = default) {
		    return await context.BunnyTypeRegistries
                .Where(tr => tr.BunnyType == bunnyType)
                .ToListAsync(ct);
        }
	    public async Task AddAsync(BunnyTypeRegistry typeRegistry, CancellationToken ct = default) {
		    var exists = await context.BunnyTypeRegistries
			    .Where(x => x.BunnyType == typeRegistry.BunnyType)
			    .Where(x => x.BunnyHandlerType == typeRegistry.BunnyHandlerType)
			    .AnyAsync(ct);

		    if (!exists) {
			    context.BunnyTypeRegistries.Add(typeRegistry);
			    await context.SaveChangesAsync(ct);
		    }
	    }
    }
}
