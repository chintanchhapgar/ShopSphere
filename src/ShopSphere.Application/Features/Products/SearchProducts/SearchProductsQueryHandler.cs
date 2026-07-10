using MediatR;
using ShopSphere.Application.Common.Models;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Products.SearchProducts;

public sealed class SearchProductsQueryHandler
    : IRequestHandler<
        SearchProductsQuery,
        Result<PagedResult<ProductSearchResponse>>>
{
    private readonly IProductReadRepository _repository;

    public SearchProductsQueryHandler(
        IProductReadRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PagedResult<ProductSearchResponse>>> Handle(
        SearchProductsQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _repository.SearchAsync(
            request.Request,
            cancellationToken);

        return Result<PagedResult<ProductSearchResponse>>
            .Success(result);
    }
}