using Microsoft.AspNetCore.Http;
using ShopSphere.Application.Interfaces;
using ShopSphere.Domain.Entities;
using ShopSphere.Infrastructure.Persistence;

namespace ShopSphere.Infrastructure.Services;

public sealed class AuditService : IAuditService
{
    private readonly ApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserService _userService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditService(
        ApplicationDbContext context,
        ICurrentUserService currentUserService,
        IUserService userService,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userService = userService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogAsync(
        string action,
        string entity,
        Guid? entityId,
        string description,
        CancellationToken cancellationToken = default)
    {
        Guid? userId = null;
        string? userName = null;

        if (Guid.TryParse(_currentUserService.UserId, out var id))
        {
            userId = id;

            var user = await _userService.GetByIdAsync(
                id.ToString(),
                cancellationToken);

            userName = user?.Email;
        }

        var ipAddress =
            _httpContextAccessor.HttpContext?
                .Connection
                .RemoteIpAddress?
                .ToString();

        var audit = AuditLog.Create(
            userId,
            userName,
            action,
            entity,
            entityId,
            description,
            ipAddress);

        await _context.AuditLogs.AddAsync(
            audit,
            cancellationToken);

        await _context.SaveChangesAsync(
            cancellationToken);
    }
}