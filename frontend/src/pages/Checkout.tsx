import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import {
  MapPin,
  Plus,
  CheckCircle,
  ArrowLeft,
  ShoppingBag,
  Truck,
  Tag,
  CreditCard,
  Shield,
  Banknote,
  Wallet,
  Lock,
} from "lucide-react";
import { addressApi } from "@/api/address.api";
import { orderApi } from "@/api/order.api";
import { stripeApi } from "@/api/stripe.api";
import { useCart } from "@/hooks/useCart";
import { useAuth } from "@/hooks/useAuth";
import type { Address } from "@/types";
import { formatPrice } from "@/utils/formatPrice";
import { cn } from "@/utils/cn";
import Spinner from "@/components/ui/Spinner";
import Button from "@/components/ui/Button";
import toast from "react-hot-toast";

// ── Payment Methods ──────────────────────────────────────────────────────────
const PAYMENT_METHODS = [
  {
    value:    "stripe",
    apiValue: 1,
    label:    "Credit / Debit Card",
    icon:     CreditCard,
    desc:     "Visa, Mastercard, RuPay via Stripe",
    badge:    "Recommended",
    color:    "bg-purple-600",
    isStripe: true,
  },
  {
    value:    "paypal",
    apiValue: 3,
    label:    "PayPal",
    icon:     Wallet,
    desc:     "Pay with your PayPal account",
    isStripe: false,
  },
  {
    value:    "cod",
    apiValue: 4,
    label:    "Cash on Delivery",
    icon:     Truck,
    desc:     "Pay in cash when delivered",
    isStripe: false,
  },
];

const Checkout = () => {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();
  const { cart, totalItems, fetchCart, clearCart } = useCart();

  const [addresses, setAddresses]                 = useState<Address[]>([]);
  const [selectedAddressId, setSelectedAddressId] = useState<string>("");
  const [selectedPayment, setSelectedPayment]     = useState<string>("stripe");
  const [isLoading, setIsLoading]                 = useState(true);
  const [isPlacingOrder, setIsPlacingOrder]       = useState(false);
  const [currentStep, setCurrentStep]             = useState<1 | 2>(1);

  // ── Load Data ──────────────────────────────────────────────────────────────
  useEffect(() => {
    if (!isAuthenticated) return;
    const load = async () => {
      setIsLoading(true);
      try {
        await fetchCart();
        const addrs = await addressApi.getAddresses();
        setAddresses(addrs);

        const defaultAddr = addrs.find((a) => a.isDefault);
        if (defaultAddr) setSelectedAddressId(defaultAddr.id);
        else if (addrs.length > 0) setSelectedAddressId(addrs[0].id);
      } catch (err) {
        console.error("Checkout load error:", err);
      } finally {
        setIsLoading(false);
      }
    };
    load();
  }, [isAuthenticated]);

  // ── Place Order + Handle Payment ───────────────────────────────────────────
  const handlePlaceOrder = async () => {
    if (!selectedAddressId) {
      toast.error("Please select a delivery address");
      return;
    }
    if (!cart?.items?.length) {
      toast.error("Your cart is empty");
      return;
    }

    const method = PAYMENT_METHODS.find((m) => m.value === selectedPayment);
    if (!method) {
      toast.error("Please select a payment method");
      return;
    }

    setIsPlacingOrder(true);

    try {
      // ── STEP 1: Create Order ─────────────────────────────────────────────
      console.log("📦 Creating order...");
      toast.loading("Creating your order...", { id: "checkout" });

      const orderResult = await orderApi.createOrder({
        addressId: selectedAddressId,
      });

      console.log("✅ Order created:", orderResult);

      const orderId =
        typeof orderResult === "string"
          ? orderResult
          : orderResult?.id ?? orderResult?.orderId ?? orderResult;

      if (!orderId) {
        throw new Error("Order created but no ID returned");
      }

      // ── STEP 2: Handle Payment Based on Method ───────────────────────────

      // ✅ STRIPE PAYMENT - Redirect to Stripe Checkout
      if (method.isStripe) {
        console.log("💳 Initiating Stripe checkout for order:", orderId);
        toast.loading("Redirecting to Stripe...", { id: "checkout" });

        try {
          const stripeResponse = await stripeApi.createCheckoutSession(orderId);
          console.log("✅ Stripe session:", stripeResponse);

          if (!stripeResponse.sessionUrl) {
            throw new Error("Failed to create Stripe session");
          }

          toast.success("Redirecting to secure payment...", { id: "checkout" });

          // Clear cart before redirect
          await clearCart();

          // Redirect to Stripe (small delay for UX)
          setTimeout(() => {
            window.location.href = stripeResponse.sessionUrl;
          }, 500);
          return;
        } catch (stripeErr) {
          console.error("❌ Stripe error:", stripeErr);
          toast.error(
            (stripeErr as Error).message || "Stripe payment failed",
            { id: "checkout" }
          );
          // Order is created but Stripe failed - redirect to payment page for retry
          setTimeout(() => navigate(`/orders/${orderId}/payment`), 1500);
          return;
        }
      }

      // ✅ COD - Just confirm order
      if (method.value === "cod") {
        await orderApi.initiatePayment(orderId, {
          paymentMethod: method.apiValue,
        });

        await clearCart();
        toast.success("Order placed! Pay on delivery.", { id: "checkout" });
        setTimeout(() => navigate(`/orders/${orderId}`), 1000);
        return;
      }

      // ✅ PAYPAL - Initiate payment then redirect
      if (method.value === "paypal") {
        await orderApi.initiatePayment(orderId, {
          paymentMethod: method.apiValue,
        });

        await clearCart();
        toast.success("Order placed!", { id: "checkout" });
        setTimeout(() => navigate(`/orders/${orderId}/payment`), 1000);
        return;
      }
    } catch (err) {
      console.error("❌ Checkout error:", err);
      toast.error(
        (err as Error).message || "Failed to place order",
        { id: "checkout" }
      );
    } finally {
      setIsPlacingOrder(false);
    }
  };

  // ── Guards ─────────────────────────────────────────────────────────────────
  if (!isAuthenticated) {
    return (
      <div className="max-w-7xl mx-auto px-4 py-24 text-center">
        <ShoppingBag className="w-20 h-20 text-gray-200 mx-auto mb-6" />
        <h2 className="text-2xl font-bold text-gray-700 mb-3">Login to checkout</h2>
        <Link to="/login" state={{ from: "/checkout" }}>
          <Button size="lg">Login</Button>
        </Link>
      </div>
    );
  }

  if (isLoading) {
    return (
      <div className="flex justify-center py-32">
        <Spinner size="lg" className="text-primary-600" />
      </div>
    );
  }

  const items = cart?.items ?? [];
  if (items.length === 0) {
    return (
      <div className="max-w-7xl mx-auto px-4 py-24 text-center">
        <ShoppingBag className="w-20 h-20 text-gray-200 mx-auto mb-6" />
        <h2 className="text-2xl font-bold text-gray-700 mb-3">Your cart is empty</h2>
        <Link to="/products"><Button size="lg">Browse Products</Button></Link>
      </div>
    );
  }

  // ── Prices ─────────────────────────────────────────────────────────────────
  const subtotal = cart?.total ?? 0;
  const discount = cart?.discountAmount ?? 0;
  const shipping = subtotal > 500 ? 0 : 49;
  const tax      = Math.round((subtotal - discount) * 0.18);
  const total    = subtotal - discount + shipping + tax;

  const selectedAddress = addresses.find((a) => a.id === selectedAddressId);
  const selectedMethod  = PAYMENT_METHODS.find((m) => m.value === selectedPayment);

  return (
    <div className="max-w-7xl mx-auto px-4 py-8">

      {/* Back */}
      <Link
        to="/cart"
        className="inline-flex items-center gap-2 text-sm text-gray-500 hover:text-primary-600 mb-6 transition"
      >
        <ArrowLeft className="w-4 h-4" /> Back to Cart
      </Link>

      {/* Header */}
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900">Checkout</h1>
        <p className="text-gray-500 mt-1">
          {totalItems} item{totalItems !== 1 ? "s" : ""} in your order
        </p>
      </div>

      {/* Steps */}
      <div className="flex items-center gap-4 mb-8">
        {[
          { step: 1, label: "Address" },
          { step: 2, label: "Payment" },
          { step: 3, label: "Confirm" },
        ].map(({ step, label }, idx) => (
          <div key={step} className="flex items-center gap-2">
            <div className={cn(
              "w-8 h-8 rounded-full flex items-center justify-center text-xs font-bold transition",
              step < currentStep
                ? "bg-green-500 text-white"
                : step === currentStep
                ? "bg-primary-600 text-white"
                : "bg-gray-200 text-gray-500"
            )}>
              {step < currentStep ? <CheckCircle className="w-4 h-4" /> : step}
            </div>
            <span className={cn(
              "text-sm font-medium",
              step <= currentStep ? "text-primary-600" : "text-gray-400"
            )}>
              {label}
            </span>
            {idx < 2 && (
              <div className={cn(
                "w-12 h-0.5",
                step < currentStep ? "bg-green-500" : "bg-gray-200"
              )} />
            )}
          </div>
        ))}
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">

        {/* Left: Steps Content */}
        <div className="lg:col-span-2 space-y-6">

          {/* Step 1: Address */}
          <div className={cn(
            "bg-white rounded-xl border shadow-sm overflow-hidden transition",
            currentStep === 1 ? "border-primary-200" : "border-gray-100"
          )}>
            <div className="px-5 py-4 border-b border-gray-100 flex items-center justify-between">
              <h2 className="text-lg font-semibold text-gray-800 flex items-center gap-2">
                <MapPin className="w-5 h-5 text-gray-400" />
                Delivery Address
                {currentStep > 1 && selectedAddress && (
                  <span className="text-xs text-green-600 bg-green-50 px-2 py-0.5 rounded-full ml-2">
                    ✓ {selectedAddress.fullName}
                  </span>
                )}
              </h2>
              <div className="flex items-center gap-2">
                {currentStep > 1 && (
                  <button onClick={() => setCurrentStep(1)} className="text-sm text-primary-600 hover:underline">
                    Change
                  </button>
                )}
                <Link to="/addresses/new" className="flex items-center gap-1 text-sm text-primary-600 hover:text-primary-700 font-medium">
                  <Plus className="w-4 h-4" /> Add
                </Link>
              </div>
            </div>

            {currentStep === 1 && (
              <>
                {addresses.length === 0 ? (
                  <div className="p-8 text-center">
                    <MapPin className="w-12 h-12 text-gray-200 mx-auto mb-3" />
                    <p className="text-gray-600 font-medium mb-4">No addresses found</p>
                    <Link to="/addresses/new">
                      <Button size="md"><Plus className="w-4 h-4" /> Add Address</Button>
                    </Link>
                  </div>
                ) : (
                  <div className="p-5 space-y-3">
                    {addresses.map((addr) => (
                      <label
                        key={addr.id}
                        className={cn(
                          "flex items-start gap-3 p-4 rounded-xl border-2 cursor-pointer transition",
                          selectedAddressId === addr.id
                            ? "border-primary-500 bg-primary-50"
                            : "border-gray-100 hover:border-gray-200"
                        )}
                      >
                        <input
                          type="radio"
                          name="address"
                          value={addr.id}
                          checked={selectedAddressId === addr.id}
                          onChange={() => setSelectedAddressId(addr.id)}
                          className="mt-1 text-primary-600 focus:ring-primary-500"
                        />
                        <div className="flex-1">
                          <div className="flex items-center gap-2">
                            <p className="font-semibold text-gray-800 text-sm">{addr.fullName}</p>
                            {addr.isDefault && (
                              <span className="px-2 py-0.5 bg-primary-100 text-primary-700 text-xs font-semibold rounded-full">
                                Default
                              </span>
                            )}
                          </div>
                          <p className="text-sm text-gray-600 mt-1">
                            {addr.addressLine1}{addr.addressLine2 && `, ${addr.addressLine2}`}
                          </p>
                          <p className="text-sm text-gray-600">
                            {addr.city}, {addr.state} {addr.postalCode}
                          </p>
                          {addr.phoneNumber && <p className="text-xs text-gray-500 mt-1">📞 {addr.phoneNumber}</p>}
                        </div>
                        {selectedAddressId === addr.id && <CheckCircle className="w-5 h-5 text-primary-600 shrink-0 mt-1" />}
                      </label>
                    ))}

                    <div className="pt-3">
                      <Button
                        fullWidth
                        size="lg"
                        onClick={() => {
                          if (!selectedAddressId) {
                            toast.error("Please select an address");
                            return;
                          }
                          setCurrentStep(2);
                        }}
                        disabled={!selectedAddressId}
                      >
                        Continue to Payment
                      </Button>
                    </div>
                  </div>
                )}
              </>
            )}
          </div>

          {/* Step 2: Payment Method */}
          {currentStep >= 2 && (
            <div className={cn(
              "bg-white rounded-xl border shadow-sm overflow-hidden transition",
              currentStep === 2 ? "border-primary-200" : "border-gray-100"
            )}>
              <div className="px-5 py-4 border-b border-gray-100 flex items-center justify-between">
                <h2 className="text-lg font-semibold text-gray-800 flex items-center gap-2">
                  <CreditCard className="w-5 h-5 text-gray-400" />
                  Payment Method
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
                      "flex items-start gap-4 p-4 rounded-xl border-2 cursor-pointer transition",
                      selectedPayment === value
                        ? "border-primary-500 bg-primary-50"
                        : "border-gray-100 hover:border-gray-200"
                    )}
                  >
                    <input
                      type="radio"
                      name="payment"
                      value={value}
                      checked={selectedPayment === value}
                      onChange={() => setSelectedPayment(value)}
                      className="mt-1 text-primary-600"
                    />
                    <div className={cn(
                      "w-11 h-11 rounded-xl flex items-center justify-center shrink-0",
                      selectedPayment === value
                        ? color || "bg-primary-100 text-primary-600"
                        : "bg-gray-100 text-gray-500",
                      selectedPayment === value && color && "text-white"
                    )}>
                      <Icon className="w-5 h-5" />
                    </div>
                    <div className="flex-1 min-w-0">
                      <div className="flex items-center gap-2 flex-wrap">
                        <p className="text-sm font-semibold text-gray-800">{label}</p>
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
                          <span className="px-2 py-0.5 bg-[#635BFF]/10 text-[#635BFF] text-xs font-bold rounded-full">
                            Stripe
                          </span>
                        )}
                      </div>
                      <p className="text-xs text-gray-500 mt-0.5">{desc}</p>
                    </div>
                    {selectedPayment === value && <CheckCircle className="w-5 h-5 text-primary-600 shrink-0" />}
                  </label>
                ))}

                {/* Stripe Info */}
                {selectedPayment === "stripe" && (
                  <div className="p-3 bg-purple-50 border border-purple-100 rounded-lg">
                    <div className="flex items-start gap-2">
                      <Lock className="w-4 h-4 text-purple-600 mt-0.5 shrink-0" />
                      <div className="text-xs text-purple-700">
                        <p className="font-semibold mb-1">You'll be redirected to Stripe</p>
                        <p>Enter your card details on Stripe's secure page. We don't store your card info.</p>
                      </div>
                    </div>
                  </div>
                )}

                {/* COD Info */}
                {selectedPayment === "cod" && (
                  <div className="p-3 bg-orange-50 border border-orange-100 rounded-lg">
                    <div className="flex items-start gap-2">
                      <Truck className="w-4 h-4 text-orange-600 mt-0.5 shrink-0" />
                      <div className="text-xs text-orange-700">
                        <p className="font-semibold mb-1">Cash on Delivery</p>
                        <p>Pay {formatPrice(total)} in cash when delivered. No online payment required.</p>
                      </div>
                    </div>
                  </div>
                )}
              </div>
            </div>
          )}

          {/* Order Items */}
          <div className="bg-white rounded-xl border border-gray-100 shadow-sm overflow-hidden">
            <div className="px-5 py-4 border-b border-gray-100">
              <h2 className="text-lg font-semibold text-gray-800 flex items-center gap-2">
                <ShoppingBag className="w-5 h-5 text-gray-400" />
                Order Items ({items.length})
              </h2>
            </div>
            <div className="divide-y divide-gray-50">
              {items.map((item) => {
                const imgSrc = item.imageUrl || `https://placehold.co/60x60/e2e8f0/64748b?text=${encodeURIComponent(item.productName.charAt(0))}`;
                return (
                  <div key={item.id} className="flex items-center gap-4 px-5 py-3">
                    <img src={imgSrc} alt={item.productName} onError={(e) => { (e.target as HTMLImageElement).src = `https://placehold.co/60x60/e2e8f0/64748b?text=${item.productName.charAt(0)}`; }} className="w-12 h-12 rounded-lg object-cover bg-gray-50 border border-gray-100" />
                    <div className="flex-1 min-w-0">
                      <p className="text-sm font-medium text-gray-800 truncate">{item.productName}</p>
                      <p className="text-xs text-gray-500">Qty: {item.quantity} × {formatPrice(item.unitPrice)}</p>
                    </div>
                    <span className="text-sm font-bold text-gray-900 shrink-0">{formatPrice(item.subtotal)}</span>
                  </div>
                );
              })}
            </div>
          </div>
        </div>

        {/* ═══════════════════════════════════════════════════════════════════ */}
        {/* Right: Summary */}
        {/* ═══════════════════════════════════════════════════════════════════ */}
        <div>
          <div className="bg-white rounded-xl border border-gray-100 shadow-sm p-6 sticky top-24">
            <h2 className="text-xl font-bold text-gray-900 mb-6">Order Summary</h2>

            {/* Selected Address */}
            {selectedAddress && (
              <div className="mb-4 p-3 bg-primary-50 border border-primary-100 rounded-lg">
                <p className="text-xs font-semibold text-primary-700 mb-1 flex items-center gap-1">
                  <MapPin className="w-3 h-3" /> Delivering to
                </p>
                <p className="text-sm font-medium text-gray-800">{selectedAddress.fullName}</p>
                <p className="text-xs text-gray-600">{selectedAddress.city}, {selectedAddress.postalCode}</p>
              </div>
            )}

            {/* Selected Payment */}
            {currentStep >= 2 && selectedMethod && (
              <div className="mb-4 p-3 bg-blue-50 border border-blue-100 rounded-lg">
                <p className="text-xs font-semibold text-blue-700 mb-1 flex items-center gap-1">
                  <CreditCard className="w-3 h-3" /> Payment
                </p>
                <p className="text-sm font-medium text-gray-800">{selectedMethod.label}</p>
                {selectedMethod.isStripe && (
                  <p className="text-xs text-purple-600 mt-1">🔒 Secured by Stripe</p>
                )}
              </div>
            )}

            {/* Coupon */}
            {cart?.couponCode && (
              <div className="mb-4 p-3 bg-green-50 border border-green-200 rounded-lg flex items-center gap-2">
                <Tag className="w-4 h-4 text-green-600" />
                <span className="text-sm font-medium text-green-700">{cart.couponCode}</span>
              </div>
            )}

            {/* Prices */}
            <div className="space-y-3 mb-6">
              <div className="flex justify-between text-sm text-gray-600">
                <span>Subtotal ({totalItems} items)</span>
                <span>{formatPrice(subtotal)}</span>
              </div>
              {discount > 0 && (
                <div className="flex justify-between text-sm text-green-600">
                  <span>Discount</span><span>-{formatPrice(discount)}</span>
                </div>
              )}
              <div className="flex justify-between text-sm text-gray-600">
                <span className="flex items-center gap-1"><Truck className="w-3.5 h-3.5" /> Shipping</span>
                <span className={cn(shipping === 0 && "text-green-600 font-medium")}>
                  {shipping === 0 ? "FREE" : formatPrice(shipping)}
                </span>
              </div>
              <div className="flex justify-between text-sm text-gray-600">
                <span>Tax (18% GST)</span>
                <span>{formatPrice(tax)}</span>
              </div>
              <hr className="border-gray-100" />
              <div className="flex justify-between font-bold text-lg text-gray-900">
                <span>Total</span>
                <span className="text-primary-600">{formatPrice(total)}</span>
              </div>
            </div>

            {/* Place Order Button - Only Step 2 */}
            {currentStep === 2 && (
              <>
                <Button
                  fullWidth
                  size="lg"
                  onClick={handlePlaceOrder}
                  isLoading={isPlacingOrder}
                  disabled={!selectedAddressId}
                >
                  {selectedMethod?.isStripe ? (
                    <>
                      <Lock className="w-5 h-5" />
                      Pay {formatPrice(total)} with Stripe
                    </>
                  ) : selectedMethod?.value === "cod" ? (
                    <>
                      <Truck className="w-5 h-5" />
                      Place Order (COD)
                    </>
                  ) : (
                    <>
                      <CreditCard className="w-5 h-5" />
                      Place Order {formatPrice(total)}
                    </>
                  )}
                </Button>

                {/* Test Card Info */}
                {selectedMethod?.isStripe && import.meta.env.DEV && (
                  <div className="mt-3 p-2 bg-yellow-50 border border-yellow-200 rounded-lg text-center">
                    <p className="text-xs font-semibold text-yellow-800">
                      🧪 Test card: <span className="font-mono">4242 4242 4242 4242</span>
                    </p>
                    <p className="text-xs text-yellow-700 mt-0.5">
                      Any future date, any CVC
                    </p>
                  </div>
                )}
              </>
            )}

            <div className="mt-6 pt-4 border-t border-gray-100 flex items-center justify-center gap-2 text-xs text-gray-400">
              <Shield className="w-4 h-4" />
              Secure Checkout • 256-bit SSL
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Checkout;