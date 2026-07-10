namespace ShopSphere.Contracts.Errors;

public static class ReviewErrors
{
    public static readonly Error AlreadyExists =
        new(
            "Review.AlreadyExists",
            "You have already reviewed this product.");

    public static readonly Error NotFound =
        new(
            "Review.NotFound",
            "Review was not found.");
}