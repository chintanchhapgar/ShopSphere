namespace ShopSphere.Application.Notifications;

public sealed record PaymentSucceededEmailModel(
    string CustomerName,
    string Email,
    string OrderNumber,
    decimal Amount,
    string PaymentMethod,
    string TransactionId);