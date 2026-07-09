using MediatR;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;

namespace ShopSphere.Application.Features.Payments.GetPayment;

public sealed class GetPaymentQueryHandler
    : IRequestHandler<GetPaymentQuery, Result<PaymentDto>>
{
    private readonly IPaymentQueries _paymentQueries;

    public GetPaymentQueryHandler(
        IPaymentQueries paymentQueries)
    {
        _paymentQueries = paymentQueries;
    }

    public async Task<Result<PaymentDto>> Handle(
        GetPaymentQuery request,
        CancellationToken cancellationToken)
    {
        var payment = await _paymentQueries
            .GetByOrderIdAsync(
                request.OrderId,
                cancellationToken);

        if (payment is null)
        {
            return Result<PaymentDto>.Failure(
                PaymentErrors.NotFound);
        }

        return Result<PaymentDto>.Success(payment);
    }
}