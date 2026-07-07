using MediatR;
using ShopSphere.Application.Features.Categories.Common;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Categories.GetCategoryById;

public sealed class GetCategoryByIdQueryHandler
    : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    private readonly ICategoryRepository _repository;

    public GetCategoryByIdQueryHandler(
        ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<CategoryDto>> Handle(
        GetCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var category = await _repository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (category is null)
        {
            return Result<CategoryDto>.Failure(
                CategoryErrors.NotFound);
        }

        var response = new CategoryDto(
            category.Id,
            category.Name,
            category.Description,
            category.ParentCategoryId,
            category.IsActive);

        return Result<CategoryDto>.Success(
            response,
            "Category retrieved successfully.");
    }
}