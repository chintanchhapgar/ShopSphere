using MediatR;
using Microsoft.Extensions.Logging;
using ShopSphere.Application.Features.Orders.CancelExpiredOrders;

namespace ShopSphere.Infrastructure.BackgroundJobs.Jobs;

public sealed class CancelExpiredOrdersJob
{
    private readonly IMediator _mediator;
    private readonly ILogger<CancelExpiredOrdersJob> _logger;

    public CancelExpiredOrdersJob(
        IMediator mediator,
        ILogger<CancelExpiredOrdersJob> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task ExecuteAsync()
    {
        var result = await _mediator.Send(new CancelExpiredOrdersCommand());

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