using FluentValidation;

namespace ShopSphere.Application.Features.Inventories.AdjustInventory;

public sealed class AdjustInventoryCommandValidator
    : AbstractValidator<AdjustInventoryCommand>
{
    public AdjustInventoryCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty();

        RuleFor(x => x.Quantity)
            .NotEqual(0);

        RuleFor(x => x.Reason)
            .NotEmpty()
            .MaximumLength(200);
    }
}