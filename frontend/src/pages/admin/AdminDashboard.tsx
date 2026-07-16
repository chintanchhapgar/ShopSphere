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
import {
  AreaChart, Area, PieChart, Pie, Cell,
  XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer,
} from "recharts";
import { adminApi } from "@/api/admin.api";
import type { DashboardStats, SalesDataItem } from "@/api/admin.api";
import type { OrderListItem } from "@/types";
import { OrderStatusColors } from "@/types";
import { formatPrice } from "@/utils/formatPrice";
import { cn } from "@/utils/cn";
import Spinner from "@/components/ui/Spinner";

// ── Pie Chart Colors ─────────────────────────────────────────────────────────
const STATUS_COLORS = {
  Pending:    "#eab308",
  Confirmed:  "#3b82f6",
  Processing: "#a855f7",
  Shipped:    "#6366f1",
  Delivered:  "#22c55e",
  Cancelled:  "#ef4444",
  Completed:  "#10b981",
};

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
  const totalSalesRevenue = salesData.reduce((acc, s) => acc + s.revenue, 0);
  const totalSalesOrders  = salesData.reduce((acc, s) => acc + s.orders, 0);

  // Order status counts
  const statusCounts: Record<string, number> = {};
  recentOrders.forEach((o) => {
    statusCounts[o.status] = (statusCounts[o.status] || 0) + 1;
  });

  // Pie chart data
  const pieData = Object.entries(statusCounts)
    .filter(([_, count]) => count > 0)
    .map(([status, count]) => ({
      name:  status,
      value: count,
      color: STATUS_COLORS[status as keyof typeof STATUS_COLORS] || "#9ca3af",
    }));

  // Chart data with formatted dates
  const chartData = salesData.map(d => ({
    date:    new Date(d.date).toLocaleDateString("en-IN", { month: "short", day: "numeric" }),
    revenue: d.revenue,
    orders:  d.orders,
  }));

  return (
    <div className="max-w-7xl mx-auto px-4 py-8">

      {/* ── Header ──────────────────────────────────────────────────────────── */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 dark:text-gray-100">Dashboard</h1>
          <p className="text-gray-500 mt-1">Welcome back, Admin</p>
        </div>
        <div className="flex items-center gap-2 text-sm text-gray-500 bg-gray-50 dark:bg-gray-800 px-4 py-2 rounded-lg">
          <Calendar className="w-4 h-4" />
          {new Date().toLocaleDateString("en-IN", {
            weekday: "long", year: "numeric", month: "long", day: "numeric",
          })}
        </div>
      </div>

      {/* ── Main Stats ──────────────────────────────────────────────────────── */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
        {[
          { label: "Total Revenue",  value: formatPrice(stats?.totalRevenue ?? 0),        icon: IndianRupee,  color: "bg-green-50 text-green-600",   sub: `Today: ${formatPrice(stats?.todayRevenue ?? 0)}` },
          { label: "Total Orders",   value: (stats?.totalOrders ?? 0).toLocaleString(),   icon: ShoppingCart, color: "bg-blue-50 text-blue-600",     sub: `Today: ${stats?.todayOrders ?? 0}` },
          { label: "Total Products", value: (stats?.totalProducts ?? 0).toLocaleString(), icon: Package,      color: "bg-purple-50 text-purple-600", sub: `Low stock: ${stats?.lowStockProducts ?? 0}` },
          { label: "Total Users",    value: (stats?.totalUsers ?? 0).toLocaleString(),    icon: Users,        color: "bg-orange-50 text-orange-600", sub: "Registered users" },
        ].map(({ label, value, icon: Icon, color, sub }) => (
          <div key={label} className="bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 shadow-sm p-5 hover:shadow-md transition">
            <div className="flex items-start justify-between mb-3">
              <div className={cn("p-2.5 rounded-xl", color)}>
                <Icon className="w-5 h-5" />
              </div>
            </div>
            <p className="text-2xl font-bold text-gray-900 dark:text-gray-100">{value}</p>
            <p className="text-xs text-gray-500 mt-1">{label}</p>
            <p className="text-xs text-gray-400 mt-0.5">{sub}</p>
          </div>
        ))}
      </div>

      {/* ── Secondary Stats ─────────────────────────────────────────────────── */}
      <div className="grid grid-cols-2 sm:grid-cols-4 gap-3 mb-8">
        {[
          { label: "Pending Orders",    value: stats?.pendingOrders ?? 0,      icon: Clock,         color: "text-yellow-600 bg-yellow-50" },
          { label: "Completed Orders",  value: stats?.completedOrders ?? 0,    icon: CheckCircle,   color: "text-green-600 bg-green-50" },
          { label: "Low Stock",         value: stats?.lowStockProducts ?? 0,   icon: AlertTriangle, color: "text-orange-600 bg-orange-50" },
          { label: "Out of Stock",      value: stats?.outOfStockProducts ?? 0, icon: BoxSelect,     color: "text-red-600 bg-red-50" },
        ].map(({ label, value, icon: Icon, color }) => (
          <div key={label} className="bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 shadow-sm p-4 flex items-center gap-3">
            <div className={cn("p-2 rounded-lg", color)}><Icon className="w-4 h-4" /></div>
            <div>
              <p className="text-xl font-bold text-gray-900 dark:text-gray-100">{value}</p>
              <p className="text-xs text-gray-500">{label}</p>
            </div>
          </div>
        ))}
      </div>

      {/* ═══════════════════════════════════════════════════════════════════════ */}
      {/* ── Row 1: Sales Chart (2 cols) + Order Distribution Pie (1 col) ────── */}
      {/* ═══════════════════════════════════════════════════════════════════════ */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6 mb-6">

        {/* Sales Chart */}
        <div className="lg:col-span-2 bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 shadow-sm p-6">
          <div className="flex items-center justify-between mb-6">
            <div>
              <h2 className="text-lg font-semibold text-gray-800 dark:text-gray-100 flex items-center gap-2">
                <BarChart3 className="w-5 h-5 text-gray-400" />
                Sales Analytics
              </h2>
              <p className="text-sm text-gray-500 mt-0.5">
                {formatPrice(totalSalesRevenue)} from {totalSalesOrders} orders
              </p>
            </div>
            <div className="flex gap-1 bg-gray-100 dark:bg-gray-700 rounded-lg p-0.5">
              {[7, 14, 30].map((d) => (
                <button
                  key={d}
                  onClick={() => setSalesDays(d)}
                  className={cn(
                    "px-3 py-1.5 text-xs font-medium rounded-md transition",
                    salesDays === d
                      ? "bg-white dark:bg-gray-800 text-primary-600 shadow-sm"
                      : "text-gray-500 hover:text-gray-700"
                  )}
                >
                  {d}D
                </button>
              ))}
            </div>
          </div>

          {salesData.length === 0 ? (
            <div className="flex items-center justify-center h-52 text-gray-400 text-sm">
              No sales data available
            </div>
          ) : (
            <ResponsiveContainer width="100%" height={300}>
              <AreaChart data={chartData}>
                <defs>
                  <linearGradient id="colorRevenue" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="5%"  stopColor="#2563eb" stopOpacity={0.4}/>
                    <stop offset="95%" stopColor="#2563eb" stopOpacity={0}/>
                  </linearGradient>
                </defs>
                <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
                <XAxis dataKey="date" tick={{ fill: "#6b7280", fontSize: 12 }} />
                <YAxis
                  tick={{ fill: "#6b7280", fontSize: 12 }}
                  tickFormatter={(value) => `₹${(value / 1000).toFixed(0)}K`}
                />
                <Tooltip
                  contentStyle={{
                    backgroundColor: "#1f2937",
                    border: "none",
                    borderRadius: "8px",
                    color: "#fff",
                  }}
                  formatter={(value: number) => [`₹${value.toLocaleString()}`, "Revenue"]}
                />
                <Area
                  type="monotone"
                  dataKey="revenue"
                  stroke="#2563eb"
                  strokeWidth={2}
                  fill="url(#colorRevenue)"
                />
              </AreaChart>
            </ResponsiveContainer>
          )}
        </div>

        {/* Order Distribution Pie */}
        <div className="bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 shadow-sm p-6">
          <h2 className="text-lg font-semibold text-gray-800 dark:text-gray-100 mb-4 flex items-center gap-2">
            <Package className="w-5 h-5 text-gray-400" />
            Order Distribution
          </h2>

          {pieData.length > 0 ? (
            <>
              <ResponsiveContainer width="100%" height={200}>
                <PieChart>
                  <Pie
                    data={pieData}
                    cx="50%"
                    cy="50%"
                    innerRadius={50}
                    outerRadius={80}
                    paddingAngle={2}
                    dataKey="value"
                  >
                    {pieData.map((entry, index) => (
                      <Cell key={`cell-${index}`} fill={entry.color} />
                    ))}
                  </Pie>
                  <Tooltip
                    contentStyle={{
                      backgroundColor: "#1f2937",
                      border: "none",
                      borderRadius: "8px",
                      color: "#fff",
                    }}
                  />
                </PieChart>
              </ResponsiveContainer>

              {/* Legend */}
              <div className="grid grid-cols-2 gap-2 mt-4">
                {pieData.map(({ name, value, color }) => (
                  <div key={name} className="flex items-center gap-2 text-xs">
                    <span className="w-3 h-3 rounded shrink-0" style={{ backgroundColor: color }} />
                    <span className="text-gray-600 dark:text-gray-400 truncate">{name}</span>
                    <span className="font-semibold text-gray-800 dark:text-gray-200 ml-auto">
                      {value}
                    </span>
                  </div>
                ))}
              </div>
            </>
          ) : (
            <div className="flex items-center justify-center h-52 text-gray-400 text-sm">
              No orders yet
            </div>
          )}
        </div>
      </div>

      {/* ═══════════════════════════════════════════════════════════════════════ */}
      {/* ── Row 2: Order Status Breakdown (Full Width) ─────────────────────── */}
      {/* ═══════════════════════════════════════════════════════════════════════ */}
      <div className="bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 shadow-sm p-6 mb-6">
        <div className="flex items-center justify-between mb-4">
          <h2 className="text-lg font-semibold text-gray-800 dark:text-gray-100 flex items-center gap-2">
            <Package className="w-5 h-5 text-gray-400" />
            Order Status Breakdown
          </h2>
          <Link to="/admin/orders" className="text-sm text-primary-600 hover:underline font-medium">
            Manage All Orders →
          </Link>
        </div>

        {/* Horizontal Grid for Order Statuses */}
        <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-7 gap-3">
          {[
            { status: "Pending",    icon: Clock,       color: "bg-yellow-50 text-yellow-700 border-yellow-200",    bar: "bg-yellow-400" },
            { status: "Confirmed",  icon: CheckCircle, color: "bg-blue-50 text-blue-700 border-blue-200",          bar: "bg-blue-400" },
            { status: "Processing", icon: RefreshCw,   color: "bg-purple-50 text-purple-700 border-purple-200",    bar: "bg-purple-400" },
            { status: "Shipped",    icon: Truck,       color: "bg-indigo-50 text-indigo-700 border-indigo-200",    bar: "bg-indigo-400" },
            { status: "Delivered",  icon: CheckCircle, color: "bg-green-50 text-green-700 border-green-200",       bar: "bg-green-400" },
            { status: "Cancelled",  icon: XCircle,     color: "bg-red-50 text-red-700 border-red-200",             bar: "bg-red-400" },
            { status: "Completed",  icon: CheckCircle, color: "bg-emerald-50 text-emerald-700 border-emerald-200", bar: "bg-emerald-400" },
          ].map(({ status, icon: Icon, color, bar }) => {
            const count = statusCounts[status] || 0;
            const total = recentOrders.length || 1;
            const pct   = (count / total) * 100;

            return (
              <div
                key={status}
                className={cn(
                  "border rounded-xl p-3 transition hover:shadow-md",
                  color
                )}
              >
                <div className="flex items-center justify-between mb-2">
                  <Icon className="w-4 h-4" />
                  <span className="text-lg font-bold">{count}</span>
                </div>
                <p className="text-xs font-medium mb-2">{status}</p>
                <div className="w-full h-1 bg-white/50 rounded-full overflow-hidden">
                  <div
                    className={cn("h-full rounded-full transition-all duration-700", bar)}
                    style={{ width: `${pct}%` }}
                  />
                </div>
              </div>
            );
          })}
        </div>
      </div>

      {/* ═══════════════════════════════════════════════════════════════════════ */}
      {/* ── Row 3: Quick Actions (1 col) + Recent Orders (2 cols) ────────── */}
      {/* ═══════════════════════════════════════════════════════════════════════ */}
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">

        {/* Quick Actions */}
        <div className="bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 shadow-sm p-6">
          <h2 className="text-lg font-semibold text-gray-800 dark:text-gray-100 mb-4">
            Quick Actions
          </h2>
          <div className="space-y-2">
            {[
              { label: "Manage Orders",   to: "/admin/orders",  icon: ShoppingCart,  color: "text-blue-600" },
              { label: "Pending Reviews",  to: "/admin/reviews", icon: MessageSquare, color: "text-purple-600" },
              { label: "View Products",    to: "/products",      icon: Package,       color: "text-green-600" },
              { label: "Analytics",        to: "/admin/analytics", icon: TrendingUp,  color: "text-orange-600" },
            ].map(({ label, to, icon: Icon, color }) => (
              <Link
                key={label}
                to={to}
                className="flex items-center justify-between p-3 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition group"
              >
                <div className="flex items-center gap-3">
                  <Icon className={cn("w-4 h-4", color)} />
                  <span className="text-sm font-medium text-gray-700 dark:text-gray-300">{label}</span>
                </div>
                <ArrowRight className="w-4 h-4 text-gray-300 group-hover:text-primary-600 transition" />
              </Link>
            ))}
          </div>
        </div>

        {/* Recent Orders */}
        <div className="lg:col-span-2 bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 shadow-sm p-6">
          <div className="flex items-center justify-between mb-4">
            <h2 className="text-lg font-semibold text-gray-800 dark:text-gray-100 flex items-center gap-2">
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
                <Link
                  key={order.id}
                  to={`/orders/${order.id}`}
                  className="flex items-center justify-between p-3 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 transition"
                >
                  <div className="flex items-center gap-3">
                    <div className="w-8 h-8 bg-gray-100 dark:bg-gray-700 rounded-lg flex items-center justify-center">
                      <Package className="w-4 h-4 text-gray-500" />
                    </div>
                    <div>
                      <p className="text-sm font-medium text-gray-800 dark:text-gray-200">{order.orderNumber}</p>
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
                    <span className="text-sm font-bold text-gray-900 dark:text-gray-100 min-w-[80px] text-right">
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