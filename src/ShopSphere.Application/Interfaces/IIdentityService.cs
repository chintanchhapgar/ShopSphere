using ShopSphere.Contracts.Authentication;

namespace ShopSphere.Application.Interfaces;

public interface IIdentityService
{
    Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(
        string firstName,
        string lastName,
        string email,
        string password);

    Task<TokenResponse?> LoginAsync(
        string email,
        string password);
}