using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Orders.CancelExpiredOrders;

public sealed record CancelExpiredOrdersCommand
    : IRequest<Result>;