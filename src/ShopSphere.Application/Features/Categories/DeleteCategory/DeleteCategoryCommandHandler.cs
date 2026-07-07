using MediatR;
using ShopSphere.Application.Services.Interfaces;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Categories.DeleteCategory;

public sealed class DeleteCategoryCommandHandler
    : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly ICategoryRepository _repository;
    private readonly ICategoryService _categoryService;
    public DeleteCategoryCommandHandler(
        ICategoryRepository repository, ICategoryService categoryService)
    {
        _repository = repository;
        _categoryService = categoryService;
    }

    public async Task<Result> Handle(
        DeleteCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var validation = await _categoryService.EnsureCanDeleteAsync(
        request.Id,
        cancellationToken);

            if (!validation.IsSuccess)
            {
                return validation;
            }

            var category = (await _categoryService.GetRequiredAsync(
                request.Id,
                cancellationToken)).Value!;

            category.Delete();

            await _repository.SaveChangesAsync(cancellationToken);

            return Result.Success("Category deleted successfully.");
    }
}