using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Categories.DeleteCategory;

public sealed record DeleteCategoryCommand(Guid Id)
    : IRequest<Result>;