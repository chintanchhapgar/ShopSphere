using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.ProductImages.SetPrimaryProductImage;

public sealed class SetPrimaryProductImageCommandHandler
    : IRequestHandler<SetPrimaryProductImageCommand, Result>
{
    private readonly IProductImageRepository _repository;

    public SetPrimaryProductImageCommandHandler(
        IProductImageRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        SetPrimaryProductImageCommand request,
        CancellationToken cancellationToken)
    {
        var image = await _repository.GetByIdAsync(
            request.ImageId,
            cancellationToken);

        if (image is null)
        {
            return Result.Failure(ProductImageErrors.NotFound);
        }

        if (image.ProductId != request.ProductId)
        {
            return Result.Failure(ProductImageErrors.NotFound);
        }

        await _repository.ClearPrimaryImageAsync(
            request.ProductId,
            cancellationToken);

        image.SetPrimary();

        await _repository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            "Primary product image updated successfully.");
    }
}