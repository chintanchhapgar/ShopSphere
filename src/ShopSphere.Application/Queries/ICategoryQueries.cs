using ShopSphere.Application.Features.Categories.Common;

namespace ShopSphere.Application.Queries;

public interface ICategoryQueries
{
    Task<IReadOnlyList<CategoryDto>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<CategoryDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);
}