using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Enums;

namespace ShopSphere.Application.Interfaces;

public interface IPaymentService
{
    Task<Result<Guid>> CreatePaymentAsync(
        Guid orderId,
        PaymentMethod method,
        CancellationToken cancellationToken);

    Task<Result> MarkPaymentSucceededAsync(
        Guid paymentId,
        string transactionId,
        CancellationToken cancellationToken);

    Task<Result> MarkPaymentFailedAsync(
        Guid paymentId,
        string reason,
        CancellationToken cancellationToken);

    Task<Result> RefundAsync(
        Guid paymentId,
        CancellationToken cancellationToken);
}