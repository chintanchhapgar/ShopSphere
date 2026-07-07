namespace ShopSphere.Contracts.Common;

public sealed class Result<T> : Result
{
    private Result(
        T? value,
        bool isSuccess,
        string? message,
        Error? error)
        : base(isSuccess, message, error)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Success(
        T value,
        string message = "Success")
    {
        return new(
            value,
            true,
            message,
            null);
    }

    public static new Result<T> Failure(Error error)
    {
        return new(
            default,
            false,
            null,
            error);
    }
}