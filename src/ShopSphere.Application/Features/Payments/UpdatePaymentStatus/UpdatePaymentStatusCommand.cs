using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Enums;

namespace ShopSphere.Application.Features.Payments.UpdatePaymentStatus;

public sealed record UpdatePaymentStatusCommand(
    Guid PaymentId,
    PaymentStatus Status,
    string? TransactionId)
    : IRequest<Result>;