using FluentValidation;

namespace ShopSphere.Application.Features.Wishlists.RemoveFromWishlist;

public sealed class RemoveFromWishlistValidator
    : AbstractValidator<RemoveFromWishlistCommand>
{
    public RemoveFromWishlistValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();
    }
}