namespace ShopSphere.Domain.Entities;

public sealed class AuditLog : AuditableEntity
{
    private AuditLog()
    {
    }

    private AuditLog(
        Guid? userId,
        string? userName,
        string action,
        string entity,
        Guid? entityId,
        string description,
        string? ipAddress)
    {
        UserId = userId;
        UserName = userName;
        Action = action;
        Entity = entity;
        EntityId = entityId;
        Description = description;
        IpAddress = ipAddress;
    }

    public Guid? UserId { get; private set; }

    public string? UserName { get; private set; }

    public string Action { get; private set; } = default!;

    public string Entity { get; private set; } = default!;

    public Guid? EntityId { get; private set; }

    public string Description { get; private set; } = default!;

    public string? IpAddress { get; private set; }

    public static AuditLog Create(
        Guid? userId,
        string? userName,
        string action,
        string entity,
        Guid? entityId,
        string description,
        string? ipAddress)
    {
        return new AuditLog(
            userId,
            userName,
            action,
            entity,
            entityId,
            description,
            ipAddress);
    }
}