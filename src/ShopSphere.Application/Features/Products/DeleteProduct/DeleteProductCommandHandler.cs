using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Products.DeleteProduct;

public sealed class DeleteProductCommandHandler
    : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IProductRepository _repository;

    public DeleteProductCommandHandler(
        IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (product is null)
        {
            return Result.Failure(
                ProductErrors.NotFound);
        }

        product.Delete();

        await _repository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            "Product deleted successfully.");
    }
}