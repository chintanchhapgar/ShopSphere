using MediatR;
using ShopSphere.Application.Features.Inventory.Common;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Inventory.GetInventoryHistory;

public sealed record GetInventoryHistoryQuery(
    Guid ProductId)
    : IRequest<Result<IReadOnlyList<InventoryTransactionDto>>>;