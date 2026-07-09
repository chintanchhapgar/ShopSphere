using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Orders.CancelOrder;

public sealed class CancelOrderCommandHandler
    : IRequestHandler<CancelOrderCommand, Result>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ICurrentUserService _currentUserService;

    public CancelOrderCommandHandler(
        IOrderRepository orderRepository,
        IInventoryRepository inventoryRepository,
        ICurrentUserService currentUserService)
    {
        _orderRepository = orderRepository;
        _inventoryRepository = inventoryRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(
        CancelOrderCommand request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(
                _currentUserService.UserId,
                out var userId))
        {
            return Result.Failure(
                UserErrors.Unauthorized);
        }

        var order = await _orderRepository.GetByIdWithItemsAsync(
            request.OrderId,
            cancellationToken);

        if (order is null)
        {
            return Result.Failure(
                OrderErrors.NotFound);
        }

        if (order.UserId != userId)
        {
            return Result.Failure(
                OrderErrors.NotFound);
        }

        if (!order.CanBeCancelled())
        {
            return Result.Failure(
                OrderErrors.CannotCancel);
        }

        var productIds = order.Items
            .Select(x => x.ProductId)
            .ToList();

        var inventories = await _inventoryRepository
            .GetByProductIdsAsync(
                productIds,
                cancellationToken);

        var inventoryLookup = inventories
            .ToDictionary(x => x.ProductId);

        foreach (var item in order.Items)
        {
            if (inventoryLookup.TryGetValue(
                    item.ProductId,
                    out var inventory))
            {
                inventory.IncreaseStock(
                    item.Quantity);
            }
        }

        order.Cancel();

        await _orderRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            "Order cancelled successfully.");
    }
}