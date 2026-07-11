using MediatR;
using Microsoft.Extensions.Logging;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Services.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Constants;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Products.CreateProduct;

public sealed class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryService _categoryService;
    private readonly IBrandService _brandService;
    private readonly ICacheService _cacheService;
    private readonly IAuditService _auditService;
    private readonly ILogger<CreateProductCommandHandler> _logger;  

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryService categoryService,
        IBrandService brandService,
        ICacheService cacheService,
        IAuditService auditService,
        ILogger<CreateProductCommandHandler> logger
        )
    {
        _productRepository = productRepository;
        _categoryService = categoryService;
        _brandService = brandService;
        _cacheService = cacheService;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var categoryValidation =
            await _categoryService.EnsureActiveAsync(
                request.CategoryId,
                cancellationToken);

                if (!categoryValidation.IsSuccess)
                {
                    return Result<Guid>.Failure(
                        categoryValidation.Error!);
                }

        var brandValidation =
            await _brandService.EnsureActiveAsync(
                request.BrandId,
                cancellationToken);

        if (!brandValidation.IsSuccess)
        {
            return Result<Guid>.Failure(
                brandValidation.Error!);
        }

        var exists = await _productRepository.ExistsBySkuAsync(
            request.SKU,
            null,
            cancellationToken);

        if (exists)
        {
            return Result<Guid>.Failure(
                ProductErrors.AlreadyExists);
        }

        var product = new Product(
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


        var added = await _productRepository.AddOrRestoreAsync(
            product,
            cancellationToken);

                if (!added)
                {
                    return Result<Guid>.Failure(
                        ProductErrors.AlreadyExists);
                }

        await _productRepository.SaveChangesAsync(
            cancellationToken);

        _logger.LogInformation(
            "Product {ProductId} created. Name: {Name}",
            product.Id,
            product.Name);

        await _auditService.LogAsync(
            AuditActions.Create,
            AuditEntities.Product,
            product.Id,
            $"Created product '{product.Name}'.",
            cancellationToken);

        await _cacheService.RemoveByPrefixAsync(
           "products",
           cancellationToken);

        return Result<Guid>.Success(
            product.Id,
            "Product created successfully.");
    }
}