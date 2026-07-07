using ShopSphere.Application.Features.Authentication.Me;
using ShopSphere.Contracts.Authentication;

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
}