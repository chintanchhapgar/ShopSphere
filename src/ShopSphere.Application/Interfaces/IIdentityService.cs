using ShopSphere.Application.Features.Authentication.EmailVerification;
using ShopSphere.Application.Features.Authentication.Me;
using ShopSphere.Application.Models;
using ShopSphere.Contracts.Authentication;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Interfaces;

public interface IIdentityService
{
    Task<RegisterResult> RegisterAsync(
        string firstName,
        string lastName,
        string email,
        string password);

    Task<TokenResponse?> LoginAsync(
        string email,
        string password);

    Task<CurrentUserResponse?> GetCurrentUserAsync(
        string userId);

    Task<PasswordResetTokenResult?> GeneratePasswordResetTokenAsync(
        string email);

    Task<bool> ResetPasswordAsync(
        string email,
        string token,
        string newPassword);

    Task<EmailVerificationResult?> GenerateEmailVerificationTokenAsync(
        string email);

    Task<bool> VerifyEmailAsync(
        string email,
        string token);

    Task<Guid?> GetUserIdByEmailAsync(
    string email);
}