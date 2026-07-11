namespace ShopSphere.Application.Features.Payments.PaymentWebhook;

public sealed record PaymentWebhookRequest(
    string TransactionId,
    string Event,
    string PaymentReference);