using ShopSphere.Domain.Entities;

namespace ShopSphere.Domain.Interfaces;

public interface IWishlistRepository
    : IRepository<Wishlist>
{
    Task<Wishlist?> GetByCustomerWithItemsAsync(
        Guid customerId,
        CancellationToken cancellationToken);

    Task<bool> AddOrRestoreItemAsync(
        Wishlist wishlist,
        Guid productId,
        CancellationToken cancellationToken);
}