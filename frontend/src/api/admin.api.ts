import axiosInstance from "./axiosInstance";
import type { OrderListItem, Review } from "@/types";

// ── Dashboard Types ──────────────────────────────────────────────────────────
export interface DashboardStats {
  totalUsers:         number;
  totalProducts:      number;
  totalOrders:        number;
  totalRevenue:       number;
  pendingOrders:      number;
  completedOrders:    number;
  lowStockProducts:   number;
  outOfStockProducts: number;
  todayOrders:        number;
  todayRevenue:       number;
}

export interface SalesDataItem {
  date:    string;
  revenue: number;
  orders:  number;
}

export const adminApi = {
  // GET /api/admin/dashboard
  getDashboard: async (): Promise<DashboardStats> => {
    const res = await axiosInstance.get("/admin/dashboard");
    console.log("=== DASHBOARD RAW ===", res.data);
    return res.data?.data ?? res.data?.value ?? res.data;
  },

  // GET /api/admin/dashboard/sales?days=30
  getSalesAnalytics: async (days: number): Promise<SalesDataItem[]> => {
    const res = await axiosInstance.get("/admin/dashboard/sales", { params: { days } });
    console.log("=== SALES RAW ===", res.data);

    const data = res.data?.data ?? res.data?.value ?? res.data;

    // Response: { data: { items: [...] } }
    if (data?.items && Array.isArray(data.items)) return data.items;
    if (Array.isArray(data)) return data;
    return [];
  },

  // GET /api/admin/orders
  getAllOrders: async (): Promise<OrderListItem[]> => {
    const res = await axiosInstance.get("/admin/orders");
    const data = res.data?.data ?? res.data?.value ?? res.data;
    return Array.isArray(data) ? data : [];
  },

  // PATCH /api/admin/orders/:id/status
  updateOrderStatus: async (id: string, status: number): Promise<void> => {
    await axiosInstance.patch(`/admin/orders/${id}/status`, { status });
  },

  // GET /api/admin/reviews/pending
  getPendingReviews: async (): Promise<Review[]> => {
    const res = await axiosInstance.get("/admin/reviews/pending");
    const data = res.data?.data ?? res.data?.value ?? res.data;
    return Array.isArray(data) ? data : [];
  },

  // PUT /api/admin/reviews/:id/approve
  approveReview: async (id: string): Promise<void> => {
    await axiosInstance.put(`/admin/reviews/${id}/approve`);
  },

  // PUT /api/admin/reviews/:id/reject
  rejectReview: async (id: string): Promise<void> => {
    await axiosInstance.put(`/admin/reviews/${id}/reject`);
  },
};