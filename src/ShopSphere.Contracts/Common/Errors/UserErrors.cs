namespace ShopSphere.Contracts.Common.Errors;

public static class UserErrors
{
    public static readonly Error NotFound =
        new("USER_NOT_FOUND", "User not found.");
}