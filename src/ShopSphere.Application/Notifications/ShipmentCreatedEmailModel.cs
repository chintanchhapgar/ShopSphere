namespace ShopSphere.Application.Notifications;

public sealed record ShipmentCreatedEmailModel(
    string CustomerName,
    string Email,
    string OrderNumber,
    string TrackingNumber,
    string Carrier,
    DateTime? EstimatedDeliveryDate);