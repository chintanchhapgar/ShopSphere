using MediatR;
using ShopSphere.Api.Extensions;
using ShopSphere.Application.Features.ProductImages.GetProductImages;

public static class ProductImageEndpoints
{
    public static IEndpointRouteBuilder MapProductImageEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products/{productId:guid}/images")
            .WithTags("Product Images")
            .RequireAuthorization();

        group.MapPost("/", UploadImage)
    .WithName("UploadProductImage")
    .WithSummary("Upload product image")
    .DisableAntiforgery();

        group.MapGet("/", GetProductImages)
    .WithName("GetProductImages")
    .WithSummary("Get product images");

        return app;
    }

    private static async Task<IResult> UploadImage(
        Guid productId,
        IFormFile file,
        bool isPrimary,
        ISender sender,
        CancellationToken cancellationToken)
    {
        await using var stream = file.OpenReadStream();

        var command = new UploadProductImageCommand(
            productId,
            stream,
            file.FileName,
            file.Length,
            isPrimary);

        var result = await sender.Send(command, cancellationToken);

        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> GetProductImages(
    Guid productId,
    ISender sender,
    CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetProductImagesQuery(productId),
            cancellationToken);

        return result.ToMinimalApiResult();
    }
}