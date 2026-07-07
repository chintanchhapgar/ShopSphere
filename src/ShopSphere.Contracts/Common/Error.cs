public sealed record Error(
    string Code,
    string Description,
    string? Field = null)
{
    public static Error Validation(
        string code,
        string description,
        string? field = null)
        => new(code, description, field);

    public static Error NotFound(
        string code,
        string description,
        string? field = null)
        => new(code, description, field);

    public static Error Conflict(
        string code,
        string description,
        string? field = null)
        => new(code, description, field);

    public static Error Unauthorized(
        string code,
        string description)
        => new(code, description);

    public static Error Forbidden(
        string code,
        string description)
        => new(code, description);
}