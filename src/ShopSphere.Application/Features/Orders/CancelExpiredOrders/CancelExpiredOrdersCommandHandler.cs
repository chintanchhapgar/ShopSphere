using MediatR;
using Microsoft.Extensions.Logging;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Orders.CancelExpiredOrders;

public sealed class CancelExpiredOrdersCommandHandler
    : IRequestHandler<CancelExpiredOrdersCommand, Result>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<CancelExpiredOrdersCommandHandler> _logger;

    public CancelExpiredOrdersCommandHandler(
        IOrderRepository orderRepository,
        ILogger<CancelExpiredOrdersCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task<Result> Handle(
        CancelExpiredOrdersCommand request,
        CancellationToken cancellationToken)
    {
        var cutoff = DateTime.UtcNow.AddMinutes(-30);

        var orders = await _orderRepository.GetExpiredPendingOrdersAsync(
            cutoff,
            cancellationToken);

        if (orders.Count == 0)
        {
            return Result.Success("No expired orders found.");
        }

        foreach (var order in orders)
        {
            if (!order.CanBeCancelled())
            {
                continue;
            }

            order.Cancel();

            _logger.LogInformation(
                "Order {OrderNumber} cancelled automatically.",
                order.OrderNumber);
        }

        await _orderRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(
            $"{orders.Count} expired orders cancelled.");
    }
}