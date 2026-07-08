using ShopSphere.Domain.Enums;

namespace ShopSphere.Domain.Entities;

public sealed class InventoryTransaction : AuditableEntity
{
    private InventoryTransaction()
    {
    }

    public InventoryTransaction(
        Guid inventoryId,
        int quantity,
        InventoryTransactionType transactionType,
        string reason,
        string? reference = null)
    {
        InventoryId = inventoryId;
        Quantity = quantity;
        TransactionType = transactionType;
        Reason = reason;
        Reference = reference;
    }

    public Guid InventoryId { get; private set; }

    public Inventory Inventory { get; private set; } = null!;

    public int Quantity { get; private set; }

    public InventoryTransactionType TransactionType { get; private set; }

    public string Reason { get; private set; } = null!;

    public string? Reference { get; private set; }
}