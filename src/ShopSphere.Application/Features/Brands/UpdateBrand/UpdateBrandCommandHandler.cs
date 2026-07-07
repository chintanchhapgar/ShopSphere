using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Brands.UpdateBrand;

public sealed class UpdateBrandCommandHandler
    : IRequestHandler<UpdateBrandCommand, Result>
{
    private readonly IBrandRepository _repository;

    public UpdateBrandCommandHandler(
        IBrandRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        UpdateBrandCommand request,
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

        return Result.Success(
            "Brand updated successfully.");
    }
}