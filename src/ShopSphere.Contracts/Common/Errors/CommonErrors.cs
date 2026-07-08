using ShopSphere.Contracts.Common;

namespace ShopSphere.Contracts.Errors;

public static class CommonErrors
{
    public static readonly Error Unauthorized =
        new(
            ErrorCodes.Unauthorized,
            "You are not authorized to perform this action.");

    public static readonly Error Forbidden =
        new(
            ErrorCodes.Forbidden,
            "You do not have permission to perform this action.");

    public static readonly Error NotFound =
        new(
            ErrorCodes.NotFound,
            "The requested resource was not found.");

    public static readonly Error ValidationFailed =
        new(
            ErrorCodes.Validation,
            "One or more validation errors occurred.");

    public static readonly Error InvalidOperation =
        new(
            ErrorCodes.Validation,
            "The requested operation is invalid.");

    public static readonly Error Unexpected =
        new(
            ErrorCodes.ServerError,
            "An unexpected error occurred.");
}