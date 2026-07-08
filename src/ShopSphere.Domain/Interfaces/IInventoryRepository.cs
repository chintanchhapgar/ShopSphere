using ShopSphere.Domain.Entities;

public interface IInventoryRepository
{
    Task<Inventory?> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Inventory inventory,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}