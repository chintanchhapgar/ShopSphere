using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Enums;

namespace ShopSphere.Application.Features.Payments.CreatePayment;

public sealed record CreatePaymentCommand(
    Guid OrderId,
    PaymentMethod Method)
    : IRequest<Result<Guid>>;