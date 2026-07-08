using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Infrastructure.Persistence.Repositories;

public sealed class InventoryTransactionRepository
    : Repository<InventoryTransaction>,
      IInventoryTransactionRepository
{
    public InventoryTransactionRepository(
        ApplicationDbContext context)
        : base(context)
    {
    }
}