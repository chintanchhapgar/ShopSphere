using MediatR;
using ShopSphere.Api.Common.Extensions;
using ShopSphere.Application.Features.Products.SearchProducts;

namespace ShopSphere.Api.Endpoints.Products;

public static class ProductSearchEndpoints
{
    public static IEndpointRouteBuilder MapProductSearchEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Products")
            .RequireRateLimiting("anonymous")
            .AllowAnonymous();

        group.MapGet(
            "/search",
            async (
                [AsParameters] ProductSearchRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new SearchProductsQuery(request),
                    cancellationToken);

                return result.ToHttpResult();
            });

        return app;
    }
}