namespace ShopSphere.Contracts.Payments;

public sealed record PaymentFailedRequest(
    string Reason);