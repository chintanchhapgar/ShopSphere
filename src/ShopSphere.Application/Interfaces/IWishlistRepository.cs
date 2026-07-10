using ShopSphere.Domain.Entities;

namespace ShopSphere.Domain.Interfaces;

public interface IWishlistRepository
{
    Task<Wishlist?> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken);

    Task<Wishlist?> GetByCustomerWithItemsAsync(
        Guid customerId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Wishlist wishlist,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}