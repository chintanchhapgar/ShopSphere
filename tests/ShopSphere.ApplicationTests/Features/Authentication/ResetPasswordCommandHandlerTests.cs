using FluentAssertions;
using Moq;
using ShopSphere.Application.Features.Authentication.ResetPassword;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common.Errors;

namespace ShopSphere.ApplicationTests.Features.Authentication;

public sealed class ResetPasswordCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityService = new();

    private readonly ResetPasswordCommandHandler _handler;

    public ResetPasswordCommandHandlerTests()
    {
        _handler = new ResetPasswordCommandHandler(
            _identityService.Object);
    }

    [Fact]
    public async Task Handle_Should_Reset_Password_Successfully()
    {
        // Arrange
        var command = new ResetPasswordCommand(
            "john@test.com",
            "token-123",
            "NewPassword@123");

        _identityService
            .Setup(x => x.ResetPasswordAsync(
                command.Email,
                command.Token,
                command.NewPassword))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Message.Should().Be("Password reset successfully.");

        _identityService.Verify(
            x => x.ResetPasswordAsync(
                command.Email,
                command.Token,
                command.NewPassword),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Reset_Fails()
    {
        // Arrange
        var command = new ResetPasswordCommand(
            "john@test.com",
            "invalid-token",
            "NewPassword@123");

        _identityService
            .Setup(x => x.ResetPasswordAsync(
                command.Email,
                command.Token,
                command.NewPassword))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();

        result.Error.Should().Be(
            AuthenticationErrors.InvalidPasswordResetToken);

        _identityService.Verify(
            x => x.ResetPasswordAsync(
                command.Email,
                command.Token,
                command.NewPassword),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Call_ResetPasswordAsync_Once()
    {
        // Arrange
        var command = new ResetPasswordCommand(
            "user@test.com",
            "token",
            "Password@123");

        _identityService
            .Setup(x => x.ResetPasswordAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        _identityService.Verify(
            x => x.ResetPasswordAsync(
                command.Email,
                command.Token,
                command.NewPassword),
            Times.Once);

        _identityService.VerifyNoOtherCalls();
    }
}