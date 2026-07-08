using ShopSphere.Domain.Enums;

namespace ShopSphere.Application.Features.Inventory.Common;

public sealed record InventoryTransactionDto(
    Guid Id,
    int Quantity,
    InventoryTransactionType TransactionType,
    string Reason,
    string? Reference,
    DateTime CreatedAtUtc,
    string? CreatedBy);