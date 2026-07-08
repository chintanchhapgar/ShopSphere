using MediatR;
using ShopSphere.Application.Features.Products;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Carts.AddCartItem;

public sealed class AddCartItemCommandHandler
    : IRequestHandler<AddCartItemCommand, Result>
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ICurrentUserService _currentUser;

    public AddCartItemCommandHandler(
        ICartRepository cartRepository,
        IProductRepository productRepository,
        IInventoryRepository inventoryRepository,
        ICurrentUserService currentUser)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
        _inventoryRepository = inventoryRepository;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        AddCartItemCommand request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUser.UserId, out var customerId))
        {
            return Result.Failure(CommonErrors.Unauthorized);
        }

        var product = await _productRepository.GetByIdAsync(
            request.ProductId,
            cancellationToken);

        if (product is null || !product.IsActive)
        {
            return Result.Failure(ProductErrors.NotFound);
        }

        var inventory = await _inventoryRepository.GetByProductIdAsync(
            request.ProductId,
            cancellationToken);

        if (inventory is null)
        {
            return Result.Failure(InventoryErrors.NotFound);
        }

        if (inventory.AvailableQuantity < request.Quantity)
        {
            return Result.Failure(InventoryErrors.InsufficientStock);
        }

        var cart = await _cartRepository.GetByCustomerIdAsync(
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
            request.Quantity,
            product.BasePrice);

        await _cartRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success("Item added to cart.");
    }
}