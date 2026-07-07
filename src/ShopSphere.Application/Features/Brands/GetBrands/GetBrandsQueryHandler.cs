using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Brands.GetBrands;

public sealed class GetBrandsQueryHandler
    : IRequestHandler<GetBrandsQuery, Result<IReadOnlyList<BrandResponse>>>
{
    private readonly IBrandRepository _repository;

    public GetBrandsQueryHandler(
        IBrandRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<BrandResponse>>> Handle(
        GetBrandsQuery request,
        CancellationToken cancellationToken)
    {
        var brands = await _repository.GetAllAsync(
            cancellationToken);

        var response = brands
            .Select(x => new BrandResponse(
                x.Id,
                x.Name,
                x.Description,
                x.IsActive))
            .ToList();

        return Result<IReadOnlyList<BrandResponse>>.Success(
            response,
            "Brands retrieved successfully.");
    }
}