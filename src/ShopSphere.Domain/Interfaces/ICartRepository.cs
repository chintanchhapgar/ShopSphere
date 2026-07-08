using ShopSphere.Domain.Entities;

namespace ShopSphere.Domain.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetByCustomerIdAsync(
        Guid customerId,
        CancellationToken cancellationToken);

    Task AddAsync(
        Cart cart,
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);

    Task<Cart?> GetByItemIdAsync(
        Guid itemId,
        CancellationToken cancellationToken);
}