using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Orders.GetOrderById;

public sealed record GetOrderByIdQuery(Guid Id)
    : IRequest<Result<OrderDetailsDto>>;