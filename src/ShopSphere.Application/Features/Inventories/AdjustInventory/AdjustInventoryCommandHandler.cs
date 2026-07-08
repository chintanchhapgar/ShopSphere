using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Entities;

namespace ShopSphere.Application.Features.Inventories.AdjustInventory;

public sealed class AdjustInventoryCommandHandler
    : IRequestHandler<AdjustInventoryCommand, Result>
{
    private readonly IInventoryRepository _inventoryRepository;

    public AdjustInventoryCommandHandler(
        IInventoryRepository inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task<Result> Handle(
        AdjustInventoryCommand request,
        CancellationToken cancellationToken)
    {
        var inventory = await _inventoryRepository.GetByProductIdAsync(
            request.ProductId,
            cancellationToken);

        if (inventory is null)
        {
            inventory = new Inventory(
                request.ProductId,
                0,
                10);

            await _inventoryRepository.AddAsync(
                inventory,
                cancellationToken);
        }

        inventory.AdjustStock(
            request.Quantity);

        await _inventoryRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success(
            "Inventory updated successfully.");
    }
}