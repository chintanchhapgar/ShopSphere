using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopSphere.Application.Features.Products.Common;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Products.GetProducts;

public sealed class GetProductsQueryHandler
    : IRequestHandler<GetProductsQuery, Result<IReadOnlyList<ProductDto>>>
{
    private readonly IProductRepository _repository;

    public GetProductsQueryHandler(
        IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<ProductDto>>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var products = await _repository.GetAllAsync(
            cancellationToken);

        var response = products
            .Select(x => new ProductDto(
                x.Id,
                x.Name,
                x.Description,
                x.SKU,
                x.BasePrice,
                x.CostPrice,
                x.CategoryId,
                x.Category.Name,
                x.BrandId,
                x.Brand.Name,
                x.IsActive))
            .ToList();

        return Result<IReadOnlyList<ProductDto>>.Success(
            response,
            "Products retrieved successfully.");
    }
}