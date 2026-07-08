using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.ProductImages.DeleteProductImage;

public sealed class DeleteProductImageCommandHandler
    : IRequestHandler<DeleteProductImageCommand, Result>
{
    private readonly IProductImageRepository _repository;
    private readonly IFileStorageService _fileStorageService;

    public DeleteProductImageCommandHandler(
        IProductImageRepository repository,
        IFileStorageService fileStorageService)
    {
        _repository = repository;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result> Handle(
        DeleteProductImageCommand request,
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

        await _fileStorageService.DeleteAsync(
            image.ImageUrl,
            cancellationToken);

        image.Delete();

        await _repository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            "Product image deleted successfully.");
    }
}