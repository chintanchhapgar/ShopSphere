using ShopSphere.Contracts.Common;

public static class AuthenticationErrors
{
    public static readonly Error InvalidCredentials =
        new(
            "AUTH_INVALID_CREDENTIALS",
            "Invalid email or password.");

    public static readonly Error Forbidden =
        new(
            "AUTH_FORBIDDEN",
            "You are not authorized to perform this action.");
}