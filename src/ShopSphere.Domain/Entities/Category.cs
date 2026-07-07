using ShopSphere.Domain.Common;

namespace ShopSphere.Domain.Entities;

public sealed class Category : AuditableEntity
{

    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public Guid? ParentCategoryId { get; private set; }

    public Category? ParentCategory { get; private set; }

    public ICollection<Category> Children { get; private set; }
        = new List<Category>();

    public ICollection<Product> Products { get; private set; } = [];

    private Category()
    {
    }

    public Category(
        string name,
        string? description,
        Guid? parentCategoryId = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        ParentCategoryId = parentCategoryId;
    }

    public void Update(
    string name,
    string? description,
    Guid? parentCategoryId)
    {
        Name = name;
        Description = description;
        ParentCategoryId = parentCategoryId;
    }

    
}