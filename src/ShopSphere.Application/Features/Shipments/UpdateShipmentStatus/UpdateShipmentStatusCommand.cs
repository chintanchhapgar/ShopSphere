using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Enums;

namespace ShopSphere.Application.Features.Shipments.UpdateShipmentStatus;

public sealed record UpdateShipmentStatusCommand(
    Guid ShipmentId,
    ShipmentStatus Status,
    string? TrackingNumber,
    string? Carrier)
    : IRequest<Result>;