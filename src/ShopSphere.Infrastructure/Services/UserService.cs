using Microsoft.AspNetCore.Identity;
using ShopSphere.Application.Interfaces;
using ShopSphere.Infrastructure.Identity;

namespace ShopSphere.Infrastructure.Services;

public sealed class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(
        UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<UserInfo?> GetByIdAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(
            userId.ToString());

        if (user is null)
        {
            return null;
        }

        return new UserInfo(
            user.Id,
            $"{user.FirstName} {user.LastName}".Trim(),
            user.Email ?? string.Empty);
    }
}