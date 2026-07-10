using MediatR;
using ShopSphere.Api.Common.Extensions;
using ShopSphere.Api.Extensions;
using ShopSphere.Application.Features.Dashboard.GetDashboard;

namespace ShopSphere.Api.Endpoints.Admin;

public static class DashboardEndpoints
{
    public static IEndpointRouteBuilder MapDashboardEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/admin")
            .WithTags("Admin Dashboard")
            .RequireAuthorization();

        group.MapGet(
            "/dashboard",
            async (
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new GetDashboardQuery(),
                    cancellationToken);

                return result.ToHttpResult();
            })
            .WithName("GetDashboard")
            .WithSummary("Get dashboard statistics")
            .WithDescription("Returns summary statistics for the admin dashboard.");

        group.MapGet(
            "/dashboard/sales",
            async (
                int? days,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new GetSalesAnalyticsQuery(days ?? 7),
                    cancellationToken);

                return result.ToHttpResult();
            })
            .WithName("GetSalesAnalytics")
            .WithSummary("Get sales analytics")
            .WithDescription("Returns daily revenue and completed order counts for the specified number of days.");

        return app;
    }
}