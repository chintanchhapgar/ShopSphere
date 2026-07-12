using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ShopSphere.Application.Features.Authentication.EmailVerification;
using ShopSphere.Application.Features.Authentication.Register;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Models;
using ShopSphere.Contracts.Common.Errors;
using System.Linq.Expressions;

public sealed class RegisterCommandHandlerTests
{
    private readonly Mock<IIdentityService> _identityService = new();
    private readonly Mock<IBackgroundJobService> _backgroundJobs = new();
    private readonly Mock<ILogger<RegisterCommandHandler>> _logger = new();

    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _handler = new RegisterCommandHandler(
            _identityService.Object,
            _backgroundJobs.Object,
            _logger.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_Registration_Fails()
    {
        // Arrange
        var command = new RegisterCommand(
            "John",
            "Doe",
            "john@test.com",
            "Password@123");

        _identityService
            .Setup(x => x.RegisterAsync(
                command.FirstName,
                command.LastName,
                command.Email,
                command.Password))
            .ReturnsAsync(new RegisterResult(
                false,
                null,
                new[] { "Email already exists" }));

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();

        result.Error.Should().Be(
            AuthenticationErrors.RegistrationFailed);

        _backgroundJobs.Verify(
            x => x.Enqueue<IEmailJob>(
                It.IsAny<Expression<Func<IEmailJob, Task>>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Queue_Email_Verification()
    {
        // Arrange
        var command = new RegisterCommand(
            "John",
            "Doe",
            "john@test.com",
            "Password@123");

        var userId = Guid.NewGuid();

        _identityService
            .Setup(x => x.RegisterAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(new RegisterResult(
                true,
                userId,
                Array.Empty<string>()));

        _identityService
            .Setup(x => x.GenerateEmailVerificationTokenAsync(
                command.Email))
            .ReturnsAsync(new EmailVerificationResult(
                userId,
                "verification-token"));

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _backgroundJobs.Verify(
            x => x.Enqueue<IEmailJob>(
                It.IsAny<Expression<Func<IEmailJob, Task>>>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Not_Queue_Email_When_Verification_Is_Null()
    {
        // Arrange
        var command = new RegisterCommand(
            "John",
            "Doe",
            "john@test.com",
            "Password@123");

        var userId = Guid.NewGuid();

        _identityService
            .Setup(x => x.RegisterAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(new RegisterResult(
                true,
                userId,
                Array.Empty<string>()));

        _identityService
            .Setup(x => x.GenerateEmailVerificationTokenAsync(
                command.Email))
            .ReturnsAsync((EmailVerificationResult?)null);

        // Act
        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        _backgroundJobs.Verify(
            x => x.Enqueue<IEmailJob>(
                It.IsAny<Expression<Func<IEmailJob, Task>>>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Call_RegisterAsync_Once()
    {
        // Arrange
        var command = new RegisterCommand(
            "John",
            "Doe",
            "john@test.com",
            "Password@123");

        var userId = Guid.NewGuid();

        _identityService
            .Setup(x => x.RegisterAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ReturnsAsync(new RegisterResult(
                true,
                userId,
                Array.Empty<string>()));

        _identityService
            .Setup(x => x.GenerateEmailVerificationTokenAsync(
                command.Email))
            .ReturnsAsync((EmailVerificationResult?)null);

        // Act
        await _handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        _identityService.Verify(
            x => x.RegisterAsync(
                command.FirstName,
                command.LastName,
                command.Email,
                command.Password),
            Times.Once);
    }

}