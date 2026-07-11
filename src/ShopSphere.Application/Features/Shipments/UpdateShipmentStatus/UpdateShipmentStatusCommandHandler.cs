using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Notifications;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Constants;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Enums;
using ShopSphere.Domain.Interfaces;
namespace ShopSphere.Application.Features.Shipments.UpdateShipmentStatus;

public sealed class UpdateShipmentStatusCommandHandler
    : IRequestHandler<UpdateShipmentStatusCommand, Result>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IBackgroundJobService _backgroundJobs;
    private readonly IAuditService _auditService;
    public UpdateShipmentStatusCommandHandler(
        IShipmentRepository shipmentRepository,
        IBackgroundJobService backgroundJobs,
        IAuditService auditService)
    {
        _shipmentRepository = shipmentRepository;
        _backgroundJobs = backgroundJobs;
        _auditService = auditService;
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

        

        switch (request.Status)
        {

            case ShipmentStatus.Processing:

                await _auditService.LogAsync(
                    AuditActions.ShipmentProcessing,
                    AuditEntities.Shipment,
                    shipment.Id,
                    $"Shipment is now processing.",
                    cancellationToken);

                break;

            case ShipmentStatus.Shipped:

                await _auditService.LogAsync(
                    AuditActions.ShipmentShipped,
                    AuditEntities.Shipment,
                    shipment.Id,
                     $"Shipment dispatched via {shipment.Carrier} ({shipment.TrackingNumber}).",
                    cancellationToken);

                _backgroundJobs.Enqueue<IEmailJob>(
                    x => x.SendShipmentCreatedAsync(
                        shipment.Id));

                break;

            case ShipmentStatus.Delivered:

                await _auditService.LogAsync(
                    AuditActions.ShipmentDelivered,
                    AuditEntities.Shipment,
                    shipment.Id,
                    $"Shipment delivered successfully.",
                    cancellationToken);

                _backgroundJobs.Enqueue<IEmailJob>(
                    x => x.SendOrderDeliveredAsync(
                        shipment.OrderId));

                break;

            case ShipmentStatus.Returned:

                await _auditService.LogAsync(
                    AuditActions.ShipmentReturned,
                    AuditEntities.Shipment,
                    shipment.Id,
                    $"Shipment returned.",
                    cancellationToken);

                break;
        }

        //if (request.Status == ShipmentStatus.Delivered) {
        //    var user = await _userService.GetByIdAsync(
        //    shipment.Order.UserId.ToString(),
        //    cancellationToken);

        //    if (user is not null)
        //    {
        //        await _notificationService.SendShipmentDeliveredAsync(
        //            new ShipmentDeliveredEmailModel(
        //                user.FullName,
        //                user.Email,
        //                shipment.Order.OrderNumber,
        //                shipment.TrackingNumber),
        //            cancellationToken);
        //    }
        //}

        return Result.Success(
            "Shipment updated successfully.");
    }
}