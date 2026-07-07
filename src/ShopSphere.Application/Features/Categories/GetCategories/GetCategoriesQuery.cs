using MediatR;
using ShopSphere.Application.Features.Categories.Common;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Categories.GetCategories;

public sealed record GetCategoriesQuery
    : IRequest<Result<IReadOnlyList<CategoryDto>>>;