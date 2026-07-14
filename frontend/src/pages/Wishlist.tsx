import { useEffect } from "react";
import { Link } from "react-router-dom";
import {
  Heart,
  Trash2,
  ShoppingCart,
  ArrowRight,
  ArrowLeft,
  ShoppingBag,
  Tag,
  CheckCircle,
  XCircle,
} from "lucide-react";
import { useWishlist } from "@/hooks/useWishlist";
import { useCart } from "@/hooks/useCart";
import { useAuth } from "@/hooks/useAuth";
import { formatPrice } from "@/utils/formatPrice";
import { cn } from "@/utils/cn";
import Spinner from "@/components/ui/Spinner";
import Button from "@/components/ui/Button";
import toast from "react-hot-toast";

const Wishlist = () => {
  const { isAuthenticated } = useAuth();
  const {
    items,
    isLoading,
    fetchWishlist,
    removeFromWishlist,
    moveToCart,
  } = useWishlist();
  const { fetchCart } = useCart();

  // ── Fetch Wishlist ─────────────────────────────────────────────────────────
  useEffect(() => {
    if (isAuthenticated) {
      fetchWishlist();
    }
  }, [isAuthenticated]);

  // ── Handlers ───────────────────────────────────────────────────────────────
  const handleRemove = async (productId: string, name: string) => {
    try {
      await removeFromWishlist(productId);
      toast.success(`${name} removed from wishlist`);
    } catch (err) {
      toast.error((err as Error).message || "Failed to remove");
    }
  };

  const handleMoveToCart = async (productId: string, name: string) => {
    try {
      await moveToCart(productId);
      await fetchCart();
      toast.success(`${name} moved to cart!`);
    } catch (err) {
      toast.error((err as Error).message || "Failed to move to cart");
    }
  };

  // ── Not Authenticated ──────────────────────────────────────────────────────
  if (!isAuthenticated) {
    return (
      <div className="max-w-7xl mx-auto px-4 py-24 text-center">
        <Heart className="w-20 h-20 text-gray-200 mx-auto mb-6" />
        <h2 className="text-2xl font-bold text-gray-700 mb-3">
          Login to view your wishlist
        </h2>
        <p className="text-gray-500 mb-8">
          Save your favorite products and access them anytime
        </p>
        <Link to="/login" state={{ from: "/wishlist" }}>
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

  return (
    <div className="max-w-7xl mx-auto px-4 py-8">

      {/* ── Header ──────────────────────────────────────────────────────────── */}
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900 flex items-center gap-3">
          <Heart className="w-8 h-8 text-red-500 fill-red-500" />
          My Wishlist
        </h1>
        <p className="text-gray-500 mt-1">
          {items.length} item{items.length !== 1 ? "s" : ""} saved
        </p>
      </div>

      {/* ── Back ────────────────────────────────────────────────────────────── */}
      <Link
        to="/products"
        className="inline-flex items-center gap-2 text-sm text-gray-500 hover:text-primary-600 mb-6 transition"
      >
        <ArrowLeft className="w-4 h-4" />
        Continue Shopping
      </Link>

      {/* ── Empty State ─────────────────────────────────────────────────────── */}
      {items.length === 0 && (
        <div className="text-center py-24 bg-gray-50 rounded-xl border border-gray-100">
          <Heart className="w-20 h-20 text-gray-200 mx-auto mb-6" />
          <h2 className="text-2xl font-bold text-gray-700 mb-3">
            Your wishlist is empty
          </h2>
          <p className="text-gray-500 mb-8">
            Start adding products you love
          </p>
          <Link to="/products">
            <Button size="lg">
              Browse Products <ArrowRight className="w-5 h-5" />
            </Button>
          </Link>
        </div>
      )}

      {/* ── Wishlist Grid ───────────────────────────────────────────────────── */}
      {items.length > 0 && (
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
          {items.map((item) => {
            const imgSrc =
              item.imageUrl ||
              `https://placehold.co/300x300/e2e8f0/64748b?text=${encodeURIComponent(
                item.name.charAt(0)
              )}`;

            return (
              <div
                key={item.productId}
                className="bg-white rounded-xl border border-gray-100 shadow-sm hover:shadow-md transition overflow-hidden group flex flex-col"
              >
                {/* ── Image ──────────────────────────────────────────────── */}
                <Link
                  to={`/products/${item.productId}`}
                  className="relative bg-gray-50 aspect-square overflow-hidden"
                >
                  <img
                    src={imgSrc}
                    alt={item.name}
                    onError={(e) => {
                      (e.target as HTMLImageElement).src = `https://placehold.co/300x300/e2e8f0/64748b?text=${item.name.charAt(0)}`;
                    }}
                    className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
                  />

                  {/* Remove Button (on hover) */}
                  <button
                    onClick={(e) => {
                      e.preventDefault();
                      e.stopPropagation();
                      handleRemove(item.productId, item.name);
                    }}
                    className="absolute top-2 right-2 p-2 bg-white rounded-full shadow-md hover:bg-red-50 transition opacity-0 group-hover:opacity-100"
                    title="Remove from wishlist"
                  >
                    <Heart className="w-4 h-4 fill-red-500 text-red-500" />
                  </button>

                  {/* Stock Badge */}
                  {!item.inStock && (
                    <div className="absolute inset-0 bg-black/40 flex items-center justify-center">
                      <span className="bg-red-500 text-white text-xs font-medium px-3 py-1 rounded-full">
                        Out of Stock
                      </span>
                    </div>
                  )}

                  {/* In Stock Badge */}
                  {item.inStock && (
                    <div className="absolute bottom-2 left-2">
                      <span className="inline-flex items-center gap-1 bg-green-500 text-white text-xs font-medium px-2 py-0.5 rounded-full">
                        <CheckCircle className="w-3 h-3" />
                        In Stock
                      </span>
                    </div>
                  )}
                </Link>

                {/* ── Details ────────────────────────────────────────────── */}
                <div className="p-4 flex flex-col flex-1">

                  {/* SKU */}
                  <p className="text-xs text-gray-400 font-mono mb-1">
                    {item.sku}
                  </p>

                  {/* Name */}
                  <Link
                    to={`/products/${item.productId}`}
                    className="hover:text-primary-600 transition"
                  >
                    <h3 className="font-semibold text-gray-800 text-sm line-clamp-2 leading-snug mb-3">
                      {item.name}
                    </h3>
                  </Link>

                  {/* Price */}
                  <p className="text-lg font-bold text-gray-900 mb-4 mt-auto">
                    {formatPrice(item.price)}
                  </p>

                  {/* Action Buttons */}
                  <div className="flex gap-2">
                    <button
                      onClick={() => handleMoveToCart(item.productId, item.name)}
                      disabled={!item.inStock}
                      className={cn(
                        "flex-1 flex items-center justify-center gap-1.5 px-3 py-2.5 text-xs font-medium rounded-lg transition",
                        "disabled:opacity-50 disabled:cursor-not-allowed",
                        item.inStock
                          ? "bg-primary-600 text-white hover:bg-primary-700"
                          : "bg-gray-100 text-gray-400"
                      )}
                    >
                      <ShoppingCart className="w-3.5 h-3.5" />
                      {item.inStock ? "Move to Cart" : "Out of Stock"}
                    </button>
                    <button
                      onClick={() => handleRemove(item.productId, item.name)}
                      className="p-2.5 border border-gray-200 rounded-lg text-gray-400 hover:text-red-500 hover:border-red-200 hover:bg-red-50 transition"
                      title="Remove"
                    >
                      <Trash2 className="w-4 h-4" />
                    </button>
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      )}

      {/* ── Bottom Bar ──────────────────────────────────────────────────────── */}
      {items.length > 0 && (
        <div className="flex flex-col sm:flex-row items-center justify-between gap-4 mt-8 p-4 bg-gray-50 rounded-xl border border-gray-100">
          <p className="text-sm text-gray-500">
            {items.length} item{items.length !== 1 ? "s" : ""} in wishlist
            {items.filter((i) => !i.inStock).length > 0 && (
              <span className="text-red-500 ml-2">
                ({items.filter((i) => !i.inStock).length} out of stock)
              </span>
            )}
          </p>
          <div className="flex gap-3">
            <Link to="/products">
              <Button variant="outline" size="md">
                <ArrowLeft className="w-4 h-4" />
                Continue Shopping
              </Button>
            </Link>
            <Link to="/cart">
              <Button size="md">
                <ShoppingBag className="w-4 h-4" />
                Go to Cart
              </Button>
            </Link>
          </div>
        </div>
      )}
    </div>
  );
};

export default Wishlist;