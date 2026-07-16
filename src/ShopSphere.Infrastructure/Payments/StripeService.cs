using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShopSphere.Application.Interfaces;
using ShopSphere.Infrastructure.Payments.DTOs;
using Stripe;
using Stripe.Checkout;

namespace ShopSphere.Infrastructure.Payments;

// ═══════════════════════════════════════════════════════════════════════════
// IMPLEMENTATION
// ═══════════════════════════════════════════════════════════════════════════
public sealed class StripeService : IStripeService
{
    private readonly ILogger<StripeService> _logger;
    private readonly string _currency;
    private readonly string _clientUrl;

    public StripeService(IConfiguration config, ILogger<StripeService> logger)
    {
        var secretKey = config["Stripe:SecretKey"]
            ?? throw new InvalidOperationException("Stripe Secret Key missing in appsettings");

        _currency = config["Stripe:Currency"] ?? "inr";
        _clientUrl = config["App:ClientUrl"] ?? "http://localhost:3000";

        StripeConfiguration.ApiKey = secretKey;
        _logger = logger;
    }

    // ────────────────────────────────────────────────────────────────────────
    // Create Stripe Checkout Session (Hosted Payment Page)
    // ────────────────────────────────────────────────────────────────────────
    public async Task<CheckoutSessionResponse> CreateCheckoutSessionAsync(
        Guid orderId,
        string orderNumber,
        decimal amount,
        string currency,
        string customerEmail,
        List<StripeLineItem> items,
        CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation(
                "Creating Stripe session for order {OrderId}, amount: {Amount}",
                orderId, amount);

            // Build line items
            var lineItems = items.Select(item => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = (currency ?? _currency).ToLower(),
                    UnitAmount = (long)(item.UnitAmount * 100),  // Convert ₹ to paise
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Name,
                        Description = item.Description,
                        Images = !string.IsNullOrEmpty(item.ImageUrl)
                            ? new List<string> { item.ImageUrl }
                            : null,
                    },
                },
                Quantity = item.Quantity,
            }).ToList();

            // Session options
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = $"{_clientUrl}/orders/{orderId}?payment=success&session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{_clientUrl}/orders/{orderId}/payment?payment=cancelled",
                CustomerEmail = customerEmail,
                Metadata = new Dictionary<string, string>
                {
                    { "order_id",     orderId.ToString() },
                    { "order_number", orderNumber },
                },
                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    Metadata = new Dictionary<string, string>
                    {
                        { "order_id",     orderId.ToString() },
                        { "order_number", orderNumber },
                    },
                },
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options, cancellationToken: ct);

            _logger.LogInformation(
                "✅ Stripe session created: {SessionId} for order {OrderId}",
                session.Id, orderId);

            return new CheckoutSessionResponse(session.Id, session.Url);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex,
                "❌ Stripe session creation failed for order {OrderId}: {Message}",
                orderId, ex.Message);
            throw;
        }
    }

    // ────────────────────────────────────────────────────────────────────────
    // Create Payment Intent (for custom checkout)
    // ────────────────────────────────────────────────────────────────────────
    public async Task<PaymentIntentResponse> CreatePaymentIntentAsync(
        Guid orderId,
        decimal amount,
        string currency,
        string customerEmail,
        CancellationToken ct = default)
    {
        try
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100),
                Currency = (currency ?? _currency).ToLower(),
                ReceiptEmail = customerEmail,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
                Metadata = new Dictionary<string, string>
                {
                    { "order_id", orderId.ToString() },
                },
            };

            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options, cancellationToken: ct);

            return new PaymentIntentResponse(
                intent.ClientSecret,
                intent.Id,
                amount);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Payment intent creation failed");
            throw;
        }
    }

    // ────────────────────────────────────────────────────────────────────────
    // Confirm Payment
    // ────────────────────────────────────────────────────────────────────────
    public async Task<bool> ConfirmPaymentAsync(
        string paymentIntentId,
        CancellationToken ct = default)
    {
        try
        {
            var service = new PaymentIntentService();
            var intent = await service.GetAsync(paymentIntentId, cancellationToken: ct);
            return intent.Status == "succeeded";
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Payment confirmation failed");
            return false;
        }
    }

    // ────────────────────────────────────────────────────────────────────────
    // Refund Payment
    // ────────────────────────────────────────────────────────────────────────
    public async Task RefundPaymentAsync(
        string chargeId,
        CancellationToken ct = default)
    {
        try
        {
            var options = new RefundCreateOptions
            {
                Charge = chargeId
            };
            var service = new RefundService();
            await service.CreateAsync(options, cancellationToken: ct);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Refund failed");
            throw;
        }
    }
}