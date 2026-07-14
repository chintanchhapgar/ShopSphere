import { useEffect, useState } from "react";
import { Star, CheckCircle, XCircle, MessageSquare } from "lucide-react";
import { adminApi } from "@/api/admin.api";
import type { Review } from "@/types";
import Spinner from "@/components/ui/Spinner";
import { cn } from "@/utils/cn";
import toast from "react-hot-toast";

const RATING_LABELS = ["", "Poor", "Fair", "Good", "Very Good", "Excellent"];

const AdminReviews = () => {
  const [reviews, setReviews]     = useState<Review[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  const loadPendingReviews = async () => {
    setIsLoading(true);
    try {
      const data = await adminApi.getPendingReviews();
      setReviews(data);
    } catch {
      setReviews([]);
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadPendingReviews();
  }, []);

  const handleApprove = async (id: string) => {
    try {
      await adminApi.approveReview(id);
      toast.success("Review approved!");
      setReviews((prev) => prev.filter((r) => r.id !== id));
    } catch (err) {
      toast.error((err as Error).message || "Failed to approve");
    }
  };

  const handleReject = async (id: string) => {
    if (!window.confirm("Reject this review?")) return;
    try {
      await adminApi.rejectReview(id);
      toast.success("Review rejected");
      setReviews((prev) => prev.filter((r) => r.id !== id));
    } catch (err) {
      toast.error((err as Error).message || "Failed to reject");
    }
  };

  if (isLoading) {
    return (
      <div className="flex justify-center py-32">
        <Spinner size="lg" className="text-primary-600" />
      </div>
    );
  }

  return (
    <div className="max-w-4xl mx-auto px-4 py-8">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900">Pending Reviews</h1>
        <p className="text-gray-500 mt-1">
          {reviews.length} review{reviews.length !== 1 ? "s" : ""} awaiting approval
        </p>
      </div>

      {reviews.length === 0 ? (
        <div className="text-center py-20 bg-gray-50 rounded-xl border border-gray-100">
          <MessageSquare className="w-14 h-14 text-gray-200 mx-auto mb-3" />
          <p className="text-gray-600 font-semibold text-lg">No pending reviews</p>
          <p className="text-sm text-gray-400 mt-1">All reviews have been processed</p>
        </div>
      ) : (
        <div className="space-y-4">
          {reviews.map((review) => (
            <div
              key={review.id}
              className="p-6 bg-white border border-gray-100 rounded-xl shadow-sm"
            >
              {/* Header */}
              <div className="flex items-start justify-between mb-4">
                <div className="flex items-center gap-3">
                  <div className="w-10 h-10 bg-gradient-to-br from-primary-400 to-primary-600 text-white rounded-full flex items-center justify-center text-sm font-bold">
                    {(review.userName || review.userId || "U")
                      .charAt(0)
                      .toUpperCase()}
                  </div>
                  <div>
                    <p className="text-sm font-semibold text-gray-800">
                      {review.userName || "Anonymous User"}
                    </p>
                    <div className="flex items-center gap-1 mt-0.5">
                      {Array.from({ length: 5 }, (_, i) => (
                        <Star
                          key={i}
                          className={cn(
                            "w-4 h-4",
                            i < review.rating
                              ? "fill-yellow-400 text-yellow-400"
                              : "text-gray-200"
                          )}
                        />
                      ))}
                      <span className="text-xs text-gray-500 ml-1">
                        {RATING_LABELS[review.rating]}
                      </span>
                    </div>
                  </div>
                </div>
                <span className="text-xs text-gray-400">
                  {new Date(review.createdAt).toLocaleDateString("en-IN", {
                    year: "numeric",
                    month: "short",
                    day: "numeric",
                  })}
                </span>
              </div>

              {/* Comment */}
              {review.comment && (
                <p className="text-sm text-gray-600 leading-relaxed mb-4 pl-[52px]">
                  {review.comment}
                </p>
              )}

              {/* Product ID */}
              <p className="text-xs text-gray-400 mb-4 pl-[52px] font-mono">
                Product: {review.productId}
              </p>

              {/* Actions */}
              <div className="flex gap-3 pl-[52px]">
                <button
                  onClick={() => handleApprove(review.id)}
                  className="flex items-center gap-2 px-4 py-2 bg-green-600 text-white text-sm font-medium rounded-lg hover:bg-green-700 transition"
                >
                  <CheckCircle className="w-4 h-4" />
                  Approve
                </button>
                <button
                  onClick={() => handleReject(review.id)}
                  className="flex items-center gap-2 px-4 py-2 bg-red-50 text-red-600 text-sm font-medium rounded-lg border border-red-200 hover:bg-red-100 transition"
                >
                  <XCircle className="w-4 h-4" />
                  Reject
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default AdminReviews;