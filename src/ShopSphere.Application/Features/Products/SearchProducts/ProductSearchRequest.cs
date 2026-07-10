namespace ShopSphere.Application.Features.Products.SearchProducts;

public sealed record ProductSearchRequest(
    string? Search,
    Guid? CategoryId,
    Guid? BrandId,
    decimal? MinPrice,
    decimal? MaxPrice,
    bool? Featured,
    bool? InStock,
    ProductSortBy SortBy = ProductSortBy.Newest,
    int Page = 1,
    int PageSize = 20);