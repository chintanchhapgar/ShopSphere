using ShopSphere.Infrastructure.Payments.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ShopSphere.Infrastructure.Payments
{
    public interface IStripeService
    {
        Task<CheckoutSessionResponse> CreateCheckoutSessionAsync(
            Guid orderId,
            string orderNumber,
            decimal amount,
            string currency,
            string customerEmail,
            List<StripeLineItem> items,
            CancellationToken ct = default);

        Task<PaymentIntentResponse> CreatePaymentIntentAsync(
            Guid orderId,
            decimal amount,
            string currency,
            string customerEmail,
            CancellationToken ct = default);

        Task<bool> ConfirmPaymentAsync(
            string paymentIntentId,
            CancellationToken ct = default);

        Task RefundPaymentAsync(
            string chargeId,
            CancellationToken ct = default);
    }
}
