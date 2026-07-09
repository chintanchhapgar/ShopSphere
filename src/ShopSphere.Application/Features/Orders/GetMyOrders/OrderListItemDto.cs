namespace ShopSphere.Application.Features.Orders.GetMyOrders;

public sealed record OrderListItemDto(
    Guid Id,
    string OrderNumber,
    DateTime OrderDate,
    string Status,
    decimal TotalAmount,
    int TotalItems);