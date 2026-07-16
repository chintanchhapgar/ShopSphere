import { useEffect, useState } from "react";
import { useParams, useNavigate, Link, useSearchParams } from "react-router-dom";
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
  ExternalLink,
  Lock,
} from "lucide-react";
import { orderApi } from "@/api/order.api";
import { stripeApi } from "@/api/stripe.api";
import type { OrderDetail, Payment } from "@/types";
import { PaymentMethodLabels, PaymentStatusLabels, PaymentStatusColors } from "@/types";
import { formatPrice } from "@/utils/formatPrice";
import { cn } from "@/utils/cn";
import Spinner from "@/components/ui/Spinner";
import Button from "@/components/ui/Button";
import toast from "react-hot-toast";

// ── Payment Methods ──────────────────────────────────────────────────────────
const PAYMENT_METHODS = [
  {
    value: 1,
    label: "Credit / Debit Card",
    icon: CreditCard,
    desc: "Powered by Stripe · Visa, Mastercard, RuPay",
    badge: "Recommended",
    color: "bg-purple-600",
    isStripe: true,
  },
  {
    value: 3,
    label: "PayPal",
    icon: Wallet,
    desc: "Pay with your PayPal account",
    isStripe: false,
  },
  {
    value: 4,
    label: "Cash on Delivery",
    icon: Truck,
    desc: "Pay in cash when your order arrives",
    badge: "No online payment",
    isStripe: false,
  },
];

// ── Status Icons ─────────────────────────────────────────────────────────────
const PaymentStatusIcon: Record<number, React.ElementType> = {
  1: Clock,
  2: CheckCircle,
  3: XCircle,
  4: Loader,
};

const PaymentPage = () => {
  const { orderId } = useParams<{ orderId: string }>();
  const navigate    = useNavigate();
  const [searchParams] = useSearchParams();

  const [order, setOrder]                       = useState<OrderDetail | null>(null);
  const [payment, setPayment]                   = useState<Payment | null>(null);
  const [selectedMethod, setSelectedMethod]     = useState(1);
  const [isLoading, setIsLoading]               = useState(true);
  const [isProcessing, setIsProcessing]         = useState(false);
  const [paymentInitiated, setPaymentInitiated] = useState(false);

  // ── Check URL Params (Stripe redirect) ─────────────────────────────────────
  useEffect(() => {
    const paymentStatus = searchParams.get("payment");
    if (paymentStatus === "success") {
      toast.success("Payment successful! 🎉");
    } else if (paymentStatus === "cancelled") {
      toast.error("Payment was cancelled");
    }
  }, [searchParams]);

  // ── Load Order & Existing Payment ──────────────────────────────────────────
  useEffect(() => {
    if (!orderId) return;
    const load = async () => {
      setIsLoading(true);
      try {
        const orderData = await orderApi.getOrderById(orderId);
        setOrder(orderData);

        try {
          const paymentData = await orderApi.getPayment(orderId);
          if (paymentData?.id) {
            setPayment(paymentData);
            setPaymentInitiated(true);
          }
        } catch {
          // No payment yet
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

    const method = PAYMENT_METHODS.find((m) => m.value === selectedMethod);

    setIsProcessing(true);
    try {
      // ── Stripe Payment (Credit/Debit) ──────────────────────────────────────
      if (method?.isStripe) {
        toast.loading("Redirecting to Stripe...", { id: "stripe-redirect" });

        try {
          const { sessionUrl } = await stripeApi.createCheckoutSession(orderId);

          if (!sessionUrl) {
            throw new Error("Failed to create Stripe session");
          }

          toast.success("Redirecting to secure payment...", { id: "stripe-redirect" });

          // Small delay for UX
          setTimeout(() => {
            window.location.href = sessionUrl;
          }, 500);
          return;
        } catch (err) {
          toast.error(
            (err as Error).message || "Stripe payment failed. Please try again.",
            { id: "stripe-redirect" }
          );
          setIsProcessing(false);
          return;
        }
      }

      // ── Other Payment Methods (PayPal, COD) ────────────────────────────────
      const paymentId = await orderApi.initiatePayment(orderId, {
        paymentMethod: selectedMethod,
      });

      // For COD
      if (selectedMethod === 4) {
        toast.success("Order confirmed! Pay on delivery.");
        setTimeout(() => navigate(`/orders/${orderId}`), 1500);
        return;
      }

      toast.success("Payment initiated!");

      const paymentData = await orderApi.getPayment(orderId);
      setPayment(paymentData);
      setPaymentInitiated(true);
    } catch (err) {
      toast.error((err as Error).message || "Payment failed");
    } finally {
      setIsProcessing(false);
    }
  };

  // ── Retry Payment (Stripe) ─────────────────────────────────────────────────
  const handleRetryStripe = async () => {
    if (!orderId) return;

    setIsProcessing(true);
    try {
      const { sessionUrl } = await stripeApi.createCheckoutSession(orderId);
      if (sessionUrl) {
        window.location.href = sessionUrl;
      }
    } catch (err) {
      toast.error((err as Error).message || "Failed to retry payment");
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
        <h2 className="text-2xl font-semibold text-gray-700 mb-3">
          Order not found
        </h2>
        <Link to="/orders">
          <Button>Back to Orders</Button>
        </Link>
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
        <h1 className="text-2xl font-bold text-gray-900 dark:text-gray-100">
          Complete Your Payment
        </h1>
        <p className="text-gray-500 mt-1">
          Order <span className="font-mono font-semibold">{order.orderNumber}</span>
          {" • "}
          <span className="text-primary-600 font-bold">{formatPrice(totalAmount)}</span>
        </p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">

        {/* ── Left: Payment Section ─────────────────────────────────────────── */}
        <div className="lg:col-span-2">

          {/* ── Payment Already Initiated ──────────────────────────────────── */}
          {paymentInitiated && payment ? (
            <div className="bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 shadow-sm p-6">
              <div className="text-center py-8">
                {(() => {
                  const Icon = PaymentStatusIcon[payment.status] || Clock;
                  const statusColor = PaymentStatusColors[payment.status] || "bg-gray-100 text-gray-700";
                  const statusLabel = PaymentStatusLabels[payment.status] || "Unknown";

                  return (
                    <>
                      <div className={cn(
                        "w-20 h-20 rounded-full flex items-center justify-center mx-auto mb-4",
                        statusColor
                      )}>
                        <Icon className={cn("w-10 h-10", payment.status === 4 && "animate-spin")} />
                      </div>
                      <h2 className="text-2xl font-bold text-gray-900 dark:text-gray-100 mb-2">
                        Payment {statusLabel}
                      </h2>
                      <p className="text-gray-500 dark:text-gray-400 mb-6">
                        {payment.status === 1 && "Your payment is being processed. Please wait for confirmation."}
                        {payment.status === 2 && "Payment completed successfully! Your order is confirmed."}
                        {payment.status === 3 && "Payment failed. Please try again or use a different method."}
                        {payment.status === 4 && "Payment has been refunded to your account."}
                      </p>
                    </>
                  );
                })()}

                {/* Payment Details */}
                <div className="bg-gray-50 dark:bg-gray-900 rounded-xl p-5 text-left max-w-md mx-auto">
                  <div className="space-y-3">
                    <div className="flex justify-between text-sm">
                      <span className="text-gray-500 dark:text-gray-400">Payment ID</span>
                      <span className="text-gray-800 dark:text-gray-200 font-mono text-xs">
                        {payment.id.slice(0, 12)}...
                      </span>
                    </div>
                    <div className="flex justify-between text-sm">
                      <span className="text-gray-500 dark:text-gray-400">Method</span>
                      <span className="text-gray-800 dark:text-gray-200 font-medium">
                        {PaymentMethodLabels[payment.method] || "Unknown"}
                      </span>
                    </div>
                    <div className="flex justify-between text-sm">
                      <span className="text-gray-500 dark:text-gray-400">Amount</span>
                      <span className="text-gray-800 dark:text-gray-200 font-bold">
                        {formatPrice(payment.amount)}
                      </span>
                    </div>
                    <div className="flex justify-between text-sm">
                      <span className="text-gray-500 dark:text-gray-400">Status</span>
                      <span className={cn(
                        "px-2 py-0.5 rounded-full text-xs font-semibold",
                        PaymentStatusColors[payment.status] || "bg-gray-100 text-gray-700"
                      )}>
                        {PaymentStatusLabels[payment.status] || "Unknown"}
                      </span>
                    </div>
                    {payment.transactionId && (
                      <div className="flex justify-between text-sm">
                        <span className="text-gray-500 dark:text-gray-400">Transaction ID</span>
                        <span className="text-gray-800 dark:text-gray-200 font-mono text-xs">
                          {payment.transactionId}
                        </span>
                      </div>
                    )}
                  </div>
                </div>

                {/* Actions */}
                <div className="flex flex-wrap justify-center gap-3 mt-6">
                  <Link to={`/orders/${order.id}`}>
                    <Button variant="primary" size="md">
                      <Package className="w-4 h-4" /> View Order
                    </Button>
                  </Link>

                  {/* Retry button for failed payments */}
                  {payment.status === 3 && (
                    <Button
                      variant="outline"
                      size="md"
                      onClick={handleRetryStripe}
                      isLoading={isProcessing}
                    >
                      <CreditCard className="w-4 h-4" /> Retry Payment
                    </Button>
                  )}

                  <Link to="/orders">
                    <Button variant="outline" size="md">All Orders</Button>
                  </Link>
                </div>
              </div>
            </div>
          ) : (
            /* ═══════════════════════════════════════════════════════════════ */
            /* ── Select Payment Method ──────────────────────────────────── */
            /* ═══════════════════════════════════════════════════════════════ */
            <div className="bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 shadow-sm overflow-hidden">
              <div className="px-5 py-4 border-b border-gray-100 dark:border-gray-700 flex items-center justify-between">
                <h2 className="text-lg font-semibold text-gray-800 dark:text-gray-100 flex items-center gap-2">
                  <CreditCard className="w-5 h-5 text-gray-400" />
                  Select Payment Method
                </h2>
                <span className="flex items-center gap-1 text-xs text-gray-400">
                  <Lock className="w-3 h-3" />
                  Secured
                </span>
              </div>

              <div className="p-5 space-y-3">
                {PAYMENT_METHODS.map(({ value, label, icon: Icon, desc, badge, color, isStripe }) => (
                  <label
                    key={value}
                    className={cn(
                      "flex items-center gap-4 p-4 rounded-xl border-2 cursor-pointer transition group",
                      selectedMethod === value
                        ? "border-primary-500 bg-primary-50 dark:bg-primary-900/20"
                        : "border-gray-100 dark:border-gray-700 hover:border-gray-200 dark:hover:border-gray-600"
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
                      "w-11 h-11 rounded-xl flex items-center justify-center shrink-0 transition",
                      selectedMethod === value
                        ? color || "bg-primary-100 text-primary-600"
                        : "bg-gray-100 dark:bg-gray-700 text-gray-500",
                      selectedMethod === value && color && "text-white"
                    )}>
                      <Icon className="w-5 h-5" />
                    </div>
                    <div className="flex-1 min-w-0">
                      <div className="flex items-center gap-2 flex-wrap">
                        <p className="text-sm font-semibold text-gray-800 dark:text-gray-100">
                          {label}
                        </p>
                        {badge && (
                          <span className={cn(
                            "px-2 py-0.5 text-xs font-semibold rounded-full",
                            badge === "Recommended"
                              ? "bg-green-100 text-green-700"
                              : "bg-gray-100 text-gray-600"
                          )}>
                            {badge}
                          </span>
                        )}
                        {isStripe && (
                          <span className="flex items-center gap-0.5 px-2 py-0.5 bg-[#635BFF]/10 text-[#635BFF] text-xs font-bold rounded-full">
                            <svg className="w-3 h-3" viewBox="0 0 24 24" fill="currentColor">
                              <path d="M13.976 9.15c-2.172-.806-3.356-1.426-3.356-2.409 0-.831.683-1.305 1.901-1.305 2.227 0 4.515.858 6.09 1.631l.89-5.494C18.252.975 15.697 0 12.165 0 9.667 0 7.589.654 6.104 1.872 4.56 3.147 3.757 4.992 3.757 7.218c0 4.039 2.467 5.76 6.476 7.219 2.585.92 3.445 1.574 3.445 2.583 0 .98-.84 1.545-2.354 1.545-1.875 0-4.965-.921-6.99-2.109l-.9 5.555C5.175 22.99 8.385 24 11.714 24c2.641 0 4.843-.624 6.328-1.813 1.664-1.305 2.525-3.236 2.525-5.732 0-4.128-2.524-5.851-6.591-7.305z"/>
                            </svg>
                            Stripe
                          </span>
                        )}
                      </div>
                      <p className="text-xs text-gray-500 dark:text-gray-400 mt-0.5">
                        {desc}
                      </p>
                    </div>
                    {selectedMethod === value && (
                      <CheckCircle className="w-5 h-5 text-primary-600 shrink-0" />
                    )}
                  </label>
                ))}
              </div>

              {/* ── Stripe Info Banner ───────────────────────────────────── */}
              {selectedMethod === 1 && (
                <div className="mx-5 mb-5 p-3 bg-purple-50 dark:bg-purple-900/20 border border-purple-100 dark:border-purple-800 rounded-lg">
                  <div className="flex items-start gap-2">
                    <Lock className="w-4 h-4 text-purple-600 mt-0.5 shrink-0" />
                    <div className="text-xs text-purple-700 dark:text-purple-300">
                      <p className="font-semibold mb-1">You'll be redirected to Stripe</p>
                      <p>
                        Enter your card details on Stripe's secure page. We don't store your card information.
                      </p>
                    </div>
                  </div>
                </div>
              )}

              {/* ── COD Info ─────────────────────────────────────────────── */}
              {selectedMethod === 4 && (
                <div className="mx-5 mb-5 p-3 bg-orange-50 dark:bg-orange-900/20 border border-orange-100 dark:border-orange-800 rounded-lg">
                  <div className="flex items-start gap-2">
                    <Truck className="w-4 h-4 text-orange-600 mt-0.5 shrink-0" />
                    <div className="text-xs text-orange-700 dark:text-orange-300">
                      <p className="font-semibold mb-1">Cash on Delivery</p>
                      <p>
                        Pay {formatPrice(totalAmount)} in cash when your order is delivered.
                        Additional handling fee may apply.
                      </p>
                    </div>
                  </div>
                </div>
              )}

              {/* Pay Button */}
              <div className="px-5 pb-5">
                <Button
                  fullWidth
                  size="lg"
                  onClick={handleInitiatePayment}
                  isLoading={isProcessing}
                >
                  {selectedMethod === 1 ? (
                    <>
                      <Lock className="w-4 h-4" />
                      Pay Securely with Stripe {formatPrice(totalAmount)}
                    </>
                  ) : selectedMethod === 4 ? (
                    <>
                      <Truck className="w-4 h-4" />
                      Confirm Cash on Delivery
                    </>
                  ) : (
                    <>
                      <CreditCard className="w-4 h-4" />
                      Pay {formatPrice(totalAmount)}
                    </>
                  )}
                </Button>

                {/* Test Card Info (Development) */}
                {selectedMethod === 1 && import.meta.env.DEV && (
                  <div className="mt-3 p-2 bg-yellow-50 dark:bg-yellow-900/20 border border-yellow-100 dark:border-yellow-800 rounded-lg">
                    <p className="text-xs text-yellow-700 dark:text-yellow-300 font-medium">
                      🧪 Test Mode - Use card: <span className="font-mono">4242 4242 4242 4242</span>
                    </p>
                    <p className="text-xs text-yellow-600 dark:text-yellow-400 mt-1">
                      Any future date, any CVC, any ZIP
                    </p>
                  </div>
                )}
              </div>
            </div>
          )}
        </div>

        {/* ═══════════════════════════════════════════════════════════════════ */}
        {/* ── Right: Order Summary ────────────────────────────────────────── */}
        {/* ═══════════════════════════════════════════════════════════════════ */}
        <div>
          <div className="bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 shadow-sm p-6 sticky top-24">
            <h3 className="text-lg font-bold text-gray-900 dark:text-gray-100 mb-4">
              Order Summary
            </h3>

            {/* Order Info */}
            <div className="mb-4 p-3 bg-gray-50 dark:bg-gray-900 rounded-lg">
              <p className="text-xs text-gray-500 dark:text-gray-400 mb-1">Order Number</p>
              <p className="font-semibold text-gray-800 dark:text-gray-100 font-mono text-sm">
                {order.orderNumber}
              </p>
            </div>

            {/* Price Breakdown */}
            <div className="space-y-3 mb-4">
              <div className="flex justify-between text-sm text-gray-600 dark:text-gray-400">
                <span>Subtotal</span>
                <span>{formatPrice(order.subTotal)}</span>
              </div>
              {order.discountAmount > 0 && (
                <div className="flex justify-between text-sm text-green-600">
                  <span>Discount</span>
                  <span>-{formatPrice(order.discountAmount)}</span>
                </div>
              )}
              <div className="flex justify-between text-sm text-gray-600 dark:text-gray-400">
                <span>Tax</span>
                <span>{formatPrice(order.taxAmount)}</span>
              </div>
              <div className="flex justify-between text-sm text-gray-600 dark:text-gray-400">
                <span>Shipping</span>
                <span className={cn(
                  order.shippingAmount === 0 && "text-green-600 font-medium"
                )}>
                  {order.shippingAmount === 0 ? "FREE" : formatPrice(order.shippingAmount)}
                </span>
              </div>
              <hr className="border-gray-100 dark:border-gray-700" />
              <div className="flex justify-between font-bold text-lg text-gray-900 dark:text-gray-100">
                <span>Total</span>
                <span className="text-primary-600">{formatPrice(totalAmount)}</span>
              </div>
            </div>

            {/* Trust Badges */}
            <div className="pt-4 border-t border-gray-100 dark:border-gray-700 space-y-3">
              <div className="flex items-center justify-center gap-2 text-xs text-gray-400">
                <Shield className="w-4 h-4" />
                Secure Payment • 256-bit SSL
              </div>

              {/* Powered by Stripe */}
              <div className="flex items-center justify-center gap-1.5 text-xs text-gray-400">
                <span>Powered by</span>
                <svg className="w-10 h-4" viewBox="0 0 60 25" fill="currentColor">
                  <path d="M59.5 14.5c0-4.7-2.3-8.4-6.6-8.4-4.4 0-7 3.7-7 8.4 0 5.5 3.1 8.3 7.6 8.3 2.2 0 3.9-.5 5.1-1.2v-3.6c-1.3.6-2.7 1-4.5 1-1.8 0-3.4-.6-3.6-2.8h9v-1.7zm-9.1-1.7c0-2 1.3-2.9 2.4-2.9 1.1 0 2.3.8 2.3 2.9h-4.7z"/>
                </svg>
              </div>

              {/* Accepted Cards */}
              {selectedMethod === 1 && (
                <div className="flex items-center justify-center gap-2 pt-2">
                  <div className="text-xs text-gray-400">We accept:</div>
                  <div className="flex gap-1">
                    {["visa", "mastercard", "amex", "rupay"].map((card) => (
                      <div key={card} className="w-8 h-5 bg-gray-100 dark:bg-gray-700 rounded flex items-center justify-center text-[8px] font-bold text-gray-500 uppercase">
                        {card.slice(0, 4)}
                      </div>
                    ))}
                  </div>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default PaymentPage;