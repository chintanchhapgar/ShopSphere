using ShopSphere.Application.Features.Payments.GetPayment;

namespace ShopSphere.Application.Queries;

public interface IPaymentQueries
{
    Task<PaymentDto?> GetByOrderIdAsync(
        Guid orderId,
        CancellationToken cancellationToken);
}