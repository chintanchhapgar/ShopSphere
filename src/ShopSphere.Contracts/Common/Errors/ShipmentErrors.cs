using ShopSphere.Contracts.Common;

namespace ShopSphere.Contracts.Errors;

public static class ShipmentErrors
{
    public static readonly Error OrderNotFound = new(
        "Shipment.OrderNotFound",
        "Order not found.");

    public static readonly Error AlreadyExists = new(
        "Shipment.AlreadyExists",
        "Shipment already exists for this order.");

    public static readonly Error NotFound = new(
        "Shipment.NotFound",
        "Shipment not found.");
}