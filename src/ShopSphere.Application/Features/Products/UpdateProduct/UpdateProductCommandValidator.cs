using FluentValidation;
using ShopSphere.Application.Features.Products.Common;

namespace ShopSphere.Application.Features.Products.UpdateProduct;

public sealed class UpdateProductCommandValidator
    : ProductCommandValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
        : base(
            x => x.Name,
            x => x.Description,
            x => x.SKU,
            x => x.BasePrice,
            x => x.CostPrice,
            x => x.CategoryId,
            x => x.BrandId)
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}