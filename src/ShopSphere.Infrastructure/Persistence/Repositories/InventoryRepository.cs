using Microsoft.EntityFrameworkCore;
using ShopSphere.Domain.Entities;
using ShopSphere.Infrastructure.Persistence;
using ShopSphere.Infrastructure.Persistence.Repositories;

public sealed class InventoryRepository
    : Repository<Inventory>,
      IInventoryRepository
{
    private readonly ApplicationDbContext _context;

    public InventoryRepository(
        ApplicationDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<Inventory?> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        return await _context.Inventories
            .FirstOrDefaultAsync(
                x => x.ProductId == productId,
                cancellationToken);
    }
}