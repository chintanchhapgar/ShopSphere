using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Categories.UpdateCategory;

public sealed record UpdateCategoryCommand(
    string Name,
    string? Description,
    Guid? ParentCategoryId)
    : IRequest<Result>
{
    public Guid Id { get; init; }
}