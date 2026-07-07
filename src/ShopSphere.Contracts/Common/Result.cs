namespace ShopSphere.Contracts.Common;

public class Result
{
    protected Result(
        bool isSuccess,
        string? message,
        Error? error)
    {
        IsSuccess = isSuccess;
        Message = message;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public string? Message { get; }

    public Error? Error { get; }

    public static Result Success(
        string message = "Success")
    {
        return new(true, message, null);
    }

    public static Result Failure(Error error)
    {
        return new(false, null, error);
    }
}