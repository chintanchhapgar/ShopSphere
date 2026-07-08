namespace ShopSphere.Application.Features.Inventories.Common;

public sealed record InventoryDto(
    Guid ProductId,
    int QuantityOnHand,
    int ReservedQuantity,
    int AvailableQuantity,
    int LowStockThreshold,
    bool IsLowStock);