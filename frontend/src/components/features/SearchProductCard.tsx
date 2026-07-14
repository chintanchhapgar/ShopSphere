import { Link } from "react-router-dom";
import { ShoppingCart, Star, Heart, Tag } from "lucide-react";
import { useState, useEffect } from "react";
import { useCart } from "@/hooks/useCart";
import { useAuth } from "@/hooks/useAuth";
import { wishlistApi } from "@/api/wishlist.api";
import { formatPrice } from "@/utils/formatPrice";
import { cn } from "@/utils/cn";
import type { ProductSearchItem } from "@/types";
import toast from "react-hot-toast";

interface SearchProductCardProps {
  product: ProductSearchItem;
  wishlistedIds?: string[];
  onWishlistChange?: () => void;
}

const SearchProductCard = ({
  product,
  wishlistedIds = [],
  onWishlistChange,
}: SearchProductCardProps) => {
  const { addToCart, isLoading }  = useCart();
  const { isAuthenticated }       = useAuth();
  const [isWishlisted, setIsWishlisted] = useState(false);
  const [imgError, setImgError]         = useState(false);
  const [wishLoading, setWishLoading]   = useState(false);

  // ✅ Sync wishlist state from parent
  useEffect(() => {
    setIsWishlisted(wishlistedIds.includes(product.id));
  }, [wishlistedIds, product.id]);

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

  const handleWishlist = async (e: React.MouseEvent) => {
    e.preventDefault();
    e.stopPropagation();
    if (!isAuthenticated) {
      toast.error("Please login to use wishlist");
      return;
    }
    setWishLoading(true);
    try {
      if (isWishlisted) {
        await wishlistApi.removeFromWishlist(product.id);
        setIsWishlisted(false);
        toast.success("Removed from wishlist");
      } else {
        await wishlistApi.addToWishlist({ productId: product.id });
        setIsWishlisted(true);
        toast.success("Added to wishlist ❤️");
      }
      onWishlistChange?.();
    } catch (err) {
      toast.error((err as Error).message || "Failed");
    } finally {
      setWishLoading(false);
    }
  };

  const imageSrc =
    !imgError && product.thumbnail
      ? product.thumbnail
      : `https://placehold.co/300x300/e2e8f0/64748b?text=${encodeURIComponent(product.name.charAt(0))}`;

  const isOutOfStock = product.stock <= 0;

  return (
    <Link
      to={`/products/${product.id}`}
      className="group bg-white rounded-xl border border-gray-100 shadow-sm hover:shadow-md hover:-translate-y-0.5 transition-all duration-200 overflow-hidden flex flex-col"
    >
      <div className="relative overflow-hidden bg-gray-50 aspect-square">
        <img src={imageSrc} alt={product.name} onError={() => setImgError(true)} className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300" />

        {isOutOfStock && (
          <div className="absolute inset-0 bg-black/40 flex items-center justify-center">
            <span className="bg-red-500 text-white text-xs font-medium px-3 py-1 rounded-full">Out of Stock</span>
          </div>
        )}

        {product.isFeatured && (
          <div className="absolute top-2 left-2">
            <span className="bg-yellow-400 text-yellow-900 text-xs font-bold px-2 py-0.5 rounded-full shadow-sm">⭐ Featured</span>
          </div>
        )}

        {/* ✅ Wishlist Button - shows filled heart if wishlisted */}
        <button
          onClick={handleWishlist}
          disabled={wishLoading}
          className={cn(
            "absolute top-2 right-2 p-1.5 rounded-full shadow-md transition",
            isWishlisted
              ? "bg-red-50 opacity-100"
              : "bg-white opacity-0 group-hover:opacity-100",
            wishLoading && "opacity-50"
          )}
        >
          <Heart
            className={cn(
              "w-4 h-4 transition",
              isWishlisted ? "fill-red-500 text-red-500" : "text-gray-400 hover:text-red-400"
            )}
          />
        </button>

        {product.stock > 0 && product.stock <= 5 && (
          <div className="absolute bottom-2 left-2">
            <span className="bg-orange-500 text-white text-xs font-medium px-2 py-0.5 rounded-full">Only {product.stock} left</span>
          </div>
        )}
      </div>

      <div className="p-4 flex flex-col flex-1">
        <div className="flex items-center justify-between gap-2 mb-1">
          <p className="text-xs text-primary-600 font-semibold truncate">{product.brand}</p>
          <span className="inline-flex items-center gap-1 text-xs text-gray-400 shrink-0">
            <Tag className="w-3 h-3" />{product.category}
          </span>
        </div>
        <h3 className="font-semibold text-gray-800 text-sm line-clamp-2 leading-snug mb-2 flex-1">{product.name}</h3>
        <div className="flex items-center gap-1.5 mb-3">
          <div className="flex items-center gap-0.5">
            {Array.from({ length: 5 }, (_, i) => (
              <Star key={i} className={cn("w-3 h-3", i < Math.round(product.averageRating) ? "fill-yellow-400 text-yellow-400" : "text-gray-200")} />
            ))}
          </div>
          <span className="text-xs text-gray-500">{product.averageRating.toFixed(1)}</span>
          <span className="text-xs text-gray-300">({product.totalReviews})</span>
        </div>
        <div className="flex items-center justify-between gap-2 mt-auto">
          <p className="text-lg font-bold text-gray-900">{formatPrice(product.price)}</p>
          <button
            onClick={handleAddToCart}
            disabled={isLoading || isOutOfStock}
            className={cn(
              "flex items-center gap-1.5 px-3 py-2 text-xs font-medium rounded-lg transition",
              "disabled:opacity-50 disabled:cursor-not-allowed",
              !isOutOfStock ? "bg-primary-600 text-white hover:bg-primary-700" : "bg-gray-100 text-gray-400"
            )}
          >
            <ShoppingCart className="w-3.5 h-3.5" />
            {isOutOfStock ? "Sold Out" : "Add"}
          </button>
        </div>
      </div>
    </Link>
  );
};

export default SearchProductCard;