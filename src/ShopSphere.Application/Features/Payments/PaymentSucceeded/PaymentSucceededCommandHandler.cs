using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Payments.PaymentSucceeded;

public sealed class PaymentSucceededCommandHandler
    : IRequestHandler<PaymentSucceededCommand, Result>
{
    private readonly IPaymentService _paymentService;

    public PaymentSucceededCommandHandler(
        IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public async Task<Result> Handle(
        PaymentSucceededCommand request,
        CancellationToken cancellationToken)
    {
        return await _paymentService.MarkPaymentSucceededAsync(
            request.PaymentId,
            request.TransactionId,
            cancellationToken);
    }
}