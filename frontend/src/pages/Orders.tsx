import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import {
  Package,
  ShoppingBag,
  ArrowRight,
  ChevronRight,
  Clock,
  Truck,
  CheckCircle,
  XCircle,
  RefreshCw,
  AlertCircle,
  Calendar,
  Search,
  IndianRupee,
} from "lucide-react";
import { orderApi } from "@/api/order.api";
import { useAuth } from "@/hooks/useAuth";
import type { OrderListItem } from "@/types";
import { OrderStatusColors } from "@/types";
import { formatPrice } from "@/utils/formatPrice";
import { cn } from "@/utils/cn";
import Spinner from "@/components/ui/Spinner";
import Button from "@/components/ui/Button";
import toast from "react-hot-toast";

// ── Status Icon Map ──────────────────────────────────────────────────────────
const StatusIcon: Record<string, React.ElementType> = {
  Pending:    Clock,
  Confirmed:  AlertCircle,
  Processing: RefreshCw,
  Shipped:    Truck,
  Delivered:  CheckCircle,
  Cancelled:  XCircle,
  Refunded:   RefreshCw,
};

const ACTIVE_STATUSES  = ["Pending", "Confirmed", "Processing", "Shipped"];
const CANCEL_STATUSES  = ["Pending", "Confirmed"];

const STATUS_FILTERS = [
  "All",
  "Pending",
  "Processing",
  "Shipped",
  "Delivered",
  "Cancelled",
];

const Orders = () => {
  const { isAuthenticated } = useAuth();

  const [orders, setOrders]             = useState<OrderListItem[]>([]);
  const [isLoading, setIsLoading]       = useState(true);
  const [error, setError]               = useState<string | null>(null);
  const [searchQuery, setSearchQuery]   = useState("");
  const [statusFilter, setStatusFilter] = useState("All");

  // ── Load Orders ────────────────────────────────────────────────────────────
  useEffect(() => {
    if (!isAuthenticated) return;
    const load = async () => {
      setIsLoading(true);
      setError(null);
      try {
        const data = await orderApi.getMyOrders();
        setOrders(data);
      } catch (err) {
        setError((err as Error).message || "Failed to load orders");
      } finally {
        setIsLoading(false);
      }
    };
    load();
  }, [isAuthenticated]);

  // ── Cancel Order ───────────────────────────────────────────────────────────
  const handleCancel = async (orderId: string) => {
    if (!window.confirm("Are you sure you want to cancel this order?")) return;
    try {
      await orderApi.cancelOrder(orderId);
      toast.success("Order cancelled");
      const updated = await orderApi.getMyOrders();
      setOrders(updated);
    } catch (err) {
      toast.error((err as Error).message || "Failed to cancel order");
    }
  };

  // ── Filter Orders ──────────────────────────────────────────────────────────
  const filteredOrders = orders.filter((order) => {
    const matchesSearch =
      !searchQuery ||
      order.orderNumber.toLowerCase().includes(searchQuery.toLowerCase()) ||
      order.id.toLowerCase().includes(searchQuery.toLowerCase());

    const matchesStatus =
      statusFilter === "All" || order.status === statusFilter;

    return matchesSearch && matchesStatus;
  });

  // ── Stats ──────────────────────────────────────────────────────────────────
  const totalOrders  = orders.length;
  const activeOrders = orders.filter((o) => ACTIVE_STATUSES.includes(o.status)).length;
  const delivered    = orders.filter((o) => o.status === "Delivered").length;
  const cancelled    = orders.filter((o) => o.status === "Cancelled").length;

  // ── Not Authenticated ──────────────────────────────────────────────────────
  if (!isAuthenticated) {
    return (
      <div className="max-w-7xl mx-auto px-4 py-24 text-center">
        <Package className="w-20 h-20 text-gray-200 mx-auto mb-6" />
        <h2 className="text-2xl font-bold text-gray-700 mb-3">
          Login to view your orders
        </h2>
        <Link to="/login" state={{ from: "/orders" }}>
          <Button size="lg">Login</Button>
        </Link>
      </div>
    );
  }

  // ── Loading ────────────────────────────────────────────────────────────────
  if (isLoading) {
    return (
      <div className="flex justify-center py-32">
        <Spinner size="lg" className="text-primary-600" />
      </div>
    );
  }

  // ── Error ──────────────────────────────────────────────────────────────────
  if (error) {
    return (
      <div className="max-w-7xl mx-auto px-4 py-20 text-center">
        <AlertCircle className="w-16 h-16 text-red-300 mx-auto mb-4" />
        <h2 className="text-xl font-semibold text-gray-700 mb-2">Failed to load orders</h2>
        <p className="text-red-500 text-sm mb-6">{error}</p>
        <Button onClick={() => window.location.reload()}>Try Again</Button>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 py-8">

      {/* ── Header ──────────────────────────────────────────────────────────── */}
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900">My Orders</h1>
        <p className="text-gray-500 mt-1">Track and manage your orders</p>
      </div>

      {/* ── Stats Cards ─────────────────────────────────────────────────────── */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-8">
        {[
          { label: "Total Orders", value: totalOrders,  color: "bg-blue-50 text-blue-700",    icon: Package     },
          { label: "Active",       value: activeOrders, color: "bg-purple-50 text-purple-700", icon: RefreshCw   },
          { label: "Delivered",    value: delivered,     color: "bg-green-50 text-green-700",   icon: CheckCircle },
          { label: "Cancelled",    value: cancelled,     color: "bg-red-50 text-red-700",       icon: XCircle     },
        ].map(({ label, value, color, icon: Icon }) => (
          <div
            key={label}
            className="bg-white rounded-xl border border-gray-100 shadow-sm p-4 flex items-center gap-3"
          >
            <div className={cn("p-2.5 rounded-xl", color)}>
              <Icon className="w-5 h-5" />
            </div>
            <div>
              <p className="text-2xl font-bold text-gray-900">{value}</p>
              <p className="text-xs text-gray-500">{label}</p>
            </div>
          </div>
        ))}
      </div>

      {/* ── Search & Filter ─────────────────────────────────────────────────── */}
      <div className="flex flex-col sm:flex-row gap-3 mb-6">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
          <input
            type="text"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            placeholder="Search by order number..."
            className="w-full pl-10 pr-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-primary-500/20 focus:border-primary-400 transition"
          />
        </div>
        <div className="flex flex-wrap gap-2">
          {STATUS_FILTERS.map((s) => (
            <button
              key={s}
              onClick={() => setStatusFilter(s)}
              className={cn(
                "px-3 py-2 rounded-lg text-sm font-medium transition",
                statusFilter === s
                  ? "bg-primary-600 text-white"
                  : "bg-gray-100 text-gray-600 hover:bg-gray-200"
              )}
            >
              {s}
            </button>
          ))}
        </div>
      </div>

      {/* ── Empty State ─────────────────────────────────────────────────────── */}
      {filteredOrders.length === 0 && (
        <div className="text-center py-20 bg-gray-50 rounded-xl border border-gray-100">
          <ShoppingBag className="w-16 h-16 text-gray-200 mx-auto mb-4" />
          <h3 className="text-xl font-semibold text-gray-700 mb-2">
            {orders.length === 0 ? "No orders yet" : "No orders match your filters"}
          </h3>
          <p className="text-gray-500 mb-6">
            {orders.length === 0
              ? "Start shopping and your orders will appear here"
              : "Try different search or filter options"}
          </p>
          {orders.length === 0 ? (
            <Link to="/products">
              <Button size="lg">Start Shopping <ArrowRight className="w-5 h-5" /></Button>
            </Link>
          ) : (
            <Button variant="outline" onClick={() => { setSearchQuery(""); setStatusFilter("All"); }}>
              Clear Filters
            </Button>
          )}
        </div>
      )}

      {/* ── Orders List ─────────────────────────────────────────────────────── */}
      <div className="space-y-4">
        {filteredOrders.map((order) => {
          const Icon        = StatusIcon[order.status] || Package;
          const statusColor = OrderStatusColors[order.status] || "bg-gray-100 text-gray-700";
          const canCancel   = CANCEL_STATUSES.includes(order.status);

          return (
            <div
              key={order.id}
              className="bg-white rounded-xl border border-gray-100 shadow-sm hover:shadow-md transition overflow-hidden"
            >
              {/* ── Order Row ──────────────────────────────────────────────── */}
              <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 p-5">

                {/* Left: Status + Order Info */}
                <div className="flex items-center gap-4">
                  {/* Status Icon Circle */}
                  <div
                    className={cn(
                      "w-12 h-12 rounded-xl flex items-center justify-center shrink-0",
                      OrderStatusColors[order.status]?.replace("text-", "bg-").replace("100", "50") || "bg-gray-50"
                    )}
                  >
                    <Icon className="w-5 h-5" />
                  </div>

                  {/* Order Info */}
                  <div>
                    <div className="flex items-center gap-2 flex-wrap">
                      <h3 className="text-sm font-bold text-gray-900">
                        {order.orderNumber}
                      </h3>
                      <span
                        className={cn(
                          "inline-flex items-center gap-1 px-2.5 py-0.5 rounded-full text-xs font-semibold",
                          statusColor
                        )}
                      >
                        {order.status}
                      </span>
                    </div>
                    <div className="flex items-center gap-3 mt-1.5">
                      <span className="flex items-center gap-1 text-xs text-gray-500">
                        <Calendar className="w-3.5 h-3.5" />
                        {new Date(order.orderDate).toLocaleDateString("en-IN", {
                          year: "numeric",
                          month: "short",
                          day: "numeric",
                        })}
                      </span>
                      <span className="text-xs text-gray-400">•</span>
                      <span className="flex items-center gap-1 text-xs text-gray-500">
                        <Package className="w-3.5 h-3.5" />
                        {order.totalItems} item{order.totalItems !== 1 ? "s" : ""}
                      </span>
                    </div>
                  </div>
                </div>

                {/* Right: Price + Actions */}
                <div className="flex items-center gap-4 sm:shrink-0">
                  <div className="text-right">
                    <p className="text-lg font-bold text-gray-900">
                      {formatPrice(order.totalAmount)}
                    </p>
                  </div>

                  <div className="flex items-center gap-2">
                    {canCancel && (
                      <button
                        onClick={(e) => {
                          e.preventDefault();
                          handleCancel(order.id);
                        }}
                        className="px-3 py-1.5 text-xs font-medium text-red-600 border border-red-200 rounded-lg hover:bg-red-50 transition"
                      >
                        Cancel
                      </button>
                    )}
                    <Link
                      to={`/orders/${order.id}`}
                      className="flex items-center gap-1 px-3 py-1.5 text-xs font-medium text-primary-600 border border-primary-200 rounded-lg hover:bg-primary-50 transition"
                    >
                      Details
                      <ChevronRight className="w-3.5 h-3.5" />
                    </Link>
                  </div>
                </div>
              </div>

              {/* ── Status Progress Bar ────────────────────────────────────── */}
              {!["Cancelled", "Refunded"].includes(order.status) && (
                <div className="px-5 pb-4">
                  <OrderProgressBar status={order.status} />
                </div>
              )}
            </div>
          );
        })}
      </div>

      {/* ── Count ───────────────────────────────────────────────────────────── */}
      {filteredOrders.length > 0 && (
        <p className="text-center text-sm text-gray-400 mt-6">
          Showing {filteredOrders.length} of {orders.length} orders
        </p>
      )}
    </div>
  );
};

// ── Order Progress Bar Component ─────────────────────────────────────────────
const PROGRESS_STEPS = [
  { status: "Pending",    label: "Placed"     },
  { status: "Confirmed",  label: "Confirmed"  },
  { status: "Processing", label: "Processing" },
  { status: "Shipped",    label: "Shipped"    },
  { status: "Delivered",  label: "Delivered"  },
];

const OrderProgressBar = ({ status }: { status: string }) => {
  const currentIdx = PROGRESS_STEPS.findIndex((s) => s.status === status);

  return (
    <div className="flex items-center gap-1">
      {PROGRESS_STEPS.map((step, idx) => {
        const isCompleted = idx <= currentIdx;
        const isCurrent   = idx === currentIdx;

        return (
          <div key={step.status} className="flex-1 flex flex-col items-center">
            {/* Bar */}
            <div className="w-full flex items-center">
              <div
                className={cn(
                  "h-1.5 w-full rounded-full transition-all duration-500",
                  isCompleted
                    ? "bg-primary-500"
                    : "bg-gray-200"
                )}
              />
            </div>
            {/* Label - only show current */}
            {isCurrent && (
              <span className="text-xs text-primary-600 font-medium mt-1">
                {step.label}
              </span>
            )}
          </div>
        );
      })}
    </div>
  );
};

export default Orders;