using FluentValidation;

namespace ShopSphere.Application.Features.Carts.AddCartItem;

public sealed class AddCartItemCommandValidator
    : AbstractValidator<AddCartItemCommand>
{
    public AddCartItemCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}