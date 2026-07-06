namespace ShopSphere.Contracts.Common;

public static class Errors
{
    public static readonly Error Validation =
        new("Validation", "One or more validation errors occurred.");

    public static readonly Error Unauthorized =
        new("Unauthorized", "Authentication is required.");

    public static readonly Error Forbidden =
        new("Forbidden", "You are not allowed to perform this action.");

    public static readonly Error NotFound =
        new("NotFound", "The requested resource was not found.");

    public static readonly Error Conflict =
        new("Conflict", "The resource already exists.");

    public static readonly Error InternalServerError =
        new("ServerError", "An unexpected error occurred.");
}