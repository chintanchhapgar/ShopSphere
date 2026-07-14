import axiosInstance from "./axiosInstance";
import type {
  OrderListItem,
  OrderDetail,
  CreateOrderCommand,
  Payment,
  PaymentRequest,
  PaymentSucceededRequest,
  PaymentFailedRequest,
} from "@/types";

export const orderApi = {
  // POST /api/orders
  createOrder: async (data: CreateOrderCommand): Promise<any> => {
    const res = await axiosInstance.post("/orders", data);
    console.log("=== CREATE ORDER RAW ===", res.data);
    return res.data?.data ?? res.data?.value ?? res.data;
  },

  // GET /api/orders/my
  getMyOrders: async (): Promise<OrderListItem[]> => {
    const res = await axiosInstance.get("/orders/my");
    const data = res.data?.data ?? res.data?.value ?? res.data;
    return Array.isArray(data) ? data : [];
  },

  // GET /api/orders/:id
  getOrderById: async (id: string): Promise<OrderDetail> => {
    const res = await axiosInstance.get(`/orders/${id}`);
    return res.data?.data ?? res.data?.value ?? res.data;
  },

  // POST /api/orders/:id/cancel
  cancelOrder: async (id: string): Promise<void> => {
    await axiosInstance.post(`/orders/${id}/cancel`);
  },

  // ── Payment APIs ──────────────────────────────────────────────────────────

  // POST /api/orders/:orderId/payment → returns paymentId
  initiatePayment: async (orderId: string, data: PaymentRequest): Promise<string> => {
    const res = await axiosInstance.post(`/orders/${orderId}/payment`, data);
    console.log("=== INITIATE PAYMENT RAW ===", res.data);
    return res.data?.data ?? res.data?.value ?? "";
  },

  // GET /api/orders/:orderId/payment → returns Payment object
  getPayment: async (orderId: string): Promise<Payment> => {
    const res = await axiosInstance.get(`/orders/${orderId}/payment`);
    console.log("=== GET PAYMENT RAW ===", res.data);
    return res.data?.data ?? res.data?.value ?? res.data;
  },

  // POST /api/orders/:paymentId/success (Admin only)
  markPaymentSuccess: async (paymentId: string, data: PaymentSucceededRequest): Promise<void> => {
    await axiosInstance.post(`/orders/${paymentId}/success`, data);
  },

  // POST /api/orders/:paymentId/failed (Admin only)
  markPaymentFailed: async (paymentId: string, data: PaymentFailedRequest): Promise<void> => {
    await axiosInstance.post(`/orders/${paymentId}/failed`, data);
  },

  // POST /api/orders/:paymentId/refund (Admin only)
  refundPayment: async (paymentId: string): Promise<void> => {
    await axiosInstance.post(`/orders/${paymentId}/refund`);
  },
};