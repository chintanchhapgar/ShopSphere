using ShopSphere.Domain.Entities;

public interface IInventoryTransactionRepository
{
    Task AddAsync(
        InventoryTransaction transaction,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}