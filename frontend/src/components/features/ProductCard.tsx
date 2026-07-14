import { Link } from "react-router-dom";
import { ShoppingCart, Star, Heart, Tag } from "lucide-react";
import { useState } from "react";
import { useCart } from "@/hooks/useCart";
import { useAuth } from "@/hooks/useAuth";
import { formatPrice } from "@/utils/formatPrice";
import { cn } from "@/utils/cn";
import type { Product } from "@/types";
import toast from "react-hot-toast";

interface ProductCardProps {
  product: Product;
}

const ProductCard = ({ product }: ProductCardProps) => {
  const { addToCart, isLoading } = useCart();
  const { isAuthenticated }      = useAuth();
  const [isWishlisted, setIsWishlisted] = useState(false);
  const [imgError, setImgError]         = useState(false);

  const handleAddToCart = async (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    if (!isAuthenticated) {
      toast.error("Please login to add items to cart");
      return;
    }
    await addToCart(product.id, 1);
    toast.success(`${product.name} added to cart!`);
  };

  const handleWishlist = (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setIsWishlisted(!isWishlisted);
    toast.success(
      isWishlisted ? "Removed from wishlist" : "Added to wishlist"
    );
  };

  // ── Placeholder image when primaryImageUrl is null ────────────────────────
  const imageSrc =
    !imgError && product.primaryImageUrl
      ? product.primaryImageUrl
      : `https://placehold.co/300x300/e2e8f0/64748b?text=${encodeURIComponent(
          product.name.charAt(0)
        )}`;

  return (
    <Link
      to={`/products/${product.id}`}
      className="group bg-white rounded-xl border border-gray-100 shadow-sm hover:shadow-md hover:-translate-y-0.5 transition-all duration-200 overflow-hidden flex flex-col"
    >
      {/* ── Image ─────────────────────────────────────────────────────────── */}
      <div className="relative overflow-hidden bg-gray-50 aspect-square">
        <img
          src={imageSrc}
          alt={product.name}
          onError={() => setImgError(true)}
          className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
        />

        {/* Out of Stock Overlay */}
        {!product.isActive && (
          <div className="absolute inset-0 bg-black/40 flex items-center justify-center">
            <span className="bg-red-500 text-white text-xs font-medium px-3 py-1 rounded-full">
              Unavailable
            </span>
          </div>
        )}

        {/* Wishlist Button */}
        <button
          onClick={handleWishlist}
          className="absolute top-2 right-2 p-1.5 bg-white rounded-full shadow-md hover:scale-110 transition opacity-0 group-hover:opacity-100"
        >
          <Heart
            className={cn(
              "w-4 h-4 transition",
              isWishlisted
                ? "fill-red-500 text-red-500"
                : "text-gray-400"
            )}
          />
        </button>

        {/* Category Badge */}
        <div className="absolute top-2 left-2">
          <span className="inline-flex items-center gap-1 px-2 py-0.5 bg-white/90 backdrop-blur-sm text-gray-600 text-xs font-medium rounded-full shadow-sm">
            <Tag className="w-3 h-3" />
            {product.categoryName}
          </span>
        </div>
      </div>

      {/* ── Info ──────────────────────────────────────────────────────────── */}
      <div className="p-4 flex flex-col flex-1">
        {/* Brand */}
        <p className="text-xs text-primary-600 font-medium mb-1">
          {product.brandName}
        </p>

        {/* Name */}
        <h3 className="font-semibold text-gray-800 text-sm line-clamp-2 leading-snug mb-2 flex-1">
          {product.name}
        </h3>

        {/* Description */}
        <p className="text-xs text-gray-400 line-clamp-1 mb-3">
          {product.description}
        </p>

        {/* Price & Add to Cart */}
        <div className="flex items-center justify-between gap-2 mt-auto">
          <div>
            <p className="text-lg font-bold text-gray-900">
              {formatPrice(product.basePrice)}
            </p>
            {product.costPrice && product.costPrice < product.basePrice && (
              <p className="text-xs text-gray-400 line-through">
                {formatPrice(product.costPrice)}
              </p>
            )}
          </div>

          <button
            onClick={handleAddToCart}
            disabled={isLoading || !product.isActive}
            className={cn(
              "flex items-center gap-1.5 px-3 py-2 text-xs font-medium rounded-lg transition",
              "disabled:opacity-50 disabled:cursor-not-allowed",
              product.isActive
                ? "bg-primary-600 text-white hover:bg-primary-700"
                : "bg-gray-100 text-gray-400"
            )}
          >
            <ShoppingCart className="w-3.5 h-3.5" />
            Add
          </button>
        </div>
      </div>
    </Link>
  );
};

export default ProductCard;