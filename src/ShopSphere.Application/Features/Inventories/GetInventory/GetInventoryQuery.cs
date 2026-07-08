using MediatR;
using ShopSphere.Application.Features.Inventories.Common;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Inventories.GetInventory;

public sealed record GetInventoryQuery(
    Guid ProductId)
    : IRequest<Result<InventoryDto>>;