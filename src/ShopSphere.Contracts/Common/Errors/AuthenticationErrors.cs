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

    public static readonly Error InvalidPasswordResetToken =
        new(
            "Authentication.InvalidPasswordResetToken",
            "Invalid or expired password reset token.");

    public static readonly Error InvalidEmailVerificationToken =
        new(
            "Authentication.InvalidEmailVerificationToken",
            "The email verification token is invalid or has expired.");

    public static readonly Error InvalidEmail =
        new("INVALID_EMAIL", "Invalid email.");

    public static readonly Error EmailNotFound =
       new("EMAIL_NOT_FOUND", "Email not found.");
}