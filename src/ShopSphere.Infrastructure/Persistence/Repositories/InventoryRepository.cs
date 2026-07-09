using Microsoft.EntityFrameworkCore;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;
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

    public void Update(Inventory inventory)
    {
        _context.Inventories.Update(inventory);
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

    public async Task<List<Inventory>> GetByProductIdsAsync(
        IEnumerable<Guid> productIds,
        CancellationToken cancellationToken)
    {
        return await _context.Inventories
            .Where(x => productIds.Contains(x.ProductId))
            .ToListAsync(cancellationToken);
    }
}