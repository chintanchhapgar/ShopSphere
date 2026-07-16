import { useEffect, useState } from "react";
import {
  BarChart, Bar, LineChart, Line, PieChart, Pie, Cell,
  AreaChart, Area,
  XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer,
} from "recharts";
import { TrendingUp, IndianRupee, ShoppingCart, Package, Users, Calendar } from "lucide-react";
import { adminApi } from "@/api/admin.api";
import { orderApi } from "@/api/order.api";
import type { OrderListItem } from "@/types";
import { formatPrice } from "@/utils/formatPrice";
import { cn } from "@/utils/cn";
import Spinner from "@/components/ui/Spinner";

const COLORS = ["#2563eb", "#22c55e", "#eab308", "#ef4444", "#a855f7", "#6366f1"];

const AdminAnalytics = () => {
  const [salesData, setSalesData] = useState<any[]>([]);
  const [orders, setOrders]       = useState<OrderListItem[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [period, setPeriod]       = useState(30);

  useEffect(() => {
    const load = async () => {
      setIsLoading(true);
      try {
        const [sales, ordersData] = await Promise.allSettled([
          adminApi.getSalesAnalytics(period),
          adminApi.getAllOrders(),
        ]);

        if (sales.status === "fulfilled") setSalesData(sales.value);
        if (ordersData.status === "fulfilled") setOrders(ordersData.value);
      } finally {
        setIsLoading(false);
      }
    };
    load();
  }, [period]);

  if (isLoading) {
    return <div className="flex justify-center py-32"><Spinner size="lg" className="text-primary-600" /></div>;
  }

  // ── Compute Stats ───────────────────────────────────────────────────────────
  const totalRevenue = orders.reduce((sum, o) => sum + o.totalAmount, 0);
  const avgOrderValue = orders.length > 0 ? totalRevenue / orders.length : 0;

  const statusData = [
    { name: "Pending",    value: orders.filter(o => o.status === "Pending").length },
    { name: "Confirmed",  value: orders.filter(o => o.status === "Confirmed").length },
    { name: "Processing", value: orders.filter(o => o.status === "Processing").length },
    { name: "Shipped",    value: orders.filter(o => o.status === "Shipped").length },
    { name: "Delivered",  value: orders.filter(o => o.status === "Delivered").length },
    { name: "Cancelled",  value: orders.filter(o => o.status === "Cancelled").length },
    { name: "Completed",  value: orders.filter(o => o.status === "Completed").length },
  ].filter(d => d.value > 0);

  const chartData = salesData.map(d => ({
    date: new Date(d.date).toLocaleDateString("en-IN", { month: "short", day: "numeric" }),
    revenue: d.revenue,
    orders: d.orders,
  }));

  return (
    <div className="max-w-7xl mx-auto px-4 py-8">

      {/* Header */}
      <div className="flex items-center justify-between mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 dark:text-gray-100">
            Analytics Dashboard
          </h1>
          <p className="text-gray-500 mt-1">Business insights and metrics</p>
        </div>
        <div className="flex gap-2">
          {[7, 30, 90].map((d) => (
            <button
              key={d}
              onClick={() => setPeriod(d)}
              className={cn(
                "px-4 py-2 rounded-lg text-sm font-medium transition",
                period === d
                  ? "bg-primary-600 text-white"
                  : "bg-gray-100 text-gray-600 hover:bg-gray-200"
              )}
            >
              {d} Days
            </button>
          ))}
        </div>
      </div>

      {/* KPI Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
        {[
          { label: "Total Revenue",   value: formatPrice(totalRevenue),   icon: IndianRupee,  color: "bg-green-50 text-green-600", trend: "+12.5%" },
          { label: "Total Orders",    value: orders.length.toString(),    icon: ShoppingCart, color: "bg-blue-50 text-blue-600",   trend: "+8.2%" },
          { label: "Avg Order Value", value: formatPrice(avgOrderValue),  icon: Package,      color: "bg-purple-50 text-purple-600", trend: "+5.3%" },
          { label: "Growth",          value: "+18.5%",                    icon: TrendingUp,   color: "bg-orange-50 text-orange-600", trend: "vs last month" },
        ].map(({ label, value, icon: Icon, color, trend }) => (
          <div key={label} className="bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 shadow-sm p-5">
            <div className="flex items-start justify-between mb-3">
              <div className={cn("p-2.5 rounded-xl", color)}>
                <Icon className="w-5 h-5" />
              </div>
              <span className="text-xs font-medium text-green-600 bg-green-50 px-2 py-0.5 rounded-full">
                {trend}
              </span>
            </div>
            <p className="text-2xl font-bold text-gray-900 dark:text-gray-100">{value}</p>
            <p className="text-xs text-gray-500 mt-1">{label}</p>
          </div>
        ))}
      </div>

      {/* Revenue Chart */}
      <div className="bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 shadow-sm p-6 mb-6">
        <h2 className="text-lg font-bold text-gray-900 dark:text-gray-100 mb-4">
          Revenue Trend
        </h2>
        <ResponsiveContainer width="100%" height={350}>
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
              contentStyle={{ backgroundColor: "#1f2937", border: "none", borderRadius: "8px", color: "#fff" }}
              formatter={(value: number) => [`₹${value.toLocaleString()}`, "Revenue"]}
            />
            <Area type="monotone" dataKey="revenue" stroke="#2563eb" strokeWidth={2} fill="url(#colorRevenue)" />
          </AreaChart>
        </ResponsiveContainer>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">

        {/* Orders Bar Chart */}
        <div className="bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 shadow-sm p-6">
          <h2 className="text-lg font-bold text-gray-900 dark:text-gray-100 mb-4">
            Daily Orders
          </h2>
          <ResponsiveContainer width="100%" height={300}>
            <BarChart data={chartData}>
              <CartesianGrid strokeDasharray="3 3" stroke="#e5e7eb" />
              <XAxis dataKey="date" tick={{ fill: "#6b7280", fontSize: 12 }} />
              <YAxis tick={{ fill: "#6b7280", fontSize: 12 }} />
              <Tooltip
                contentStyle={{ backgroundColor: "#1f2937", border: "none", borderRadius: "8px", color: "#fff" }}
              />
              <Bar dataKey="orders" fill="#8b5cf6" radius={[8, 8, 0, 0]} />
            </BarChart>
          </ResponsiveContainer>
        </div>

        {/* Status Pie */}
        <div className="bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 shadow-sm p-6">
          <h2 className="text-lg font-bold text-gray-900 dark:text-gray-100 mb-4">
            Order Status Distribution
          </h2>
          <ResponsiveContainer width="100%" height={300}>
            <PieChart>
              <Pie
                data={statusData}
                cx="50%"
                cy="50%"
                labelLine={false}
                label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}
                outerRadius={100}
                fill="#8884d8"
                dataKey="value"
              >
                {statusData.map((_, index) => (
                  <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                ))}
              </Pie>
              <Tooltip
                contentStyle={{ backgroundColor: "#1f2937", border: "none", borderRadius: "8px", color: "#fff" }}
              />
            </PieChart>
          </ResponsiveContainer>
        </div>
      </div>
    </div>
  );
};

export default AdminAnalytics;