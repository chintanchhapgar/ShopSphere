using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.ProductImages.UpdateProductImageDisplayOrder;

public sealed class UpdateProductImageDisplayOrderCommandHandler
    : IRequestHandler<UpdateProductImageDisplayOrderCommand, Result>
{
    private readonly IProductImageRepository _repository;

    public UpdateProductImageDisplayOrderCommandHandler(
        IProductImageRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        UpdateProductImageDisplayOrderCommand request,
        CancellationToken cancellationToken)
    {
        var image = await _repository.GetByIdAsync(
            request.ImageId,
            cancellationToken);

        if (image is null ||
            image.ProductId != request.ProductId)
        {
            return Result.Failure(ProductImageErrors.NotFound);
        }

        image.SetDisplayOrder(
            request.DisplayOrder);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            "Display order updated successfully.");
    }
}