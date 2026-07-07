using FluentValidation;

namespace ShopSphere.Application.Features.Products.CreateProduct;

public sealed class CreateProductCommandValidator
    : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty();

        RuleFor(x => x.SKU)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.BasePrice)
            .GreaterThan(0);

        RuleFor(x => x.CostPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.CostPrice.HasValue);

        RuleFor(x => x.CategoryId)
            .NotEmpty();

        RuleFor(x => x.BrandId)
            .NotEmpty();
    }
}