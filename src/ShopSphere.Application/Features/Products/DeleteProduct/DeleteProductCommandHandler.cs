using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Constants;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Products.DeleteProduct;

public sealed class DeleteProductCommandHandler
    : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IProductRepository _repository;
    private readonly ICacheService _cacheService;
    private readonly IAuditService _auditService;
    public DeleteProductCommandHandler(
        IProductRepository repository,
        ICacheService cacheService,
        IAuditService auditService)
    {
        _repository = repository;
        _cacheService = cacheService;
        _auditService = auditService;
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

        await _auditService.LogAsync(
            AuditActions.Delete,
            AuditEntities.Product,
            product.Id,
            $"Deleted product '{product.Name}'.",
            cancellationToken);

        await _cacheService.RemoveByPrefixAsync(
           "products",
           cancellationToken);

        return Result.Success(
            "Product deleted successfully.");
    }
}