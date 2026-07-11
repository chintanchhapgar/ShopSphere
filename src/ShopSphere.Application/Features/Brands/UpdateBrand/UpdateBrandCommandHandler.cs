using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Services.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Constants;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Brands.UpdateBrand;

public sealed class UpdateBrandCommandHandler
    : IRequestHandler<UpdateBrandCommand, Result>
{
    private readonly IBrandRepository _repository;
    private readonly IBrandService _brandService;
    private readonly ICacheService _cacheService;
    private readonly IAuditService _auditService;

    public UpdateBrandCommandHandler(
        IBrandRepository repository, 
        IBrandService brandService,
        ICacheService cacheService,
        IAuditService auditService)
    {
        _repository = repository;
        _brandService = brandService;
        _cacheService = cacheService;
        _auditService = auditService;
    }

    public async Task<Result> Handle(
        UpdateBrandCommand request,
        CancellationToken cancellationToken)
    {
        var brandResult = await _brandService.GetRequiredAsync(
            request.Id,
            cancellationToken);

        if (!brandResult.IsSuccess)
        {
            return Result.Failure(brandResult.Error!);
        }

        var brand = brandResult.Value!;

        var exists = await _repository.ExistsByNameAsync(
            request.Name,
            request.Id,
            cancellationToken);

        if (exists)
        {
            return Result.Failure(
                BrandErrors.AlreadyExists);
        }

        brand.Update(
            request.Name,
            request.Description);

        await _repository.SaveChangesAsync(
            cancellationToken);

        await _auditService.LogAsync(
            AuditActions.Update,
            AuditEntities.Brand,
            brand.Id,
            $"Updated brand '{brand.Name}'.",
            cancellationToken);

        await _cacheService.RemoveByPrefixAsync(
          "brands",
          cancellationToken);

        return Result.Success(
            "Brand updated successfully.");
    }
}