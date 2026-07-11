using MediatR;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Payments.RefundPayment;

public sealed class RefundPaymentCommandHandler
    : IRequestHandler<RefundPaymentCommand, Result>
{
    private readonly IPaymentService _paymentService;

    public RefundPaymentCommandHandler(
        IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    public Task<Result> Handle(
        RefundPaymentCommand request,
        CancellationToken cancellationToken)
    {
        return _paymentService.RefundAsync(
            request.PaymentId,
            cancellationToken);
    }
}