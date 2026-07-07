using MediatR;
using ShopSphere.Application.Features.Brands.GetBrands;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Brands.GetBrandById;

public sealed class GetBrandByIdQueryHandler
    : IRequestHandler<GetBrandByIdQuery, Result<BrandResponse>>
{
    private readonly IBrandRepository _repository;

    public GetBrandByIdQueryHandler(
        IBrandRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<BrandResponse>> Handle(
        GetBrandByIdQuery request,
        CancellationToken cancellationToken)
    {
        var brand = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (brand is null)
        {
            return Result<BrandResponse>.Failure(
                BrandErrors.NotFound);
        }

        return Result<BrandResponse>.Success(
            new BrandResponse(
                brand.Id,
                brand.Name,
                brand.Description,
                brand.IsActive),
            "Brand retrieved successfully.");
    }
}