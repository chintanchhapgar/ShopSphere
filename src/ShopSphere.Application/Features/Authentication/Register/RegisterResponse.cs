namespace ShopSphere.Application.Features.Authentication.Register;

public sealed record RegisterResponse(
    bool Success,
    string Message);