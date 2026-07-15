import axiosInstance from "./axiosInstance";
import type { WishlistItem, AddToWishlistCommand } from "@/types";

export const wishlistApi = {
  // GET /api/wishlist
  // Response: { data: { items: [...] }, success, message }
  getWishlist: async (): Promise<WishlistItem[]> => {
    const res = await axiosInstance.get("/wishlist");
    //console.log("=== WISHLIST RAW ===", res.data);

    const data  = res.data?.data ?? res.data?.value ?? res.data;
    const items = data?.items ?? data;
    return Array.isArray(items) ? items : [];
  },

  // POST /api/wishlist
  // Response: { success, message }
  addToWishlist: async (data: AddToWishlistCommand): Promise<void> => {
    await axiosInstance.post("/wishlist", data);
  },

  // DELETE /api/wishlist/:productId
  removeFromWishlist: async (productId: string): Promise<void> => {
    await axiosInstance.delete(`/wishlist/${productId}`);
  },

  // POST /api/wishlist/:productId/move-to-cart
  moveToCart: async (productId: string): Promise<void> => {
    await axiosInstance.post(`/wishlist/${productId}/move-to-cart`);
  },
};