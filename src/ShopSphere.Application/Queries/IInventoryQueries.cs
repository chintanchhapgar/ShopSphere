using ShopSphere.Application.Features.Inventories.Common;

namespace ShopSphere.Application.Queries;

public interface IInventoryQueries
{
    Task<InventoryDto?> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken);
}