using MediatR;
using ShopSphere.Application.Services.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Products.CreateProduct;

public sealed class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryService _categoryService;
    private readonly IBrandService _brandService;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryService categoryService,
        IBrandService brandService)
    {
        _productRepository = productRepository;
        _categoryService = categoryService;
        _brandService = brandService;
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
            request.BrandId);

        await _productRepository.AddAsync(
            product,
            cancellationToken);

        await _productRepository.SaveChangesAsync(
            cancellationToken);

        return Result<Guid>.Success(
            product.Id,
            "Product created successfully.");
    }
}