using MediatR;
using Microsoft.Extensions.Logging;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Notifications;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Constants;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Shipments.CreateShipment;

public sealed class CreateShipmentCommandHandler
    : IRequestHandler<CreateShipmentCommand, Result<Guid>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IShipmentRepository _shipmentRepository; 
    private readonly IBackgroundJobService _backgroundJobs;
    private readonly IAuditService _auditService;
    private readonly ILogger<CreateShipmentCommandHandler> _logger;

    public CreateShipmentCommandHandler(
    IOrderRepository orderRepository,
    IShipmentRepository shipmentRepository,
    IBackgroundJobService backgroundJobs,
    IAuditService auditService,
    ILogger<CreateShipmentCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _shipmentRepository = shipmentRepository;
        _backgroundJobs = backgroundJobs;
        _auditService = auditService;
        _logger = logger;
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

        _logger.LogInformation(
            "Shipment {ShipmentId} created for Order {OrderNumber}.",
            shipment.Id,
            order.OrderNumber);

        await _auditService.LogAsync(
            AuditActions.ShipmentCreated,
            AuditEntities.Shipment,
            shipment.Id,
            $"Shipment created for order '{order.OrderNumber}'.",
            cancellationToken);


        //var user = await _userService.GetByIdAsync(
        //order.UserId.ToString(),
        //cancellationToken);

        //if (user is not null)
        //{
        //    await _notificationService.SendShipmentCreatedAsync(
        //        new ShipmentCreatedEmailModel(
        //            user.FullName,
        //            user.Email,
        //            order.OrderNumber,
        //            shipment.TrackingNumber,
        //            shipment.Carrier,
        //            shipment.CreatedAtUtc.AddDays(10)),
        //        cancellationToken);
        //}

        _backgroundJobs.Enqueue<IEmailJob>(
            x => x.SendShipmentCreatedAsync(
                shipment.Id));

        _logger.LogInformation(
            "Shipment creation email queued for Order {OrderNumber}.",
            order.OrderNumber);

        return Result<Guid>.Success(
            shipment.Id,
            "Shipment created successfully.");
    }
}