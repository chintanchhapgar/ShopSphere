using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Products;

public static class ProductErrors
{
    public static readonly Error NotFound =
        new(
            "PRODUCT_NOT_FOUND",
            "Product not found.");

    public static readonly Error AlreadyExists =
        new(
            "PRODUCT_ALREADY_EXISTS",
            "A product with the same SKU already exists.",
            "sku");
}