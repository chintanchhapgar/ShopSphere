using MediatR;
using ShopSphere.Application.Services.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Products.UpdateProduct;

public sealed class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IProductRepository _repository;
    private readonly ICategoryService _categoryService;
    private readonly IBrandService _brandService;

    public UpdateProductCommandHandler(
        IProductRepository repository,
        ICategoryService categoryService,
        IBrandService brandService)
    {
        _repository = repository;
        _categoryService = categoryService;
        _brandService = brandService;
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
            request.BrandId);

        await _repository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            "Product updated successfully.");
    }
}