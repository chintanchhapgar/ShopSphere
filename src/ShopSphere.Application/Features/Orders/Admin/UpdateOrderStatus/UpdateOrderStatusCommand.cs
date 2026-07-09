using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Enums;

namespace ShopSphere.Application.Features.Orders.Admin.UpdateOrderStatus;

public sealed record UpdateOrderStatusCommand(
    Guid OrderId,
    OrderStatus Status)
    : IRequest<Result>;