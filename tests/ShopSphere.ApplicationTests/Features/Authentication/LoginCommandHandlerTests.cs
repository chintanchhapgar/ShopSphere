using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ShopSphere.Application.Features.Authentication.Login;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Authentication;
using ShopSphere.Contracts.Common.Errors;

public sealed class LoginCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identity = new();
    private readonly Mock<ILogger<LoginCommandHandler>> _logger = new();

    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _handler = new LoginCommandHandler(
            _identity.Object,
            _logger.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Credentials_Are_Invalid()
    {
        // Arrange
        var command = new LoginCommand(
            "john@test.com",
            "WrongPassword");

        _identity
            .Setup(x => x.LoginAsync(
                command.Email,
                command.Password))
            .ReturnsAsync((TokenResponse?)null);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();

        result.Error.Should().Be(
            AuthenticationErrors.InvalidCredentials);

        _identity.Verify(
            x => x.LoginAsync(
                command.Email,
                command.Password),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Token_When_Login_Succeeds()
    {
        // Arrange
        var command = new LoginCommand(
            "john@test.com",
            "Password@123");

        var token = new TokenResponse(
            "jwt-access-token",
            DateTime.UtcNow.AddHours(1));

        _identity
            .Setup(x => x.LoginAsync(
                command.Email,
                command.Password))
            .ReturnsAsync(token);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Value.Should().NotBeNull();
        result.Value.Should().Be(token);

        _identity.Verify(
            x => x.LoginAsync(
                command.Email,
                command.Password),
            Times.Once);
    }
}