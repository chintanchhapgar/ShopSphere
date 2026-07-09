using ShopSphere.Domain.Entities;

namespace ShopSphere.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<bool> ExistsBySkuAsync(
        string sku,
        Guid? excludeId,
        CancellationToken cancellationToken);
}