using MediatR;
using ShopSphere.Application.Features.Products.Common;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Products.GetProductById;

public sealed class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IProductRepository _repository;

    public GetProductByIdQueryHandler(
        IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<ProductDto>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (product is null)
        {
            return Result<ProductDto>.Failure(
                ProductErrors.NotFound);
        }

        return Result<ProductDto>.Success(
            new ProductDto(
                product.Id,
                product.Name,
                product.Description,
                product.SKU,
                product.BasePrice,
                product.CostPrice,
                product.CategoryId,
                product.Category.Name,
                product.BrandId,
                product.Brand.Name,
                product.IsActive),
            "Product retrieved successfully.");
    }
}