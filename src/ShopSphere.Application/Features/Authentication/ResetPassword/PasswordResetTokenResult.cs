namespace ShopSphere.Application.Models;

public sealed record PasswordResetTokenResult(
    Guid UserId,
    string Token);