using ShopSphere.Contracts.Common;

namespace ShopSphere.Contracts.Errors;

public static class PaymentErrors
{
    public static readonly Error AlreadyExists = new(
        "Payment.AlreadyExists",
        "Payment already exists for this order.");

    public static readonly Error OrderNotFound = new(
        "Payment.OrderNotFound",
        "Order not found.");

    public static readonly Error NotFound = new(
        "Payment.NotFound",
        "Payment not found.");

    public static readonly Error InvalidStatus = new(
        "Payment.InvalidStatus",
        "Invalid payment status change.");
}