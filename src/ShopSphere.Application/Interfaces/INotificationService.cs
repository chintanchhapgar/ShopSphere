using ShopSphere.Application.Notifications;

namespace ShopSphere.Application.Interfaces;

public interface INotificationService
{
    Task SendOrderPlacedAsync(
    OrderPlacedEmailModel model,
    CancellationToken cancellationToken = default);

    Task SendPaymentSucceededAsync(
        PaymentSucceededEmailModel model,
        CancellationToken cancellationToken = default);

    Task SendShipmentCreatedAsync(
        ShipmentCreatedEmailModel model,
        CancellationToken cancellationToken = default);

    Task SendShipmentDeliveredAsync(
        ShipmentDeliveredEmailModel model,
        CancellationToken cancellationToken = default);

    Task SendWelcomeEmailAsync(
        WelcomeEmailModel model,
        CancellationToken cancellationToken = default);

}