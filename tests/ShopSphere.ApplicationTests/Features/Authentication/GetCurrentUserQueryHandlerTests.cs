using FluentAssertions;
using Moq;
using ShopSphere.Application.Features.Authentication.Me;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common.Errors;

public sealed class GetCurrentUserQueryHandlerTests
{
    private readonly Mock<ICurrentUserService> _currentUser = new();
    private readonly Mock<IIdentityService> _identity = new();

    private readonly GetCurrentUserQueryHandler _handler;

    public GetCurrentUserQueryHandlerTests()
    {
        _handler = new GetCurrentUserQueryHandler(
            _currentUser.Object,
            _identity.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_UserId_Is_Null()
    {
        // Arrange
        _currentUser
            .Setup(x => x.UserId)
            .Returns((string?)null);

        // Act
        var result = await _handler.Handle(
            new GetCurrentUserQuery(),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();

        result.Error.Should().Be(AuthenticationErrors.Unauthorized);

        _identity.Verify(
            x => x.GetCurrentUserAsync(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_User_Not_Found()
    {
        // Arrange
        _currentUser
            .Setup(x => x.UserId)
            .Returns("user-1");

        _identity
            .Setup(x => x.GetCurrentUserAsync("user-1"))
            .ReturnsAsync((CurrentUserResponse?)null);

        // Act
        var result = await _handler.Handle(
            new GetCurrentUserQuery(),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();

        result.Error.Should().Be(UserErrors.NotFound);
    }
    [Fact]
    public async Task Handle_Should_Return_Current_User()
    {
        // Arrange
        _currentUser
            .Setup(x => x.UserId)
            .Returns("user-1");

        var response = new CurrentUserResponse(
            "user-1",
            "john@test.com",
            "John",
            "Doe",
            new List<string> { "Customer" });

        _identity
            .Setup(x => x.GetCurrentUserAsync("user-1"))
            .ReturnsAsync(response);

        // Act
        var result = await _handler.Handle(
            new GetCurrentUserQuery(),
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.Should().NotBeNull();

        result.Value!.Id.Should().Be("user-1");
        result.Value.Email.Should().Be("john@test.com");
        result.Value.FirstName.Should().Be("John");
        result.Value.LastName.Should().Be("Doe");
        result.Value.Roles.Should().ContainSingle();
        result.Value.Roles.Should().Contain("Customer");

        _identity.Verify(
            x => x.GetCurrentUserAsync("user-1"),
            Times.Once);
    }
}