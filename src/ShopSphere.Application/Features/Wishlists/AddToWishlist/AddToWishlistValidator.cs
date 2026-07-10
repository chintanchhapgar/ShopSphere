using FluentValidation;

namespace ShopSphere.Application.Features.Wishlists.AddToWishlist;

public sealed class AddToWishlistValidator
    : AbstractValidator<AddToWishlistCommand>
{
    public AddToWishlistValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();
    }
}