using MediatR;
using ShopSphere.Application.Features.Carts.ClearCart;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Carts.ClearCart;

public sealed class ClearCartCommandHandler
    : IRequestHandler<ClearCartCommand, Result>
{
    private readonly ICartRepository _cartRepository;
    private readonly ICurrentUserService _currentUser;

    public ClearCartCommandHandler(
        ICartRepository cartRepository,
        ICurrentUserService currentUser)
    {
        _cartRepository = cartRepository;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        ClearCartCommand request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(_currentUser.UserId, out var customerId))
        {
            return Result.Failure(CommonErrors.Unauthorized);
        }

        var cart = await _cartRepository.GetByCustomerIdAsync(
            customerId,
            cancellationToken);

        if (cart is null || !cart.Items.Any())
        {
            return Result.Success("Cart is already empty.");
        }

        _cartRepository.RemoveItems(cart);

        await _cartRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success("Cart cleared successfully.");
    }
}