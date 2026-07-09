using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Carts.RemoveCoupon;

public sealed class RemoveCouponCommandHandler
    : IRequestHandler<RemoveCouponCommand, Result>
{
    private readonly ICartRepository _cartRepository;
    private readonly ICurrentUserService _currentUserService;

    public RemoveCouponCommandHandler(
        ICartRepository cartRepository,
        ICurrentUserService currentUserService)
    {
        _cartRepository = cartRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(
        RemoveCouponCommand request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
            _currentUserService.UserId,
            out var customerId))
        {
            return Result.Failure(
                UserErrors.Unauthorized);
        }

        var cart = await _cartRepository.GetByCustomerWithItemsAsync(
            customerId,
            cancellationToken);

        if (cart is null)
        {
            return Result.Failure(
                CartErrors.NotFound);
        }

        if (!cart.CouponId.HasValue)
        {
            return Result.Failure(CartErrors.CouponNotApplied);
        }

        cart.RemoveCoupon();

        await _cartRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            "Coupon removed successfully.");
    }
}