using MediatR;
using ShopSphere.Application.Features.Dashboard.GetSalesAnalytics;
using ShopSphere.Contracts.Common;

public sealed record GetSalesAnalyticsQuery(
    int Days = 7)
    : IRequest<Result<SalesAnalyticsResponse>>;