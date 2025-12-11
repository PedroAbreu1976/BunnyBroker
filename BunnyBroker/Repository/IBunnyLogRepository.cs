using BunnyBroker.Entities;
using Microsoft.EntityFrameworkCore;

namespace BunnyBroker.Repository;

public interface IBunnyLogRepository {
	Task<IEnumerable<BunnyLog>> GetAllAsync(DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default);
    Task<BunnyLog> GetByIdsAsync(Guid bunnyMessageId, string bunnyHandlerType, CancellationToken ct = default);
	Task<IEnumerable<BunnyLog>> GetByMessageIdAsync(Guid bunnyMessageId, CancellationToken ct = default);
	Task<IEnumerable<BunnyLog>> GetByHandlerTypeAsync(
		string bunnyHandlerType, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default);
	Task<IEnumerable<BunnyLog>> GetByBunnyTypeAsync(
		string bunnyType, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default);
	Task UpdateAsync(BunnyLog log, CancellationToken ct = default);
}

public class BunnyLogRepository(BunnyDbContext context) : IBunnyLogRepository {
	public async Task<IEnumerable<BunnyLog>> GetAllAsync(DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default) {
		return await context.BunnyLogs
			.Where(log => fromDate == null || log.ProcessedAt >= fromDate)
			.Where(log => toDate == null || log.ProcessedAt <= toDate)
			.OrderByDescending(log => log.BunnyMessage.CreatedAt)
			.ToListAsync(ct);
    }

	public async Task<BunnyLog> GetByIdsAsync(Guid bunnyMessageId, string bunnyHandlerType, CancellationToken ct = default) {
		return await context.BunnyLogs.FindAsync(bunnyMessageId, bunnyHandlerType, ct)
			?? throw new KeyNotFoundException($"BunnyLog with MessageId {bunnyMessageId} and HandlerType {bunnyHandlerType} not found.");
	}
	public async Task<IEnumerable<BunnyLog>> GetByMessageIdAsync(Guid bunnyMessageId, CancellationToken ct = default) {
		return await context.BunnyLogs
			.Where(log => log.BunnyMessageId == bunnyMessageId)
			.ToListAsync(ct);
	}
	public async Task<IEnumerable<BunnyLog>> GetByHandlerTypeAsync(
		string bunnyHandlerType, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default) {
		return await context.BunnyLogs
			.Where(log => log.BunnyHandlerType == bunnyHandlerType)
			.Where(log => fromDate == null || log.ProcessedAt >= fromDate)
			.Where(log => toDate == null || log.ProcessedAt <= toDate)
			.OrderByDescending(log => log.BunnyMessage.CreatedAt)
			.ToListAsync(ct);
	}
	public async Task<IEnumerable<BunnyLog>> GetByBunnyTypeAsync(
		string bunnyType, DateTime? fromDate = null, DateTime? toDate = null, CancellationToken ct = default) {
		return await context.BunnyLogs
			.Where(log => log.BunnyTypeRegistry.BunnyType == bunnyType)
			.Where(log => fromDate == null || log.ProcessedAt >= fromDate)
			.Where(log => toDate == null || log.ProcessedAt <= toDate)
			.ToListAsync(ct);
	}

	public async Task UpdateAsync(BunnyLog bunnyLog, CancellationToken ct = default) {
		context.BunnyLogs.Update(bunnyLog);
        await context.SaveChangesAsync(ct);
	}
} 
