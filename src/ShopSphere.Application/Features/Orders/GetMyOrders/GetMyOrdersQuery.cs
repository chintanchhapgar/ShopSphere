using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Orders.GetMyOrders;

public sealed record GetMyOrdersQuery()
    : IRequest<Result<IReadOnlyList<OrderListItemDto>>>;