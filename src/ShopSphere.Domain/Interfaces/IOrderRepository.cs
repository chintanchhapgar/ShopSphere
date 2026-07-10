using ShopSphere.Domain.Entities;

namespace ShopSphere.Domain.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken = default);

    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Order?> GetByOrderNumberAsync(
        string orderNumber,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);

    Task<Order?> GetByIdWithItemsAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<Order?> GetByIdWithDetailsAsync(
        Guid orderId,
        CancellationToken cancellationToken);
}