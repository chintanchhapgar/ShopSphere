namespace ShopSphere.Infrastructure.Payments.DTOs;
public sealed record StripeLineItem(
    string Name,
    string? Description,
    decimal UnitAmount,
    int Quantity,
    string? ImageUrl);