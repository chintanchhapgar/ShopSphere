using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using ShopSphere.Application.Features.Orders.CompleteDeliveredOrders;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;
using ShopSphere.Infrastructure.BackgroundJobs.Jobs;

namespace ShopSphere.ApplicationTests.Infrastructure.BackgroundJobs;

public sealed class CompleteDeliveredOrdersJobTests
{
    private readonly Mock<IMediator> _mediator = new();
    private readonly Mock<ILogger<CompleteDeliveredOrdersJob>> _logger = new();

    private readonly CompleteDeliveredOrdersJob _job;

    public CompleteDeliveredOrdersJobTests()
    {
        _job = new CompleteDeliveredOrdersJob(
            _mediator.Object,
            _logger.Object);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Send_Command()
    {
        // Arrange
        _mediator
            .Setup(x => x.Send(
                It.IsAny<CompleteDeliveredOrdersCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success("Completed"));

        // Act
        await _job.ExecuteAsync();

        // Assert
        _mediator.Verify(
            x => x.Send(
                It.IsAny<CompleteDeliveredOrdersCommand>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_LogInformation_When_Successful()
    {
        // Arrange
        _mediator
            .Setup(x => x.Send(
                It.IsAny<CompleteDeliveredOrdersCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success("Orders completed"));

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
                It.IsAny<CompleteDeliveredOrdersCommand>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure(
                Error.Validation("VALIDATION", "Job failed")));

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