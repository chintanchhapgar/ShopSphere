using ShopSphere.Api.Endpoints.Authentication;
using ShopSphere.Api.Endpoints.Brands;
using ShopSphere.Api.Endpoints.Categories;
using ShopSphere.Api.Endpoints.Coupons;
using ShopSphere.Api.Endpoints.Orders;
using ShopSphere.Api.Endpoints.Payments;
using ShopSphere.Api.Endpoints.Products;
using ShopSphere.Api.Endpoints.Reviews;
using ShopSphere.Api.Endpoints.Shipments;
using ShopSphere.Api.Endpoints.Test;
using ShopSphere.Api.Endpoints.Wishlists;

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
        app.MapOrderEndpoints();
        app.MapAdminOrderEndpoints();
        app.MapPaymentEndpoints();
        app.MapShipmentEndpoints();
        app.MapCouponEndpoints();
        app.MapTestEndpoints();
        app.MapWishlistEndpoints();
        app.MapReviewEndpoints();
        app.MapAdminReviewEndpoints();
        app.MapProductSearchEndpoints();

        return app;
    }
}