using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Notifications;
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
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;

    public CreateShipmentCommandHandler(
        IOrderRepository orderRepository,
        IShipmentRepository shipmentRepository, 
        IUserService userService,
        INotificationService notificationService)
    {
        _orderRepository = orderRepository;
        _shipmentRepository = shipmentRepository;
        _userService = userService;
        _notificationService = notificationService;
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


        var user = await _userService.GetByIdAsync(
        order.UserId.ToString(),
        cancellationToken);

        if (user is not null)
        {
            await _notificationService.SendShipmentCreatedAsync(
                new ShipmentCreatedEmailModel(
                    user.FullName,
                    user.Email,
                    order.OrderNumber,
                    shipment.TrackingNumber,
                    shipment.Carrier,
                    shipment.CreatedAtUtc.AddDays(10)),
                cancellationToken);
        }

        return Result<Guid>.Success(
            shipment.Id,
            "Shipment created successfully.");
    }
}