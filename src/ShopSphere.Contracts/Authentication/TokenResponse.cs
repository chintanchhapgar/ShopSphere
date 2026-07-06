namespace ShopSphere.Contracts.Authentication;

public sealed record TokenResponse(
    string AccessToken,
    DateTime ExpiresAt);