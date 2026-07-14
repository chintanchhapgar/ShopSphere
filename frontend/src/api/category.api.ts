import axiosInstance from "./axiosInstance";
import type { Category, CreateCategoryCommand, UpdateCategoryCommand, ChangeCategoryStatusRequest } from "@/types";

export const categoryApi = {
  // GET /api/categories
  getAll: async (): Promise<Category[]> => {
    const res = await axiosInstance.get("/categories");
    console.log("=== CATEGORIES RAW ===", res.data);
    const data = res.data;
    if (Array.isArray(data)) return data;
    if (Array.isArray(data?.data)) return data.data;
    return [];
  },

  // GET /api/categories/:id
  getById: async (id: string): Promise<Category> => {
    const res = await axiosInstance.get(`/categories/${id}`);
    return res.data?.data ?? res.data;
  },

  // POST /api/categories
  create: async (data: CreateCategoryCommand): Promise<Category> => {
    const res = await axiosInstance.post("/categories", data);
    return res.data?.data ?? res.data;
  },

  // PUT /api/categories/:id
  update: async (id: string, data: UpdateCategoryCommand): Promise<Category> => {
    const res = await axiosInstance.put(`/categories/${id}`, data);
    return res.data?.data ?? res.data;
  },

  // DELETE /api/categories/:id
  delete: async (id: string): Promise<void> => {
    await axiosInstance.delete(`/categories/${id}`);
  },

  // PATCH /api/categories/:id/status
  changeStatus: async (id: string, data: ChangeCategoryStatusRequest): Promise<void> => {
    await axiosInstance.patch(`/categories/${id}/status`, data);
  },
};