import axiosInstance from "./axiosInstance";
import type {
  Cart,
  AddCartItemRequest,
  UpdateCartItemRequest,
  ApplyCouponCommand,
} from "@/types";

// ── Helper to unwrap both response shapes ────────────────────────────────────
const unwrapCart = (resData: any): Cart => {
  // Shape 1: { value: { items, total } }
  const data = resData?.value ?? resData?.data ?? resData;

  console.log("=== UNWRAPPED CART ===", data);

  return {
    items:          Array.isArray(data?.items) ? data.items : [],
    total:          data?.total ?? 0,
    couponCode:     data?.couponCode ?? null,
    discountAmount: data?.discountAmount ?? 0,
  };
};

export const cartApi = {
  // GET /api/cart
  getCart: async (): Promise<Cart> => {
    const res = await axiosInstance.get("/cart");
    console.log("=== CART RAW ===", res.data);
    return unwrapCart(res.data);
  },

  // POST /api/cart/items
  addItem: async (data: AddCartItemRequest): Promise<Cart> => {
    const res = await axiosInstance.post("/cart/items", data);
    console.log("=== ADD ITEM RAW ===", res.data);
    // Add item may not return full cart, so re-fetch
    return unwrapCart(res.data);
  },

  // PUT /api/cart/items/:itemId
  updateItem: async (
    itemId: string,
    data: UpdateCartItemRequest
  ): Promise<Cart> => {
    const res = await axiosInstance.put(`/cart/items/${itemId}`, data);
    console.log("=== UPDATE ITEM RAW ===", res.data);
    return unwrapCart(res.data);
  },

  // DELETE /api/cart/items/:itemId
  removeItem: async (itemId: string): Promise<Cart> => {
    const res = await axiosInstance.delete(`/cart/items/${itemId}`);
    console.log("=== REMOVE ITEM RAW ===", res.data);
    return unwrapCart(res.data);
  },

  // DELETE /api/cart
  clearCart: async (): Promise<void> => {
    await axiosInstance.delete("/cart");
  },

  // POST /api/cart/coupon
  applyCoupon: async (data: ApplyCouponCommand): Promise<Cart> => {
    const res = await axiosInstance.post("/cart/coupon", data);
    return unwrapCart(res.data);
  },

  // DELETE /api/cart/coupon
  removeCoupon: async (): Promise<Cart> => {
    const res = await axiosInstance.delete("/cart/coupon");
    return unwrapCart(res.data);
  },
};