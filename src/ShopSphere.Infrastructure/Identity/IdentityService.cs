using Microsoft.AspNetCore.Identity;
using ShopSphere.Application.Features.Authentication.EmailVerification;
using ShopSphere.Application.Features.Authentication.Me;
using ShopSphere.Application.Interfaces;
using ShopSphere.Application.Models;
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

    public async Task<RegisterResult> RegisterAsync(
    string firstName,
    string lastName,
    string email,
    string password)
    {
        var user = new ApplicationUser
        {
            FirstName = firstName,
            LastName = lastName,
            UserName = email,
            Email = email
        };

        var result = await _userManager.CreateAsync(
            user,
            password);

        if (!result.Succeeded)
        {
            return new RegisterResult(
                false,
                null,
                result.Errors
                    .Select(x => x.Description)
                    .ToList());
        }

        await _userManager.AddToRoleAsync(
            user,
            Roles.Customer);

        return new RegisterResult(
            true,
            Guid.Parse(user.Id),
            Array.Empty<string>());
    }

    public async Task<TokenResponse?> LoginAsync(
        string email,
        string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return null;

        if (!user.EmailConfirmed)
        {
            return null;
        }

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

    public async Task<CurrentUserResponse?> GetCurrentUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);

        return new CurrentUserResponse(
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            roles);
    }


    public async Task<PasswordResetTokenResult?> GeneratePasswordResetTokenAsync(
         string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return null;
        }

        var token =
            await _userManager.GeneratePasswordResetTokenAsync(
                user);

        return new PasswordResetTokenResult(
            Guid.Parse(user.Id),
            token);
    }

    public async Task<bool> ResetPasswordAsync(
        string email,
        string token,
        string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return false;
        }

        var result = await _userManager.ResetPasswordAsync(
            user,
            token,
            newPassword);

        return result.Succeeded;
    }

    public async Task<EmailVerificationResult?>
        GenerateEmailVerificationTokenAsync(
        string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return null;

        var token =
            await _userManager.GenerateEmailConfirmationTokenAsync(user);

        return new EmailVerificationResult(
            Guid.Parse(user.Id),
            token);
    }

    public async Task<bool> VerifyEmailAsync(
        string email,
        string token)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            return false;

        var result =
            await _userManager.ConfirmEmailAsync(
                user,
                token);

        return result.Succeeded;
    }

    public async Task<Guid?> GetUserIdByEmailAsync(
        string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return null;
        }

        return Guid.Parse(user.Id);
    }
}