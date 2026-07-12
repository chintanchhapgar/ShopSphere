namespace ShopSphere.Application.Features.Authentication.Register;

public sealed record RegisterResponse(
    bool Succeeded,
    string? UserId,
    string Message);