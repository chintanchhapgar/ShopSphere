using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Inventories.AdjustInventory;

public sealed record AdjustInventoryCommand(
    Guid ProductId,
    int Quantity,
    string Reason)
    : IRequest<Result>;