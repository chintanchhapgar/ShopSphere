namespace ShopSphere.Application.Interfaces;

public interface IAuditService
{
    Task LogAsync(
        string action,
        string entity,
        Guid? entityId,
        string description,
        CancellationToken cancellationToken = default);
}