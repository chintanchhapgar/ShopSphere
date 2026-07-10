using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Wishlists.RemoveFromWishlist;

public sealed record RemoveFromWishlistCommand(
    Guid ProductId)
    : IRequest<Result>;