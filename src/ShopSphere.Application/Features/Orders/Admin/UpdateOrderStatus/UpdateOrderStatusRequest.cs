using ShopSphere.Domain.Enums;

namespace ShopSphere.Contracts.Orders;

public sealed record UpdateOrderStatusRequest(
    OrderStatus Status);