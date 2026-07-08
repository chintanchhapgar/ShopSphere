using MediatR;
using ShopSphere.Application.Features.Brands.GetBrands;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Brands.GetBrandById;

public sealed class GetBrandByIdQueryHandler
    : IRequestHandler<GetBrandByIdQuery, Result<BrandDto>>
{
    private readonly IBrandQueries _queries;

    public GetBrandByIdQueryHandler(
        IBrandQueries queries)
    {
        _queries = queries;
    }

    public async Task<Result<BrandDto>> Handle(
        GetBrandByIdQuery request,
        CancellationToken cancellationToken)
    {
        var brand = await _queries.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (brand is null)
        {
            return Result<BrandDto>.Failure(
                BrandErrors.NotFound);
        }

        return Result<BrandDto>
            .Success(brand, "Brand retrieved successfully.");
    }
}