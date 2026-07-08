namespace ShopSphere.Domain.Enums;

public enum InventoryTransactionType
{
    InitialStock = 1,
    Adjustment = 2,
    Reservation = 3,
    Release = 4,
    Sale = 5,
    Return = 6
}