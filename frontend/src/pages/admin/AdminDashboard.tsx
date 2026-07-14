import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import {
  Package,
  ShoppingCart,
  Users,
  IndianRupee,
  TrendingUp,
  Clock,
  CheckCircle,
  Truck,
  XCircle,
  RefreshCw,
  MessageSquare,
  ArrowRight,
  BarChart3,
  Calendar,
  AlertTriangle,
  BoxSelect,
} from "lucide-react";
import { adminApi } from "@/api/admin.api";
import type { DashboardStats, SalesDataItem } from "@/api/admin.api";
import type { OrderListItem } from "@/types";
import { OrderStatusColors } from "@/types";
import { formatPrice } from "@/utils/formatPrice";
import { cn } from "@/utils/cn";
import Spinner from "@/components/ui/Spinner";

const AdminDashboard = () => {
  const [stats, setStats]               = useState<DashboardStats | null>(null);
  const [salesData, setSalesData]       = useState<SalesDataItem[]>([]);
  const [recentOrders, setRecentOrders] = useState<OrderListItem[]>([]);
  const [isLoading, setIsLoading]       = useState(true);
  const [salesDays, setSalesDays]       = useState(30);

  // ── Load Data ──────────────────────────────────────────────────────────────
  useEffect(() => {
    const load = async () => {
      setIsLoading(true);
      try {
        const [dashRes, salesRes, ordersRes] = await Promise.allSettled([
          adminApi.getDashboard(),
          adminApi.getSalesAnalytics(salesDays),
          adminApi.getAllOrders(),
        ]);

        if (dashRes.status === "fulfilled") setStats(dashRes.value);
        if (salesRes.status === "fulfilled") setSalesData(salesRes.value);
        if (ordersRes.status === "fulfilled") setRecentOrders(ordersRes.value.slice(0, 10));
      } catch (err) {
        console.error("Dashboard error:", err);
      } finally {
        setIsLoading(false);
      }
    };
    load();
  }, [salesDays]);

  if (isLoading) {
    return (
      <div className="flex justify-center py-32">
        <Spinner size="lg" className="text-primary-600" />
      </div>
    );
  }

  // ── Computed ────────────────────────────────────────────────────────────────
  const maxRevenue        = salesData.length > 0 ? Math.max(...salesData.map((s) => s.revenue), 1) : 1;
  const totalSalesRevenue = salesData.reduce((acc, s) => acc + s.revenue, 0);
  const totalSalesOrders  = salesData.reduce((acc, s) => acc + s.orders, 0);

  // Order status counts from recent orders
  const statusCounts: Record<string, number> = {};
  recentOrders.forEach((o) => {
    statusCounts[o.status] = (statusCounts[o.status] || 0) + 1;
  });

  return (
    <div className="max-w-7xl mx-auto px-4 py-8">

      {/* ── Header ──────────────────────────────────────────────────────────── */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Dashboard</h1>
          <p className="text-gray-500 mt-1">Welcome back, Admin</p>
        </div>
        <div className="flex items-center gap-2 text-sm text-gray-500 bg-gray-50 px-4 py-2 rounded-lg">
          <Calendar className="w-4 h-4" />
          {new Date().toLocaleDateString("en-IN", {
            weekday: "long", year: "numeric", month: "long", day: "numeric",
          })}
        </div>
      </div>

      {/* ── Main Stats ──────────────────────────────────────────────────────── */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
        {[
          { label: "Total Revenue",  value: formatPrice(stats?.totalRevenue ?? 0),        icon: IndianRupee,  color: "bg-green-50 text-green-600",  sub: `Today: ${formatPrice(stats?.todayRevenue ?? 0)}` },
          { label: "Total Orders",   value: (stats?.totalOrders ?? 0).toLocaleString(),   icon: ShoppingCart, color: "bg-blue-50 text-blue-600",    sub: `Today: ${stats?.todayOrders ?? 0}` },
          { label: "Total Products", value: (stats?.totalProducts ?? 0).toLocaleString(), icon: Package,      color: "bg-purple-50 text-purple-600", sub: `Low stock: ${stats?.lowStockProducts ?? 0}` },
          { label: "Total Users",    value: (stats?.totalUsers ?? 0).toLocaleString(),    icon: Users,        color: "bg-orange-50 text-orange-600", sub: "Registered users" },
        ].map(({ label, value, icon: Icon, color, sub }) => (
          <div key={label} className="bg-white rounded-xl border border-gray-100 shadow-sm p-5 hover:shadow-md transition">
            <div className="flex items-start justify-between mb-3">
              <div className={cn("p-2.5 rounded-xl", color)}>
                <Icon className="w-5 h-5" />
              </div>
            </div>
            <p className="text-2xl font-bold text-gray-900">{value}</p>
            <p className="text-xs text-gray-500 mt-1">{label}</p>
            <p className="text-xs text-gray-400 mt-0.5">{sub}</p>
          </div>
        ))}
      </div>

      {/* ── Secondary Stats ─────────────────────────────────────────────────── */}
      <div className="grid grid-cols-2 sm:grid-cols-4 gap-3 mb-8">
        {[
          { label: "Pending Orders",    value: stats?.pendingOrders ?? 0,      icon: Clock,          color: "text-yellow-600 bg-yellow-50" },
          { label: "Completed Orders",  value: stats?.completedOrders ?? 0,    icon: CheckCircle,    color: "text-green-600 bg-green-50" },
          { label: "Low Stock",         value: stats?.lowStockProducts ?? 0,   icon: AlertTriangle,  color: "text-orange-600 bg-orange-50" },
          { label: "Out of Stock",      value: stats?.outOfStockProducts ?? 0, icon: BoxSelect,      color: "text-red-600 bg-red-50" },
        ].map(({ label, value, icon: Icon, color }) => (
          <div key={label} className="bg-white rounded-xl border border-gray-100 shadow-sm p-4 flex items-center gap-3">
            <div className={cn("p-2 rounded-lg", color)}><Icon className="w-4 h-4" /></div>
            <div>
              <p className="text-xl font-bold text-gray-900">{value}</p>
              <p className="text-xs text-gray-500">{label}</p>
            </div>
          </div>
        ))}
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">

        {/* ── Sales Chart ───────────────────────────────────────────────────── */}
        <div className="lg:col-span-2 bg-white rounded-xl border border-gray-100 shadow-sm p-6">
          <div className="flex items-center justify-between mb-6">
            <div>
              <h2 className="text-lg font-semibold text-gray-800 flex items-center gap-2">
                <BarChart3 className="w-5 h-5 text-gray-400" />
                Sales Analytics
              </h2>
              <p className="text-sm text-gray-500 mt-0.5">
                {formatPrice(totalSalesRevenue)} from {totalSalesOrders} orders
              </p>
            </div>
            <div className="flex gap-1 bg-gray-100 rounded-lg p-0.5">
              {[7, 14, 30].map((d) => (
                <button
                  key={d}
                  onClick={() => setSalesDays(d)}
                  className={cn(
                    "px-3 py-1.5 text-xs font-medium rounded-md transition",
                    salesDays === d ? "bg-white text-primary-600 shadow-sm" : "text-gray-500 hover:text-gray-700"
                  )}
                >
                  {d}D
                </button>
              ))}
            </div>
          </div>

          {/* Bar Chart */}
          {salesData.length === 0 ? (
            <div className="flex items-center justify-center h-52 text-gray-400 text-sm">
              No sales data available
            </div>
          ) : (
            <div className="relative">
              {/* Y-axis labels */}
              <div className="absolute left-0 top-0 bottom-8 w-16 flex flex-col justify-between text-xs text-gray-400">
                <span>{formatPrice(maxRevenue)}</span>
                <span>{formatPrice(maxRevenue / 2)}</span>
                <span>₹0</span>
              </div>

              {/* Bars */}
              <div className="ml-16 flex items-end gap-0.5 h-52">
                {salesData.map((day, idx) => {
                  const height = maxRevenue > 0 ? (day.revenue / maxRevenue) * 100 : 0;
                  const hasData = day.revenue > 0 || day.orders > 0;

                  return (
                    <div key={idx} className="flex-1 flex flex-col items-center group relative">
                      {/* Tooltip */}
                      <div className="absolute bottom-full mb-2 hidden group-hover:block z-10">
                        <div className="bg-gray-900 text-white text-xs rounded-lg px-3 py-2 whitespace-nowrap shadow-lg">
                          <p className="font-semibold">
                            {new Date(day.date).toLocaleDateString("en-IN", { month: "short", day: "numeric", year: "numeric" })}
                          </p>
                          <p className="mt-1">Revenue: {formatPrice(day.revenue)}</p>
                          <p>Orders: {day.orders}</p>
                        </div>
                      </div>

                      {/* Bar */}
                      <div
                        className={cn(
                          "w-full rounded-t transition-all duration-300 cursor-pointer min-h-[2px]",
                          hasData
                            ? "bg-primary-400 hover:bg-primary-600"
                            : "bg-gray-100 hover:bg-gray-200"
                        )}
                        style={{ height: `${Math.max(height, 1)}%` }}
                      />
                    </div>
                  );
                })}
              </div>

              {/* X-axis labels */}
              <div className="ml-16 flex justify-between mt-2">
                {salesData
                  .filter((_, idx) => idx % Math.ceil(salesData.length / 6) === 0 || idx === salesData.length - 1)
                  .map((day, idx) => (
                    <span key={idx} className="text-xs text-gray-400">
                      {new Date(day.date).toLocaleDateString("en-IN", { month: "short", day: "numeric" })}
                    </span>
                  ))}
              </div>
            </div>
          )}
        </div>

        {/* ── Order Status Breakdown ──────────────────────────────────────── */}
        <div className="bg-white rounded-xl border border-gray-100 shadow-sm p-6">
          <h2 className="text-lg font-semibold text-gray-800 mb-4 flex items-center gap-2">
            <Package className="w-5 h-5 text-gray-400" />
            Order Status
          </h2>

          <div className="space-y-3">
            {[
              { status: "Pending",    icon: Clock,       barColor: "bg-yellow-400" },
              { status: "Confirmed",  icon: CheckCircle, barColor: "bg-blue-400" },
              { status: "Processing", icon: RefreshCw,   barColor: "bg-purple-400" },
              { status: "Shipped",    icon: Truck,       barColor: "bg-indigo-400" },
              { status: "Delivered",  icon: CheckCircle, barColor: "bg-green-400" },
              { status: "Cancelled",  icon: XCircle,     barColor: "bg-red-400" },
              { status: "Completed",  icon: CheckCircle, barColor: "bg-emerald-400" },
            ].map(({ status, icon: Icon, barColor }) => {
              const count = statusCounts[status] || 0;
              const total = recentOrders.length || 1;
              const pct   = (count / total) * 100;
              const statusColor = OrderStatusColors[status] || "bg-gray-100 text-gray-700";

              return (
                <div key={status} className="flex items-center gap-3">
                  <div className={cn("p-1.5 rounded-lg shrink-0", statusColor.split(" ")[0].replace("text-", "bg-").replace("700", "50"))}>
                    <Icon className={cn("w-3.5 h-3.5", statusColor.split(" ")[1] || "text-gray-600")} />
                  </div>
                  <div className="flex-1">
                    <div className="flex justify-between text-sm mb-1">
                      <span className="text-gray-700 font-medium">{status}</span>
                      <span className="text-gray-500 font-semibold">{count}</span>
                    </div>
                    <div className="w-full h-1.5 bg-gray-100 rounded-full overflow-hidden">
                      <div className={cn("h-full rounded-full transition-all duration-700", barColor)} style={{ width: `${pct}%` }} />
                    </div>
                  </div>
                </div>
              );
            })}
          </div>

          <div className="mt-4 pt-4 border-t border-gray-100 text-center">
            <Link to="/admin/orders" className="text-sm text-primary-600 hover:underline font-medium">
              Manage All Orders →
            </Link>
          </div>
        </div>
      </div>

      {/* ── Quick Actions + Recent Orders ──────────────────────────────────── */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mt-6">

        {/* Quick Actions */}
        <div className="bg-white rounded-xl border border-gray-100 shadow-sm p-6">
          <h2 className="text-lg font-semibold text-gray-800 mb-4">Quick Actions</h2>
          <div className="space-y-2">
            {[
              { label: "Manage Orders",   to: "/admin/orders",  icon: ShoppingCart,  color: "text-blue-600" },
              { label: "Pending Reviews",  to: "/admin/reviews", icon: MessageSquare, color: "text-purple-600" },
              { label: "View Products",    to: "/products",      icon: Package,       color: "text-green-600" },
              { label: "My Profile",       to: "/profile",       icon: Users,         color: "text-orange-600" },
            ].map(({ label, to, icon: Icon, color }) => (
              <Link key={label} to={to} className="flex items-center justify-between p-3 rounded-lg hover:bg-gray-50 transition group">
                <div className="flex items-center gap-3">
                  <Icon className={cn("w-4 h-4", color)} />
                  <span className="text-sm font-medium text-gray-700">{label}</span>
                </div>
                <ArrowRight className="w-4 h-4 text-gray-300 group-hover:text-primary-600 transition" />
              </Link>
            ))}
          </div>
        </div>

        {/* Recent Orders */}
        <div className="lg:col-span-2 bg-white rounded-xl border border-gray-100 shadow-sm p-6">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-lg font-semibold text-gray-800 flex items-center gap-2">
              <Clock className="w-5 h-5 text-gray-400" />
              Recent Orders
            </h2>
            <Link to="/admin/orders" className="text-sm text-primary-600 hover:underline">View All</Link>
          </div>

          {recentOrders.length === 0 ? (
            <div className="text-center py-8 text-gray-400 text-sm">No orders yet</div>
          ) : (
            <div className="space-y-2">
              {recentOrders.slice(0, 8).map((order) => (
                <Link key={order.id} to={`/orders/${order.id}`} className="flex items-center justify-between p-3 rounded-lg hover:bg-gray-50 transition">
                  <div className="flex items-center gap-3">
                    <div className="w-8 h-8 bg-gray-100 rounded-lg flex items-center justify-center">
                      <Package className="w-4 h-4 text-gray-500" />
                    </div>
                    <div>
                      <p className="text-sm font-medium text-gray-800">{order.orderNumber}</p>
                      <p className="text-xs text-gray-500">
                        {new Date(order.orderDate).toLocaleDateString("en-IN", { month: "short", day: "numeric" })}
                        {" • "}{order.totalItems} items
                      </p>
                    </div>
                  </div>
                  <div className="flex items-center gap-3">
                    <span className={cn("px-2 py-0.5 rounded-full text-xs font-semibold", OrderStatusColors[order.status] || "bg-gray-100 text-gray-700")}>
                      {order.status}
                    </span>
                    <span className="text-sm font-bold text-gray-900 min-w-[80px] text-right">
                      {formatPrice(order.totalAmount)}
                    </span>
                  </div>
                </Link>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default AdminDashboard;