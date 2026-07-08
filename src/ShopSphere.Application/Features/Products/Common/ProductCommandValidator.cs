using FluentValidation;

namespace ShopSphere.Application.Features.Products.Common;

public abstract class ProductCommandValidator<T> : AbstractValidator<T>
{
    protected ProductCommandValidator(
        Func<T, string> name,
        Func<T, string> description,
        Func<T, string> sku,
        Func<T, decimal> basePrice,
        Func<T, decimal?> costPrice,
        Func<T, Guid> categoryId,
        Func<T, Guid> brandId)
    {
        RuleFor(x => name(x))
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => description(x))
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => sku(x))
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(100)
            .Matches("^[A-Za-z0-9_-]+$")
            .WithMessage("SKU can only contain letters, numbers, hyphens and underscores.");

        RuleFor(x => basePrice(x))
            .GreaterThan(0);

        RuleFor(x => costPrice(x))
            .GreaterThanOrEqualTo(0)
            .When(x => costPrice(x).HasValue);

        RuleFor(x => x)
            .Must(x => !costPrice(x).HasValue || costPrice(x)! <= basePrice(x))
            .WithName("CostPrice")
            .WithMessage("Cost price cannot be greater than base price.");

        RuleFor(x => categoryId(x))
            .NotEmpty();

        RuleFor(x => brandId(x))
            .NotEmpty();
    }
}