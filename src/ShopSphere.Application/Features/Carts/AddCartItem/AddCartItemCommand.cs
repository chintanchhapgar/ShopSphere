using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Carts.AddCartItem;

public sealed record AddCartItemCommand(
    Guid ProductId,
    int Quantity)
    : IRequest<Result>;