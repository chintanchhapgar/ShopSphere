using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Orders.CompleteDeliveredOrders;

public sealed record CompleteDeliveredOrdersCommand
    : IRequest<Result>;