using ShopSphere.Domain.Enums;

namespace ShopSphere.Domain.Entities;

public sealed class Shipment : AuditableEntity
{
    private Shipment()
    {
    }

    private Shipment(
        Guid orderId)
    {
        OrderId = orderId;
        Status = ShipmentStatus.Pending;
    }

    public Guid OrderId { get; private set; }

    public Order Order { get; private set; } = null!;

    public string? TrackingNumber { get; private set; }

    public string? Carrier { get; private set; }

    public ShipmentStatus Status { get; private set; }

    public DateTime? ShippedAtUtc { get; private set; }

    public DateTime? DeliveredAtUtc { get; private set; }


    public static Shipment Create(
        Guid orderId)
    {
        return new Shipment(orderId);
    }


    public void MarkProcessing()
    {
        Status = ShipmentStatus.Processing;
    }


    public void MarkShipped(
        string trackingNumber,
        string carrier)
    {
        TrackingNumber = trackingNumber;
        Carrier = carrier;
        Status = ShipmentStatus.Shipped;
        ShippedAtUtc = DateTime.UtcNow;
    }


    public void MarkDelivered()
    {
        Status = ShipmentStatus.Delivered;
        DeliveredAtUtc = DateTime.UtcNow;
    }


    public void MarkReturned()
    {
        Status = ShipmentStatus.Returned;
    }
}