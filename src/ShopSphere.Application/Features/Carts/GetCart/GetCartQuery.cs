using MediatR;
using ShopSphere.Application.Features.Carts.Common;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Carts.GetCart;

public sealed record GetCartQuery
    : IRequest<Result<CartDto>>;