using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Payments.PaymentWebhook;

public sealed class PaymentWebhookCommandHandler
    : IRequestHandler<PaymentWebhookCommand, Result>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentService _paymentService;

    public PaymentWebhookCommandHandler(
        IPaymentRepository paymentRepository,
        IPaymentService paymentService)
    {
        _paymentRepository = paymentRepository;
        _paymentService = paymentService;
    }

    public async Task<Result> Handle(
        PaymentWebhookCommand request,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByGatewayReferenceAsync(
            request.TransactionId,
            cancellationToken);

        if (payment is null)
        {
            return Result.Failure(
                PaymentErrors.NotFound);
        }

        switch (request.Event.ToLowerInvariant())
        {
            case "payment.success":

                return await _paymentService.MarkPaymentSucceededAsync(
                    payment.Id,
                    request.TransactionId,
                    request.PaymentReference,
                    cancellationToken);

            case "payment.failed":

                return await _paymentService.MarkPaymentFailedAsync(
                    payment.Id,
                    "Gateway reported payment failure.",
                    cancellationToken);

            case "payment.refunded":

                return await _paymentService.RefundAsync(
                    payment.Id,
                    cancellationToken);

            default:

                return Result.Success(
                    "Webhook ignored.");
        }
    }
}