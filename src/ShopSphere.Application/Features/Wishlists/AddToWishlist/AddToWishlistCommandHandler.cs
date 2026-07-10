using MediatR;
using ShopSphere.Application.Features.Products;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Wishlists.AddToWishlist;

public sealed class AddToWishlistCommandHandler
    : IRequestHandler<AddToWishlistCommand, Result>
{
    private readonly IWishlistRepository _wishlistRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUserService _currentUserService;

    public AddToWishlistCommandHandler(
        IWishlistRepository wishlistRepository,
        IProductRepository productRepository,
        ICurrentUserService currentUserService)
    {
        _wishlistRepository = wishlistRepository;
        _productRepository = productRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(
        AddToWishlistCommand request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
            _currentUserService.UserId,
            out var customerId))
        {
            return Result.Failure(
                UserErrors.Unauthorized);
        }

        var product = await _productRepository.GetByIdAsync(
            request.ProductId,
            cancellationToken);

        if (product is null)
        {
            return Result.Failure(
                ProductErrors.NotFound);
        }

        var wishlist = await _wishlistRepository.GetByCustomerWithItemsAsync(
            customerId,
            cancellationToken);

        if (wishlist is null)
        {
            wishlist = Wishlist.Create(customerId);

            await _wishlistRepository.AddAsync(
                wishlist,
                cancellationToken);
        }

        var added = await _wishlistRepository.AddOrRestoreItemAsync(
            wishlist,
            request.ProductId,
            cancellationToken);

        if (!added)
        {
            return Result.Success(
                "Product is already in your wishlist.");
        }

        await _wishlistRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            "Product added to wishlist.");
    }
}