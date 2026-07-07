using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Brands.DeleteBrand;

public sealed record DeleteBrandCommand(Guid Id)
    : IRequest<Result>;