using ShopSphere.Domain.Entities;

namespace ShopSphere.Domain.Interfaces;

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

    void Update(Inventory inventory);

    Task<List<Inventory>> GetByProductIdsAsync(
        IEnumerable<Guid> productIds,
        CancellationToken cancellationToken);

    Task<bool> AddOrRestoreAsync(
        Inventory inventory,
        CancellationToken cancellationToken);

}