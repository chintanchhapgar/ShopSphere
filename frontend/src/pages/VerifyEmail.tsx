import { useEffect, useState } from "react";
import { useSearchParams, Link } from "react-router-dom";
import { CheckCircle, XCircle, Loader, Package } from "lucide-react";
import { authApi } from "@/api/auth.api";
import { APP_NAME } from "@/utils/constants";

type Status = "loading" | "success" | "error";

const VerifyEmail = () => {
  const [searchParams] = useSearchParams();
  const [status, setStatus]   = useState<Status>("loading");
  const [message, setMessage] = useState("");

  const email = searchParams.get("email") || "";
  const token = searchParams.get("token") || "";

  useEffect(() => {
    if (!email || !token) {
      setStatus("error");
      setMessage("Invalid verification link. Missing email or token.");
      return;
    }
    verifyEmail();
  }, []);

  const verifyEmail = async () => {
    setStatus("loading");
    try {
      await authApi.verifyEmail({ email, token });
      setStatus("success");
      setMessage("Your email has been verified successfully!");
    } catch (err) {
      setStatus("error");
      setMessage(
        (err as Error).message ||
          "Verification failed. The link may be expired or invalid."
      );
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-primary-50 flex items-center justify-center px-4">
      <div className="w-full max-w-md">

        {/* Logo */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-12 h-12 bg-primary-600 text-white rounded-xl mb-4">
            <Package className="w-6 h-6" />
          </div>
          <h1 className="text-2xl font-bold text-gray-900">{APP_NAME}</h1>
        </div>

        {/* Card */}
        <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-8 text-center">

          {/* ── Loading ───────────────────────────────────────────────────── */}
          {status === "loading" && (
            <div className="flex flex-col items-center gap-4">
              <div className="w-16 h-16 bg-primary-50 rounded-full flex items-center justify-center">
                <Loader className="w-8 h-8 text-primary-600 animate-spin" />
              </div>
              <h2 className="text-xl font-semibold text-gray-800">
                Verifying your email...
              </h2>
              <p className="text-gray-500 text-sm">Please wait a moment</p>
            </div>
          )}

          {/* ── Success ───────────────────────────────────────────────────── */}
          {status === "success" && (
            <div className="flex flex-col items-center gap-4">
              <div className="w-16 h-16 bg-green-50 rounded-full flex items-center justify-center">
                <CheckCircle className="w-8 h-8 text-green-500" />
              </div>
              <h2 className="text-xl font-semibold text-gray-800">
                Email Verified!
              </h2>
              <p className="text-gray-500 text-sm">{message}</p>
              <p className="text-gray-400 text-xs">
                Account:{" "}
                <span className="font-medium text-gray-600">{email}</span>
              </p>
              <Link
                to="/login"
                className="mt-2 w-full inline-flex items-center justify-center px-6 py-3 bg-primary-600 text-white font-medium rounded-xl hover:bg-primary-700 transition"
              >
                Continue to Login
              </Link>
            </div>
          )}

          {/* ── Error ─────────────────────────────────────────────────────── */}
          {status === "error" && (
            <div className="flex flex-col items-center gap-4">
              <div className="w-16 h-16 bg-red-50 rounded-full flex items-center justify-center">
                <XCircle className="w-8 h-8 text-red-500" />
              </div>
              <h2 className="text-xl font-semibold text-gray-800">
                Verification Failed
              </h2>
              <p className="text-gray-500 text-sm">{message}</p>
              <div className="flex flex-col gap-2 w-full mt-2">
                <button
                  onClick={verifyEmail}
                  className="w-full px-6 py-3 bg-primary-600 text-white font-medium rounded-xl hover:bg-primary-700 transition"
                >
                  Try Again
                </button>
                <Link
                  to="/register"
                  className="w-full px-6 py-3 border border-gray-200 text-gray-700 font-medium rounded-xl hover:bg-gray-50 transition text-center"
                >
                  Back to Register
                </Link>
              </div>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default VerifyEmail;