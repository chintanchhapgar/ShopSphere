using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Orders.CancelOrder;

public sealed record CancelOrderCommand(
    Guid OrderId)
    : IRequest<Result>;