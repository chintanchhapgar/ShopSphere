using MediatR;
using ShopSphere.Application.Features.Orders.CompleteDeliveredOrders;

namespace ShopSphere.Infrastructure.BackgroundJobs.Jobs;

public sealed class CompleteDeliveredOrdersJob
{
    private readonly IMediator _mediator;

    public CompleteDeliveredOrdersJob(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task ExecuteAsync()
    {
        await _mediator.Send(new CompleteDeliveredOrdersCommand());
    }
}