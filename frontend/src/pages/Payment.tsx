import { useEffect, useState } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import {
  CreditCard,
  Banknote,
  Wallet,
  Truck,
  ArrowLeft,
  CheckCircle,
  Clock,
  XCircle,
  Shield,
  Package,
  Loader,
} from "lucide-react";
import { orderApi } from "@/api/order.api";
import type { OrderDetail, Payment } from "@/types";
import { PaymentMethodLabels, PaymentStatusLabels, PaymentStatusColors } from "@/types";
import { formatPrice } from "@/utils/formatPrice";
import { cn } from "@/utils/cn";
import Spinner from "@/components/ui/Spinner";
import Button from "@/components/ui/Button";
import toast from "react-hot-toast";

const PAYMENT_METHODS = [
  { value: 1, label: "Credit Card", icon: CreditCard, desc: "Visa, Mastercard, RuPay" },
  { value: 2, label: "Debit Card",  icon: Banknote,   desc: "All major banks"        },
  { value: 3, label: "PayPal",      icon: Wallet,     desc: "Pay with PayPal"         },
  { value: 4, label: "Cash on Delivery", icon: Truck, desc: "Pay when delivered"      },
];

const PaymentStatusIcon: Record<number, React.ElementType> = {
  1: Clock,
  2: CheckCircle,
  3: XCircle,
  4: Loader,
};

const PaymentPage = () => {
  const { orderId } = useParams<{ orderId: string }>();
  const navigate    = useNavigate();

  const [order, setOrder]                       = useState<OrderDetail | null>(null);
  const [payment, setPayment]                   = useState<Payment | null>(null);
  const [selectedMethod, setSelectedMethod]     = useState(1);
  const [isLoading, setIsLoading]               = useState(true);
  const [isProcessing, setIsProcessing]         = useState(false);
  const [paymentInitiated, setPaymentInitiated] = useState(false);

  // ── Load Order & Check Existing Payment ────────────────────────────────────
  useEffect(() => {
    if (!orderId) return;
    const load = async () => {
      setIsLoading(true);
      try {
        const orderData = await orderApi.getOrderById(orderId);
        setOrder(orderData);

        // Check if payment already exists
        try {
          const paymentData = await orderApi.getPayment(orderId);
          if (paymentData?.id) {
            setPayment(paymentData);
            setPaymentInitiated(true);
          }
        } catch {
          // No payment yet - that's ok
        }
      } catch (err) {
        toast.error((err as Error).message || "Failed to load order");
      } finally {
        setIsLoading(false);
      }
    };
    load();
  }, [orderId]);

  // ── Initiate Payment ───────────────────────────────────────────────────────
  const handleInitiatePayment = async () => {
    if (!orderId) return;

    setIsProcessing(true);
    try {
      const paymentId = await orderApi.initiatePayment(orderId, {
        paymentMethod: selectedMethod,
      });

      toast.success("Payment initiated!");

      // Fetch payment details
      const paymentData = await orderApi.getPayment(orderId);
      setPayment(paymentData);
      setPaymentInitiated(true);

      // For COD, show success immediately
      if (selectedMethod === 4) {
        toast.success("Order confirmed! Pay on delivery.");
      }
    } catch (err) {
      toast.error((err as Error).message || "Payment failed");
    } finally {
      setIsProcessing(false);
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

  if (!order) {
    return (
      <div className="max-w-7xl mx-auto px-4 py-20 text-center">
        <Package className="w-16 h-16 text-gray-200 mx-auto mb-4" />
        <h2 className="text-2xl font-semibold text-gray-700 mb-3">Order not found</h2>
        <Link to="/orders"><Button>Back to Orders</Button></Link>
      </div>
    );
  }

  const totalAmount = order.totalAmount ?? 0;

  return (
    <div className="max-w-4xl mx-auto px-4 py-8">

      {/* ── Back ────────────────────────────────────────────────────────────── */}
      <button
        onClick={() => navigate(-1)}
        className="flex items-center gap-2 text-sm text-gray-500 hover:text-primary-600 mb-6 transition"
      >
        <ArrowLeft className="w-4 h-4" /> Back
      </button>

      {/* ── Header ──────────────────────────────────────────────────────────── */}
      <div className="mb-8">
        <h1 className="text-2xl font-bold text-gray-900">Payment</h1>
        <p className="text-gray-500 mt-1">
          Order {order.orderNumber} • {formatPrice(totalAmount)}
        </p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">

        {/* ── Left ────────────────────────────────────────────────────────────── */}
        <div className="lg:col-span-2">

          {/* ── Payment Already Initiated ──────────────────────────────────── */}
          {paymentInitiated && payment ? (
            <div className="bg-white rounded-xl border border-gray-100 shadow-sm p-6">
              <div className="text-center py-8">
                {/* Status Icon */}
                {(() => {
                  const Icon = PaymentStatusIcon[payment.status] || Clock;
                  const statusColor = PaymentStatusColors[payment.status] || "bg-gray-100 text-gray-700";
                  const statusLabel = PaymentStatusLabels[payment.status] || "Unknown";

                  return (
                    <>
                      <div className={cn("w-20 h-20 rounded-full flex items-center justify-center mx-auto mb-4", statusColor)}>
                        <Icon className="w-10 h-10" />
                      </div>
                      <h2 className="text-2xl font-bold text-gray-900 mb-2">
                        Payment {statusLabel}
                      </h2>
                      <p className="text-gray-500 mb-6">
                        {payment.status === 1 && "Your payment is being processed. Please wait for confirmation."}
                        {payment.status === 2 && "Payment completed successfully!"}
                        {payment.status === 3 && "Payment failed. Please try again or contact support."}
                        {payment.status === 4 && "Payment has been refunded."}
                      </p>
                    </>
                  );
                })()}

                {/* Payment Details */}
                <div className="bg-gray-50 rounded-xl p-5 text-left max-w-md mx-auto">
                  <div className="space-y-3">
                    <div className="flex justify-between text-sm">
                      <span className="text-gray-500">Payment ID</span>
                      <span className="text-gray-800 font-mono text-xs">{payment.id.slice(0, 12)}...</span>
                    </div>
                    <div className="flex justify-between text-sm">
                      <span className="text-gray-500">Method</span>
                      <span className="text-gray-800 font-medium">{PaymentMethodLabels[payment.method] || "Unknown"}</span>
                    </div>
                    <div className="flex justify-between text-sm">
                      <span className="text-gray-500">Amount</span>
                      <span className="text-gray-800 font-bold">{formatPrice(payment.amount)}</span>
                    </div>
                    <div className="flex justify-between text-sm">
                      <span className="text-gray-500">Status</span>
                      <span className={cn("px-2 py-0.5 rounded-full text-xs font-semibold", PaymentStatusColors[payment.status] || "bg-gray-100 text-gray-700")}>
                        {PaymentStatusLabels[payment.status] || "Unknown"}
                      </span>
                    </div>
                    {payment.transactionId && (
                      <div className="flex justify-between text-sm">
                        <span className="text-gray-500">Transaction ID</span>
                        <span className="text-gray-800 font-mono text-xs">{payment.transactionId}</span>
                      </div>
                    )}
                  </div>
                </div>

                {/* Actions */}
                <div className="flex justify-center gap-3 mt-6">
                  <Link to={`/orders/${order.id}`}>
                    <Button variant="primary" size="md">
                      <Package className="w-4 h-4" /> View Order
                    </Button>
                  </Link>
                  <Link to="/orders">
                    <Button variant="outline" size="md">All Orders</Button>
                  </Link>
                </div>
              </div>
            </div>
          ) : (
            /* ── Select Payment Method ──────────────────────────────────────── */
            <div className="bg-white rounded-xl border border-gray-100 shadow-sm overflow-hidden">
              <div className="px-5 py-4 border-b border-gray-100">
                <h2 className="text-lg font-semibold text-gray-800 flex items-center gap-2">
                  <CreditCard className="w-5 h-5 text-gray-400" />
                  Select Payment Method
                </h2>
              </div>

              <div className="p-5 space-y-3">
                {PAYMENT_METHODS.map(({ value, label, icon: Icon, desc }) => (
                  <label
                    key={value}
                    className={cn(
                      "flex items-center gap-4 p-4 rounded-xl border-2 cursor-pointer transition",
                      selectedMethod === value
                        ? "border-primary-500 bg-primary-50"
                        : "border-gray-100 hover:border-gray-200"
                    )}
                  >
                    <input
                      type="radio"
                      name="paymentMethod"
                      value={value}
                      checked={selectedMethod === value}
                      onChange={() => setSelectedMethod(value)}
                      className="text-primary-600 focus:ring-primary-500"
                    />
                    <div className={cn(
                      "w-10 h-10 rounded-xl flex items-center justify-center",
                      selectedMethod === value ? "bg-primary-100 text-primary-600" : "bg-gray-100 text-gray-500"
                    )}>
                      <Icon className="w-5 h-5" />
                    </div>
                    <div className="flex-1">
                      <p className="text-sm font-semibold text-gray-800">{label}</p>
                      <p className="text-xs text-gray-500">{desc}</p>
                    </div>
                    {selectedMethod === value && (
                      <CheckCircle className="w-5 h-5 text-primary-600 shrink-0" />
                    )}
                  </label>
                ))}
              </div>

              {/* Pay Button */}
              <div className="px-5 pb-5">
                <Button
                  fullWidth
                  size="lg"
                  onClick={handleInitiatePayment}
                  isLoading={isProcessing}
                >
                  <CreditCard className="w-5 h-5" />
                  Pay {formatPrice(totalAmount)}
                </Button>
              </div>
            </div>
          )}
        </div>

        {/* ── Right: Summary ───────────────────────────────────────────────── */}
        <div>
          <div className="bg-white rounded-xl border border-gray-100 shadow-sm p-6 sticky top-24">
            <h3 className="text-lg font-bold text-gray-900 mb-4">Order Summary</h3>

            <div className="space-y-3 mb-4">
              <div className="flex justify-between text-sm text-gray-600">
                <span>Order Number</span>
                <span className="font-medium text-gray-800">{order.orderNumber}</span>
              </div>
              <div className="flex justify-between text-sm text-gray-600">
                <span>Subtotal</span>
                <span>{formatPrice(order.subTotal)}</span>
              </div>
              {order.discountAmount > 0 && (
                <div className="flex justify-between text-sm text-green-600">
                  <span>Discount</span>
                  <span>-{formatPrice(order.discountAmount)}</span>
                </div>
              )}
              <div className="flex justify-between text-sm text-gray-600">
                <span>Tax</span>
                <span>{formatPrice(order.taxAmount)}</span>
              </div>
              <div className="flex justify-between text-sm text-gray-600">
                <span>Shipping</span>
                <span className={cn(order.shippingAmount === 0 && "text-green-600 font-medium")}>
                  {order.shippingAmount === 0 ? "FREE" : formatPrice(order.shippingAmount)}
                </span>
              </div>
              <hr className="border-gray-100" />
              <div className="flex justify-between font-bold text-lg text-gray-900">
                <span>Total</span>
                <span className="text-primary-600">{formatPrice(totalAmount)}</span>
              </div>
            </div>

            {/* Security */}
            <div className="pt-4 border-t border-gray-100 flex items-center justify-center gap-2 text-xs text-gray-400">
              <Shield className="w-4 h-4" />
              Secure Payment • 256-bit SSL
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default PaymentPage;