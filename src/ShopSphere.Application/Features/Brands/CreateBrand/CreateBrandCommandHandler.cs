using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Brands.CreateBrand;

public sealed class CreateBrandCommandHandler
    : IRequestHandler<CreateBrandCommand, Result<Guid>>
{
    private readonly IBrandRepository _repository;
    private readonly ICacheService _cacheService;

    public CreateBrandCommandHandler(
        IBrandRepository repository,
        ICacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
    }

    public async Task<Result<Guid>> Handle(
        CreateBrandCommand request,
        CancellationToken cancellationToken)
    {
        var exists = await _repository.ExistsByNameAsync(
            request.Name,
            null,
            cancellationToken);

        if (exists)
        {
            return Result<Guid>.Failure(
                BrandErrors.AlreadyExists);
        }

        var brand = new Brand(
         request.Name,
         request.Description);

        var added = await _repository.AddOrRestoreAsync(
            brand,
            cancellationToken);

        if (!added)
        {
            return Result<Guid>.Failure(
                BrandErrors.AlreadyExists);
        }

        await _repository.SaveChangesAsync(
            cancellationToken);

        await _cacheService.RemoveByPrefixAsync(
          "brands",
          cancellationToken);

        return Result<Guid>.Success(
            brand.Id,
            "Brand created successfully.");
    }
}