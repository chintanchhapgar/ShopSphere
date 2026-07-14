import axiosInstance from "./axiosInstance";
import type { Brand, CreateBrandCommand, UpdateBrandCommand, ChangeBrandStatusRequest } from "@/types";

export const brandApi = {
  // GET /api/brands
  getAll: async (): Promise<Brand[]> => {
    const res = await axiosInstance.get("/brands");
    console.log("=== BRANDS RAW ===", res.data);
    const data = res.data;
    if (Array.isArray(data)) return data;
    if (Array.isArray(data?.data)) return data.data;
    return [];
  },

  // GET /api/brands/:id
  getById: async (id: string): Promise<Brand> => {
    const res = await axiosInstance.get(`/brands/${id}`);
    return res.data?.data ?? res.data;
  },

  // POST /api/brands
  create: async (data: CreateBrandCommand): Promise<Brand> => {
    const res = await axiosInstance.post("/brands", data);
    return res.data?.data ?? res.data;
  },

  // PUT /api/brands/:id
  update: async (id: string, data: UpdateBrandCommand): Promise<Brand> => {
    const res = await axiosInstance.put(`/brands/${id}`, data);
    return res.data?.data ?? res.data;
  },

  // DELETE /api/brands/:id
  delete: async (id: string): Promise<void> => {
    await axiosInstance.delete(`/brands/${id}`);
  },

  // PATCH /api/brands/:id/status
  changeStatus: async (id: string, data: ChangeBrandStatusRequest): Promise<void> => {
    await axiosInstance.patch(`/brands/${id}/status`, data);
  },
};