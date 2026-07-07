using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Application.Features.Brands.GetBrands;

namespace ShopSphere.Application.Features.Brands.GetBrandById;

public sealed record GetBrandByIdQuery(Guid Id)
    : IRequest<Result<BrandResponse>>;