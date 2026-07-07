namespace ShopSphere.Contracts.Common.Errors;

public static class AuthenticationErrors
{
    public static readonly Error InvalidCredentials =
        new("AUTH_INVALID_CREDENTIALS", "Invalid email or password.");

    public static readonly Error Unauthorized =
        new("AUTH_UNAUTHORIZED", "Authentication is required.");

    public static readonly Error Forbidden =
        new("AUTH_FORBIDDEN", "You are not authorized to perform this action.");

    public static readonly Error RegistrationFailed =
        new("REGISTRATION FAILED", "Registration failed");
}