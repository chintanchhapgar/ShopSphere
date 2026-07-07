using System;

namespace ShopSphere.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAtUtc { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public string? UpdatedBy { get; private set; }

    public bool IsActive { get; private set; } = true;

    public bool IsDeleted { get; private set; }

    public void SetCreated(
        DateTime createdAtUtc,
        string? createdBy)
    {
        CreatedAtUtc = createdAtUtc;
        CreatedBy = createdBy;

        IsActive = true;
        IsDeleted = false;
    }

    public void SetUpdated(
        DateTime updatedAtUtc,
        string? updatedBy)
    {
        UpdatedAtUtc = updatedAtUtc;
        UpdatedBy = updatedBy;
    }

    public void SoftDelete(
        DateTime deletedAtUtc,
        string? deletedBy)
    {
        IsDeleted = true;
        IsActive = false;

        UpdatedAtUtc = deletedAtUtc;
        UpdatedBy = deletedBy;
    }

    public void SetStatus(bool isActive)
    {
        IsActive = isActive;
    }
}