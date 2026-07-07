using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Categories.UpdateCategory;

public sealed class UpdateCategoryCommandHandler
    : IRequestHandler<UpdateCategoryCommand, Result>
{
    private readonly ICategoryRepository _repository;

    public UpdateCategoryCommandHandler(
        ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        UpdateCategoryCommand request,
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

        var duplicateExists = await _repository.ExistsByNameAsync(
            request.Name,
            request.Id,
            cancellationToken);

        if (duplicateExists)
        {
            return Result.Failure(
                CategoryErrors.DuplicateName);
        }

        if (request.ParentCategoryId.HasValue)
        {
            var parentExists = await _repository.ExistsAsync(
                request.ParentCategoryId.Value,
                cancellationToken);

            if (!parentExists)
            {
                return Result.Failure(
                    CategoryErrors.ParentNotFound);
            }
        }

        category.Update(
            request.Name,
            request.Description,
            request.ParentCategoryId);

        await _repository.SaveChangesAsync(cancellationToken);

        return Result.Success(
            "Category updated successfully.");
    }
}