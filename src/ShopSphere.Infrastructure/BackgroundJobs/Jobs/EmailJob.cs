using Microsoft.AspNetCore.Identity;
using ShopSphere.Application.Interfaces;
using ShopSphere.Domain.Interfaces;
using ShopSphere.Infrastructure.Identity;
using ShopSphere.Infrastructure.Persistence.Repositories;

namespace ShopSphere.Infrastructure.BackgroundJobs.Jobs;

public sealed class EmailJob
    : IEmailJob
{
    private readonly IOrderRepository _orderRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateRenderer _templateRenderer;
    private readonly IShipmentRepository _shipmentRepository;

    public EmailJob(
        IOrderRepository orderRepository,
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        IEmailTemplateRenderer templateRenderer,
        IShipmentRepository shipmentRepository)
    {
        _orderRepository = orderRepository;
        _userManager = userManager;
        _emailService = emailService;
        _templateRenderer = templateRenderer;
        _shipmentRepository = shipmentRepository;
    }

    public async Task SendWelcomeAsync(
        Guid userId)
    {
        var user = await _userManager.FindByIdAsync(
            userId.ToString());

        if (user is null ||
            string.IsNullOrWhiteSpace(user.Email))
        {
            return;
        }

        var body = await _templateRenderer.RenderAsync(
            "Welcome",
            new Dictionary<string, string>
            {
                ["CustomerName"] = $"{user.FirstName} {user.LastName}".Trim()
            });

        await _emailService.SendAsync(
            user.Email,
            "Welcome to ShopSphere",
            body);
    }

    public async Task SendOrderConfirmationAsync(
        Guid orderId)
    {
        var order = await _orderRepository.GetByIdWithDetailsAsync(
            orderId,
            CancellationToken.None);

        if (order is null)
        {
            return;
        }

        var customer = await _userManager.FindByIdAsync(
            order.UserId.ToString());

        if (customer is null ||
            string.IsNullOrWhiteSpace(customer.Email))
        {
            return;
        }

        var body = await _templateRenderer.RenderAsync(
            "OrderPlaced",
            new Dictionary<string, string>
            {
                ["CustomerName"] = customer.FirstName,
                ["OrderNumber"] = order.OrderNumber,
                ["TotalAmount"] = order.TotalAmount.ToString("N2")
            });

        await _emailService.SendAsync(
            customer.Email,
            $"Order Confirmation - {order.OrderNumber}",
            body);
    }

    public async Task SendShipmentCreatedAsync(
     Guid shipmentId)
    {
        var shipment =
            await _shipmentRepository.GetByIdWithDetailsAsync(
                shipmentId,
                CancellationToken.None);

        if (shipment is null)
        {
            return;
        }

        var customer =
            await _userManager.FindByIdAsync(
                shipment.Order.UserId.ToString());

        if (customer is null ||
            string.IsNullOrWhiteSpace(customer.Email))
        {
            return;
        }

        var body =
            await _templateRenderer.RenderAsync(
                "ShipmentCreated",
                new Dictionary<string, string>
                {
                    ["CustomerName"] =
                        $"{customer.FirstName} {customer.LastName}".Trim(),

                    ["OrderNumber"] =
                        shipment.Order.OrderNumber,

                    ["TrackingNumber"] =
                        shipment.TrackingNumber ?? "Pending",

                    ["Carrier"] =
                        shipment.Carrier ?? "Not Assigned",

                    ["EstimatedDelivery"] =
                        shipment.CreatedAtUtc
                            .AddDays(7)
                            .ToString("dd MMM yyyy")
                });

        await _emailService.SendAsync(
            customer.Email,
            $"Shipment Created - {shipment.Order.OrderNumber}",
            body);
    }

    public async Task SendOrderDeliveredAsync(
        Guid orderId)
    {
        var order =
            await _orderRepository.GetByIdWithDetailsAsync(
                orderId,
                CancellationToken.None);

        if (order is null)
        {
            return;
        }

        var customer =
            await _userManager.FindByIdAsync(
                order.UserId.ToString());

        if (customer is null ||
            string.IsNullOrWhiteSpace(customer.Email))
        {
            return;
        }

        var shipment =
            await _shipmentRepository.GetByOrderIdAsync(
                orderId,
                CancellationToken.None);

        var body =
            await _templateRenderer.RenderAsync(
                "ShipmentDelivered",
                new Dictionary<string, string>
                {
                    ["CustomerName"] =
                        $"{customer.FirstName} {customer.LastName}".Trim(),

                    ["OrderNumber"] =
                        order.OrderNumber,

                    ["TrackingNumber"] =
                        shipment?.TrackingNumber ?? "-"
                });

        await _emailService.SendAsync(
            customer.Email,
            $"Order Delivered - {order.OrderNumber}",
            body);
    }

    public async Task SendPasswordResetAsync(
        Guid userId,
        string token)
    {
        var user = await _userManager.FindByIdAsync(
            userId.ToString());

        if (user is null ||
            string.IsNullOrWhiteSpace(user.Email))
        {
            return;
        }

        var resetLink =
            $"https://localhost:7065/reset-password?email={Uri.EscapeDataString(user.Email)}&token={Uri.EscapeDataString(token)}";

        var body =
            await _templateRenderer.RenderAsync(
                "PasswordReset",
                new Dictionary<string, string>
                {
                    ["CustomerName"] =
                        $"{user.FirstName} {user.LastName}".Trim(),
                    ["ResetLink"] = resetLink
                });

        await _emailService.SendAsync(
            user.Email,
            "Reset your ShopSphere password",
            body);
    }

    public async Task SendEmailVerificationAsync(
    Guid userId,
    string token)
    {
        var user =
            await _userManager.FindByIdAsync(
                userId.ToString());

        if (user is null ||
            string.IsNullOrWhiteSpace(user.Email))
        {
            return;
        }

        var verifyLink =
            $"http://localhost:5173/verify-email" +
            $"?email={Uri.EscapeDataString(user.Email)}" +
            $"&token={Uri.EscapeDataString(token)}";

        var body =
            await _templateRenderer.RenderAsync(
                "VerifyEmail",
                new Dictionary<string, string>
                {
                    ["CustomerName"] =
                        $"{user.FirstName} {user.LastName}".Trim(),
                    ["VerifyLink"] = verifyLink
                });

        await _emailService.SendAsync(
            user.Email,
            "Verify your ShopSphere account",
            body);
    }    
}