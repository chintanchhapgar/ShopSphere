using FluentAssertions;
using Moq;
using ShopSphere.Application.Features.Brands.CreateBrand;
using ShopSphere.ApplicationTests.Common;
using ShopSphere.Domain.Entities;

namespace ShopSphere.ApplicationTests.Features.Brands;

public class CreateBrandCommandHandlerTests : HandlerTestBase
{
    private readonly CreateBrandCommandHandler _handler;

    public CreateBrandCommandHandlerTests()
    {
        _handler = new CreateBrandCommandHandler(
            BrandRepository.Object,
            CacheService.Object,
            AuditService.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_Brand()
    {
        // Arrange
        var command = new CreateBrandCommand(
            "Apple",
            "Apple Inc.");

        BrandRepository
            .Setup(x => x.ExistsByNameAsync(
                command.Name,
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        BrandRepository
            .Setup(x => x.AddOrRestoreAsync(
                It.IsAny<Brand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        BrandRepository.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        AuditService.Verify(
            x => x.LogAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Once);

        CacheService.Verify(
            x => x.RemoveByPrefixAsync(
                "brands",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BrandAlreadyExists()
    {
        // Arrange
        var command = new CreateBrandCommand(
            "Apple",
            null);

        BrandRepository
            .Setup(x => x.ExistsByNameAsync(
                command.Name,
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();

        BrandRepository.Verify(
            x => x.AddOrRestoreAsync(
                It.IsAny<Brand>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        BrandRepository.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Never);

        AuditService.Verify(
            x => x.LogAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);

        CacheService.Verify(
            x => x.RemoveByPrefixAsync(
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
}