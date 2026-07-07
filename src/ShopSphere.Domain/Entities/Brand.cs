namespace ShopSphere.Domain.Entities;

public sealed class Brand : AuditableEntity
{
    public string Name { get; private set; }

    public string? Description { get; private set; }

    public ICollection<Product> Products { get; private set; } = [];

    private Brand()
    {
    }

    public Brand(
        string name,
        string? description)
    {
        Name = name.Trim();
        Description = description?.Trim();
    }

    public void Update(
        string name,
        string? description)
    {
        Name = name.Trim();
        Description = description?.Trim();
    }
}