
namespace ShopSphere.Infrastructure.Payments.DTOs;
public sealed record PaymentIntentResponse(
    string ClientSecret,
    string PaymentIntentId,
    decimal Amount);