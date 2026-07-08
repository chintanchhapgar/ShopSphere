using Microsoft.EntityFrameworkCore;
using ShopSphere.Application.Features.Categories.Common;
using ShopSphere.Application.Queries;
using ShopSphere.Infrastructure.Persistence;

namespace ShopSphere.Infrastructure.Queries;

public sealed class CategoryQueries : ICategoryQueries
{
    private readonly ApplicationDbContext _context;

    public CategoryQueries(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new CategoryDto(
                x.Id,
                x.Name,
                x.Description,
                x.ParentCategoryId,
                x.IsActive))
            .ToListAsync(cancellationToken);
    }

    public async Task<CategoryDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new CategoryDto(
                x.Id,
                x.Name,
                x.Description,
                x.ParentCategoryId,
                x.IsActive))
            .FirstOrDefaultAsync(cancellationToken);
    }
}