using FluentAssertions;
using Moq;
using ShopSphere.Application.Features.Categories.CreateCategory;
using ShopSphere.ApplicationTests.Common;
using ShopSphere.Domain.Entities;

namespace ShopSphere.ApplicationTests.Features.Categories;

public class CreateCategoryCommandHandlerTests : HandlerTestBase
{
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _handler = new CreateCategoryCommandHandler(
            CategoryRepository.Object,
            CacheService.Object,
            AuditService.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_Category()
    {
        var command = new CreateCategoryCommand(
            "Electronics",
            "Electronic products",
            null);

        CategoryRepository
            .Setup(x => x.ExistsByNameAsync(
                command.Name,
                null,
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        CategoryRepository
            .Setup(x => x.AddOrRestoreAsync(
                It.IsAny<Category>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        CategoryRepository.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }
}