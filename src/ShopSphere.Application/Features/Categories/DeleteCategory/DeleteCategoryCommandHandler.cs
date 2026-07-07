using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Categories.DeleteCategory;

public sealed class DeleteCategoryCommandHandler
    : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly ICategoryRepository _repository;

    public DeleteCategoryCommandHandler(
        ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        DeleteCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (category is null)
        {
            return Result.Failure(
                CategoryErrors.NotFound);
        }

        var hasChildren = await _repository.HasChildrenAsync(
            request.Id,
            cancellationToken);

        if (hasChildren)
        {
            return Result.Failure(
                CategoryErrors.HasChildCategories);
        }

        _repository.Remove(category);

        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success(
            "Category deleted successfully.");
    }
}