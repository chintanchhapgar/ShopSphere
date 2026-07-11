using ShopSphere.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopSphere.Application.Features.Payments.PaymentGateway
{
    public sealed class PaymentGatewayRequest
    {
        public Guid PaymentId { get; init; }

        public decimal Amount { get; init; }

        public string Currency { get; init; } = "INR";

        public PaymentMethod Method { get; init; }

        public string OrderNumber { get; init; } = string.Empty;

        public string CustomerEmail { get; init; } = string.Empty;
    }
}
