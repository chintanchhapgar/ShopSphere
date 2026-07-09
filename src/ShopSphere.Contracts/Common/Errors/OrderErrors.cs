using ShopSphere.Contracts.Common;

namespace ShopSphere.Contracts.Errors;

public static class OrderErrors
{
    public static readonly Error NotFound = new(
        "Order.NotFound",
        "Order not found.");

    public static readonly Error OrderNumberAlreadyExists = new(
        "Order.OrderNumberAlreadyExists",
        "Order number already exists.");

    public static readonly Error InvalidStatus = new(
        "Order.InvalidStatus",
        "Invalid order status.");

    public static readonly Error CannotCancel = new(
        "Order.CannotCancel",
        "This order cannot be cancelled.");
}