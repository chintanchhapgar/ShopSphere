using MediatR;
using ShopSphere.Api.Extensions;
using ShopSphere.Application.Features.ProductImages.DeleteProductImage;
using ShopSphere.Application.Features.ProductImages.GetProductImages;
using ShopSphere.Application.Features.ProductImages.SetPrimaryProductImage;
using ShopSphere.Application.Features.ProductImages.UpdateProductImageDisplayOrder;

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

        group.MapPatch(
            "/{imageId:guid}/primary",
            SetPrimaryImage);

        group.MapDelete(
            "/{imageId:guid}",
            DeleteProductImage);

        group.MapPatch(
            "/{imageId:guid}/display-order",
            UpdateDisplayOrder);

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

    private static async Task<IResult> SetPrimaryImage(
        Guid productId,
        Guid imageId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new SetPrimaryProductImageCommand(
            productId,
            imageId);

        var result = await sender.Send(
            command,
            cancellationToken);

        return result.ToMinimalApiResult();
    }
    private static async Task<IResult> DeleteProductImage(
        Guid productId,
        Guid imageId,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProductImageCommand(
            productId,
            imageId);

        var result = await sender.Send(
            command,
            cancellationToken);

        return result.ToMinimalApiResult();
    }

    private static async Task<IResult> UpdateDisplayOrder(
        Guid productId,
        Guid imageId,
        UpdateProductImageDisplayOrderRequest request,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var command = new UpdateProductImageDisplayOrderCommand(
            productId,
            imageId,
            request.DisplayOrder);

        var result = await sender.Send(
            command,
            cancellationToken);

        return result.ToMinimalApiResult();
    }
}