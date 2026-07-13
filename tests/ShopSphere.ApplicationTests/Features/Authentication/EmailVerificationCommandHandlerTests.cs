using FluentAssertions;
using Moq;
using ShopSphere.Application.Features.Authentication.EmailVerification;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common.Errors;

namespace ShopSphere.ApplicationTests.Features.Authentication;

public sealed class EmailVerificationCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityService = new();
    private readonly Mock<IBackgroundJobService> _backgroundJobs = new();

    private readonly EmailVerificationCommandHandler _handler;

    public EmailVerificationCommandHandlerTests()
    {
        _handler = new EmailVerificationCommandHandler(
            _identityService.Object,
            _backgroundJobs.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Verification_Fails()
    {
        // Arrange
        var command = new EmailVerificationCommand(
            "john@test.com",
            "invalid-token");

        _identityService
            .Setup(x => x.VerifyEmailAsync(
                command.Email,
                command.Token))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();

        result.Error.Should()
            .Be(AuthenticationErrors.InvalidEmailVerificationToken);

        _identityService.Verify(
            x => x.GetUserIdByEmailAsync(
                It.IsAny<string>()),
            Times.Never);

        _backgroundJobs.Verify(
            x => x.Enqueue<IEmailJob>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IEmailJob, Task>>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_Verification_Succeeds_And_User_Not_Found()
    {
        // Arrange
        var command = new EmailVerificationCommand(
            "john@test.com",
            "valid-token");

        _identityService
            .Setup(x => x.VerifyEmailAsync(
                command.Email,
                command.Token))
            .ReturnsAsync(true);

        _identityService
            .Setup(x => x.GetUserIdByEmailAsync(
                command.Email))
            .ReturnsAsync((Guid?)null);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _backgroundJobs.Verify(
            x => x.Enqueue<IEmailJob>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IEmailJob, Task>>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Queue_Welcome_Email_When_User_Exists()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var command = new EmailVerificationCommand(
            "john@test.com",
            "valid-token");

        _identityService
            .Setup(x => x.VerifyEmailAsync(
                command.Email,
                command.Token))
            .ReturnsAsync(true);

        _identityService
            .Setup(x => x.GetUserIdByEmailAsync(
                command.Email))
            .ReturnsAsync(userId);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _backgroundJobs.Verify(
            x => x.Enqueue<IEmailJob>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IEmailJob, Task>>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_Message_When_Verification_Succeeds()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var command = new EmailVerificationCommand(
            "john@test.com",
            "valid-token");

        _identityService
            .Setup(x => x.VerifyEmailAsync(
                command.Email,
                command.Token))
            .ReturnsAsync(true);

        _identityService
            .Setup(x => x.GetUserIdByEmailAsync(
                command.Email))
            .ReturnsAsync(userId);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Message.Should()
            .Be("Email verified successfully.");
    }
}