using Microsoft.AspNetCore.Mvc;
using ShopSphere.Application.Interfaces;
using ShopSphere.Contracts.AI;
using ShopSphere.Contracts.Common;
using ShopSphere.Infrastructure.AI;
using System.Security.Claims;

namespace ShopSphere.Api.Endpoints;

public static class AIChatEndpoints
{
    public static void MapAIChatEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/ai")
            .WithTags("AI Assistant")
            .RequireAuthorization();

        // POST /api/ai/chat
        group.MapPost("/chat", async (
            [FromBody] ChatRequest request,
            IAIChatService aiService,
            ClaimsPrincipal user,
            CancellationToken ct) =>
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? user.FindFirst("sub")?.Value;

            var response = await aiService.SendMessageAsync(request, userId, ct);

            return Results.Ok(new ApiResponse<ChatResponse>
            {
                Success = true,
                Data = response,
                Message = "Success"
            });
        })
        .WithName("AIChat")
        .WithSummary("Chat with AI shopping assistant");

        group.MapPost("/generate-description", async (
            [FromBody] GenerateDescriptionRequest request,
            IAIChatService aiService,
            CancellationToken ct) =>
        {
            var description = await aiService.GenerateProductDescriptionAsync(
                request.ProductName,
                request.Category,
                request.Brand,
                request.ShortInfo,
                ct);

            return Results.Ok(new ApiResponse<object>
            {
                Success = true,
                Data = new { description },
                Message = "Description generated"
            });
        })
        .WithName("GenerateDescription");
    }


}

public record GenerateDescriptionRequest(
    string ProductName,
    string Category,
    string Brand,
    string? ShortInfo);