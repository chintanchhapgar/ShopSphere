using MediatR;
using ShopSphere.Application.Features.Categories.Common;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Categories.GetCategories;

public sealed class GetCategoriesQueryHandler
    : IRequestHandler<GetCategoriesQuery, Result<IReadOnlyList<CategoryDto>>>
{
    private readonly ICategoryRepository _repository;

    public GetCategoriesQueryHandler(
        ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<CategoryDto>>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await _repository.GetAllAsync(
            cancellationToken);

        var response = categories
            .Select(category => new CategoryDto(
                category.Id,
                category.Name,
                category.Description,
                category.ParentCategoryId,
                category.IsActive))
            .ToList();

        return Result<IReadOnlyList<CategoryDto>>.Success(
            response,
            "Categories retrieved successfully.");
    }
}