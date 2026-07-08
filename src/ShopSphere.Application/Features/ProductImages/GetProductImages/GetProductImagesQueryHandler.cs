using MediatR;
using ShopSphere.Contracts.Common;
using ShopSphere.Domain.Interfaces;

namespace ShopSphere.Application.Features.ProductImages.GetProductImages;

public sealed class GetProductImagesQueryHandler
    : IRequestHandler<GetProductImagesQuery, Result<IReadOnlyList<ProductImageDto>>>
{
    private readonly IProductImageRepository _repository;

    public GetProductImagesQueryHandler(
        IProductImageRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<ProductImageDto>>> Handle(
        GetProductImagesQuery request,
        CancellationToken cancellationToken)
    {
        var images = await _repository.GetByProductIdAsync(
            request.ProductId,
            cancellationToken);

        var result = images
            .Select(x => new ProductImageDto(
                x.Id,
                x.ImageUrl,
                x.DisplayOrder,
                x.IsPrimary))
            .ToList();

        return Result<IReadOnlyList<ProductImageDto>>.Success(result);
    }
}