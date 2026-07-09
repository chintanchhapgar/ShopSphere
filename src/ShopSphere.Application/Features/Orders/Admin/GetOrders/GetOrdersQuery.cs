using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Orders.Admin.GetOrders;

public sealed record GetOrdersQuery()
    : IRequest<Result<IReadOnlyList<AdminOrderListDto>>>;