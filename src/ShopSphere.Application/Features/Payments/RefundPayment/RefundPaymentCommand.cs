using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Payments.RefundPayment;

public sealed record RefundPaymentCommand(
    Guid PaymentId)
    : IRequest<Result>;