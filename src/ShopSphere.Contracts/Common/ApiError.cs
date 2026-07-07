public sealed record ApiError(
    string Code,
    string Description,
    string? Field = null);