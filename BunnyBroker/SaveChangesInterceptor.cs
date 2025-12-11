using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;


namespace BunnyBroker
{
    public abstract class SaveChangesInterceptor<T> : SaveChangesInterceptor
		where T : DbContext {
	    public abstract ValueTask<InterceptionResult<int>> SavingChangesAsync(
		    T context, InterceptionResult<int> result, CancellationToken cancellationToken = default);

	    /// <inheritdoc />
	    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
		    DbContextEventData eventData, InterceptionResult<int> result,
		    CancellationToken cancellationToken = new CancellationToken()) {
			if (eventData.Context is T context) {
				return SavingChangesAsync(context, result, cancellationToken);
            }
            return base.SavingChangesAsync(eventData, result, cancellationToken);
	    }
    }

}
