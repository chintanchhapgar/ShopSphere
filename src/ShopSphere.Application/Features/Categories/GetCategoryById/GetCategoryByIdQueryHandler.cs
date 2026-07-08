using MediatR;
using ShopSphere.Application.Features.Brands.GetBrands;
using ShopSphere.Application.Features.Categories.Common;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;
using ShopSphere.Contracts.Errors;
using ShopSphere.Domain.Entities;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Categories.GetCategoryById;

public sealed class GetCategoryByIdQueryHandler
    : IRequestHandler<GetCategoryByIdQuery, Result<CategoryDto>>
{
    private readonly ICategoryQueries _queries;

    public GetCategoryByIdQueryHandler(
        ICategoryQueries queries)
    {
        _queries = queries;
    }

    public async Task<Result<CategoryDto>> Handle(
        GetCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var category = await _queries.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (category is null)
        {
            return Result<CategoryDto>.Failure(
                CategoryErrors.NotFound);
        }

        return Result<CategoryDto>
           .Success(category, "Category retrieved successfully.");
    }
}