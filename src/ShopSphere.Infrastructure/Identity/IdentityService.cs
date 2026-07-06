using Microsoft.AspNetCore.Identity;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.Authentication;
using ShopSphere.Domain.Constants;

namespace ShopSphere.Infrastructure.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenProvider _tokenProvider;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenProvider tokenProvider)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenProvider = tokenProvider;
    }

    public async Task<(bool Succeeded, IEnumerable<string> Errors)> RegisterAsync(
        string firstName,
        string lastName,
        string email,
        string password)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return (
                false,
                result.Errors.Select(e => e.Description));
        }

        await _userManager.AddToRoleAsync(user, Roles.Customer);

        return (true, Enumerable.Empty<string>());
    }

    public async Task<TokenResponse?> LoginAsync(
        string email,
        string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return null;

        var result = await _signInManager.CheckPasswordSignInAsync(
            user,
            password,
            false);

        if (!result.Succeeded)
            return null;

        var roles = await _userManager.GetRolesAsync(user);

        return await _tokenProvider.CreateAsync(
            user.Id,
            user.Email!,
            roles);
    }
}