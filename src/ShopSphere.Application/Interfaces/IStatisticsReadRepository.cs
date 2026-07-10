using ShopSphere.Application.Features.Dashboard.GetDashboard;
using ShopSphere.Application.Features.Dashboard.GetSalesAnalytics;

namespace ShopSphere.Application.Interfaces;

public interface IStatisticsReadRepository
{
    Task<DashboardResponse> GetDashboardAsync(
        CancellationToken cancellationToken);

    Task<SalesAnalyticsResponse> GetSalesAnalyticsAsync(
        int days,
        CancellationToken cancellationToken);
}