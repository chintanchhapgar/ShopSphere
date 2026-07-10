using Microsoft.EntityFrameworkCore;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Infrastructure.Persistence.Repositories;

public sealed class InventoryRepository
    : Repository<Inventory>,
      IInventoryRepository
{
    public InventoryRepository(
        ApplicationDbContext context)
        : base(context)
    {
    }

    public void Update(Inventory inventory)
    {
        Context.Inventories.Update(inventory);
    }

    public async Task<Inventory?> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken)
    {
        return await Context.Inventories
            .FirstOrDefaultAsync(
                x => x.ProductId == productId,
                cancellationToken);
    }

    public async Task<List<Inventory>> GetByProductIdsAsync(
        IEnumerable<Guid> productIds,
        CancellationToken cancellationToken)
    {
        return await Context.Inventories
            .Where(x => productIds.Contains(x.ProductId))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> AddOrRestoreAsync(
        Inventory inventory,
        CancellationToken cancellationToken)
    {
        var existing = await Context.Inventories
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.ProductId == inventory.ProductId,
                cancellationToken);

        if (existing is null)
        {
            await AddAsync(
                inventory,
                cancellationToken);

            return true;
        }

        if (!existing.IsDeleted)
        {
            return false;
        }

        existing.Restore();

        return true;
    }
}