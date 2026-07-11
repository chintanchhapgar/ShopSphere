using MediatR;
using ShopSphere.Application.Features.Brands.GetBrands;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;

namespace ShopSphere.Application.Features.Brands.GetBrandById;

public sealed class GetBrandByIdQueryHandler
    : IRequestHandler<GetBrandByIdQuery, Result<BrandDto>>
{
    private readonly IBrandQueries _queries;
    private readonly ICacheService _cacheService;

    public GetBrandByIdQueryHandler(
        IBrandQueries queries,
        ICacheService cacheService)
    {
        _queries = queries;
        _cacheService = cacheService;
    }

    public async Task<Result<BrandDto>> Handle(
        GetBrandByIdQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"brands:{request.Id}";

        var cached = await _cacheService.GetAsync<BrandDto>(
            cacheKey,
            cancellationToken);

        if (cached is not null)
        {
            return Result<BrandDto>.Success(
                cached,
                "Brand retrieved from cache.");
        }

        var brand = await _queries.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (brand is null)
        {
            return Result<BrandDto>.Failure(
                BrandErrors.NotFound);
        }

        await _cacheService.SetAsync(
            cacheKey,
            brand,
            TimeSpan.FromMinutes(15),
            cancellationToken);

        return Result<BrandDto>.Success(
            brand,
            "Brand retrieved successfully.");
    }
}