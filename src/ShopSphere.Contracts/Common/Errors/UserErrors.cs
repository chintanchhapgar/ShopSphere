namespace ShopSphere.Contracts.Common.Errors;

public static class UserErrors
{
    public static readonly Error NotFound =
        new("USER_NOT_FOUND", "User not found.");

    public static readonly Error Unauthorized =
        new(
            ErrorCodes.Unauthorized,
            "You are not authorized to perform this action.");
}