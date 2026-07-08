using MediatR;
using Microsoft.Extensions.Options;
using ShopSphere.Application.Features.Products;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

using System.IO;

namespace ShopSphere.Application.Features.ProductImages.UploadProductImage;

public sealed class UploadProductImageCommandHandler
    : IRequestHandler<UploadProductImageCommand, Result<Guid>>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductImageRepository _productImageRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IFileValidationService _fileValidationService;

    public UploadProductImageCommandHandler(
    IProductRepository productRepository,
    IProductImageRepository productImageRepository,
    IFileStorageService fileStorageService,
    IFileValidationService fileValidationService)
    {
        _productRepository = productRepository;
        _productImageRepository = productImageRepository;
        _fileStorageService = fileStorageService;
        _fileValidationService = fileValidationService;
    }
        
    public async Task<Result<Guid>> Handle(
        UploadProductImageCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(
            request.ProductId,
            cancellationToken);

        if (product is null)
        {
            return Result<Guid>.Failure(ProductErrors.NotFound);
        }

        var validation = _fileValidationService.Validate(
            request.FileName,
            request.FileSize);

        if (!validation.IsSuccess)
        {
            return Result<Guid>.Failure(validation.Error!);
        }

        var imageUrl = await _fileStorageService.UploadAsync(
            request.FileStream,
            request.FileName,
            cancellationToken);

        if (request.IsPrimary)
        {
            await _productImageRepository.ClearPrimaryImageAsync(
                request.ProductId,
                cancellationToken);
        }

        var image = new ProductImage(
            request.ProductId,
            imageUrl,
            0,
            request.IsPrimary);

        await _productImageRepository.AddAsync(
            image,
            cancellationToken);

        await _productImageRepository.SaveChangesAsync(
            cancellationToken);

        return Result<Guid>.Success(
            image.Id,
            "Product image uploaded successfully.");
    }
}