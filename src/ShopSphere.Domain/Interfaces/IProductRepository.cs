using ShopSphere.Domain.Entities;

namespace ShopSphere.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<bool> ExistsBySkuAsync(
        string sku,
        Guid? excludeId,
        CancellationToken cancellationToken);
}