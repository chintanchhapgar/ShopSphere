using ShopSphere.Domain.Enums;

namespace ShopSphere.Application.Features.Shipments.GetShipment;

public sealed record ShipmentDto(
    Guid Id,
    Guid OrderId,
    ShipmentStatus Status,
    string? TrackingNumber,
    string? Carrier,
    DateTime? ShippedAtUtc,
    DateTime? DeliveredAtUtc,
    DateTime CreatedAtUtc);