import axiosInstance from "./axiosInstance";
import type { Review, ReviewsResponse, AddReviewRequest } from "@/types";

export const reviewApi = {
  // GET /api/reviews/:productId
  // Response: { data: { statistics: {...}, reviews: [...] }, success, message }
  getProductReviews: async (productId: string): Promise<ReviewsResponse> => {
    const res = await axiosInstance.get(`/reviews/${productId}`);
    console.log("=== REVIEWS RAW ===", res.data);

    const data = res.data?.data ?? res.data?.value ?? res.data;

    return {
      statistics: data?.statistics ?? {
        averageRating: 0,
        totalReviews:  0,
        fiveStar:      0,
        fourStar:      0,
        threeStar:     0,
        twoStar:       0,
        oneStar:       0,
      },
      reviews: Array.isArray(data?.reviews) ? data.reviews : [],
    };
  },

  // POST /api/reviews/:productId
  // Response: { data: "review-id", success, message }
  addReview: async (
    productId: string,
    data: AddReviewRequest
  ): Promise<string> => {
    const res = await axiosInstance.post(`/reviews/${productId}`, data);
    console.log("=== ADD REVIEW RAW ===", res.data);
    return res.data?.data ?? res.data?.value ?? "";
  },
};