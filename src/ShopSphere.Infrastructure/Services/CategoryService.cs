using ShopSphere.Application.Services.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Infrastructure.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(
        ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Category>> GetRequiredAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var category = await _repository.GetByIdAsync(
            id,
            cancellationToken);

        if (category is null)
        {
            return Result<Category>.Failure(
                CategoryErrors.NotFound);
        }

        return Result<Category>.Success(category);
    }

    public async Task<Result> EnsureCanDeleteAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var category = await _repository.GetByIdAsync(
            id,
            cancellationToken);

        if (category is null)
        {
            return Result.Failure(
                CategoryErrors.NotFound);
        }

        var hasChildren = await _repository.HasChildrenAsync(
            id,
            cancellationToken);

        if (hasChildren)
        {
            return Result.Failure(
                CategoryErrors.HasChildCategories);
        }

        return Result.Success();
    }
}