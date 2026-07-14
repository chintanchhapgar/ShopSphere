import { useEffect, useState } from "react";
import { useParams, Link, useNavigate } from "react-router-dom";
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
} from "lucide-react";
import { orderApi } from "@/api/order.api";
import type { OrderDetail as OrderDetailType } from "@/types";
import { OrderStatusColors } from "@/types";
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
  const { id }   = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [order, setOrder]         = useState<OrderDetailType | null>(null);
  const [isLoading, setIsLoading] = useState(true);

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

  // ── Cancel ─────────────────────────────────────────────────────────────────
  const handleCancel = async () => {
    if (!id || !window.confirm("Are you sure you want to cancel this order?"))
      return;
    try {
      await orderApi.cancelOrder(id);
      toast.success("Order cancelled successfully");
      const updated = await orderApi.getOrderById(id);
      setOrder(updated);
    } catch (err) {
      toast.error((err as Error).message || "Failed to cancel");
    }
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
        <h2 className="text-2xl font-semibold text-gray-700 mb-3">
          Order not found
        </h2>
        <Link to="/orders">
          <Button>Back to Orders</Button>
        </Link>
      </div>
    );
  }

  // ── Computed ────────────────────────────────────────────────────────────────
  const Icon        = StatusIcon[order.status] || Package;
  const statusColor = OrderStatusColors[order.status] || "bg-gray-100 text-gray-700";
  const canCancel   = ["Pending", "Confirmed"].includes(order.status);
  const isCancelled = ["Cancelled", "Refunded"].includes(order.status);
  const items       = order.items ?? [];

  const currentStepIdx = TIMELINE_STEPS.findIndex(
    (s) => s.status === order.status
  );

  const totalQty = items.reduce((acc, i) => acc + i.quantity, 0);

  return (
    <div className="max-w-5xl mx-auto px-4 py-8">

      {/* ── Back ──────────────────────────────────────────────────────────── */}
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
          <h1 className="text-2xl font-bold text-gray-900">
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
            <span className="flex items-center gap-1 text-xs text-gray-500">
              <Calendar className="w-3.5 h-3.5" />
              {new Date(order.orderDate).toLocaleDateString("en-IN", {
                year: "numeric",
                month: "long",
                day: "numeric",
              })}
            </span>
            <span className="flex items-center gap-1 text-xs text-gray-500">
              <Package className="w-3.5 h-3.5" />
              {totalQty} item{totalQty !== 1 ? "s" : ""}
            </span>
          </div>
        </div>
        {canCancel && (
          <button
            onClick={handleCancel}
            className="px-4 py-2 text-sm font-medium text-red-600 border border-red-200 rounded-lg hover:bg-red-50 transition"
          >
            Cancel Order
          </button>
        )}
      </div>

      {/* ── Order Timeline ────────────────────────────────────────────────── */}
      {!isCancelled && (
        <div className="mb-8 p-6 bg-white rounded-xl border border-gray-100 shadow-sm">
          <h3 className="text-sm font-semibold text-gray-700 mb-6">
            Order Progress
          </h3>
          <div className="flex items-center justify-between relative">
            {/* Background line */}
            <div className="absolute top-4 left-0 right-0 h-0.5 bg-gray-200" />
            {/* Progress line */}
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
                <div
                  key={step.status}
                  className="flex flex-col items-center relative z-10"
                >
                  <div
                    className={cn(
                      "w-8 h-8 rounded-full flex items-center justify-center text-xs font-bold border-2 transition",
                      isCompleted
                        ? "bg-primary-600 border-primary-600 text-white"
                        : "bg-white border-gray-300 text-gray-400"
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
                        ? "text-gray-700"
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
        <div className="mb-8 p-4 bg-red-50 border border-red-200 rounded-xl flex items-center gap-3">
          <XCircle className="w-5 h-5 text-red-500 shrink-0" />
          <p className="text-sm text-red-700 font-medium">
            This order has been {order.status.toLowerCase()}
          </p>
        </div>
      )}

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">

        {/* ── Order Items ─────────────────────────────────────────────────── */}
        <div className="lg:col-span-2">
          <div className="bg-white rounded-xl border border-gray-100 shadow-sm overflow-hidden">
            <div className="px-5 py-4 border-b border-gray-100 flex items-center justify-between">
              <h3 className="text-sm font-semibold text-gray-700">
                Order Items ({items.length})
              </h3>
            </div>

            <div className="divide-y divide-gray-50">
              {items.map((item, idx) => {
                const imgSrc =
                  item.productImage ||
                  `https://placehold.co/80x80/e2e8f0/64748b?text=${encodeURIComponent(
                    item.productName.charAt(0)
                  )}`;

                return (
                  <div
                    key={`${item.productId}-${idx}`}
                    className="flex items-center gap-4 px-5 py-4 hover:bg-gray-50 transition"
                  >
                    {/* Image */}
                    <Link to={`/products/${item.productId}`} className="shrink-0">
                      <img
                        src={imgSrc}
                        alt={item.productName}
                        onError={(e) => {
                          (e.target as HTMLImageElement).src = `https://placehold.co/80x80/e2e8f0/64748b?text=${item.productName.charAt(0)}`;
                        }}
                        className="w-16 h-16 rounded-lg object-cover bg-gray-50 border border-gray-100"
                      />
                    </Link>

                    {/* Info */}
                    <div className="flex-1 min-w-0">
                      <Link
                        to={`/products/${item.productId}`}
                        className="text-sm font-semibold text-gray-800 hover:text-primary-600 truncate block transition"
                      >
                        {item.productName}
                      </Link>
                      <p className="text-xs text-gray-400 mt-0.5 font-mono">
                        SKU: {item.productSku}
                      </p>
                      <p className="text-xs text-gray-500 mt-1">
                        {item.quantity} × {formatPrice(item.unitPrice)}
                      </p>
                    </div>

                    {/* Line Total */}
                    <span className="text-sm font-bold text-gray-900 shrink-0">
                      {formatPrice(item.lineTotal)}
                    </span>
                  </div>
                );
              })}
            </div>
          </div>
        </div>

        {/* ── Sidebar ─────────────────────────────────────────────────────── */}
        <div className="space-y-4">

          {/* ── Payment Summary ────────────────────────────────────────────── */}
          <div className="bg-white rounded-xl border border-gray-100 shadow-sm p-5">
            <h3 className="text-sm font-semibold text-gray-700 mb-4 flex items-center gap-2">
              <Receipt className="w-4 h-4 text-gray-400" />
              Payment Summary
            </h3>
            <div className="space-y-3">
              <div className="flex justify-between text-sm text-gray-600">
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

              <div className="flex justify-between text-sm text-gray-600">
                <span>Tax (GST)</span>
                <span>{formatPrice(order.taxAmount)}</span>
              </div>

              <div className="flex justify-between text-sm text-gray-600">
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

              <hr className="border-gray-100" />

              <div className="flex justify-between font-bold text-lg text-gray-900">
                <span>Total</span>
                <span className="text-primary-600">
                  {formatPrice(order.totalAmount)}
                </span>
              </div>
            </div>
          </div>

          {/* ── Shipping Address ───────────────────────────────────────────── */}
          <div className="bg-white rounded-xl border border-gray-100 shadow-sm p-5">
            <h3 className="text-sm font-semibold text-gray-700 mb-3 flex items-center gap-2">
              <MapPin className="w-4 h-4 text-gray-400" />
              Shipping Address
            </h3>
            <div className="text-sm text-gray-600 space-y-1.5">
              <p className="font-semibold text-gray-800">
                {order.shippingName}
              </p>
              <p>{order.addressLine1}</p>
              {order.addressLine2 && <p>{order.addressLine2}</p>}
              <p>
                {order.city}, {order.state} {order.postalCode}
              </p>
              <p>{order.country}</p>
              <div className="flex items-center gap-1.5 pt-2 text-gray-500 border-t border-gray-100 mt-2">
                <Phone className="w-3.5 h-3.5" />
                <span>{order.phoneNumber}</span>
              </div>
            </div>
          </div>

          {/* ── Order Info ─────────────────────────────────────────────────── */}
          <div className="bg-gray-50 rounded-xl border border-gray-100 p-5 space-y-3">
            <div>
              <h3 className="text-xs font-semibold text-gray-400 uppercase tracking-wider">
                Order Number
              </h3>
              <p className="text-sm text-gray-700 font-medium mt-0.5">
                {order.orderNumber}
              </p>
            </div>
            <div>
              <h3 className="text-xs font-semibold text-gray-400 uppercase tracking-wider">
                Order ID
              </h3>
              <p className="text-xs text-gray-600 font-mono break-all mt-0.5">
                {order.id}
              </p>
            </div>
            <div>
              <h3 className="text-xs font-semibold text-gray-400 uppercase tracking-wider">
                Order Date
              </h3>
              <p className="text-sm text-gray-700 mt-0.5">
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
          <div className="bg-blue-50 rounded-xl border border-blue-100 p-5 text-center">
            <p className="text-sm text-blue-700 font-medium mb-1">
              Need help with this order?
            </p>
            <p className="text-xs text-blue-600">
              Contact support at support@shopsphere.com
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

export default OrderDetail;