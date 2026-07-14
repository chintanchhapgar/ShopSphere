import { useState, useCallback } from "react";
import { wishlistApi } from "@/api/wishlist.api";
import type { WishlistItem } from "@/types";

export const useWishlist = () => {
  const [items, setItems]         = useState<WishlistItem[]>([]);
  const [isLoading, setIsLoading] = useState(false);

  const fetchWishlist = useCallback(async () => {
    setIsLoading(true);
    try {
      const data = await wishlistApi.getWishlist();
      setItems(data);
    } catch {
      setItems([]);
    } finally {
      setIsLoading(false);
    }
  }, []);

  const addToWishlist = async (productId: string) => {
    await wishlistApi.addToWishlist({ productId });
    await fetchWishlist();
  };

  const removeFromWishlist = async (productId: string) => {
    await wishlistApi.removeFromWishlist(productId);
    setItems((prev) => prev.filter((i) => i.productId !== productId));
  };

  const moveToCart = async (productId: string) => {
    await wishlistApi.moveToCart(productId);
    setItems((prev) => prev.filter((i) => i.productId !== productId));
  };

  const isWishlisted = (productId: string) =>
    items.some((i) => i.productId === productId);

  const toggleWishlist = async (productId: string): Promise<boolean> => {
    if (isWishlisted(productId)) {
      await removeFromWishlist(productId);
      return false;
    } else {
      await addToWishlist(productId);
      return true;
    }
  };

  // All wishlisted product IDs for quick lookup
  const wishlistedIds = items.map((i) => i.productId);

  return {
    items,
    isLoading,
    totalItems: items.length,
    wishlistedIds,
    fetchWishlist,
    addToWishlist,
    removeFromWishlist,
    moveToCart,
    isWishlisted,
    toggleWishlist,
  };
};