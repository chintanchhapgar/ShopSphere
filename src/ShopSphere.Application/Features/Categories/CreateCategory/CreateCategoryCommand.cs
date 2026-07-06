using MediatR;

namespace ShopSphere.Application.Features.Categories.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    string? Description,
    Guid? ParentCategoryId)
    : IRequest<Guid>;