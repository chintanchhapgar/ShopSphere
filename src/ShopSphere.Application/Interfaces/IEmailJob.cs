namespace ShopSphere.Application.Interfaces;

public interface IEmailJob
{
    Task SendOrderConfirmationAsync(
        Guid orderId);

    Task SendShipmentCreatedAsync(
        Guid shipmentId);

    Task SendOrderDeliveredAsync(
        Guid orderId);

    Task SendWelcomeAsync(Guid userId);
}