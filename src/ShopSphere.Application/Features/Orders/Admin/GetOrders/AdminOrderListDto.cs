namespace ShopSphere.Application.Features.Orders.Admin.GetOrders;

public sealed record AdminOrderListDto(
    Guid Id,
    string OrderNumber,
    Guid UserId,
    string Status,
    decimal TotalAmount,
    int TotalItems,
    DateTime OrderDate);