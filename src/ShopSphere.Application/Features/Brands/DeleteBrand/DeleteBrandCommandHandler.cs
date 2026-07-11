using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Constants;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Brands.DeleteBrand;

public sealed class DeleteBrandCommandHandler
    : IRequestHandler<DeleteBrandCommand, Result>
{
    private readonly IBrandRepository _repository;
    private readonly ICacheService _cacheService;
    private readonly IAuditService _auditService;

    public DeleteBrandCommandHandler(
        IBrandRepository repository,
        ICacheService cacheService,
        IAuditService auditService)
    {
        _repository = repository;
        _cacheService = cacheService;
        _auditService = auditService;
    }

    public async Task<Result> Handle(
        DeleteBrandCommand request,
        CancellationToken cancellationToken)
    {
        var brand = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (brand is null)
        {
            return Result.Failure(
                BrandErrors.NotFound);
        }

        _repository.Remove(brand);

        await _repository.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync(
            AuditActions.Delete,
            AuditEntities.Brand,
            brand.Id,
            $"Deleted brand '{brand.Name}'.",
            cancellationToken);

        await _cacheService.RemoveByPrefixAsync(
          "brands",
          cancellationToken);

        return Result.Success(
            "Brand deleted successfully.");
    }
}