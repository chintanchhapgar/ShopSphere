namespace ShopSphere.Application.Models;

public sealed record RegisterResult(
    bool Succeeded,
    Guid? UserId,
    IReadOnlyList<string> Errors);