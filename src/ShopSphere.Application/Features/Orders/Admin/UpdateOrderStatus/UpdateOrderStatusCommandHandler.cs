using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Orders.Admin.UpdateOrderStatus;

public sealed class UpdateOrderStatusCommandHandler
    : IRequestHandler<UpdateOrderStatusCommand, Result>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IShipmentRepository _shipmentRepository;

    public UpdateOrderStatusCommandHandler(
    IOrderRepository orderRepository,
    IShipmentRepository shipmentRepository)
    {
        _orderRepository = orderRepository;
        _shipmentRepository = shipmentRepository;
    }

    public async Task<Result> Handle(
        UpdateOrderStatusCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(
            request.OrderId,
            cancellationToken);

        if (order is null)
        {
            return Result.Failure(
                OrderErrors.NotFound);
        }

        try
        {
            switch (request.Status)
            {
                case OrderStatus.Confirmed:

                    order.Confirm();

                    var existingShipment =
                        await _shipmentRepository.GetByOrderIdAsync(
                            order.Id,
                            cancellationToken);

                    if (existingShipment is null)
                    {
                        var shipment = Shipment.Create(order.Id);

                        await _shipmentRepository.AddAsync(
                            shipment,
                            cancellationToken);
                    }

                    break;

                case OrderStatus.Processing:
                    order.StartProcessing();
                    break;

                case OrderStatus.Shipped:
                    order.MarkShipped();
                    break;

                case OrderStatus.Delivered:
                    order.MarkDelivered();
                    break;

                case OrderStatus.Cancelled:
                    order.Cancel();
                    break;

                default:
                    return Result.Failure(
                        OrderErrors.InvalidStatus);
            }
        }
        catch (InvalidOperationException)
        {
            return Result.Failure(
                OrderErrors.InvalidStatus);
        }

        await _orderRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            "Order status updated successfully.");
    }
}