using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Payments.PaymentFailed;

public sealed class PaymentFailedCommandHandler
    : IRequestHandler<PaymentFailedCommand, Result>
{
    private readonly IPaymentService _paymentService;

    public PaymentFailedCommandHandler(
        IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public Task<Result> Handle(
        PaymentFailedCommand request,
        CancellationToken cancellationToken)
    {
        return _paymentService.MarkPaymentFailedAsync(
            request.PaymentId,
            request.Reason,
            cancellationToken);
    }
}