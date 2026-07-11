using ShopSphere.Application.Features.Payments.PaymentGateway;

namespace ShopSphere.Application.Interfaces;

public interface IPaymentGateway
{
    Task<PaymentGatewayResponse> CreatePaymentAsync(
        PaymentGatewayRequest request,
        CancellationToken cancellationToken);

    Task<bool> VerifyPaymentAsync(
        string transactionId,
        CancellationToken cancellationToken);

    Task RefundAsync(
        string transactionId,
        CancellationToken cancellationToken);
}