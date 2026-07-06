using Microsoft.AspNetCore.Http;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Api.Common.Extensions;

public static class HttpResultExtensions
{
    public static IResult ToHttpResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok();
        }

        return result.Error!.Code switch
        {
            "Validation" => Results.BadRequest(result.Error),

            "Unauthorized" => Results.Unauthorized(),

            "Forbidden" => Results.Forbid(),

            "NotFound" => Results.NotFound(result.Error),

            "Conflict" => Results.Conflict(result.Error),

            _ => Results.Problem(result.Error.Description)
        };
    }

    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(result.Value);
        }

        return result.Error!.Code switch
        {
            "Validation" => Results.BadRequest(result.Error),

            "Unauthorized" => Results.Unauthorized(),

            "Forbidden" => Results.Forbid(),

            "NotFound" => Results.NotFound(result.Error),

            "Conflict" => Results.Conflict(result.Error),

            _ => Results.Problem(result.Error.Description)
        };
    }
}