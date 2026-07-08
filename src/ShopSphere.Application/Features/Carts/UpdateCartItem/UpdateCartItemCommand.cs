using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Carts.UpdateCartItem;

public sealed record UpdateCartItemCommand(
    Guid ItemId,
    int Quantity)
    : IRequest<Result>;