using Microsoft.EntityFrameworkCore;
using ShopSphere.Application.Features.Dashboard.GetDashboard;
using ShopSphere.Application.Features.Dashboard.GetSalesAnalytics;
using ShopSphere.Application.Interfaces;
using ShopSphere.Domain.Enums;

namespace ShopSphere.Infrastructure.Persistence.Repositories;

public sealed class StatisticsReadRepository
    : IStatisticsReadRepository
{
    private readonly ApplicationDbContext _context;

    public StatisticsReadRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<DashboardResponse> GetDashboardAsync(
        CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.Date;

        var totalUsers = await _context.Users
            .CountAsync(cancellationToken);

        var totalProducts = await _context.Products
            .CountAsync(cancellationToken);

        var totalOrders = await _context.Orders
            .CountAsync(cancellationToken);

        var totalRevenue = await _context.Orders
            .Where(x => x.Status == OrderStatus.Completed)
            .SumAsync(
                x => (decimal?)x.TotalAmount,
                cancellationToken) ?? 0m;

        var pendingOrders = await _context.Orders
            .CountAsync(
                x => x.Status == OrderStatus.Pending,
                cancellationToken);

        var completedOrders = await _context.Orders
            .CountAsync(
                x => x.Status == OrderStatus.Completed,
                cancellationToken);

        // Don't use computed properties (AvailableQuantity / IsLowStock)
        var lowStockProducts = await _context.Inventories
            .CountAsync(
                x =>
                    (x.QuantityOnHand - x.ReservedQuantity) > 0 &&
                    (x.QuantityOnHand - x.ReservedQuantity) <= x.LowStockThreshold,
                cancellationToken);

        var outOfStockProducts = await _context.Inventories
            .CountAsync(
                x =>
                    (x.QuantityOnHand - x.ReservedQuantity) <= 0,
                cancellationToken);

        var todayOrders = await _context.Orders
            .CountAsync(
                x => x.CreatedAtUtc >= today,
                cancellationToken);

        var todayRevenue = await _context.Orders
            .Where(x =>
                x.CreatedAtUtc >= today &&
                x.Status == OrderStatus.Completed)
            .SumAsync(
                x => (decimal?)x.TotalAmount,
                cancellationToken) ?? 0m;

        return new DashboardResponse(
            totalUsers,
            totalProducts,
            totalOrders,
            totalRevenue,
            pendingOrders,
            completedOrders,
            lowStockProducts,
            outOfStockProducts,
            todayOrders,
            todayRevenue);
    }

    public async Task<SalesAnalyticsResponse> GetSalesAnalyticsAsync(
    int days,
    CancellationToken cancellationToken)
    {
        var fromDate = DateTime.UtcNow.Date.AddDays(-(days - 1));

        var sales = await _context.Orders
            .AsNoTracking()
            .Where(x =>
                x.Status == OrderStatus.Completed &&
                x.CreatedAtUtc >= fromDate)
            .GroupBy(x => x.CreatedAtUtc.Date)
            .Select(x => new
            {
                Date = x.Key,
                Revenue = x.Sum(o => o.TotalAmount),
                Orders = x.Count()
            })
            .ToListAsync(cancellationToken);

        var items = new List<SalesAnalyticsPoint>();

        for (var date = fromDate; date <= DateTime.UtcNow.Date; date = date.AddDays(1))
        {
            var day = sales.FirstOrDefault(x => x.Date == date);

            items.Add(
                new SalesAnalyticsPoint(
                    DateOnly.FromDateTime(date),
                    day?.Revenue ?? 0m,
                    day?.Orders ?? 0));
        }

        return new SalesAnalyticsResponse(items);
    }
}