import axiosInstance from "./axiosInstance";
import type { Address, CreateAddressCommand, UpdateAddressCommand } from "@/types";

export const addressApi = {
  // GET /api/addresses
  // Response: { data: [...], success, message }
  getAddresses: async (): Promise<Address[]> => {
    const res = await axiosInstance.get("/addresses");
    console.log("=== ADDRESSES RAW ===", res.data);

    const data = res.data?.data ?? res.data?.value ?? res.data;

    // data is directly an array
    if (Array.isArray(data)) return data;

    // data might have items key
    if (Array.isArray(data?.items)) return data.items;

    return [];
  },

  // POST /api/addresses
  createAddress: async (data: CreateAddressCommand): Promise<Address> => {
    const res = await axiosInstance.post("/addresses", data);
    return res.data?.data ?? res.data?.value ?? res.data;
  },

  // PUT /api/addresses/:id
  updateAddress: async (id: string, data: UpdateAddressCommand): Promise<Address> => {
    const res = await axiosInstance.put(`/addresses/${id}`, data);
    return res.data?.data ?? res.data?.value ?? res.data;
  },

  // DELETE /api/addresses/:id
  deleteAddress: async (id: string): Promise<void> => {
    await axiosInstance.delete(`/addresses/${id}`);
  },

  // PUT /api/addresses/:id/default
  setDefaultAddress: async (id: string): Promise<void> => {
    await axiosInstance.put(`/addresses/${id}/default`);
  },
};