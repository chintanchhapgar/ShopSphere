namespace ShopSphere.Application.Features.Dashboard.GetDashboard;

public sealed record DashboardResponse(
    int TotalUsers,
    int TotalProducts,
    int TotalOrders,
    decimal TotalRevenue,
    int PendingOrders,
    int CompletedOrders,
    int LowStockProducts,
    int OutOfStockProducts,
    int TodayOrders,
    decimal TodayRevenue);