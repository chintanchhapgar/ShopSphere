using Microsoft.AspNetCore.Http;
using ShopSphere.Contracts.Common;

namespace ShopSphere.Api.Common.Extensions;

public static class HttpResultExtensions
{
    public static IResult ToHttpResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(new ApiResponse
            {
                Success = true,
                Message = result.Message!
            });
        }

        return MapFailure(result.Error!);
    }

    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Results.Ok(new ApiResponse<T>
            {
                Success = true,
                Message = result.Message!,
                Data = result.Value
            });
        }

        return MapFailure(result.Error!);
    }

    public static IResult ToCreatedHttpResult<T>(
        this Result<T> result,
        string location)
    {
        if (result.IsFailure)
        {
            return MapFailure(result.Error!);
        }

        return Results.Created(
            location,
            new ApiResponse<T>
            {
                Success = true,
                Message = result.Message!,
                Data = result.Value
            });
    }

    private static IResult MapFailure(Error error)
    {
        var response = new ApiResponse
        {
            Success = false,
            Message = error.Description,
            Errors =
            [
                new ApiError(
                    error.Code,
                    error.Description)
            ]
        };

        return error.Code switch
        {
            "VALIDATION_ERROR" =>
                Results.BadRequest(response),

            "AUTH_INVALID_CREDENTIALS" =>
                Results.Json(
                    response,
                    statusCode: StatusCodes.Status401Unauthorized),

            "AUTH_FORBIDDEN" =>
                Results.Json(
                    response,
                    statusCode: StatusCodes.Status403Forbidden),

            "CATEGORY_NOT_FOUND" =>
                Results.NotFound(response),

            "CATEGORY_ALREADY_EXISTS" =>
                Results.Conflict(response),

            _ =>
                Results.Json(
                    response,
                    statusCode: StatusCodes.Status500InternalServerError)
        };
    }
}