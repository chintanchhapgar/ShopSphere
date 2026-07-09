using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Notifications;
using ShopSphere.Infrastructure.Helpers;

namespace ShopSphere.Infrastructure.Services;

public sealed class NotificationService
    : INotificationService
{
    private readonly IEmailService _emailService;

    public NotificationService(
        IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task SendOrderPlacedAsync(
         OrderPlacedEmailModel model,
         CancellationToken cancellationToken = default)
    {
        var html = EmailTemplateReader.Read("OrderPlaced.html");

        html = html
            .Replace("{{CustomerName}}", model.CustomerName)
            .Replace("{{OrderNumber}}", model.OrderNumber)
            .Replace("{{TotalAmount}}", model.TotalAmount.ToString("0.00"));

        await _emailService.SendAsync(
            model.Email,
            "Your ShopSphere Order Confirmation",
            html,
            cancellationToken);
    }

    public Task SendPaymentSucceededAsync(
        Guid paymentId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SendShipmentCreatedAsync(
        Guid shipmentId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SendShipmentDeliveredAsync(
        Guid shipmentId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SendWelcomeEmailAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}