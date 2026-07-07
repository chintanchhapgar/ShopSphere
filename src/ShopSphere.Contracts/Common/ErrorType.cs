using ShopSphere.Contracts.Common;

public static class Errors
{
    public static readonly Error InvalidCredentials =
     new(
         "AUTH_INVALID_CREDENTIALS",
         "Invalid email or password.");

    public static readonly Error Forbidden =
        new(
            "AUTH_FORBIDDEN",
            "You are not authorized to perform this action.");

    public static readonly Error Validation =
        new(
            "VALIDATION_ERROR",
            "Validation failed.");

    public static readonly Error CategoryNotFound =
        new(
            "CATEGORY_NOT_FOUND",
            "Category not found.");

    public static readonly Error CategoryAlreadyExists =
        new(
            "CATEGORY_ALREADY_EXISTS",
            "Category already exists.");

    public static readonly Error InternalServerError =
        new(
            "INTERNAL_SERVER_ERROR",
            "An unexpected error occurred.");
}