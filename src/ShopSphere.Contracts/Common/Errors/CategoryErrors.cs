namespace ShopSphere.Contracts.Errors;

using ShopSphere.Contracts.Common;

public static class CategoryErrors
{
    public static readonly Error AlreadyExists =
        new(
            "CATEGORY_ALREADY_EXISTS",
            "A category with the same name already exists.");

    public static readonly Error NotFound =
        new(
            "CATEGORY_NOT_FOUND",
            "Category not found.");

    public static readonly Error DuplicateName =
    new(
        "CATEGORY_DUPLICATE_NAME",
        "A category with the same name already exists.");

    public static readonly Error ParentNotFound =
        new(
            "CATEGORY_PARENT_NOT_FOUND",
            "Parent category was not found.");

    public static readonly Error HasChildCategories =
    new(
        "CATEGORY_HAS_CHILDREN",
        "Category cannot be deleted because it has child categories.");
}