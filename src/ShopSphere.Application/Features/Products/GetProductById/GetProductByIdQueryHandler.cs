using MediatR;
using ShopSphere.Application.Features.Products.Common;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Products.GetProductById;

public sealed class GetProductByIdQueryHandler
    : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IProductQueries _queries;

    public GetProductByIdQueryHandler(
        IProductQueries queries)
    {
        _queries = queries;
    }

    public async Task<Result<ProductDto>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _queries.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (product is null)
        {
            return Result<ProductDto>.Failure(
                ProductErrors.NotFound);
        }

        return Result<ProductDto>
            .Success(product, "Product retrieved successfully.");
    }
}