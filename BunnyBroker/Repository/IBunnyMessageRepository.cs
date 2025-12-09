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
		var result = await context.BunnyMessages
			.Where(x=>x.Id== id)
			.Select(m => new BunnyMessage {
				BunnyType = m.BunnyType,
				Body = m.Body,
				CreatedAt = m.CreatedAt,
				Id = m.Id,
				BunnyLogs = m.BunnyLogs.Select(l => new BunnyLog
				{
					BunnyHandlerType = l.BunnyHandlerType,
					BunnyTypeRegistry = l.BunnyTypeRegistry,
					ErrorMessage = l.ErrorMessage,
					ProcessedAt = l.ProcessedAt,
					Sucess = l.Sucess
                }).ToList()

            })
			.FirstOrDefaultAsync(ct) ?? throw new KeyNotFoundException($"BunnyMessage with Id {id} not found.");

		var handlers = await context.BunnyTypeRegistries
			.Where(x => x.BunnyType == result.BunnyType)
			.Select(x => x.BunnyHandlerType)
			.ToListAsync(ct);

		foreach (var handler in handlers) {
			if (!result.BunnyLogs.Any(l => l.BunnyHandlerType == handler)) {
				result.BunnyLogs.Add(new BunnyLog {
					BunnyHandlerType = handler,
					ProcessedAt = null,
					ErrorMessage = null
				});
			}
        }

        return result; 
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
