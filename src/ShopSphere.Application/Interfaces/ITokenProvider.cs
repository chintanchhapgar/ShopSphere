using ShopSphere.Contracts.Authentication;

namespace ShopSphere.Application.Interfaces;

public interface ITokenProvider
{
    Task<TokenResponse> CreateAsync(
        string userId,
        string email,
        IList<string> roles);
}