using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Payments.PaymentWebhook;

public sealed record PaymentWebhookCommand(
    string TransactionId,
    string Event,
    string PaymentReference)
    : IRequest<Result>;