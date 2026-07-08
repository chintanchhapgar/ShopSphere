using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Carts.UpdateCartItem;

public sealed class UpdateCartItemCommandHandler
    : IRequestHandler<UpdateCartItemCommand, Result>
{
    private readonly ICartRepository _cartRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ICurrentUserService _currentUser;

    public UpdateCartItemCommandHandler(
        ICartRepository cartRepository,
        IInventoryRepository inventoryRepository,
        ICurrentUserService currentUser)
    {
        _cartRepository = cartRepository;
        _inventoryRepository = inventoryRepository;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        UpdateCartItemCommand request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUser.UserId, out var customerId))
        {
            return Result.Failure(CommonErrors.Unauthorized);
        }

        var cart = await _cartRepository.GetByItemIdAsync(
            request.ItemId,
            cancellationToken);

        if (cart is null || cart.CustomerId != customerId)
        {
            return Result.Failure(CommonErrors.NotFound);
        }

        var item = cart.Items.First(i => i.Id == request.ItemId);

        var inventory = await _inventoryRepository.GetByProductIdAsync(
            item.ProductId,
            cancellationToken);

        if (inventory is null)
        {
            return Result.Failure(InventoryErrors.NotFound);
        }

        // Replace AvailableQuantity with your actual property if needed
        if (inventory.AvailableQuantity < request.Quantity)
        {
            return Result.Failure(InventoryErrors.InsufficientStock);
        }

        cart.UpdateQuantity(
            request.ItemId,
            request.Quantity);

        await _cartRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success("Cart updated successfully.");
    }
}