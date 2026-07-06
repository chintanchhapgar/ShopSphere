using ShopSphere.Api.Endpoints.Authentication;
using ShopSphere.Api.Endpoints.Categories;

namespace ShopSphere.Api.Endpoints;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(
        this IEndpointRouteBuilder app)
    {
        app.MapAuthenticationEndpoints();
        app.MapCategoryEndpoints();

        return app;
    }
}