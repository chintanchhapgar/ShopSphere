using Microsoft.AspNetCore.Identity;
using ShopSphere.Application.Interfaces;
using ShopSphere.Domain.Interfaces;
using ShopSphere.Infrastructure.Identity;

namespace ShopSphere.Infrastructure.BackgroundJobs.Jobs;

public sealed class EmailJob
    : IEmailJob
{
    private readonly IOrderRepository _orderRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateRenderer _templateRenderer;

    public EmailJob(
        IOrderRepository orderRepository,
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        IEmailTemplateRenderer templateRenderer)
    {
        _orderRepository = orderRepository;
        _userManager = userManager;
        _emailService = emailService;
        _templateRenderer = templateRenderer;
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
                ["FullName"] = user.LastName + " " + user.FirstName
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
        // TODO:
        // Load shipment
        // Load customer
        // Render ShipmentCreated.html
        // Send email

        await Task.CompletedTask;
    }

    public async Task SendOrderDeliveredAsync(
        Guid orderId)
    {
        // TODO:
        // Load order
        // Load customer
        // Render OrderDelivered.html
        // Send email

        await Task.CompletedTask;
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
                    ["FullName"] =
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
                    ["FullName"] =
                        $"{user.FirstName} {user.LastName}".Trim(),
                    ["VerifyLink"] = verifyLink
                });

        await _emailService.SendAsync(
            user.Email,
            "Verify your ShopSphere account",
            body);
    }    
}