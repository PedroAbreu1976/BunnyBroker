using System.Collections.Concurrent;

using BunnyBroker.Contracts;


namespace BunnyBroker;

public interface IBunnyTypeRegistryRepository {
	Task<BunnyTypeRegistry> AddAsync(BunnyTypeRegistry item, CancellationToken ct = default);
	Task<BunnyTypeRegistry?> RemoveAsync(string bunnyHandlerType, CancellationToken ct = default);
	Task<IEnumerable<BunnyTypeRegistry>> RemoveAllAsync(string bunnyType, CancellationToken ct = default);
	Task<IEnumerable<BunnyTypeRegistry>> GetAllAsync(CancellationToken ct = default);
	Task<IEnumerable<BunnyTypeRegistry>> GetByBunnyTypeAsync(string bunnyType, CancellationToken ct = default);
}

public class InMemoryBunnyTypeRegistryRepository : IBunnyTypeRegistryRepository {
	private readonly ConcurrentDictionary<string, BunnyTypeRegistry> _cache = new();

    public Task<BunnyTypeRegistry> AddAsync(BunnyTypeRegistry item, CancellationToken ct = default) {
        _cache.AddOrUpdate(item.BunnyHandlerType, item);
        return Task.FromResult(item);
    }
	public Task<BunnyTypeRegistry?> RemoveAsync(string bunnyHandlerType, CancellationToken ct = default) {
		_cache.TryRemove(bunnyHandlerType, out var result);
		return Task.FromResult(result);
	}
	public Task<IEnumerable<BunnyTypeRegistry>> RemoveAllAsync(string bunnyType, CancellationToken ct = default) {
		var result = new List<BunnyTypeRegistry>();
		foreach (var item in _cache.Values.Where(x=>x.BunnyType == bunnyType)) {
			if (_cache.TryRemove(item.BunnyHandlerType, out var existingItem)) {
				result.Add(existingItem);
			}
		}

		return Task.FromResult(result.AsEnumerable());
	}
	public Task<IEnumerable<BunnyTypeRegistry>> GetAllAsync(CancellationToken ct = default) {
		return Task.FromResult(_cache.Values.AsEnumerable());
	}
	public Task<IEnumerable<BunnyTypeRegistry>> GetByBunnyTypeAsync(string bunnyType, CancellationToken ct = default) {
		var result = new List<BunnyTypeRegistry>();
        foreach (var item in _cache.Values.Where(x => x.BunnyType == bunnyType)) {
	        result.Add(item);
        }

        return Task.FromResult(result.AsEnumerable());
	}
}





	

