using ShopSphere.Domain.Entities;

namespace ShopSphere.Domain.Interfaces;

public interface IOrderItemRepository
{
    Task AddRangeAsync(
        IEnumerable<OrderItem> orderItems,
        CancellationToken cancellationToken = default);
}