import axiosInstance from "./axiosInstance";
import type {
  Cart,
  AddCartItemRequest,
  UpdateCartItemRequest,
  ApplyCouponCommand,
} from "@/types";

const unwrapCart = (resData: any): Cart => {
  const data = resData?.value ?? resData?.data ?? resData;
  return {
    items:          Array.isArray(data?.items) ? data.items : [],
    total:          data?.total ?? 0,
    couponCode:     data?.couponCode ?? data?.appliedCoupon ?? null,
    discountAmount: data?.discountAmount ?? data?.discount ?? 0,
  };
};

export const cartApi = {
  getCart: async (): Promise<Cart> => {
    const res = await axiosInstance.get("/cart");
    return unwrapCart(res.data);
  },

  addItem: async (data: AddCartItemRequest): Promise<void> => {
    await axiosInstance.post("/cart/items", data);
  },

  updateItem: async (itemId: string, data: UpdateCartItemRequest): Promise<void> => {
    await axiosInstance.put(`/cart/items/${itemId}`, data);
  },

  removeItem: async (itemId: string): Promise<void> => {
    await axiosInstance.delete(`/cart/items/${itemId}`);
  },

  clearCart: async (): Promise<void> => {
    await axiosInstance.delete("/cart");
  },

  // POST /api/cart/coupon
  applyCoupon: async (data: ApplyCouponCommand): Promise<void> => {
    await axiosInstance.post("/cart/coupon", data);
  },

  // DELETE /api/cart/coupon
  removeCoupon: async (): Promise<void> => {
    await axiosInstance.delete("/cart/coupon");
  },
};