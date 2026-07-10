public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAtUtc { get; private set; }

    public string? CreatedBy { get; private set; }

    public DateTime? UpdatedAtUtc { get; private set; }

    public string? UpdatedBy { get; private set; }

    public bool IsActive { get; protected set; } = true;

    public bool IsDeleted { get; private set; }

    public void SetCreated(
        DateTime createdAtUtc,
        string? createdBy)
    {
        CreatedAtUtc = createdAtUtc;
        CreatedBy = createdBy;
    }

    public void SetUpdated(
        DateTime updatedAtUtc,
        string? updatedBy)
    {
        UpdatedAtUtc = updatedAtUtc;
        UpdatedBy = updatedBy;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public virtual void Delete()
    {
        IsDeleted = true;
        IsActive = false;
    }

    public virtual void Restore()
    {
        IsDeleted = false;
        IsActive = true;
    }
}