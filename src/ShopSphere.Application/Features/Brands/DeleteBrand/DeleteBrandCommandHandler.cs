using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Brands.DeleteBrand;

public sealed class DeleteBrandCommandHandler
    : IRequestHandler<DeleteBrandCommand, Result>
{
    private readonly IBrandRepository _repository;
    private readonly ICacheService _cacheService;

    public DeleteBrandCommandHandler(
        IBrandRepository repository,
        ICacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
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

        await _cacheService.RemoveByPrefixAsync(
          "brands",
          cancellationToken);

        return Result.Success(
            "Brand deleted successfully.");
    }
}