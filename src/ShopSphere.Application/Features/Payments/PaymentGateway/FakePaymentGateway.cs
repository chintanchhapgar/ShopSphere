using ShopSphere.Application.Features.Payments.PaymentGateway;
using ShopSphere.Application.Interfaces;

namespace ShopSphere.Application.Features.Payments.PaymentGateway;

public sealed class FakePaymentGateway
    : IPaymentGateway
{
    public Task<PaymentGatewayResponse> CreatePaymentAsync(
        PaymentGatewayRequest request,
        CancellationToken cancellationToken)
    {
        var response = new PaymentGatewayResponse
        {
            Success = true,
            TransactionId = Guid.NewGuid().ToString("N"),
            GatewayReference = $"FAKE-{Guid.NewGuid():N}"
        };

        return Task.FromResult(response);
    }

    public Task<bool> VerifyPaymentAsync(
        string transactionId,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(true);
    }

    public Task RefundAsync(
        string transactionId,
        CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}