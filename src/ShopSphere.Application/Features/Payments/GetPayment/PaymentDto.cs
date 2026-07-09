using ShopSphere.Domain.Enums;

namespace ShopSphere.Application.Features.Payments.GetPayment;

public sealed record PaymentDto(
    Guid Id,
    Guid OrderId,
    decimal Amount,
    PaymentStatus Status,
    PaymentMethod Method,
    string? TransactionId,
    DateTime CreatedAtUtc);