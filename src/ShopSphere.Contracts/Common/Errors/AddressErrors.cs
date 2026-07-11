using ShopSphere.Contracts.Common;

namespace ShopSphere.Contracts.Errors;

public static class AddressErrors
{
    public static readonly Error NotFound =
        new(
            "Address.NotFound",
            "Address was not found.");

    public static readonly Error Unauthorized =
        new(
            "Address.Unauthorized",
            "You are not authorized to use this address.");
}