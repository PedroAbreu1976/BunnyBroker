using BunnyBroker.Entities;
using Microsoft.EntityFrameworkCore;


namespace BunnyBroker.Repository;

public interface IBunnyMessageRepository {
    Task<BunnyMessage> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<BunnyMessage>> GetByTypeAsync(string type, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default);
    Task<IEnumerable<BunnyMessage>> GetAllAsync(DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default);
    Task AddAsync(BunnyMessage message, CancellationToken ct = default);
}

public class BunnyMessageRepository(BunnyDbContext context) : IBunnyMessageRepository {

	public async Task<BunnyMessage> GetByIdAsync(Guid id, CancellationToken ct = default) {
		return await context.BunnyMessages
			.Where(x=>x.Id== id)
			.Include(x=>x.BunnyLogs)
			.AsNoTracking()
			.FirstOrDefaultAsync(ct) ?? throw new KeyNotFoundException($"BunnyMessage with Id {id} not found.");
    }
	public async Task<IEnumerable<BunnyMessage>> GetByTypeAsync(string type, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default) {
		return await context.BunnyMessages
			.Where(m => m.BunnyType == type &&
						(fromDate == null || m.CreatedAt >= fromDate) &&
						(toDate == null || m.CreatedAt <= toDate))
			.AsNoTracking()
            .ToListAsync(ct);
    }
	public async Task<IEnumerable<BunnyMessage>> GetAllAsync(DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default) {
		return await context.BunnyMessages
			.Where(m => (fromDate == null || m.CreatedAt >= fromDate) &&
						(toDate == null || m.CreatedAt <= toDate))
			.AsNoTracking()
            .ToListAsync(ct);
    }
	public async Task AddAsync(BunnyMessage message, CancellationToken ct = default) {
		context.BunnyMessages.Add(message);
		await context.SaveChangesAsync(ct);
    }
}
