namespace ShopSphere.Application.Features.Dashboard.GetSalesAnalytics;

public sealed record SalesAnalyticsPoint(
    DateOnly Date,
    decimal Revenue,
    int Orders);

public sealed record SalesAnalyticsResponse(
    IReadOnlyList<SalesAnalyticsPoint> Items);