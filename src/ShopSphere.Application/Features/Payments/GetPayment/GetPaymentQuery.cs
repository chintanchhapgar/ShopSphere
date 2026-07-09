using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Payments.GetPayment;

public sealed record GetPaymentQuery(
    Guid OrderId)
    : IRequest<Result<PaymentDto>>;