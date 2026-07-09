namespace ShopSphere.Application.Notifications;

public sealed record OrderPlacedEmailModel(
    string CustomerName,
    string Email,
    string OrderNumber,
    decimal TotalAmount);