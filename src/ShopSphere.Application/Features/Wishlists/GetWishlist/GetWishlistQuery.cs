using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Wishlists.GetWishlist;

public sealed record GetWishlistQuery
    : IRequest<Result<WishlistResponse>>;