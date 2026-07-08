using Microsoft.EntityFrameworkCore;
using ShopSphere.Application.Features.Inventory.Common;
using ShopSphere.Application.Queries;
using ShopSphere.Infrastructure.Persistence;

namespace ShopSphere.Infrastructure.Queries;

public sealed class InventoryTransactionQueries
    : IInventoryTransactionQueries
{
    private readonly ApplicationDbContext _context;

    public InventoryTransactionQueries(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<InventoryTransactionDto>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        return await _context.InventoryTransactions
            .AsNoTracking()
            .Where(x => x.Inventory.ProductId == productId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new InventoryTransactionDto(
                x.Id,
                x.Quantity,
                x.TransactionType,
                x.Reason,
                x.Reference,
                x.CreatedAtUtc,
                x.CreatedBy))
            .ToListAsync(cancellationToken);
    }
}