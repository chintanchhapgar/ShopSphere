using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Carts.ClearCart;

public sealed record ClearCartCommand
    : IRequest<Result>;