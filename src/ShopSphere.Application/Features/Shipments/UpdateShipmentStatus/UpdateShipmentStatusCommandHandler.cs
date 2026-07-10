using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Notifications;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Enums;
using ShopSphere.Domain.Interfaces;
namespace ShopSphere.Application.Features.Shipments.UpdateShipmentStatus;

public sealed class UpdateShipmentStatusCommandHandler
    : IRequestHandler<UpdateShipmentStatusCommand, Result>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IUserService _userService;
    private readonly INotificationService _notificationService;

    public UpdateShipmentStatusCommandHandler(
        IShipmentRepository shipmentRepository,
        IUserService userService,
        INotificationService notificationService)
    {
        _shipmentRepository = shipmentRepository;
        _userService = userService;
        _notificationService = notificationService;
    }

    public async Task<Result> Handle(
        UpdateShipmentStatusCommand request,
        CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(
            request.ShipmentId,
            cancellationToken);

        if (shipment is null)
            return Result.Failure(
                ShipmentErrors.NotFound);

        switch (request.Status)
        {
            case ShipmentStatus.Processing:

                shipment.MarkProcessing();

                shipment.Order.StartProcessing();

                break;

            case ShipmentStatus.Shipped:

                shipment.MarkShipped(
                    request.TrackingNumber!,
                    request.Carrier!);

                shipment.Order.MarkShipped();

                break;

            case ShipmentStatus.Delivered:

                shipment.MarkDelivered();

                shipment.Order.MarkDelivered();

                break;

            case ShipmentStatus.Returned:

                shipment.MarkReturned();

                break;
        }

        await _shipmentRepository.SaveChangesAsync(
            cancellationToken);

        if (request.Status == ShipmentStatus.Delivered) {
            var user = await _userService.GetByIdAsync(
            shipment.Order.UserId.ToString(),
            cancellationToken);

            if (user is not null)
            {
                await _notificationService.SendShipmentDeliveredAsync(
                    new ShipmentDeliveredEmailModel(
                        user.FullName,
                        user.Email,
                        shipment.Order.OrderNumber,
                        shipment.TrackingNumber),
                    cancellationToken);
            }
        }

        return Result.Success(
            "Shipment updated successfully.");
    }
}