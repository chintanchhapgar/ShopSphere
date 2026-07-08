using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Carts.RemoveCartItem;

public sealed record RemoveCartItemCommand(
    Guid ItemId)
    : IRequest<Result>;