using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Notifications;
using ShopSphere.Infrastructure.Email.Helpers;

namespace ShopSphere.Infrastructure.Email.Services;

public sealed class EmailNotificationService
    : INotificationService
{
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateRenderer _renderer;
    public EmailNotificationService(
    IEmailService emailService,
    IEmailTemplateRenderer renderer)
    {
        _emailService = emailService;
        _renderer = renderer;
    }

    public async Task SendOrderPlacedAsync(
         OrderPlacedEmailModel model,
         CancellationToken cancellationToken = default)
    {
        var html = _renderer.Render(
            "OrderPlaced",
            new Dictionary<string, string>
            {
                ["CustomerName"] = model.CustomerName,
                ["OrderNumber"] = model.OrderNumber,
                ["TotalAmount"] = model.TotalAmount.ToString("N2")
            });

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

    public async Task SendWelcomeEmailAsync(
        WelcomeEmailModel model,
        CancellationToken cancellationToken = default)
    {
        var html = _renderer.Render(
            "Welcome",
            new Dictionary<string, string>
            {
                ["FullName"] = model.FullName
            });

        await _emailService.SendAsync(
            model.Email,
            "Welcome to ShopSphere!",
            html,
            cancellationToken);
    }
}