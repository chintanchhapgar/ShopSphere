using ShopSphere.Domain.Enums;

namespace ShopSphere.Contracts.Payments;

public sealed record PaymentRequest(
    PaymentMethod PaymentMethod);