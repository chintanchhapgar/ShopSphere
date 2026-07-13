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
        var html = await _renderer.RenderAsync(
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

    public async Task SendPaymentSucceededAsync(
         PaymentSucceededEmailModel model,
         CancellationToken cancellationToken = default)
    {
        var html = await _renderer.RenderAsync(
            "PaymentSucceeded",
            new Dictionary<string, string>
            {
                ["CustomerName"] = model.CustomerName,
                ["OrderNumber"] = model.OrderNumber,
                ["Amount"] = model.Amount.ToString("N2"),
                ["PaymentMethod"] = model.PaymentMethod,
                ["TransactionId"] = model.TransactionId
            });

        await _emailService.SendAsync(
            model.Email,
            "Payment Successful",
            html,
            cancellationToken);
    }

    public async Task SendShipmentCreatedAsync(
        ShipmentCreatedEmailModel model,
        CancellationToken cancellationToken = default)
    {
        var html = await _renderer.RenderAsync(
            "ShipmentCreated",
            new Dictionary<string, string>
            {
                ["CustomerName"] = model.CustomerName,
                ["OrderNumber"] = model.OrderNumber,
                ["TrackingNumber"] = model.TrackingNumber,
                ["Carrier"] = model.Carrier,
                ["EstimatedDelivery"] =
                    model.EstimatedDeliveryDate?.ToString("dd MMM yyyy") ?? "TBD"
            });

        await _emailService.SendAsync(
            model.Email,
            "Your Order Has Shipped",
            html,
            cancellationToken);
    }

    public async Task SendShipmentDeliveredAsync(
        ShipmentDeliveredEmailModel model,
        CancellationToken cancellationToken = default)
    {
        var html = await _renderer.RenderAsync(
            "ShipmentDelivered",
            new Dictionary<string, string>
            {
                ["CustomerName"] = model.CustomerName,
                ["OrderNumber"] = model.OrderNumber,
                ["TrackingNumber"] = model.TrackingNumber
            });

        await _emailService.SendAsync(
            model.Email,
            "Order Delivered",
            html,
            cancellationToken);
    }

    public async Task SendWelcomeEmailAsync(
        WelcomeEmailModel model,
        CancellationToken cancellationToken = default)
    {
        var html = await _renderer.RenderAsync(
            "Welcome",
            new Dictionary<string, string>
            {
                ["CustomerName"] = model.FullName
            });

        await _emailService.SendAsync(
            model.Email,
            "Welcome to ShopSphere!",
            html,
            cancellationToken);
    }
}