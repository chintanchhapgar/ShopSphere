using ShopSphere.Contracts.Common;

namespace ShopSphere.Application.Features.Brands;

public static class BrandErrors
{
    public static readonly Error AlreadyExists =
        new(
            "BRAND_ALREADY_EXISTS",
            "A brand with the same name already exists.",
            "name");

    public static readonly Error NotFound =
        new(
            "BRAND_NOT_FOUND",
            "Brand not found.");

    public static readonly Error Inactive =
        new(
            "BRAND_INACTIVE",
            "Brand is inactive.");

}