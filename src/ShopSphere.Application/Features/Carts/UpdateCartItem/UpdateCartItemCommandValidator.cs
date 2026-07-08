using FluentValidation;

namespace ShopSphere.Application.Features.Carts.UpdateCartItem;

public sealed class UpdateCartItemCommandValidator
    : AbstractValidator<UpdateCartItemCommand>
{
    public UpdateCartItemCommandValidator()
    {
        RuleFor(x => x.ItemId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}