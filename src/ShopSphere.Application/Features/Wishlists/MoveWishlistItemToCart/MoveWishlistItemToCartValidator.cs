using FluentValidation;

namespace ShopSphere.Application.Features.Wishlists.MoveWishlistItemToCart;

public sealed class MoveWishlistItemToCartValidator
    : AbstractValidator<MoveWishlistItemToCartCommand>
{
    public MoveWishlistItemToCartValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();
    }
}