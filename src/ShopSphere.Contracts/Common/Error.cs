namespace ShopSphere.Contracts.Common;

public sealed record Error(
    string Code,
    string Description);