using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Payments.PaymentFailed;

public sealed record PaymentFailedCommand(
    Guid PaymentId,
    string Reason)
    : IRequest<Result>;