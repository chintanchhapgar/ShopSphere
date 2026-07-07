using ShopSphere.Application.Services.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Application.Features.Brands;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Infrastructure.Services;

public sealed class BrandService : IBrandService
{
    private readonly IBrandRepository _repository;

    public BrandService(
        IBrandRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Brand>> GetRequiredAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var brand = await _repository.GetByIdAsync(
            id,
            cancellationToken);

        if (brand is null)
        {
            return Result<Brand>.Failure(
                BrandErrors.NotFound);
        }

        return Result<Brand>.Success(brand);
    }

    public async Task<Result> EnsureActiveAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        var result = await GetRequiredAsync(
            id,
            cancellationToken);

        if (!result.IsSuccess)
        {
            return Result.Failure(result.Error!);
        }

        if (!result.Value!.IsActive)
        {
            return Result.Failure(
                BrandErrors.Inactive);
        }

        return Result.Success();
    }
}