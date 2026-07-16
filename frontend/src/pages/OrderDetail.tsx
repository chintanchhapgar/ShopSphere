import { useEffect, useState } from "react";
import { useParams, Link, useNavigate, useSearchParams } from "react-router-dom";
import {
  ArrowLeft,
  Package,
  Calendar,
  Clock,
  Truck,
  CheckCircle,
  XCircle,
  RefreshCw,
  AlertCircle,
  MapPin,
  Phone,
  Receipt,
  Tag,
  CreditCard,
} from "lucide-react";
import { orderApi } from "@/api/order.api";
import type { OrderDetail as OrderDetailType } from "@/types";
import { OrderStatusColors } from "@/types";
import { formatPrice } from "@/utils/formatPrice";
import { cn } from "@/utils/cn";
import { useAppDispatch } from "@/redux/store";
import { fetchCartThunk } from "@/redux/slices/cartSlice";
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
  Refunded:   RefreshCw,
};

// ── Timeline Steps ───────────────────────────────────────────────────────────
const TIMELINE_STEPS = [
  { status: "Pending",    label: "Order Placed"  },
  { status: "Confirmed",  label: "Confirmed"     },
  { status: "Processing", label: "Processing"    },
  { status: "Shipped",    label: "Shipped"       },
  { status: "Delivered",  label: "Delivered"     },
];

const OrderDetail = () => {
  const { id }              = useParams<{ id: string }>();
  const navigate            = useNavigate();
  const dispatch            = useAppDispatch();
  const [searchParams, setSearchParams] = useSearchParams();

  const [order, setOrder]     = useState<OrderDetailType | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [isCancelling, setIsCancelling] = useState(false);

  // ── Handle Stripe Redirect Success/Cancel ──────────────────────────────────
  useEffect(() => {
    const paymentStatus = searchParams.get("payment");
    const sessionId     = searchParams.get("session_id");

    if (paymentStatus === "success") {
      toast.success("Payment successful! 🎉", { duration: 5000 });

      // ✅ Refresh cart (should be empty after order)
      dispatch(fetchCartThunk());

      console.log("Stripe session:", sessionId);

      // Clean URL params
      searchParams.delete("payment");
      searchParams.delete("session_id");
      setSearchParams(searchParams, { replace: true });
    } else if (paymentStatus === "cancelled") {
      toast.error("Payment was cancelled. You can retry payment.", { duration: 5000 });

      searchParams.delete("payment");
      setSearchParams(searchParams, { replace: true });
    }
  }, []);

  // ── Load Order ─────────────────────────────────────────────────────────────
  useEffect(() => {
    if (!id) return;
    const load = async () => {
      setIsLoading(true);
      try {
        const data = await orderApi.getOrderById(id);
        setOrder(data);
      } catch (err) {
        toast.error((err as Error).message || "Failed to load order");
      } finally {
        setIsLoading(false);
      }
    };
    load();
  }, [id]);

  // ── Cancel Order ───────────────────────────────────────────────────────────
  const handleCancel = async () => {
    if (!id) return;
    if (!window.confirm("Are you sure you want to cancel this order? This action cannot be undone.")) {
      return;
    }

    setIsCancelling(true);
    try {
      await orderApi.cancelOrder(id);
      toast.success("Order cancelled successfully");

      // Reload order
      const updated = await orderApi.getOrderById(id);
      setOrder(updated);
    } catch (err) {
      toast.error((err as Error).message || "Failed to cancel order");
    } finally {
      setIsCancelling(false);
    }
  };

  // ── Pay Now Button ─────────────────────────────────────────────────────────
  const handlePayNow = () => {
    if (!id) return;
    navigate(`/orders/${id}/payment`);
  };

  // ── Loading ────────────────────────────────────────────────────────────────
  if (isLoading) {
    return (
      <div className="flex justify-center py-32">
        <Spinner size="lg" className="text-primary-600" />
      </div>
    );
  }

  // ── Not Found ──────────────────────────────────────────────────────────────
  if (!order) {
    return (
      <div className="max-w-7xl mx-auto px-4 py-20 text-center">
        <Package className="w-16 h-16 text-gray-200 mx-auto mb-4" />
        <h2 className="text-2xl font-semibold text-gray-700 dark:text-gray-300 mb-3">
          Order not found
        </h2>
        <Link to="/orders">
          <Button>Back to Orders</Button>
        </Link>
      </div>
    );
  }

  // ── Computed Values ────────────────────────────────────────────────────────
  const Icon        = StatusIcon[order.status] || Package;
  const statusColor = OrderStatusColors[order.status] || "bg-gray-100 text-gray-700";
  const canCancel   = ["Pending", "Confirmed"].includes(order.status);
  const isCancelled = ["Cancelled", "Refunded"].includes(order.status);
  const isPending   = order.status === "Pending";
  const items       = order.items ?? [];

  const currentStepIdx = TIMELINE_STEPS.findIndex((s) => s.status === order.status);
  const totalQty       = items.reduce((acc, i) => acc + i.quantity, 0);

  return (
    <div className="max-w-5xl mx-auto px-4 py-8">

      {/* ── Back Button ────────────────────────────────────────────────────── */}
      <button
        onClick={() => navigate("/orders")}
        className="flex items-center gap-2 text-sm text-gray-500 hover:text-primary-600 mb-6 transition"
      >
        <ArrowLeft className="w-4 h-4" />
        Back to Orders
      </button>

      {/* ── Header ────────────────────────────────────────────────────────── */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-8">
        <div>
          <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">
            {order.orderNumber}
          </h1>
          <div className="flex items-center gap-3 mt-2 flex-wrap">
            <span
              className={cn(
                "inline-flex items-center gap-1.5 px-3 py-1 rounded-full text-xs font-semibold",
                statusColor
              )}
            >
              <Icon className="w-3.5 h-3.5" />
              {order.status}
            </span>
            <span className="flex items-center gap-1 text-xs text-gray-500 dark:text-gray-400">
              <Calendar className="w-3.5 h-3.5" />
              {new Date(order.orderDate).toLocaleDateString("en-IN", {
                year: "numeric", month: "long", day: "numeric",
              })}
            </span>
            <span className="flex items-center gap-1 text-xs text-gray-500 dark:text-gray-400">
              <Package className="w-3.5 h-3.5" />
              {totalQty} item{totalQty !== 1 ? "s" : ""}
            </span>
          </div>
        </div>

        {/* Action Buttons */}
        <div className="flex flex-wrap gap-2">
          {isPending && (
            <Button
              onClick={handlePayNow}
              size="md"
              className="bg-green-600 hover:bg-green-700"
            >
              <CreditCard className="w-4 h-4" />
              Pay Now
            </Button>
          )}
          {canCancel && (
            <button
              onClick={handleCancel}
              disabled={isCancelling}
              className="flex items-center gap-2 px-4 py-2 text-sm font-medium text-red-600 border border-red-200 rounded-lg hover:bg-red-50 disabled:opacity-50 transition"
            >
              {isCancelling ? (
                <Spinner size="sm" />
              ) : (
                <>
                  <XCircle className="w-4 h-4" />
                  Cancel Order
                </>
              )}
            </button>
          )}
        </div>
      </div>

      {/* ── Order Timeline ────────────────────────────────────────────────── */}
      {!isCancelled && (
        <div className="mb-8 p-6 bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 shadow-sm">
          <h3 className="text-sm font-semibold text-gray-700 dark:text-gray-300 mb-6">
            Order Progress
          </h3>
          <div className="flex items-center justify-between relative">
            {/* Background Line */}
            <div className="absolute top-4 left-0 right-0 h-0.5 bg-gray-200 dark:bg-gray-700" />

            {/* Progress Line */}
            <div
              className="absolute top-4 left-0 h-0.5 bg-primary-600 transition-all duration-500"
              style={{
                width:
                  currentStepIdx >= 0
                    ? `${(currentStepIdx / (TIMELINE_STEPS.length - 1)) * 100}%`
                    : "0%",
              }}
            />

            {TIMELINE_STEPS.map((step, idx) => {
              const isCompleted = idx <= currentStepIdx;
              const isCurrent   = idx === currentStepIdx;
              return (
                <div key={step.status} className="flex flex-col items-center relative z-10">
                  <div
                    className={cn(
                      "w-8 h-8 rounded-full flex items-center justify-center text-xs font-bold border-2 transition",
                      isCompleted
                        ? "bg-primary-600 border-primary-600 text-white"
                        : "bg-white dark:bg-gray-800 border-gray-300 dark:border-gray-600 text-gray-400"
                    )}
                  >
                    {isCompleted ? (
                      <CheckCircle className="w-4 h-4" />
                    ) : (
                      idx + 1
                    )}
                  </div>
                  <span
                    className={cn(
                      "text-xs mt-2 font-medium text-center whitespace-nowrap",
                      isCurrent
                        ? "text-primary-600"
                        : isCompleted
                        ? "text-gray-700 dark:text-gray-300"
                        : "text-gray-400"
                    )}
                  >
                    {step.label}
                  </span>
                </div>
              );
            })}
          </div>
        </div>
      )}

      {/* ── Cancelled Banner ──────────────────────────────────────────────── */}
      {isCancelled && (
        <div className="mb-8 p-4 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-xl flex items-center gap-3">
          <XCircle className="w-5 h-5 text-red-500 shrink-0" />
          <p className="text-sm text-red-700 dark:text-red-300 font-medium">
            This order has been {order.status.toLowerCase()}
          </p>
        </div>
      )}

      {/* ── Pending Payment Banner ────────────────────────────────────────── */}
      {isPending && (
        <div className="mb-8 p-4 bg-yellow-50 dark:bg-yellow-900/20 border border-yellow-200 dark:border-yellow-800 rounded-xl flex items-center justify-between gap-3">
          <div className="flex items-center gap-3">
            <AlertCircle className="w-5 h-5 text-yellow-600 shrink-0" />
            <div>
              <p className="text-sm text-yellow-800 dark:text-yellow-300 font-semibold">
                Payment Pending
              </p>
              <p className="text-xs text-yellow-700 dark:text-yellow-400 mt-0.5">
                Complete your payment to confirm this order
              </p>
            </div>
          </div>
          <Button
            onClick={handlePayNow}
            size="sm"
            className="bg-yellow-600 hover:bg-yellow-700 shrink-0"
          >
            Pay Now
          </Button>
        </div>
      )}

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">

        {/* ══════════════════════════════════════════════════════════════════ */}
        {/* ── Left: Order Items ─────────────────────────────────────────────── */}
        {/* ══════════════════════════════════════════════════════════════════ */}
        <div className="lg:col-span-2">
          <div className="bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 shadow-sm overflow-hidden">
            <div className="px-5 py-4 border-b border-gray-100 dark:border-gray-700 flex items-center justify-between">
              <h3 className="text-sm font-semibold text-gray-700 dark:text-gray-300">
                Order Items ({items.length})
              </h3>
            </div>

            <div className="divide-y divide-gray-50 dark:divide-gray-700">
              {items.map((item, idx) => {
                const imgSrc =
                  item.productImage ||
                  `https://placehold.co/80x80/e2e8f0/64748b?text=${encodeURIComponent(
                    item.productName.charAt(0)
                  )}`;

                return (
                  <div
                    key={`${item.productId}-${idx}`}
                    className="flex items-center gap-4 px-5 py-4 hover:bg-gray-50 dark:hover:bg-gray-700 transition"
                  >
                    {/* Image */}
                    <Link to={`/products/${item.productId}`} className="shrink-0">
                      <img
                        src={imgSrc}
                        alt={item.productName}
                        onError={(e) => {
                          (e.target as HTMLImageElement).src =
                            `https://placehold.co/80x80/e2e8f0/64748b?text=${item.productName.charAt(0)}`;
                        }}
                        className="w-16 h-16 rounded-lg object-cover bg-gray-50 dark:bg-gray-900 border border-gray-100 dark:border-gray-700"
                      />
                    </Link>

                    {/* Info */}
                    <div className="flex-1 min-w-0">
                      <Link
                        to={`/products/${item.productId}`}
                        className="text-sm font-semibold text-gray-800 dark:text-gray-200 hover:text-primary-600 truncate block transition"
                      >
                        {item.productName}
                      </Link>
                      {item.productSku && (
                        <p className="text-xs text-gray-400 mt-0.5 font-mono">
                          SKU: {item.productSku}
                        </p>
                      )}
                      <p className="text-xs text-gray-500 dark:text-gray-400 mt-1">
                        {item.quantity} × {formatPrice(item.unitPrice)}
                      </p>
                    </div>

                    {/* Line Total */}
                    <span className="text-sm font-bold text-gray-900 dark:text-gray-100 shrink-0">
                      {formatPrice(item.lineTotal)}
                    </span>
                  </div>
                );
              })}
            </div>
          </div>
        </div>

        {/* ══════════════════════════════════════════════════════════════════ */}
        {/* ── Right: Sidebar ────────────────────────────────────────────────── */}
        {/* ══════════════════════════════════════════════════════════════════ */}
        <div className="space-y-4">

          {/* ── Payment Summary ────────────────────────────────────────────── */}
          <div className="bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 shadow-sm p-5">
            <h3 className="text-sm font-semibold text-gray-700 dark:text-gray-300 mb-4 flex items-center gap-2">
              <Receipt className="w-4 h-4 text-gray-400" />
              Payment Summary
            </h3>
            <div className="space-y-3">
              <div className="flex justify-between text-sm text-gray-600 dark:text-gray-400">
                <span>Subtotal</span>
                <span>{formatPrice(order.subTotal)}</span>
              </div>

              {order.discountAmount > 0 && (
                <div className="flex justify-between text-sm text-green-600">
                  <span className="flex items-center gap-1">
                    <Tag className="w-3.5 h-3.5" />
                    Discount
                  </span>
                  <span>-{formatPrice(order.discountAmount)}</span>
                </div>
              )}

              <div className="flex justify-between text-sm text-gray-600 dark:text-gray-400">
                <span>Tax (GST)</span>
                <span>{formatPrice(order.taxAmount)}</span>
              </div>

              <div className="flex justify-between text-sm text-gray-600 dark:text-gray-400">
                <span className="flex items-center gap-1">
                  <Truck className="w-3.5 h-3.5" />
                  Shipping
                </span>
                <span
                  className={cn(
                    order.shippingAmount === 0 &&
                      "text-green-600 font-medium"
                  )}
                >
                  {order.shippingAmount === 0
                    ? "FREE"
                    : formatPrice(order.shippingAmount)}
                </span>
              </div>

              <hr className="border-gray-100 dark:border-gray-700" />

              <div className="flex justify-between font-bold text-lg text-gray-900 dark:text-gray-100">
                <span>Total</span>
                <span className="text-primary-600">
                  {formatPrice(order.totalAmount)}
                </span>
              </div>
            </div>
          </div>

          {/* ── Shipping Address ───────────────────────────────────────────── */}
          <div className="bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 shadow-sm p-5">
            <h3 className="text-sm font-semibold text-gray-700 dark:text-gray-300 mb-3 flex items-center gap-2">
              <MapPin className="w-4 h-4 text-gray-400" />
              Shipping Address
            </h3>
            <div className="text-sm text-gray-600 dark:text-gray-400 space-y-1.5">
              <p className="font-semibold text-gray-800 dark:text-gray-200">
                {order.shippingName}
              </p>
              <p>{order.addressLine1}</p>
              {order.addressLine2 && <p>{order.addressLine2}</p>}
              <p>
                {order.city}, {order.state} {order.postalCode}
              </p>
              <p>{order.country}</p>
              {order.phoneNumber && (
                <div className="flex items-center gap-1.5 pt-2 text-gray-500 dark:text-gray-400 border-t border-gray-100 dark:border-gray-700 mt-2">
                  <Phone className="w-3.5 h-3.5" />
                  <span>{order.phoneNumber}</span>
                </div>
              )}
            </div>
          </div>

          {/* ── Order Info ─────────────────────────────────────────────────── */}
          <div className="bg-gray-50 dark:bg-gray-900 rounded-xl border border-gray-100 dark:border-gray-700 p-5 space-y-3">
            <div>
              <h3 className="text-xs font-semibold text-gray-400 uppercase tracking-wider">
                Order Number
              </h3>
              <p className="text-sm text-gray-700 dark:text-gray-300 font-medium mt-0.5">
                {order.orderNumber}
              </p>
            </div>
            <div>
              <h3 className="text-xs font-semibold text-gray-400 uppercase tracking-wider">
                Order ID
              </h3>
              <p className="text-xs text-gray-600 dark:text-gray-400 font-mono break-all mt-0.5">
                {order.id}
              </p>
            </div>
            <div>
              <h3 className="text-xs font-semibold text-gray-400 uppercase tracking-wider">
                Order Date
              </h3>
              <p className="text-sm text-gray-700 dark:text-gray-300 mt-0.5">
                {new Date(order.orderDate).toLocaleDateString("en-IN", {
                  weekday: "long",
                  year:    "numeric",
                  month:   "long",
                  day:     "numeric",
                })}
              </p>
            </div>
          </div>

          {/* ── Need Help ──────────────────────────────────────────────────── */}
          <div className="bg-blue-50 dark:bg-blue-900/20 rounded-xl border border-blue-100 dark:border-blue-800 p-5 text-center">
            <p className="text-sm text-blue-700 dark:text-blue-300 font-medium mb-1">
              Need help with this order?
            </p>
            <p className="text-xs text-blue-600 dark:text-blue-400 mb-3">
              Contact support at support@shopsphere.com
            </p>
            <Link
              to="/contact"
              className="text-xs text-blue-600 dark:text-blue-400 font-semibold hover:underline"
            >
              Contact Support →
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};

export default OrderDetail;