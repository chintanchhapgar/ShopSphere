import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import {
  Trash2, Plus, Minus, ShoppingBag, ArrowRight, ArrowLeft,
  Tag, X, Truck, CheckCircle, Loader,
} from "lucide-react";
import { useCart } from "@/hooks/useCart";
import { useAuth } from "@/hooks/useAuth";
import { applyCouponThunk } from "@/redux/slices/cartSlice";
import { formatPrice } from "@/utils/formatPrice";
import Button from "@/components/ui/Button";
import Spinner from "@/components/ui/Spinner";
import { cn } from "@/utils/cn";
import toast from "react-hot-toast";

const Cart = () => {
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();
  const {
    cart, isLoading, totalItems, couponCode,
    fetchCart, removeFromCart, updateQuantity, clearCart,
    applyCoupon, removeCoupon,
  } = useCart();

  const [couponInput, setCouponInput]     = useState("");
  const [couponLoading, setCouponLoading] = useState(false);

  useEffect(() => {
    if (isAuthenticated) fetchCart();
  }, [isAuthenticated]);

  const handleRemove = async (itemId: string, name: string) => {
    await removeFromCart(itemId);
    toast.success(`${name} removed`);
  };

  const handleUpdateQty = async (itemId: string, newQty: number) => {
    if (newQty < 1) return;
    await updateQuantity(itemId, newQty);
  };

  const handleClearCart = async () => {
    if (!window.confirm("Clear entire cart?")) return;
    await clearCart();
    toast.success("Cart cleared");
  };

  const handleApplyCoupon = async () => {
    const code = couponInput.trim().toUpperCase();
    if (!code) { toast.error("Enter a coupon code"); return; }

    setCouponLoading(true);
    try {
      const result = await applyCoupon(code);
      // Check if thunk was fulfilled
      if (applyCouponThunk.fulfilled.match(result)) {
        toast.success(`Coupon "${code}" applied! 🎉`);
        setCouponInput("");
      } else {
        toast.error((result.payload as string) || "Invalid coupon");
      }
    } catch (err) {
      toast.error((err as Error).message || "Invalid coupon");
    } finally {
      setCouponLoading(false);
    }
  };

  const handleRemoveCoupon = async () => {
    try {
      await removeCoupon();
      toast.success("Coupon removed");
    } catch (err) {
      toast.error((err as Error).message || "Failed");
    }
  };

  // ── Guards ─────────────────────────────────────────────────────────────────
  if (!isAuthenticated) {
    return (
      <div className="max-w-7xl mx-auto px-4 py-24 text-center">
        <ShoppingBag className="w-20 h-20 text-gray-200 mx-auto mb-6" />
        <h2 className="text-2xl font-bold text-gray-700 mb-3">Login to view your cart</h2>
        <Link to="/login" state={{ from: "/cart" }}><Button size="lg">Login</Button></Link>
      </div>
    );
  }

  if (isLoading && !cart) {
    return <div className="flex justify-center py-32"><Spinner size="lg" className="text-primary-600" /></div>;
  }

  const items = cart?.items ?? [];
  if (items.length === 0) {
    return (
      <div className="max-w-7xl mx-auto px-4 py-24 text-center">
        <ShoppingBag className="w-20 h-20 text-gray-200 mx-auto mb-6" />
        <h2 className="text-2xl font-bold text-gray-700 mb-3">Your cart is empty</h2>
        <p className="text-gray-500 mb-8">Add some products to get started</p>
        <Link to="/products"><Button size="lg">Start Shopping <ArrowRight className="w-5 h-5" /></Button></Link>
      </div>
    );
  }

  // ── Prices ─────────────────────────────────────────────────────────────────
  const subtotal    = cart?.total ?? 0;
  const discount    = cart?.discountAmount ?? 0;
  const hasCoupon   = !!couponCode;
  const afterDiscount = subtotal - discount;
  const shipping    = afterDiscount > 500 ? 0 : 49;
  const tax         = Math.round(afterDiscount * 0.18);
  const total       = afterDiscount + shipping + tax;
  const freeShipGap = Math.max(0, 500 - afterDiscount);

  return (
    <div className="max-w-7xl mx-auto px-4 py-8">

      <div className="flex items-center justify-between mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Shopping Cart</h1>
          <p className="text-gray-500 mt-1">{totalItems} item{totalItems !== 1 ? "s" : ""}</p>
        </div>
        <button onClick={handleClearCart} className="text-sm text-red-500 hover:text-red-700 flex items-center gap-1 transition">
          <Trash2 className="w-4 h-4" /> Clear
        </button>
      </div>

      <Link to="/products" className="inline-flex items-center gap-2 text-sm text-gray-500 hover:text-primary-600 mb-6 transition">
        <ArrowLeft className="w-4 h-4" /> Continue Shopping
      </Link>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">

        {/* ── Items ───────────────────────────────────────────────────────────── */}
        <div className="lg:col-span-2 space-y-4">
          {items.map((item) => {
            const imgSrc = item.imageUrl || `https://placehold.co/120x120/e2e8f0/64748b?text=${encodeURIComponent(item.productName.charAt(0))}`;
            return (
              <div key={item.id} className="flex gap-4 bg-white rounded-xl border border-gray-100 shadow-sm p-4 hover:shadow-md transition">
                <Link to={`/products/${item.productId}`} className="shrink-0">
                  <img src={imgSrc} alt={item.productName} onError={(e) => { (e.target as HTMLImageElement).src = `https://placehold.co/120x120/e2e8f0/64748b?text=${item.productName.charAt(0)}`; }} className="w-24 h-24 object-cover rounded-lg bg-gray-50" />
                </Link>
                <div className="flex-1 min-w-0">
                  <Link to={`/products/${item.productId}`} className="hover:text-primary-600 transition">
                    <h3 className="font-semibold text-gray-800 truncate">{item.productName}</h3>
                  </Link>
                  <p className="text-primary-600 font-bold text-lg mt-1">{formatPrice(item.unitPrice)}</p>
                  <div className="flex items-center justify-between mt-3">
                    <div className="flex items-center border border-gray-200 rounded-lg overflow-hidden">
                      <button onClick={() => handleUpdateQty(item.id, item.quantity - 1)} disabled={item.quantity <= 1 || isLoading} className="p-2 hover:bg-gray-50 disabled:opacity-40 transition"><Minus className="w-3.5 h-3.5" /></button>
                      <span className="px-4 text-sm font-bold min-w-[2.5rem] text-center">{item.quantity}</span>
                      <button onClick={() => handleUpdateQty(item.id, item.quantity + 1)} disabled={isLoading} className="p-2 hover:bg-gray-50 disabled:opacity-40 transition"><Plus className="w-3.5 h-3.5" /></button>
                    </div>
                    <div className="flex items-center gap-4">
                      <span className="font-bold text-gray-900">{formatPrice(item.subtotal)}</span>
                      <button onClick={() => handleRemove(item.id, item.productName)} className="p-2 text-red-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition"><Trash2 className="w-4 h-4" /></button>
                    </div>
                  </div>
                </div>
              </div>
            );
          })}
        </div>

        {/* ── Summary ─────────────────────────────────────────────────────────── */}
        <div>
          <div className="bg-white rounded-xl border border-gray-100 shadow-sm p-6 sticky top-24">
            <h2 className="text-xl font-bold text-gray-900 mb-6">Order Summary</h2>

            {/* ── Coupon ──────────────────────────────────────────────────────── */}
            <div className="mb-6">
              <p className="text-sm font-medium text-gray-700 mb-2 flex items-center gap-1">
                <Tag className="w-4 h-4 text-gray-400" /> Coupon Code
              </p>

              {hasCoupon ? (
                <div className="p-3 bg-green-50 border border-green-200 rounded-xl">
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-2">
                      <CheckCircle className="w-4 h-4 text-green-600" />
                      <div>
                        <span className="text-sm font-bold text-green-700 font-mono">{couponCode}</span>
                        {discount > 0 ? (
                          <p className="text-xs text-green-600 mt-0.5">You save {formatPrice(discount)}</p>
                        ) : (
                          <p className="text-xs text-green-600 mt-0.5">Applied! Discount at checkout</p>
                        )}
                      </div>
                    </div>
                    <button onClick={handleRemoveCoupon} className="p-1.5 text-green-600 hover:text-green-800 hover:bg-green-100 rounded-lg transition">
                      <X className="w-4 h-4" />
                    </button>
                  </div>
                </div>
              ) : (
                <div className="space-y-2">
                  <div className="flex gap-2">
                    <div className="relative flex-1">
                      <Tag className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
                      <input
                        type="text"
                        value={couponInput}
                        onChange={(e) => setCouponInput(e.target.value.toUpperCase())}
                        onKeyDown={(e) => { if (e.key === "Enter") { e.preventDefault(); handleApplyCoupon(); } }}
                        placeholder="Enter code"
                        className="w-full pl-10 pr-3 py-2.5 border border-gray-200 rounded-lg text-sm font-mono focus:outline-none focus:ring-2 focus:ring-primary-500/20 focus:border-primary-400 transition uppercase"
                      />
                    </div>
                    <button
                      onClick={handleApplyCoupon}
                      disabled={couponLoading || !couponInput.trim()}
                      className={cn(
                        "px-4 py-2.5 text-sm font-medium rounded-lg transition flex items-center gap-1 shrink-0",
                        couponInput.trim() ? "bg-primary-600 text-white hover:bg-primary-700" : "bg-gray-100 text-gray-400 cursor-not-allowed",
                        couponLoading && "opacity-50"
                      )}
                    >
                      {couponLoading ? <Loader className="w-4 h-4 animate-spin" /> : "Apply"}
                    </button>
                  </div>
                  <p className="text-xs text-gray-400">💡 Have a coupon? Enter it above</p>
                </div>
              )}
            </div>

            {/* ── Prices ──────────────────────────────────────────────────────── */}
            <div className="space-y-3 mb-6">
              <div className="flex justify-between text-sm text-gray-600">
                <span>Subtotal ({totalItems} items)</span>
                <span>{formatPrice(subtotal)}</span>
              </div>

              {discount > 0 && (
                <div className="flex justify-between text-sm text-green-600 font-medium">
                  <span className="flex items-center gap-1"><Tag className="w-3.5 h-3.5" /> Discount</span>
                  <span>-{formatPrice(discount)}</span>
                </div>
              )}

              <div className="flex justify-between text-sm text-gray-600">
                <span className="flex items-center gap-1"><Truck className="w-3.5 h-3.5" /> Shipping</span>
                <span className={cn(shipping === 0 && "text-green-600 font-medium")}>{shipping === 0 ? "FREE" : formatPrice(shipping)}</span>
              </div>

              <div className="flex justify-between text-sm text-gray-600">
                <span>Tax (18% GST)</span>
                <span>{formatPrice(tax)}</span>
              </div>

              {shipping > 0 && freeShipGap > 0 && (
                <div className="p-2.5 bg-blue-50 border border-blue-100 rounded-lg">
                  <p className="text-xs text-blue-700">💡 Add {formatPrice(freeShipGap)} more for <span className="font-semibold">free shipping!</span></p>
                </div>
              )}

              <hr className="border-gray-100" />

              <div className="flex justify-between font-bold text-lg text-gray-900">
                <span>Total</span>
                <span className="text-primary-600">{formatPrice(total)}</span>
              </div>
            </div>

            <Button fullWidth size="lg" onClick={() => navigate("/checkout")}>
              Proceed to Checkout <ArrowRight className="w-5 h-5" />
            </Button>

            <Link to="/products">
              <Button fullWidth variant="ghost" size="md" className="mt-3">
                <ArrowLeft className="w-4 h-4" /> Continue Shopping
              </Button>
            </Link>

            <div className="mt-6 pt-4 border-t border-gray-100 text-center">
              <p className="text-xs text-gray-400">🔒 Secure Checkout • 256-bit SSL</p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Cart;