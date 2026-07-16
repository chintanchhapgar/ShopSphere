import axiosInstance from "./axiosInstance";

export interface StripeCheckoutResponse {
  sessionId: string;
  sessionUrl: string;
}

export const stripeApi = {
  // POST /api/stripe/checkout/:orderId
  createCheckoutSession: async (orderId: string): Promise<StripeCheckoutResponse> => {
    const res = await axiosInstance.post(`/stripe/checkout/${orderId}`);
    console.log("=== STRIPE SESSION ===", res.data);

    const data = res.data?.data ?? res.data?.value ?? res.data;

    return {
      sessionId:  data?.sessionId  || "",
      sessionUrl: data?.sessionUrl || "",
    };
  },

  // GET /api/stripe/session/:sessionId
  getSessionStatus: async (sessionId: string): Promise<any> => {
    const res = await axiosInstance.get(`/stripe/session/${sessionId}`);
    return res.data?.data ?? res.data?.value ?? res.data;
  },
};