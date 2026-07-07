
namespace ShopSphere.Domain.Interfaces;

public interface IRepository<TEntity>
    where TEntity : Entity
{
    Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken);

    Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<TEntity>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<bool> ExistsAsync(
        Guid id,
        CancellationToken cancellationToken);

    void Remove(
        TEntity entity);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}