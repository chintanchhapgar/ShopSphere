using MediatR;
using Microsoft.Extensions.Logging;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Orders.CompleteDeliveredOrders;

public sealed class CompleteDeliveredOrdersCommandHandler
    : IRequestHandler<CompleteDeliveredOrdersCommand, Result>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<CompleteDeliveredOrdersCommandHandler> _logger;

    public CompleteDeliveredOrdersCommandHandler(
        IOrderRepository orderRepository,
        ILogger<CompleteDeliveredOrdersCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(
        CompleteDeliveredOrdersCommand request,
        CancellationToken cancellationToken)
    {
        var cutoff = DateTime.UtcNow.AddDays(-7);

        var orders =
            await _orderRepository.GetDeliveredOrdersForCompletionAsync(
                cutoff,
                cancellationToken);

        foreach (var order in orders)
        {
            if (!order.CanBeCompleted())
            {
                continue;
            }

            order.Complete();

            _logger.LogInformation(
                "Order {OrderNumber} completed automatically.",
                order.OrderNumber);
        }

        await _orderRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(
            $"{orders.Count} orders completed.");
    }
}