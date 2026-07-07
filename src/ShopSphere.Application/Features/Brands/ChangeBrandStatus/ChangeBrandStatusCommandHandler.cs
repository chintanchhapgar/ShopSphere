using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Brands.ChangeBrandStatus;

public sealed class ChangeBrandStatusCommandHandler
    : IRequestHandler<ChangeBrandStatusCommand, Result>
{
    private readonly IBrandRepository _repository;

    public ChangeBrandStatusCommandHandler(
        IBrandRepository repository)
    {
        _repository = repository;
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

        brand.SetStatus(request.IsActive);

        await _repository.SaveChangesAsync(cancellationToken);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            $"Brand {(request.IsActive ? "activated" : "deactivated")} successfully.");
    }
}