using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Brands.ChangeBrandStatus;

public sealed class ChangeBrandStatusCommandHandler
    : IRequestHandler<ChangeBrandStatusCommand, Result>
{
    private readonly IBrandRepository _repository;
    private readonly ICacheService _cacheService;
    public ChangeBrandStatusCommandHandler(
        IBrandRepository repository,
        ICacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
    }

    public async Task<Result> Handle(
        ChangeBrandStatusCommand request,
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

        if (request.IsActive)
        {
            brand.Activate();
        }
        else
        {
            brand.Deactivate();
        }

        await _repository.SaveChangesAsync(cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        await _cacheService.RemoveByPrefixAsync(
          "brands",
          cancellationToken);

        return Result.Success(
            $"Brand {(request.IsActive ? "activated" : "deactivated")} successfully.");
    }
}