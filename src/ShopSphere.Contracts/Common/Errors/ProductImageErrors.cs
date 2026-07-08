using ShopSphere.Contracts.Common;

namespace ShopSphere.Contracts.Errors;

public static class ProductImageErrors
{
    public static readonly Error EmptyFile =
        new(
            "PRODUCT_IMAGE_EMPTY_FILE",
            "The uploaded file is empty.");

    public static readonly Error FileTooLarge =
        new(
            "PRODUCT_IMAGE_FILE_TOO_LARGE",
            "The uploaded file exceeds the maximum allowed size.");

    public static readonly Error InvalidExtension =
        new(
            "PRODUCT_IMAGE_INVALID_EXTENSION",
            "The uploaded file type is not supported.");
}