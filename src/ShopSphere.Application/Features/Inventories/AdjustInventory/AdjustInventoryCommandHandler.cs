using MediatR;
using ShopSphere.Application.Features.Inventories.AdjustInventory;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Common.Errors;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Enums;
using ShopSphere.Domain.Interfaces;
using InventoryEntity = ShopSphere.Domain.Entities.Inventory;

namespace ShopSphere.Application.Features.Inventory.AdjustInventory;

public sealed class AdjustInventoryCommandHandler
    : IRequestHandler<AdjustInventoryCommand, Result>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IInventoryTransactionRepository _transactionRepository;

    public AdjustInventoryCommandHandler(
        IInventoryRepository inventoryRepository,
        IInventoryTransactionRepository transactionRepository)
    {
        _inventoryRepository = inventoryRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<Result> Handle(
        AdjustInventoryCommand request,
        CancellationToken cancellationToken)
    {
        var inventory = await _inventoryRepository.GetByProductIdAsync(
            request.ProductId,
            cancellationToken);

        var transactionType = InventoryTransactionType.Adjustment;

        if (inventory is null)
        {
            inventory = new InventoryEntity(
                request.ProductId,
                0,
                10);

            await _inventoryRepository.AddAsync(
                inventory,
                cancellationToken);

            transactionType = InventoryTransactionType.InitialStock;
        }

        inventory.AdjustStock(request.Quantity);

        var transaction = new InventoryTransaction(
            inventory.Id,
            request.Quantity,
            transactionType,
            request.Reason);

        var added = await _inventoryRepository.AddOrRestoreAsync(
             inventory,
             cancellationToken);

                if (!added)
                {
                    return Result<Guid>.Failure(
                        InventoryErrors.AlreadyExists);
                }

        await _inventoryRepository.SaveChangesAsync(
            cancellationToken);

        return Result.Success("Inventory updated successfully.");
    }
}