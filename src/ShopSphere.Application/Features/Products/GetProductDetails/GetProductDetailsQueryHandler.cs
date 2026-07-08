using MediatR;
using ShopSphere.Application.Features.Products;
using ShopSphere.Application.Features.Products.Common;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;

public sealed class GetProductDetailsQueryHandler
    : IRequestHandler<GetProductDetailsQuery, Result<ProductDetailsDto>>
{
    private readonly IProductQueries _queries;

    public GetProductDetailsQueryHandler(
        IProductQueries queries)
    {
        _queries = queries;
    }

    public async Task<Result<ProductDetailsDto>> Handle(
        GetProductDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _queries.GetDetailsAsync(
            request.Id,
            cancellationToken);

        if (product is null)
        {
            return Result<ProductDetailsDto>.Failure(
                ProductErrors.NotFound);
        }

        return Result<ProductDetailsDto>.Success(product);
    }
}