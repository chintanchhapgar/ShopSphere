import axiosInstance from "./axiosInstance";
import type {
  Cart,
  AddCartItemRequest,
  UpdateCartItemRequest,
  ApplyCouponCommand,
} from "@/types";

const emptyCart: Cart = {
  items: [],
  total: 0,
  couponCode: null,
  discountAmount: 0,
};

const unwrapCart = (resData: any): Cart => {
  const data = resData?.value ?? resData?.data ?? resData;

  if (!data) return emptyCart;

  return {
    items:          Array.isArray(data?.items) ? data.items : [],
    total:          data?.total ?? 0,
    couponCode:     data?.couponCode ?? data?.appliedCoupon ?? null,
    discountAmount: data?.discountAmount ?? data?.discount ?? 0,
  };
};

export const cartApi = {
  // GET /api/cart
  getCart: async (): Promise<Cart> => {
    try {
      const res = await axiosInstance.get("/cart");
      console.log("=== CART RAW ===", res.data);
      return unwrapCart(res.data);
    } catch (err: any) {
      // If cart doesn't exist, return empty
      if (err?.response?.status === 404) {
        return emptyCart;
      }
      throw err;
    }
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

  applyCoupon: async (data: ApplyCouponCommand): Promise<void> => {
    await axiosInstance.post("/cart/coupon", data);
  },

  removeCoupon: async (): Promise<void> => {
    await axiosInstance.delete("/cart/coupon");
  },
};