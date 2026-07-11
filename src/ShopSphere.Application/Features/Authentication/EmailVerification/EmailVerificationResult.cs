namespace ShopSphere.Application.Features.Authentication.EmailVerification;

public sealed record EmailVerificationResult(
    Guid UserId,
    string Token);