using MediatR;
using Microsoft.Extensions.Logging;
using ShopSphere.Application.Features.Orders.CompleteDeliveredOrders;

namespace ShopSphere.Infrastructure.BackgroundJobs.Jobs;

public sealed class CompleteDeliveredOrdersJob
{
    private readonly IMediator _mediator;
    private readonly ILogger<CompleteDeliveredOrdersJob> _logger;

    public CompleteDeliveredOrdersJob(
        IMediator mediator,
        ILogger<CompleteDeliveredOrdersJob> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var result = await _mediator.Send(
            new CompleteDeliveredOrdersCommand());

        if (result.IsSuccess)
        {
            _logger.LogInformation(result.Message);
        }
        else
        {
            _logger.LogWarning(result.Message);
        }
    }
}