import axiosInstance from "./axiosInstance";
import type { Inventory, AdjustInventoryRequest } from "@/types";

export const inventoryApi = {
  // GET /api/products/:productId/inventory
  getInventory: async (productId: string): Promise<Inventory> => {
    const res = await axiosInstance.get<Inventory>(
      `/products/${productId}/inventory`
    );
    return res.data;
  },

  // POST /api/products/:productId/inventory/adjust
  adjustInventory: async (
    productId: string,
    data: AdjustInventoryRequest
  ): Promise<void> => {
    await axiosInstance.post(`/products/${productId}/inventory/adjust`, data);
  },

  // GET /api/products/:productId/inventory/history
  getInventoryHistory: async (productId: string): Promise<unknown[]> => {
    const res = await axiosInstance.get(
      `/products/${productId}/inventory/history`
    );
    return res.data;
  },
};