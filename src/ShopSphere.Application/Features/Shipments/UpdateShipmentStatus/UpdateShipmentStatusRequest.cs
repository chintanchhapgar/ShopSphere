using ShopSphere.Domain.Enums;

namespace ShopSphere.Application.Features.Shipments.UpdateShipmentStatus;

public sealed record UpdateShipmentStatusRequest(
    ShipmentStatus Status,
    string? TrackingNumber,
    string? Carrier);