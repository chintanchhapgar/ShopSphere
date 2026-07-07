using ShopSphere.Contracts.Common;

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; init; }
}