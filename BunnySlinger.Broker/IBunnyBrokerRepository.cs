using System.Collections.Concurrent;

using BunnySlinger.Broker.Contracts;


namespace BunnySlinger.Broker;

public interface IBunnyBrokerRepository {
    Task<IEnumerable<BunnyMessage>> GetAllAsync(CancellationToken ct = default);
    Task<BunnyMessage?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> AddAsync(BunnyMessage bunny, CancellationToken ct = default);
    Task<bool> SetProcessedAsync(BunnyMessage bunny, CancellationToken ct = default);
}

public class InMemoryBunnyBrokerRepository : IBunnyBrokerRepository {
    private readonly ConcurrentDictionary<Guid,BunnyMessage> _bunnies = new();

    public Task<IEnumerable<BunnyMessage>> GetAllAsync(CancellationToken ct = default) {
	    return Task.FromResult(_bunnies.Values.OrderBy(x=>x.CreatedAt).AsEnumerable());
    }
        
    public Task<BunnyMessage?> GetByIdAsync(Guid id, CancellationToken ct = default) {
	    _bunnies.TryGetValue(id, out var result);
	    return Task.FromResult(result);
    }

    public Task<bool> AddAsync(BunnyMessage bunny, CancellationToken ct = default) {
	    if (_bunnies.Keys.Count > 1000) {
            var id = _bunnies.Values.OrderBy(x=>x.CreatedAt).First().Id;
            _bunnies.TryRemove(id, out _);
        }
	    return Task.FromResult(_bunnies.TryAdd(bunny.Id, bunny));
    }

    public Task<bool> SetProcessedAsync(BunnyMessage bunny, CancellationToken ct = default) {
	    if (_bunnies.TryGetValue(bunny.Id, out var result)) {
            result.IsProcessed = true;
 }
        return Task.FromResult(true);
    }
}
