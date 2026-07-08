using MediatR;
using ShopSphere.Application.Features.Carts.RemoveCartItem;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Carts.RemoveCartItem;

public sealed class RemoveCartItemCommandHandler
    : IRequestHandler<RemoveCartItemCommand, Result>
{
    private readonly ICartRepository _cartRepository;
    private readonly ICurrentUserService _currentUser;

    public RemoveCartItemCommandHandler(
        ICartRepository cartRepository,
        ICurrentUserService currentUser)
    {
        _cartRepository = cartRepository;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        RemoveCartItemCommand request,
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

        cart.RemoveItem(request.ItemId);

        await _cartRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success("Item removed from cart.");
    }
}