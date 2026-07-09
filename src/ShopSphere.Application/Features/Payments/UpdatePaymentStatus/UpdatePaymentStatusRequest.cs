using ShopSphere.Domain.Enums;

namespace ShopSphere.Contracts.Payments;

public sealed record UpdatePaymentStatusRequest(
    PaymentStatus Status,
    string? TransactionId);