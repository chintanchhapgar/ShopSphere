using MediatR;
using ShopSphere.Application.Features.Inventories.Common;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;
using ShopSphere.Contracts.Errors;

namespace ShopSphere.Application.Features.Inventories.GetInventory;

public sealed class GetInventoryQueryHandler
    : IRequestHandler<GetInventoryQuery, Result<InventoryDto>>
{
    private readonly IInventoryQueries _queries;

    public GetInventoryQueryHandler(
        IInventoryQueries queries)
    {
        _queries = queries;
    }

    public async Task<Result<InventoryDto>> Handle(
        GetInventoryQuery request,
        CancellationToken cancellationToken)
    {
        var inventory = await _queries.GetByProductIdAsync(
            request.ProductId,
            cancellationToken);

        if (inventory is null)
        {
            return Result<InventoryDto>.Failure(
                InventoryErrors.NotFound);
        }

        return Result<InventoryDto>.Success(inventory);
    }
}