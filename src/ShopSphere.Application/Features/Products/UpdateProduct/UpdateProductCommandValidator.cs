using FluentValidation;

namespace ShopSphere.Application.Features.Products.UpdateProduct;

public sealed class UpdateProductCommandValidator
    : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

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