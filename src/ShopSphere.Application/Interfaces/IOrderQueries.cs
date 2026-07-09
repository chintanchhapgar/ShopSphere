using ShopSphere.Application.Features.Orders.Admin.GetOrders;
using ShopSphere.Application.Features.Orders.GetMyOrders;
using ShopSphere.Application.Features.Orders.GetOrderById;

namespace ShopSphere.Application.Queries;

public interface IOrderQueries
{
    Task<IReadOnlyList<OrderListItemDto>> GetMyOrdersAsync(
        Guid customerId,
        CancellationToken cancellationToken);

    Task<OrderDetailsDto?> GetByIdAsync(
        Guid customerId,
        Guid orderId,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<AdminOrderListDto>> GetAllAsync(
        CancellationToken cancellationToken);
}