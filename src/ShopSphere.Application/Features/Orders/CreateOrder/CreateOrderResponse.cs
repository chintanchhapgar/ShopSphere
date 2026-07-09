namespace ShopSphere.Application.Features.Orders.CreateOrder;

public sealed record CreateOrderResponse(
    Guid OrderId,
    string OrderNumber);