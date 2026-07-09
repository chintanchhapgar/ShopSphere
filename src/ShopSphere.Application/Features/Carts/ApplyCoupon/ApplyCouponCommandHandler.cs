using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Carts.ApplyCoupon;

public sealed class ApplyCouponCommandHandler
    : IRequestHandler<ApplyCouponCommand, Result>
{
    private readonly ICartRepository _cartRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly ICurrentUserService _currentUserService;

    public ApplyCouponCommandHandler(
        ICartRepository cartRepository,
        ICouponRepository couponRepository,
        ICurrentUserService currentUserService)
    {
        _cartRepository = cartRepository;
        _couponRepository = couponRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(
        ApplyCouponCommand request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
            _currentUserService.UserId,
            out var userId))
        {
            return Result.Failure(
                UserErrors.Unauthorized);
        }

        var cart = await _cartRepository.GetByCustomerWithItemsAsync(
            userId,
            cancellationToken);

        if (cart is null)
        {
            return Result.Failure(
                CartErrors.NotFound);
        }

        if (!cart.Items.Any())
        {
            return Result.Failure(
                CartErrors.Empty);
        }

        var coupon = await _couponRepository.GetByCodeAsync(
            request.CouponCode,
            cancellationToken);

        if (coupon is null)
        {
            return Result.Failure(
                CouponErrors.NotFound);
        }

        if (!coupon.IsActive)
        {
            return Result.Failure(
                CouponErrors.Invalid);
        }

        if (!coupon.CanBeUsed())
        {
            return Result.Failure(
                CouponErrors.UsageLimitReached);
        }

        if (coupon.IsExpired)
        {
            return Result.Failure(
                CouponErrors.Expired);
        }

        if (cart.SubTotal < coupon.MinimumOrderAmount)
        {
            return Result.Failure(
                CouponErrors.Invalid);
        }

        cart.ApplyCoupon(coupon);

        await _cartRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            "Coupon applied successfully.");
    }
}