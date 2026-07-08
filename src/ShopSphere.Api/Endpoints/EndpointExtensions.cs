using ShopSphere.Api.Endpoints.Authentication;
using ShopSphere.Api.Endpoints.Brands;
using ShopSphere.Api.Endpoints.Categories;
using ShopSphere.Api.Endpoints.Products;

namespace ShopSphere.Api.Endpoints;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapAuthenticationEndpoints();
        app.MapCategoryEndpoints();
        app.MapBrandEndpoints();
        app.MapProductEndpoints();
        app.MapProductImageEndpoints();
        app.MapInventoryEndpoints();
        app.MapCartEndpoints();

        return app;
    }
}