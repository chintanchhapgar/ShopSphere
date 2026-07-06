using ShopSphere.Domain.Entities;

namespace ShopSphere.Domain.Interfaces;

public interface ICategoryRepository
{
    Task AddAsync(
        Category category,
        CancellationToken cancellationToken);

    Task<Category?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<List<Category>> GetAllAsync(
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);

    void Remove(Category category);
}