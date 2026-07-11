namespace ShopSphere.Contracts.Payments;

public sealed record PaymentSucceededRequest(
    string TransactionId);