using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ShopSphere.Application.Interfaces;
using ShopSphere.Domain.Common;

namespace ShopSphere.Infrastructure.Persistence.Interceptors;

public sealed class AuditableEntityInterceptor
    : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUser;

    public AuditableEntityInterceptor(
        ICurrentUserService currentUser)
    {
        _currentUser = currentUser;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(
            eventData,
            result,
            cancellationToken);
    }

    private void UpdateEntities(DbContext? context)
    {
        if (context is null)
            return;

        var utcNow = DateTime.UtcNow;
        var userId = _currentUser.UserId ?? "System";

        foreach (var entry in context.ChangeTracker
                     .Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetCreated(
                        utcNow,
                        userId);
                    break;

                case EntityState.Modified:
                    entry.Entity.SetUpdated(
                        utcNow,
                        userId);
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;

                    entry.Entity.SoftDelete(
                        utcNow,
                        userId);
                    break;
            }
        }
    }
}