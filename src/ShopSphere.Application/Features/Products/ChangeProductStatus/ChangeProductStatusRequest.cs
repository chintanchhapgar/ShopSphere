namespace ShopSphere.Contracts.Products;

public sealed record ChangeProductStatusRequest(
    bool IsActive);