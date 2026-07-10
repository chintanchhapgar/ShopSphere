namespace ShopSphere.Application.Notifications;

public sealed record ShipmentDeliveredEmailModel(
    string CustomerName,
    string Email,
    string OrderNumber,
    string TrackingNumber);