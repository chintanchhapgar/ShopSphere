namespace ShopSphere.Contracts.Authentication;

public sealed record RegisterResult(
    bool Succeeded,
    IReadOnlyList<string> Errors);