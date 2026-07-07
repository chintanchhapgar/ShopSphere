using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Brands.DeleteBrand;

public sealed class DeleteBrandCommandHandler
    : IRequestHandler<DeleteBrandCommand, Result>
{
    private readonly IBrandRepository _repository;

    public DeleteBrandCommandHandler(
        IBrandRepository repository)
    {
        _repository = repository;
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

        return Result.Success(
            "Brand deleted successfully.");
    }
}