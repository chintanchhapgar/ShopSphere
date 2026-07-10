using Microsoft.EntityFrameworkCore;
using ShopSphere.Domain.Interfaces;
using System.Linq.Expressions;

namespace ShopSphere.Infrastructure.Persistence.Repositories;

public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : Entity
{
    protected readonly ApplicationDbContext Context;

    protected DbSet<TEntity> Entities => Context.Set<TEntity>();

    public Repository(ApplicationDbContext context)
    {
        Context = context;
    }

    public virtual async Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken)
    {
        await Entities.AddAsync(entity, cancellationToken);
    }

    public virtual async Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await Entities.FirstOrDefaultAsync(
            x => x.Id == id,
            cancellationToken);
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await Entities
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public virtual async Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await Entities.AnyAsync(
            x => x.Id == id,
            cancellationToken);
    }

    public virtual void Remove(TEntity entity)
    {
        if (entity is AuditableEntity auditableEntity)
        {
            auditableEntity.Delete();

            Context.Entry(entity).State = EntityState.Modified;

            return;
        }

        Entities.Remove(entity);
    }

    public virtual async Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        await Context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task<TEntity?> FirstOrDefaultIgnoreQueryFiltersAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken)
    {
        return await Entities
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                predicate,
                cancellationToken);
    }
}