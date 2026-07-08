namespace ShopSphere.Application.Features.Carts.Common;

public sealed record CartDto(
    IReadOnlyList<CartItemDto> Items,
    decimal Total);