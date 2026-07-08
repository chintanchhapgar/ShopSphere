using ShopSphere.Domain.Entities;

public interface IProductImageRepository
{
    Task AddAsync(
        ProductImage image,
        CancellationToken cancellationToken);

    Task<ProductImage?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<ProductImage>> GetByProductIdAsync(
        Guid productId,
        CancellationToken cancellationToken);

    Task ClearPrimaryImageAsync(
        Guid productId,
        CancellationToken cancellationToken);

    void Remove(ProductImage image);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);

}