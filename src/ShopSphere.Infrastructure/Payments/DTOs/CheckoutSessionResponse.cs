namespace ShopSphere.Infrastructure.Payments.DTOs;
public sealed record CheckoutSessionResponse(
    string SessionId,
    string SessionUrl); 