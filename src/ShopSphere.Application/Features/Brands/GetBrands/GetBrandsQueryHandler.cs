using MediatR;
using ShopSphere.Application.Features.Categories.Common;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Brands.GetBrands;

public sealed class GetBrandsQueryHandler
    : IRequestHandler<GetBrandsQuery, Result<IReadOnlyList<BrandDto>>>
{
    private readonly IBrandQueries _queries;

    public GetBrandsQueryHandler(
        IBrandQueries queries)
    {
        _queries = queries;
    }

    public async Task<Result<IReadOnlyList<BrandDto>>> Handle(
        GetBrandsQuery request,
        CancellationToken cancellationToken)
    {
        var brands = await _queries.GetAllAsync(cancellationToken);

        return Result<IReadOnlyList<BrandDto>>
            .Success(brands, "Brands retrieved successfully.");
    }
}