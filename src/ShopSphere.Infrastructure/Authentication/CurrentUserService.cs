using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ShopSphere.Application.Interfaces;

namespace ShopSphere.Infrastructure.Authentication;

public sealed class CurrentUserService(
    IHttpContextAccessor httpContextAccessor)
    : ICurrentUserService
{
    public string? UserId =>
        httpContextAccessor.HttpContext?
            .User?
            .FindFirstValue(ClaimTypes.NameIdentifier);

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}