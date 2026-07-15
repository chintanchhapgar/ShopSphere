import axiosInstance from "./axiosInstance";
import type { ChatRequest, ChatResponse } from "@/types";

export const aiApi = {
  // POST /api/ai/chat
  sendMessage: async (data: ChatRequest): Promise<ChatResponse> => {
    const res = await axiosInstance.post("/ai/chat", data);
    //console.log("=== AI CHAT RESPONSE ===", res.data);

    const responseData = res.data?.data ?? res.data?.value ?? res.data;

    return {
      message:  responseData?.message  || "Sorry, I couldn't process that.",
      products: responseData?.products || null,
    };
  },

  generateDescription: async (data: {
    productName: string;
    category: string;
    brand: string;
    shortInfo?: string;
  }): Promise<string> => {
    const res = await axiosInstance.post("/ai/generate-description", data);
    const result = res.data?.data ?? res.data?.value ?? res.data;
    return result?.description || "";
  },
};