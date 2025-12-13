using BunnyBroker.Entities;
using Microsoft.EntityFrameworkCore;

namespace BunnyBroker.Repository;

public interface IBunnyMessageRepository {
    Task<BunnyMessage> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<BunnyMessage>> GetByTypeAsync(string type, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default);
    Task<IEnumerable<BunnyMessage>> GetAllAsync(DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default);

    Task<IEnumerable<BunnyMessage>> GetUnprocessedForHandler(
	    string handlerType, DateTime? fromDate = null, CancellationToken ct = default);
    Task AddAsync(BunnyMessage message, CancellationToken ct = default);
}

public class BunnyMessageRepository(BunnyDbContext context, IBunnyTypeRegistryRepository bunnyTypeRegistryRepository) : IBunnyMessageRepository {

	public Task<BunnyMessage> GetByIdAsync(Guid id, CancellationToken ct = default) {
		return context.BunnyMessages
			.Where(x=>x.Id== id)
			.Include(x=>x.BunnyLogs)
			.AsNoTracking()
			.FirstAsync(ct);
    }
	public async Task<IEnumerable<BunnyMessage>> GetByTypeAsync(string type, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default) {
		return await context.BunnyMessages
			.Where(m => m.BunnyType == type)
			.Where(m => fromDate == null || m.CreatedAt >= fromDate)
			.Where(m => toDate == null || m.CreatedAt <= toDate)
			.AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<BunnyMessage>> GetAllAsync(DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default) {
		return await context.BunnyMessages
			.Where(m => fromDate == null || m.CreatedAt >= fromDate)
			.Where(m => toDate == null || m.CreatedAt <= toDate)
			.AsNoTracking()
            .ToListAsync(ct);
    }

	public async Task<IEnumerable<BunnyMessage>> GetUnprocessedForHandler(
		string handlerType, DateTime? fromDate = null, CancellationToken ct = default) {

		return await context.BunnyLogs
			.Where(x => x.BunnyHandlerType == handlerType)
			.Where(x=> x.ProcessedAt == null || !x.Sucess)
			.Select(x=>x.BunnyMessage)
			.Distinct()
			.ToListAsync(ct);
    }

	public async Task AddAsync(BunnyMessage message, CancellationToken ct = default) {
		message.CreatedAt = DateTime.UtcNow;
        

        var bunnyTypeRegistries = bunnyTypeRegistryRepository.GetByBunnyTypeAsync(message.BunnyType, ct);
        foreach (var typeRegistry in await bunnyTypeRegistries) {
	        context.BunnyLogs.Add(new BunnyLog {
				BunnyMessageId = message.Id,
                BunnyHandlerType = typeRegistry.BunnyHandlerType,
				//BunnyTypeRegistry = typeRegistry
            });
        }

        context.BunnyMessages.Add(message);

        await context.SaveChangesAsync(ct);
    }
}
