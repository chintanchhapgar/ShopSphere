using MediatR;
using Microsoft.Extensions.Logging;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Services.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Constants;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Products.UpdateProduct;

public sealed class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IProductRepository _repository;
    private readonly ICategoryService _categoryService;
    private readonly IBrandService _brandService;
    private readonly ICacheService _cacheService;
    private readonly IAuditService _auditService;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(
        IProductRepository repository,
        ICategoryService categoryService,
        IBrandService brandService,
        ICacheService cacheService,
        IAuditService auditService,
        ILogger<UpdateProductCommandHandler> logger)
    {
        _repository = repository;
        _categoryService = categoryService;
        _brandService = brandService;
        _cacheService = cacheService;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<Result> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (product is null)
        {
            return Result.Failure(ProductErrors.NotFound);
        }

        var categoryValidation =
            await _categoryService.EnsureActiveAsync(
                request.CategoryId,
                cancellationToken);

        if (!categoryValidation.IsSuccess)
            return categoryValidation;

        var brandValidation =
            await _brandService.EnsureActiveAsync(
                request.BrandId,
                cancellationToken);

        if (!brandValidation.IsSuccess)
            return brandValidation;

        var exists = await _repository.ExistsBySkuAsync(
            request.SKU,
            request.Id,
            cancellationToken);

        if (exists)
        {
            return Result.Failure(
                ProductErrors.AlreadyExists);
        }

        product.Update(
            request.Name,
            request.Description,
            request.SKU,
            request.BasePrice,
            request.CostPrice,
            request.CategoryId,
            request.BrandId,
            request.Slug,
            request.Barcode,
            request.Weight);

        await _repository.SaveChangesAsync(
            cancellationToken);

        _logger.LogWarning(
            "Product {ProductId} deleted.",
            product.Id);

        await _auditService.LogAsync(
            AuditActions.Update,
            AuditEntities.Product,
            product.Id,
            $"Updated product '{product.Name}'.",
            cancellationToken);

        await _cacheService.RemoveByPrefixAsync(
            "products",
            cancellationToken);

        return Result.Success(
            "Product updated successfully.");
    }
}