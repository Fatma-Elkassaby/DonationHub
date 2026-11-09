using DonationHub.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DonationHub.Infrastructure.Persistence.Interceptors
{
    public class SoftDeleteInterceptor : SaveChangesInterceptor
    {
        private readonly TimeProvider _timeProvider;

        public SoftDeleteInterceptor(TimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }
        private void SoftDelete(DbContext? context)
        {
            if (context is null)
                return;

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry is not
                    {
                        State: EntityState.Deleted,
                        Entity: ISoftDeletable entity
                    })
                    continue;

                if (entity.IsDeleted)
                    continue;

                entry.State = EntityState.Modified;

                entity.Delete(_timeProvider.GetUtcNow().UtcDateTime);
            }
        }
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData,InterceptionResult<int> result)
        {
            SoftDelete(eventData.Context);

            return base.SavingChanges(eventData, result);
        }
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,InterceptionResult<int> result,CancellationToken cancellationToken = default)
        {
            SoftDelete(eventData.Context);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
