using ShopSphere.Contracts.Common;

namespace ShopSphere.Contracts.Errors;
public static class PaymentErrors
{
    public static readonly Error NotFound =
        new(
            "Payment.NotFound",
            "Payment was not found.");

    public static readonly Error AlreadyExists =
        new(
            "Payment.AlreadyExists",
            "Payment already exists for this order.");

    public static readonly Error OrderNotFound =
        new(
            "Payment.OrderNotFound",
            "Order was not found.");

    public static readonly Error InvalidTransaction =
        new(
            "Payment.InvalidTransaction",
            "The payment transaction could not be verified.");
}