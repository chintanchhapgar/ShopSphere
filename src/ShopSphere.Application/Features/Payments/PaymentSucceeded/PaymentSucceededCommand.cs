using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Payments.PaymentSucceeded;

public sealed record PaymentSucceededCommand(
    Guid PaymentId,
    string TransactionId)
    : IRequest<Result>;