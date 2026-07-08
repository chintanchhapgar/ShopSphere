using MediatR;
using ShopSphere.Application.Features.Inventory.Common;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Inventory.GetInventoryHistory;

public sealed class GetInventoryHistoryQueryHandler
    : IRequestHandler<GetInventoryHistoryQuery, Result<IReadOnlyList<InventoryTransactionDto>>>
{
    private readonly IInventoryTransactionQueries _queries;

    public GetInventoryHistoryQueryHandler(
        IInventoryTransactionQueries queries)
    {
        _queries = queries;
    }

    public async Task<Result<IReadOnlyList<InventoryTransactionDto>>> Handle(
        GetInventoryHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var history = await _queries.GetByProductIdAsync(
            request.ProductId,
            cancellationToken);

        return Result<IReadOnlyList<InventoryTransactionDto>>
            .Success(history);
    }
}