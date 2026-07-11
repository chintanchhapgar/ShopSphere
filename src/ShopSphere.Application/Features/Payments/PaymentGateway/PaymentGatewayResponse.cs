using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSphere.Application.Features.Payments.PaymentGateway
{
    public sealed class PaymentGatewayResponse
    {
        public bool Success { get; init; }

        public string TransactionId { get; init; } = string.Empty;

        public string GatewayReference { get; init; } = string.Empty;

        public string? ErrorMessage { get; init; }
    }
}
