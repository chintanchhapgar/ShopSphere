using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Interfaces;
using ShopSphere.Infrastructure.Payments;
using ShopSphere.Infrastructure.Payments.DTOs;
using Stripe;
using System.Security.Claims;

namespace ShopSphere.Api.Endpoints;

public static class StripeEndpoints
{
    public static void MapStripeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/stripe").WithTags("Stripe");

        // ═══════════════════════════════════════════════════════════════════
        // POST /api/stripe/checkout/{orderId}
        // ═══════════════════════════════════════════════════════════════════
        group.MapPost("/checkout/{orderId:guid}", async (
            Guid orderId,
            IStripeService stripeService,
            IOrderRepository orderRepo,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = user.FindFirst(ClaimTypes.Email)?.Value ?? "customer@example.com";

            // Get order from database
            var order = await orderRepo.GetByIdAsync(orderId, ct);
            if (order is null)
            {
                return Results.NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Order not found"
                });
            }

            // Build line items from order
            var lineItems = order.Items.Select(item => new StripeLineItem(
                item.ProductName ?? "Product",
                null,
                item.UnitPrice,
                item.Quantity,
                null
            )).ToList();

            // Create Stripe checkout session
            var session = await stripeService.CreateCheckoutSessionAsync(
                orderId,
                order.OrderNumber ?? orderId.ToString(),
                order.TotalAmount,
                "inr",
                email,
                lineItems,
                ct);

            return Results.Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new
                {
                    sessionId = session.SessionId,
                    sessionUrl = session.SessionUrl,
                },
                Message = "Stripe session created"
            });
        })
        .RequireAuthorization()
        .WithName("CreateStripeCheckout");

        // ═══════════════════════════════════════════════════════════════════
        // POST /api/stripe/webhook
        // ═══════════════════════════════════════════════════════════════════
        group.MapPost("/webhook", async (
            HttpRequest request,
            IConfiguration config,
            ILogger<Program> logger) =>
        {
            var json = await new StreamReader(request.Body).ReadToEndAsync();

            try
            {
                var stripeSignature = request.Headers["Stripe-Signature"];
                var webhookSecret = config["Stripe:WebhookSecret"];

                if (string.IsNullOrEmpty(webhookSecret))
                {
                    logger.LogWarning("Stripe webhook secret not configured");
                    return Results.Ok();
                }

                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignature,
                    webhookSecret);

                logger.LogInformation("Stripe webhook: {EventType}", stripeEvent.Type);

                switch (stripeEvent.Type)
                {
                    case "checkout.session.completed":
                        var session = stripeEvent.Data.Object as Stripe.Checkout.Session;
                        logger.LogInformation(
                            "Payment completed for session {SessionId}",
                            session?.Id);
                        // TODO: Update order payment status in database
                        break;

                    case "payment_intent.succeeded":
                        var intent = stripeEvent.Data.Object as PaymentIntent;
                        logger.LogInformation(
                            "Payment succeeded for intent {IntentId}",
                            intent?.Id);
                        // TODO: Handle successful payment
                        break;

                    case "payment_intent.payment_failed":
                        var failedIntent = stripeEvent.Data.Object as PaymentIntent;
                        logger.LogWarning(
                            "Payment failed for intent {IntentId}",
                            failedIntent?.Id);
                        // TODO: Handle failed payment
                        break;
                }

                return Results.Ok();
            }
            catch (StripeException ex)
            {
                logger.LogError(ex, "Stripe webhook error");
                return Results.BadRequest();
            }
        });
    }
}