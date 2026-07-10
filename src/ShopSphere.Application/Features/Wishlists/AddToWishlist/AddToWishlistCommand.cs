using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Wishlists.AddToWishlist;

public sealed record AddToWishlistCommand(
    Guid ProductId)
    : IRequest<Result>;