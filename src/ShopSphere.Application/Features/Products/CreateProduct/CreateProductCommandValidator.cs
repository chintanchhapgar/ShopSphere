using ShopSphere.Application.Features.Products.Common;

namespace ShopSphere.Application.Features.Products.CreateProduct;

public sealed class CreateProductCommandValidator
    : ProductCommandValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
        : base(
            x => x.Name,
            x => x.Description,
            x => x.SKU,
            x => x.BasePrice,
            x => x.CostPrice,
            x => x.CategoryId,
            x => x.BrandId)
    {
    }
}