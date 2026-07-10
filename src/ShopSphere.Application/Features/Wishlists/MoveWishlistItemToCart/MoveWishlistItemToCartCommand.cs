using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Wishlists.MoveWishlistItemToCart;

public sealed record MoveWishlistItemToCartCommand(
    Guid ProductId)
    : IRequest<Result>;