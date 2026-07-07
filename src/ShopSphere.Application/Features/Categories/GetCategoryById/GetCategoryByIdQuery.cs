using MediatR;
using ShopSphere.Application.Features.Categories.Common;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Categories.GetCategoryById;

public sealed record GetCategoryByIdQuery(Guid Id)
    : IRequest<Result<CategoryDto>>;