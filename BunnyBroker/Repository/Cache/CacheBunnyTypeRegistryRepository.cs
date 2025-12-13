using BunnyBroker.Entities;

using Microsoft.Extensions.Caching.Hybrid;


namespace BunnyBroker.Repository.Cache
{
    public class CacheBunnyTypeRegistryRepository(IBunnyTypeRegistryRepository repository, HybridCache cache) : IBunnyTypeRegistryRepository
    {
	    public async Task<IEnumerable<BunnyTypeRegistry>> GetAllAsync(CancellationToken ct = default) {
		    return await cache.GetOrCreateAsync<IEnumerable<BunnyTypeRegistry>>(
				"BunnyTypeRegistryRepository_GetAll",
				async _ => await repository.GetAllAsync(ct),
				cancellationToken: ct
			);
        }
	    public async Task<BunnyTypeRegistry> GetByHandlerTypeAsync(string handlerType, CancellationToken ct = default) {
		    var items = await GetAllAsync(ct);
			return items.FirstOrDefault(x => x.BunnyHandlerType == handlerType) ?? throw new KeyNotFoundException($"BunnyTypeRegistry with HandlerType {handlerType} not found."); ;
        }
	    public async Task<IEnumerable<BunnyTypeRegistry>> GetByBunnyTypeAsync(string bunnyType, CancellationToken ct = default) {
		    var items = await GetAllAsync(ct);
			return items.Where(x => x.BunnyType == bunnyType);
        }
	    public async Task AddAsync(BunnyTypeRegistry typeRegistry, CancellationToken ct = default) {
		    await repository.AddAsync(typeRegistry, ct);
			await cache.RemoveAsync("BunnyTypeRegistryRepository_GetAll", ct);
        }
    }
}
