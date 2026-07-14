import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import {
  Package,
  CheckCircle,
  XCircle,
  RefreshCw,
  Clock,
  CreditCard,
  Search,
  AlertCircle,
  Truck,
  ChevronDown,
  ArrowRight,
  Calendar,
} from "lucide-react";
import { adminApi } from "@/api/admin.api";
import { orderApi } from "@/api/order.api";
import type { OrderListItem, Payment } from "@/types";
import {
  OrderStatusColors,
  OrderStatusEnum,
  AdminStatusTransitions,
  PaymentStatusLabels,
  PaymentStatusColors,
  PaymentMethodLabels,
} from "@/types";
import { formatPrice } from "@/utils/formatPrice";
import { cn } from "@/utils/cn";
import Spinner from "@/components/ui/Spinner";
import Button from "@/components/ui/Button";
import toast from "react-hot-toast";

// ── Status Icons ─────────────────────────────────────────────────────────────
const StatusIcon: Record<string, React.ElementType> = {
  Pending:    Clock,
  Confirmed:  AlertCircle,
  Processing: RefreshCw,
  Shipped:    Truck,
  Delivered:  CheckCircle,
  Cancelled:  XCircle,
  Completed:  CheckCircle,
};

const STATUS_FILTERS = ["All", "Pending", "Confirmed", "Processing", "Shipped", "Delivered", "Cancelled", "Completed"];

const AdminOrders = () => {
  const [orders, setOrders]             = useState<OrderListItem[]>([]);
  const [isLoading, setIsLoading]       = useState(true);
  const [searchQuery, setSearchQuery]   = useState("");
  const [statusFilter, setStatusFilter] = useState("All");

  // ── Status Update State ────────────────────────────────────────────────────
  const [updatingOrderId, setUpdatingOrderId] = useState<string | null>(null);

  // ── Payment Modal State ────────────────────────────────────────────────────
  const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null);
  const [payment, setPayment]                 = useState<Payment | null>(null);
  const [paymentLoading, setPaymentLoading]   = useState(false);
  const [txnId, setTxnId]                     = useState("");
  const [failReason, setFailReason]           = useState("");

  // ── Load Orders ────────────────────────────────────────────────────────────
  const loadOrders = async () => {
    try {
      const data = await adminApi.getAllOrders();
      setOrders(data);
    } catch (err) {
      toast.error((err as Error).message || "Failed to load orders");
    }
  };

  useEffect(() => {
    const load = async () => {
      setIsLoading(true);
      await loadOrders();
      setIsLoading(false);
    };
    load();
  }, []);

  // ── Update Order Status ────────────────────────────────────────────────────
  const handleStatusUpdate = async (orderId: string, newStatus: string) => {
    const statusNum = OrderStatusEnum[newStatus];
    if (!statusNum) return;

    setUpdatingOrderId(orderId);
    try {
      await adminApi.updateOrderStatus(orderId, statusNum);
      toast.success(`Order updated to ${newStatus}`);
      await loadOrders();
    } catch (err) {
      toast.error((err as Error).message || "Failed to update status");
    } finally {
      setUpdatingOrderId(null);
    }
  };

  // ── Payment Actions ────────────────────────────────────────────────────────
  const handleViewPayment = async (orderId: string) => {
    setSelectedOrderId(orderId);
    setPaymentLoading(true);
    try {
      const p = await orderApi.getPayment(orderId);
      setPayment(p);
    } catch {
      setPayment(null);
    } finally {
      setPaymentLoading(false);
    }
  };

  const handlePaymentSuccess = async () => {
    if (!payment || !txnId.trim()) {
      toast.error("Enter transaction ID");
      return;
    }
    try {
      await orderApi.markPaymentSuccess(payment.id, {
        transactionId: txnId.trim(),
        gatewayReference: `REF-${Date.now()}`,
      });
      toast.success("Payment marked as Paid!");
      closePaymentModal();
      await loadOrders();
    } catch (err) {
      toast.error((err as Error).message || "Failed");
    }
  };

  const handlePaymentFailed = async () => {
    if (!payment || !failReason.trim()) {
      toast.error("Enter failure reason");
      return;
    }
    try {
      await orderApi.markPaymentFailed(payment.id, { reason: failReason.trim() });
      toast.success("Payment marked as Failed");
      closePaymentModal();
      await loadOrders();
    } catch (err) {
      toast.error((err as Error).message || "Failed");
    }
  };

  const handleRefund = async () => {
    if (!payment || !window.confirm("Refund this payment?")) return;
    try {
      await orderApi.refundPayment(payment.id);
      toast.success("Payment refunded");
      closePaymentModal();
      await loadOrders();
    } catch (err) {
      toast.error((err as Error).message || "Failed");
    }
  };

  const closePaymentModal = () => {
    setSelectedOrderId(null);
    setPayment(null);
    setTxnId("");
    setFailReason("");
  };

  // ── Filter ─────────────────────────────────────────────────────────────────
  const filtered = orders.filter((o) => {
    const matchSearch = !searchQuery || o.orderNumber.toLowerCase().includes(searchQuery.toLowerCase());
    const matchStatus = statusFilter === "All" || o.status === statusFilter;
    return matchSearch && matchStatus;
  });

  // ── Stats ──────────────────────────────────────────────────────────────────
  const stats = {
    total:      orders.length,
    pending:    orders.filter((o) => o.status === "Pending").length,
    processing: orders.filter((o) => ["Confirmed", "Processing"].includes(o.status)).length,
    shipped:    orders.filter((o) => o.status === "Shipped").length,
    delivered:  orders.filter((o) => o.status === "Delivered").length,
    cancelled:  orders.filter((o) => o.status === "Cancelled").length,
  };

  if (isLoading) {
    return (
      <div className="flex justify-center py-32">
        <Spinner size="lg" className="text-primary-600" />
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 py-8">

      {/* ── Header ──────────────────────────────────────────────────────────── */}
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900">Manage Orders</h1>
        <p className="text-gray-500 mt-1">{orders.length} total orders</p>
      </div>

      {/* ── Stats ───────────────────────────────────────────────────────────── */}
      <div className="grid grid-cols-2 md:grid-cols-6 gap-3 mb-8">
        {[
          { label: "Total",      value: stats.total,      color: "bg-blue-50 text-blue-700",   icon: Package },
          { label: "Pending",    value: stats.pending,    color: "bg-yellow-50 text-yellow-700", icon: Clock },
          { label: "Processing", value: stats.processing, color: "bg-purple-50 text-purple-700", icon: RefreshCw },
          { label: "Shipped",    value: stats.shipped,    color: "bg-indigo-50 text-indigo-700", icon: Truck },
          { label: "Delivered",  value: stats.delivered,  color: "bg-green-50 text-green-700",  icon: CheckCircle },
          { label: "Cancelled",  value: stats.cancelled,  color: "bg-red-50 text-red-700",     icon: XCircle },
        ].map(({ label, value, color, icon: Icon }) => (
          <button
            key={label}
            onClick={() => setStatusFilter(label === "Processing" ? "Processing" : label === "Total" ? "All" : label)}
            className={cn("bg-white rounded-xl border border-gray-100 shadow-sm p-3 flex items-center gap-3 hover:shadow-md transition text-left", statusFilter === label && "ring-2 ring-primary-500")}
          >
            <div className={cn("p-2 rounded-lg", color)}><Icon className="w-4 h-4" /></div>
            <div>
              <p className="text-xl font-bold text-gray-900">{value}</p>
              <p className="text-xs text-gray-500">{label}</p>
            </div>
          </button>
        ))}
      </div>

      {/* ── Filters ─────────────────────────────────────────────────────────── */}
      <div className="flex flex-col sm:flex-row gap-3 mb-6">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
          <input type="text" value={searchQuery} onChange={(e) => setSearchQuery(e.target.value)} placeholder="Search by order number..." className="w-full pl-10 pr-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-primary-500/20" />
        </div>
        <div className="flex flex-wrap gap-2">
          {STATUS_FILTERS.map((s) => (
            <button key={s} onClick={() => setStatusFilter(s)} className={cn("px-3 py-2 rounded-lg text-xs font-medium transition", statusFilter === s ? "bg-primary-600 text-white" : "bg-gray-100 text-gray-600 hover:bg-gray-200")}>
              {s}
            </button>
          ))}
        </div>
      </div>

      {/* ── Orders List ─────────────────────────────────────────────────────── */}
      {filtered.length === 0 ? (
        <div className="text-center py-16 bg-gray-50 rounded-xl border border-gray-100">
          <Package className="w-14 h-14 text-gray-200 mx-auto mb-3" />
          <p className="text-gray-600 font-semibold">No orders found</p>
        </div>
      ) : (
        <div className="space-y-3">
          {filtered.map((order) => {
            const Icon = StatusIcon[order.status] || Package;
            const statusColor = OrderStatusColors[order.status] || "bg-gray-100 text-gray-700";
            const allowedTransitions = AdminStatusTransitions[order.status] || [];
            const isUpdating = updatingOrderId === order.id;

            return (
              <div key={order.id} className="bg-white rounded-xl border border-gray-100 shadow-sm hover:shadow-md transition">
                <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 p-5">

                  {/* Left: Order Info */}
                  <div className="flex items-center gap-4">
                    <div className={cn("w-10 h-10 rounded-xl flex items-center justify-center shrink-0", statusColor.replace("text-", "bg-").split(" ")[0])}>
                      <Icon className="w-5 h-5" />
                    </div>
                    <div>
                      <div className="flex items-center gap-2 flex-wrap">
                        <Link to={`/orders/${order.id}`} className="text-sm font-bold text-primary-600 hover:underline">
                          {order.orderNumber}
                        </Link>
                        <span className={cn("px-2.5 py-0.5 rounded-full text-xs font-semibold", statusColor)}>
                          {order.status}
                        </span>
                      </div>
                      <div className="flex items-center gap-3 mt-1">
                        <span className="flex items-center gap-1 text-xs text-gray-500">
                          <Calendar className="w-3 h-3" />
                          {new Date(order.orderDate).toLocaleDateString("en-IN", { month: "short", day: "numeric", year: "numeric" })}
                        </span>
                        <span className="text-xs text-gray-400">•</span>
                        <span className="text-xs text-gray-500">{order.totalItems} items</span>
                        <span className="text-xs text-gray-400">•</span>
                        <span className="text-sm font-bold text-gray-900">{formatPrice(order.totalAmount)}</span>
                      </div>
                    </div>
                  </div>

                  {/* Right: Actions */}
                  <div className="flex items-center gap-2 flex-wrap">

                    {/* Status Update Buttons */}
                    {allowedTransitions.length > 0 && (
                      <div className="flex items-center gap-1">
                        {allowedTransitions.map((newStatus) => {
                          const isCancel = newStatus === "Cancelled";
                          return (
                            <button
                              key={newStatus}
                              onClick={() => {
                                if (isCancel && !window.confirm("Cancel this order?")) return;
                                handleStatusUpdate(order.id, newStatus);
                              }}
                              disabled={isUpdating}
                              className={cn(
                                "flex items-center gap-1 px-3 py-1.5 text-xs font-medium rounded-lg transition",
                                isUpdating && "opacity-50",
                                isCancel
                                  ? "text-red-600 border border-red-200 hover:bg-red-50"
                                  : "text-primary-600 border border-primary-200 hover:bg-primary-50"
                              )}
                            >
                              {isUpdating ? (
                                <Spinner size="sm" />
                              ) : (
                                <>
                                  <ArrowRight className="w-3 h-3" />
                                  {newStatus}
                                </>
                              )}
                            </button>
                          );
                        })}
                      </div>
                    )}

                    {/* Payment Button */}
                    <button
                      onClick={() => handleViewPayment(order.id)}
                      className="flex items-center gap-1 px-3 py-1.5 text-xs font-medium text-gray-600 border border-gray-200 rounded-lg hover:bg-gray-50 transition"
                    >
                      <CreditCard className="w-3 h-3" />
                      Payment
                    </button>

                    {/* View Detail */}
                    <Link
                      to={`/orders/${order.id}`}
                      className="flex items-center gap-1 px-3 py-1.5 text-xs font-medium text-gray-600 border border-gray-200 rounded-lg hover:bg-gray-50 transition"
                    >
                      Details
                    </Link>
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      )}

      {/* ── Results Count ───────────────────────────────────────────────────── */}
      {filtered.length > 0 && (
        <p className="text-center text-sm text-gray-400 mt-6">
          Showing {filtered.length} of {orders.length} orders
        </p>
      )}

      {/* ── Payment Modal ─────────────────────────────────────────────────────── */}
      {selectedOrderId && (
        <>
          <div className="fixed inset-0 bg-black/50 z-40" onClick={closePaymentModal} />
          <div className="fixed inset-0 flex items-center justify-center z-50 p-4">
            <div className="bg-white rounded-2xl shadow-xl w-full max-w-lg p-6 max-h-[90vh] overflow-y-auto">
              <h3 className="text-lg font-bold text-gray-900 mb-4 flex items-center gap-2">
                <CreditCard className="w-5 h-5 text-gray-400" />
                Payment Details
              </h3>

              {paymentLoading ? (
                <div className="flex justify-center py-8"><Spinner className="text-primary-600" /></div>
              ) : !payment ? (
                <div className="text-center py-8">
                  <AlertCircle className="w-12 h-12 text-gray-200 mx-auto mb-3" />
                  <p className="text-gray-600 font-medium">No payment found</p>
                  <p className="text-sm text-gray-400 mt-1">Payment hasn't been initiated yet</p>
                </div>
              ) : (
                <>
                  {/* Payment Info */}
                  <div className="bg-gray-50 rounded-xl p-4 mb-4 space-y-2">
                    <div className="flex justify-between text-sm">
                      <span className="text-gray-500">Payment ID</span>
                      <span className="font-mono text-xs text-gray-700">{payment.id.slice(0, 16)}...</span>
                    </div>
                    <div className="flex justify-between text-sm">
                      <span className="text-gray-500">Amount</span>
                      <span className="font-bold text-gray-900">{formatPrice(payment.amount)}</span>
                    </div>
                    <div className="flex justify-between text-sm">
                      <span className="text-gray-500">Method</span>
                      <span className="font-medium">{PaymentMethodLabels[payment.method] || "Unknown"}</span>
                    </div>
                    <div className="flex justify-between text-sm">
                      <span className="text-gray-500">Status</span>
                      <span className={cn("px-2 py-0.5 rounded-full text-xs font-semibold", PaymentStatusColors[payment.status])}>
                        {PaymentStatusLabels[payment.status] || "Unknown"}
                      </span>
                    </div>
                    {payment.transactionId && (
                      <div className="flex justify-between text-sm">
                        <span className="text-gray-500">Transaction</span>
                        <span className="font-mono text-xs">{payment.transactionId}</span>
                      </div>
                    )}
                  </div>

                  {/* Pending → Success or Failed */}
                  {payment.status === 1 && (
                    <div className="space-y-4">
                      <div className="p-4 bg-green-50 border border-green-200 rounded-xl">
                        <p className="text-sm font-medium text-green-700 mb-2">Mark as Paid</p>
                        <input type="text" value={txnId} onChange={(e) => setTxnId(e.target.value)} placeholder="Transaction ID (e.g. TXN123456)" className="w-full px-3 py-2 border border-green-200 rounded-lg text-sm mb-2 focus:outline-none focus:ring-2 focus:ring-green-500/20" />
                        <button onClick={handlePaymentSuccess} className="w-full px-4 py-2 bg-green-600 text-white text-sm font-medium rounded-lg hover:bg-green-700 transition flex items-center justify-center gap-2">
                          <CheckCircle className="w-4 h-4" /> Confirm Payment
                        </button>
                      </div>
                      <div className="p-4 bg-red-50 border border-red-200 rounded-xl">
                        <p className="text-sm font-medium text-red-700 mb-2">Mark as Failed</p>
                        <input type="text" value={failReason} onChange={(e) => setFailReason(e.target.value)} placeholder="Reason (e.g. Insufficient funds)" className="w-full px-3 py-2 border border-red-200 rounded-lg text-sm mb-2 focus:outline-none focus:ring-2 focus:ring-red-500/20" />
                        <button onClick={handlePaymentFailed} className="w-full px-4 py-2 bg-red-600 text-white text-sm font-medium rounded-lg hover:bg-red-700 transition flex items-center justify-center gap-2">
                          <XCircle className="w-4 h-4" /> Mark Failed
                        </button>
                      </div>
                    </div>
                  )}

                  {/* Paid → Refund */}
                  {payment.status === 2 && (
                    <button onClick={handleRefund} className="w-full px-4 py-2 bg-gray-600 text-white text-sm font-medium rounded-lg hover:bg-gray-700 transition flex items-center justify-center gap-2">
                      <RefreshCw className="w-4 h-4" /> Refund Payment
                    </button>
                  )}

                  {/* Failed or Refunded */}
                  {(payment.status === 3 || payment.status === 4) && (
                    <p className="text-sm text-gray-500 text-center py-2">
                      No actions available for {PaymentStatusLabels[payment.status]?.toLowerCase()} payments
                    </p>
                  )}
                </>
              )}

              <button onClick={closePaymentModal} className="w-full mt-4 px-4 py-2 border border-gray-200 text-gray-700 text-sm font-medium rounded-lg hover:bg-gray-50 transition">
                Close
              </button>
            </div>
          </div>
        </>
      )}
    </div>
  );
};

export default AdminOrders;