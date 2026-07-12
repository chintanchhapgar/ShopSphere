using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ShopSphere.Application.Features.Brands;
using ShopSphere.Application.Features.Products.CreateProduct;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Services.Interfaces;
using ShopSphere.ApplicationTests.Common;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Entities;

namespace ShopSphere.ApplicationTests.Features.Products;

public class CreateProductCommandHandlerTests : HandlerTestBase
{
    private readonly Mock<ICategoryService> _categoryService = new();
    private readonly Mock<IBrandService> _brandService = new();
    private readonly Mock<ILogger<CreateProductCommandHandler>> _logger = new();

    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _handler = new CreateProductCommandHandler(
            ProductRepository.Object,
            _categoryService.Object,
            _brandService.Object,
            CacheService.Object,
            AuditService.Object,
            _logger.Object);
    }

    private static CreateProductCommand CreateCommand() =>
        new(
            "iPhone 16",
            "Latest iPhone",
            "IPHONE-001",
            1000,
            800,
            Guid.NewGuid(),
            Guid.NewGuid(),
            "iphone-16",
            null,
            null);

    [Fact]
    public async Task Handle_Should_Create_Product()
    {
        var command = CreateCommand();

        _categoryService
            .Setup(x => x.EnsureActiveAsync(command.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _brandService
            .Setup(x => x.EnsureActiveAsync(command.BrandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        ProductRepository
            .Setup(x => x.ExistsBySkuAsync(command.SKU, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        ProductRepository
            .Setup(x => x.AddOrRestoreAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        ProductRepository.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        AuditService.Verify(x =>
            x.LogAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        CacheService.Verify(x =>
            x.RemoveByPrefixAsync(
                "products",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Category_Is_Invalid()
    {
        var command = CreateCommand();

        _categoryService
            .Setup(x => x.EnsureActiveAsync(command.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(CategoryErrors.NotFound));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();

        ProductRepository.Verify(x =>
            x.ExistsBySkuAsync(
                It.IsAny<string>(),
                null,
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Brand_Is_Invalid()
    {
        var command = CreateCommand();

        _categoryService
            .Setup(x => x.EnsureActiveAsync(command.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _brandService
            .Setup(x => x.EnsureActiveAsync(command.BrandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(BrandErrors.NotFound));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();

        ProductRepository.Verify(x =>
            x.ExistsBySkuAsync(
                It.IsAny<string>(),
                null,
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Sku_Already_Exists()
    {
        var command = CreateCommand();

        _categoryService
            .Setup(x => x.EnsureActiveAsync(command.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _brandService
            .Setup(x => x.EnsureActiveAsync(command.BrandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        ProductRepository
            .Setup(x => x.ExistsBySkuAsync(command.SKU, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();

        ProductRepository.Verify(x =>
            x.AddOrRestoreAsync(
                It.IsAny<Product>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AddOrRestore_Fails()
    {
        var command = CreateCommand();

        _categoryService
            .Setup(x => x.EnsureActiveAsync(command.CategoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        _brandService
            .Setup(x => x.EnsureActiveAsync(command.BrandId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        ProductRepository
            .Setup(x => x.ExistsBySkuAsync(command.SKU, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        ProductRepository
            .Setup(x => x.AddOrRestoreAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();

        ProductRepository.Verify(x =>
            x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);
    }
}