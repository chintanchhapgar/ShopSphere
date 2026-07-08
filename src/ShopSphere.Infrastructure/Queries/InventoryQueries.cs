using Microsoft.EntityFrameworkCore;
using ShopSphere.Application.Features.Inventories.Common;
using ShopSphere.Application.Queries;
using ShopSphere.Infrastructure.Persistence;

namespace ShopSphere.Infrastructure.Queries;

public sealed class InventoryQueries : IInventoryQueries
{
    private readonly ApplicationDbContext _context;

    public InventoryQueries(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<InventoryDto?> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        return await _context.Inventories
            .AsNoTracking()
            .Where(x => x.ProductId == productId)
            .Select(x => new InventoryDto(
                x.ProductId,
                x.QuantityOnHand,
                x.ReservedQuantity,
                x.QuantityOnHand - x.ReservedQuantity,
                x.LowStockThreshold,
                (x.QuantityOnHand - x.ReservedQuantity) <= x.LowStockThreshold))
            .FirstOrDefaultAsync(cancellationToken);
    }
}