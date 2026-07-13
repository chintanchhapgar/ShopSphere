using FluentAssertions;
using Moq;
using ShopSphere.Application.Features.Authentication.EmailVerification;
using ShopSphere.Application.Features.Authentication.ForgotPassword;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Models;
using ShopSphere.Contracts.Common.Errors;
using System.Linq.Expressions;

namespace ShopSphere.ApplicationTests.Features.Authentication;

public sealed class ForgotPasswordCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityService = new();
    private readonly Mock<IBackgroundJobService> _backgroundJobs = new();

    private readonly ForgotPasswordCommandHandler _handler;

    public ForgotPasswordCommandHandlerTests()
    {
        _handler = new ForgotPasswordCommandHandler(
            _identityService.Object,
            _backgroundJobs.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_Email_Not_Found()
    {
        // Arrange
        var command = new ForgotPasswordCommand("john@test.com");

        _identityService
            .Setup(x => x.GeneratePasswordResetTokenAsync(command.Email))
            .ReturnsAsync((PasswordResetTokenResult?)null);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Error.Should().BeNull();

        result.Message.Should().Be(
            "If an account exists with this email, a password reset link has been sent.");

        _backgroundJobs.Verify(
            x => x.Enqueue<IEmailJob>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IEmailJob, Task>>>()),
            Times.Never);

        _backgroundJobs.Verify(
            x => x.Enqueue<IEmailJob>(
                It.IsAny<Expression<Func<IEmailJob, Task>>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Queue_Reset_Email_When_Token_Generated()
    {
        // Arrange
        var command = new ForgotPasswordCommand("john@test.com");

        var token = new PasswordResetTokenResult(
            Guid.NewGuid(),
            "reset-token");

        _identityService
            .Setup(x => x.GeneratePasswordResetTokenAsync(command.Email))
            .ReturnsAsync(token);

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
    public async Task Handle_Should_ReturnSuccess_Message()
    {
        // Arrange
        var command = new ForgotPasswordCommand("john@test.com");

        _identityService
            .Setup(x => x.GeneratePasswordResetTokenAsync(command.Email))
            .ReturnsAsync(new PasswordResetTokenResult(
                Guid.NewGuid(),
                "token"));

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be(
    "If an account exists with this email, a password reset link has been sent.");
    }

    [Fact]
    public async Task Handle_Should_Call_GeneratePasswordResetToken_Once()
    {
        // Arrange
        var command = new ForgotPasswordCommand("john@test.com");

        _identityService
            .Setup(x => x.GeneratePasswordResetTokenAsync(command.Email))
            .ReturnsAsync(new PasswordResetTokenResult(
                Guid.NewGuid(),
                "token"));

        // Act
        await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        _identityService.Verify(
            x => x.GeneratePasswordResetTokenAsync(command.Email),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Enqueue_ResetPassword_Email_With_Correct_Expression()
    {
        // Arrange
        var token = new PasswordResetTokenResult(
            Guid.NewGuid(),
            "reset-token");

        _identityService
            .Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(token);

        // Act
        await _handler.Handle(
            new ForgotPasswordCommand("john@test.com"),
            CancellationToken.None);

        // Assert
        _backgroundJobs.Verify(
            x => x.Enqueue<IEmailJob>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IEmailJob, Task>>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_When_Reset_Token_Is_Generated()
    {
        // Arrange
        var token = new PasswordResetTokenResult(
            Guid.NewGuid(),
            "reset-token");

        _identityService
            .Setup(x => x.GeneratePasswordResetTokenAsync(
                "john@test.com"))
            .ReturnsAsync(token);

        var command = new ForgotPasswordCommand(
            "john@test.com");

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
    public async Task Handle_Should_Return_Failure_When_Email_Not_Found()
    {
        // Arrange
        _identityService
            .Setup(x => x.GeneratePasswordResetTokenAsync(
                "john@test.com"))
            .ReturnsAsync((PasswordResetTokenResult?)null);

        var command = new ForgotPasswordCommand(
            "john@test.com");

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        result.Error.Should().BeNull();

        result.Message.Should().Be(
            "If an account exists with this email, a password reset link has been sent.");

        _backgroundJobs.Verify(
            x => x.Enqueue<IEmailJob>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IEmailJob, Task>>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Queue_Reset_Email()
    {
        // Arrange
        var token = new PasswordResetTokenResult(
            Guid.NewGuid(),
            "reset-token");

        _identityService
            .Setup(x => x.GeneratePasswordResetTokenAsync(
                It.IsAny<string>()))
            .ReturnsAsync(token);

        // Act
        await _handler.Handle(
            new ForgotPasswordCommand("john@test.com"),
            CancellationToken.None);

        // Assert
        _backgroundJobs.Verify(
            x => x.Enqueue<IEmailJob>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IEmailJob, Task>>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Not_Queue_Email_When_Token_Generation_Fails()
    {
        // Arrange
        _identityService
            .Setup(x => x.GeneratePasswordResetTokenAsync(
                It.IsAny<string>()))
            .ReturnsAsync((PasswordResetTokenResult?)null);

        // Act
        await _handler.Handle(
            new ForgotPasswordCommand("john@test.com"),
            CancellationToken.None);

        // Assert
        _backgroundJobs.Verify(
            x => x.Enqueue<IEmailJob>(
                It.IsAny<System.Linq.Expressions.Expression<Func<IEmailJob, Task>>>()),
            Times.Never);
    }
}