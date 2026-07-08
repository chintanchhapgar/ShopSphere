using ShopSphere.Application.Features.Inventory.Common;

namespace ShopSphere.Application.Queries;

public interface IInventoryTransactionQueries
{
    Task<IReadOnlyList<InventoryTransactionDto>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken);
}