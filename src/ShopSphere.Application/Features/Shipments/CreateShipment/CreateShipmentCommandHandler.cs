using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Shipments.CreateShipment;

public sealed class CreateShipmentCommandHandler
    : IRequestHandler<CreateShipmentCommand, Result<Guid>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IShipmentRepository _shipmentRepository;

    public CreateShipmentCommandHandler(
        IOrderRepository orderRepository,
        IShipmentRepository shipmentRepository)
    {
        _orderRepository = orderRepository;
        _shipmentRepository = shipmentRepository;
    }

    public async Task<Result<Guid>> Handle(
        CreateShipmentCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(
            request.OrderId,
            cancellationToken);

        if (order is null)
        {
            return Result<Guid>.Failure(
                ShipmentErrors.OrderNotFound);
        }

        var existingShipment =
            await _shipmentRepository.GetByOrderIdAsync(
                request.OrderId,
                cancellationToken);

        if (existingShipment is not null)
        {
            return Result<Guid>.Failure(
                ShipmentErrors.AlreadyExists);
        }

        var shipment = Shipment.Create(
            order.Id);

        await _shipmentRepository.AddAsync(
            shipment,
            cancellationToken);

        await _shipmentRepository.SaveChangesAsync(
            cancellationToken);

        return Result<Guid>.Success(
            shipment.Id,
            "Shipment created successfully.");
    }
}