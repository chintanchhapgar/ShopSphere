using MediatR;
using ShopSphere.Application.Features.Products;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Wishlists.MoveWishlistItemToCart;

public sealed class MoveWishlistItemToCartCommandHandler
    : IRequestHandler<MoveWishlistItemToCartCommand, Result>
{
    private readonly IWishlistRepository _wishlistRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUserService _currentUserService;

    public MoveWishlistItemToCartCommandHandler(
        IWishlistRepository wishlistRepository,
        ICartRepository cartRepository,
        IProductRepository productRepository,
        ICurrentUserService currentUserService)
    {
        _wishlistRepository = wishlistRepository;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(
        MoveWishlistItemToCartCommand request,
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

        if (wishlist is null || !wishlist.Contains(request.ProductId))
        {
            return Result.Failure(
                WishlistErrors.NotFound);
        }

        var product = await _productRepository.GetByIdAsync(
            request.ProductId,
            cancellationToken);

        if (product is null)
        {
            return Result.Failure(
                ProductErrors.NotFound);
        }

        var cart = await _cartRepository.GetByCustomerWithItemsAsync(
            customerId,
            cancellationToken);

        if (cart is null)
        {
            cart = new Cart(customerId);

            await _cartRepository.AddAsync(
                cart,
                cancellationToken);
        }

        cart.AddItem(
            product.Id,
            1,
            product.BasePrice);

        wishlist.RemoveItem(product.Id);

        await _cartRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            "Product moved to cart.");
    }
}