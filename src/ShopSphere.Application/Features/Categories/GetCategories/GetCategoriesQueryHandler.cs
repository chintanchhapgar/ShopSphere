using MediatR;
using ShopSphere.Application.Features.Categories.Common;
using ShopSphere.Application.Queries;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.Categories.GetCategories;

public sealed class GetCategoriesQueryHandler
    : IRequestHandler<GetCategoriesQuery, Result<IReadOnlyList<CategoryDto>>>
{
    private readonly ICategoryQueries _queries;

    public GetCategoriesQueryHandler(
        ICategoryQueries queries)
    {
        _queries = queries;
    }

    public async Task<Result<IReadOnlyList<CategoryDto>>> Handle(
        GetCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var categories = await _queries.GetAllAsync(cancellationToken);

        return Result<IReadOnlyList<CategoryDto>>
            .Success(categories, "Categories retrieved successfully.");
    }
}