using ShopSphere.Application.Notifications;

namespace ShopSphere.Application.Interfaces;

public interface INotificationService
{
    Task SendOrderPlacedAsync(
    OrderPlacedEmailModel model,
    CancellationToken cancellationToken = default);

    Task SendPaymentSucceededAsync(
        Guid paymentId,
        CancellationToken cancellationToken = default);

    Task SendShipmentCreatedAsync(
        Guid shipmentId,
        CancellationToken cancellationToken = default);

    Task SendShipmentDeliveredAsync(
        Guid shipmentId,
        CancellationToken cancellationToken = default);

    Task SendWelcomeEmailAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

}