using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Wishlists.GetWishlist;

public sealed class GetWishlistQueryHandler
    : IRequestHandler<GetWishlistQuery, Result<WishlistResponse>>
{
    private readonly IWishlistRepository _wishlistRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetWishlistQueryHandler(
        IWishlistRepository wishlistRepository,
        ICurrentUserService currentUserService)
    {
        _wishlistRepository = wishlistRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<WishlistResponse>> Handle(
        GetWishlistQuery request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
            _currentUserService.UserId,
            out var customerId))
        {
            return Result<WishlistResponse>.Failure(
                UserErrors.Unauthorized);
        }

        var wishlist = await _wishlistRepository.GetByCustomerWithItemsAsync(
            customerId,
            cancellationToken);

        if (wishlist is null)
        {
            return Result<WishlistResponse>.Success(
                new WishlistResponse([]));
        }

        var items = wishlist.Items
            .Select(item => new WishlistItemResponse(
                item.ProductId,
                item.Product.Name,
                item.Product.SKU,
                item.Product.CostPrice ?? 0,
                item.Product.Images.FirstOrDefault()?.ImageUrl,
                item.Product.Inventory?.AvailableQuantity > 0))
            .ToList();

        return Result<WishlistResponse>.Success(
            new WishlistResponse(items));
    }
}