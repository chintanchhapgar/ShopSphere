using MediatR;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Brands.GetBrands;

public sealed record GetBrandsQuery
    : IRequest<Result<IReadOnlyList<BrandResponse>>>;