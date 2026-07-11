using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Constants;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Brands.CreateBrand;

public sealed class CreateBrandCommandHandler
    : IRequestHandler<CreateBrandCommand, Result<Guid>>
{
    private readonly IBrandRepository _repository;
    private readonly ICacheService _cacheService;
    private readonly IAuditService _auditService;

    public CreateBrandCommandHandler(
        IBrandRepository repository,
        ICacheService cacheService,
        IAuditService auditService)
    {
        _repository = repository;
        _cacheService = cacheService;
        _auditService = auditService;
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

        await _auditService.LogAsync(
            AuditActions.Create,
            AuditEntities.Brand,
            brand.Id,
            $"Created brand '{brand.Name}'.",
            cancellationToken);

        await _cacheService.RemoveByPrefixAsync(
          "brands",
          cancellationToken);

        return Result<Guid>.Success(
            brand.Id,
            "Brand created successfully.");
    }
}