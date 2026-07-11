using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Orders.CreateOrder;

public sealed record CreateOrderCommand(
    Guid AddressId)
    : IRequest<Result<CreateOrderResponse>>;