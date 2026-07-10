using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Wishlists.RemoveFromWishlist;

public sealed class RemoveFromWishlistCommandHandler
    : IRequestHandler<RemoveFromWishlistCommand, Result>
{
    private readonly IWishlistRepository _wishlistRepository;
    private readonly ICurrentUserService _currentUserService;

    public RemoveFromWishlistCommandHandler(
        IWishlistRepository wishlistRepository,
        ICurrentUserService currentUserService)
    {
        _wishlistRepository = wishlistRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(
        RemoveFromWishlistCommand request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
            _currentUserService.UserId,
            out var customerId))
        {
            return Result.Failure(
                UserErrors.Unauthorized);
        }

        var wishlist = await _wishlistRepository.GetByCustomerWithItemsAsync(
            customerId,
            cancellationToken);

        if (wishlist is null)
        {
            return Result.Success(
                "Wishlist is empty.");
        }

        wishlist.RemoveItem(request.ProductId);

        await _wishlistRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            "Product removed from wishlist.");
    }
}