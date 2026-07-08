using Microsoft.AspNetCore.Http;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Api.Extensions;

public static class EndpointResultExtensions
{
    public static IResult ToMinimalApiResult(
        this Result result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(result);
        }

        return Results.BadRequest(result);
    }

    public static IResult ToMinimalApiResult<T>(
        this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(result);
        }

        return Results.BadRequest(result);
    }
}