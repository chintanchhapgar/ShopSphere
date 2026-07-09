namespace ShopSphere.Application.Features.Orders.GetOrderById;

public sealed record OrderItemDto(
    Guid ProductId,
    string ProductName,
    string ProductSku,
    string? ProductImage,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);