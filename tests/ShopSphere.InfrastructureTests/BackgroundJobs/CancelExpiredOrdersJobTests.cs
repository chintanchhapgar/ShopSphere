using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using ShopSphere.Application.Features.Orders.CancelExpiredOrders;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;
using ShopSphere.Infrastructure.BackgroundJobs.Jobs;

namespace ShopSphere.InfrastructureTests.BackgroundJobs;

public sealed class CancelExpiredOrdersJobTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly Mock<ILogger<CancelExpiredOrdersJob>> _logger = new();

    private readonly CancelExpiredOrdersJob _job;

    public CancelExpiredOrdersJobTests()
    {
        _job = new CancelExpiredOrdersJob(
            _mediator.Object,
            _logger.Object);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Send_Command()
    {
        // Arrange
        _mediator
            .Setup(x => x.Send(
                It.IsAny<CancelExpiredOrdersCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success("Completed"));

        // Act
        await _job.ExecuteAsync();

        // Assert
        _mediator.Verify(
            x => x.Send(
                It.IsAny<CancelExpiredOrdersCommand>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_LogInformation_When_Successful()
    {
        // Arrange
        _mediator
            .Setup(x => x.Send(
                It.IsAny<CancelExpiredOrdersCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success("Orders cancelled"));

        // Act
        await _job.ExecuteAsync();

        // Assert
        _logger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_LogWarning_When_Failed()
    {
        // Arrange
        _mediator
            .Setup(x => x.Send(
                It.IsAny<CancelExpiredOrdersCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(JobErrors.JobFailed));

        // Act
        await _job.ExecuteAsync();

        // Assert
        _logger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}