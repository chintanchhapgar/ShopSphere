import { useState } from "react";
import { Link, useSearchParams, useNavigate } from "react-router-dom";
import { Lock, Package, CheckCircle, Eye, EyeOff, XCircle } from "lucide-react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { authApi } from "@/api/auth.api";
import Input from "@/components/ui/Input";
import Button from "@/components/ui/Button";
import { APP_NAME } from "@/utils/constants";
import { cn } from "@/utils/cn";
import toast from "react-hot-toast";

// ── Password Rules ───────────────────────────────────────────────────────────
const PASSWORD_RULES = [
  { label: "At least 8 characters",        test: (v: string) => v.length >= 8 },
  { label: "One uppercase letter (A-Z)",   test: (v: string) => /[A-Z]/.test(v) },
  { label: "One lowercase letter (a-z)",   test: (v: string) => /[a-z]/.test(v) },
  { label: "One number (0-9)",             test: (v: string) => /[0-9]/.test(v) },
  { label: "One special character (!@#$)", test: (v: string) => /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(v) },
];

const schema = z
  .object({
    newPassword: z
      .string()
      .min(8, "Password must be at least 8 characters")
      .regex(/[A-Z]/, "Must contain at least one uppercase letter")
      .regex(/[a-z]/, "Must contain at least one lowercase letter")
      .regex(/[0-9]/, "Must contain at least one number")
      .regex(
        /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/,
        "Must contain at least one special character"
      ),
    confirmPassword: z.string(),
  })
  .refine((d) => d.newPassword === d.confirmPassword, {
    message: "Passwords don't match",
    path: ["confirmPassword"],
  });

type FormData = z.infer<typeof schema>;

const ResetPassword = () => {
  const [searchParams] = useSearchParams();
  const navigate        = useNavigate();

  const [isSuccess, setIsSuccess]       = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirm, setShowConfirm]   = useState(false);

  const email = searchParams.get("email") || "";
  const token = searchParams.get("token") || "";

  const {
    register,
    handleSubmit,
    watch,
    formState: { errors, isSubmitting },
  } = useForm<FormData>({
    resolver: zodResolver(schema),
  });

  const passwordValue = watch("newPassword") || "";

  const onSubmit = async (data: FormData) => {
    if (!email || !token) {
      toast.error("Invalid reset link");
      return;
    }
    try {
      await authApi.resetPassword({
        email,
        token,
        newPassword: data.newPassword,
      });
      setIsSuccess(true);
      toast.success("Password reset successfully!");
    } catch (err) {
      toast.error((err as Error).message || "Failed to reset password");
    }
  };

  // Invalid link
  if (!email || !token) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-gray-50 to-primary-50 flex items-center justify-center px-4">
        <div className="w-full max-w-md">
          <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-8 text-center">
            <div className="w-16 h-16 bg-red-50 rounded-full flex items-center justify-center mx-auto mb-4">
              <XCircle className="w-8 h-8 text-red-500" />
            </div>
            <h2 className="text-xl font-bold text-gray-900 mb-2">Invalid Reset Link</h2>
            <p className="text-gray-500 text-sm mb-6">This link is invalid or has expired.</p>
            <Link to="/forgot-password"><Button fullWidth>Request New Link</Button></Link>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-primary-50 flex items-center justify-center px-4">
      <div className="w-full max-w-md">

        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-12 h-12 bg-primary-600 text-white rounded-xl mb-4">
            <Package className="w-6 h-6" />
          </div>
          <h1 className="text-2xl font-bold text-gray-900">{APP_NAME}</h1>
          <p className="text-gray-500 mt-1">Create a new password</p>
        </div>

        <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-8">
          {!isSuccess ? (
            <>
              <div className="mb-6">
                <h2 className="text-xl font-bold text-gray-900">Reset Password</h2>
                <p className="text-sm text-gray-500 mt-1">
                  For <span className="font-medium text-primary-600">{email}</span>
                </p>
              </div>

              <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">

                {/* New Password */}
                <Input
                  label="New Password"
                  type={showPassword ? "text" : "password"}
                  placeholder="••••••••"
                  leftIcon={<Lock className="w-4 h-4" />}
                  rightIcon={
                    <button type="button" onClick={() => setShowPassword(!showPassword)} className="hover:text-gray-600 transition">
                      {showPassword ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                    </button>
                  }
                  error={errors.newPassword?.message}
                  {...register("newPassword")}
                />

                {/* Password Strength */}
                {passwordValue.length > 0 && (
                  <div className="space-y-1.5 p-3 bg-gray-50 rounded-xl border border-gray-100">
                    <p className="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-2">
                      Password Requirements
                    </p>
                    {PASSWORD_RULES.map(({ label, test }) => {
                      const passed = test(passwordValue);
                      return (
                        <div key={label} className={cn("flex items-center gap-2 text-xs transition", passed ? "text-green-600" : "text-gray-400")}>
                          {passed ? <CheckCircle className="w-3.5 h-3.5 shrink-0" /> : <XCircle className="w-3.5 h-3.5 shrink-0" />}
                          {label}
                        </div>
                      );
                    })}

                    {/* Strength Bar */}
                    {(() => {
                      const score = PASSWORD_RULES.filter(({ test }) => test(passwordValue)).length;
                      const pct   = (score / PASSWORD_RULES.length) * 100;
                      const color =
                        score <= 1 ? "bg-red-500" :
                        score <= 2 ? "bg-orange-500" :
                        score <= 3 ? "bg-yellow-500" :
                        score <= 4 ? "bg-blue-500" :
                        "bg-green-500";
                      const label =
                        score <= 1 ? "Weak" :
                        score <= 2 ? "Fair" :
                        score <= 3 ? "Good" :
                        score <= 4 ? "Strong" :
                        "Excellent";

                      return (
                        <div className="mt-2">
                          <div className="flex justify-between mb-1">
                            <span className="text-xs text-gray-500">Strength</span>
                            <span className={cn("text-xs font-semibold", score <= 2 ? "text-red-600" : score <= 3 ? "text-yellow-600" : "text-green-600")}>
                              {label}
                            </span>
                          </div>
                          <div className="w-full h-1.5 bg-gray-200 rounded-full overflow-hidden">
                            <div className={cn("h-full rounded-full transition-all duration-300", color)} style={{ width: `${pct}%` }} />
                          </div>
                        </div>
                      );
                    })()}
                  </div>
                )}

                {/* Confirm Password */}
                <Input
                  label="Confirm Password"
                  type={showConfirm ? "text" : "password"}
                  placeholder="••••••••"
                  leftIcon={<Lock className="w-4 h-4" />}
                  rightIcon={
                    <button type="button" onClick={() => setShowConfirm(!showConfirm)} className="hover:text-gray-600 transition">
                      {showConfirm ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                    </button>
                  }
                  error={errors.confirmPassword?.message}
                  {...register("confirmPassword")}
                />

                <Button type="submit" fullWidth size="lg" isLoading={isSubmitting}>
                  <Lock className="w-4 h-4" /> Reset Password
                </Button>
              </form>

              <div className="mt-6 text-center">
                <Link to="/login" className="text-sm text-gray-500 hover:text-primary-600 transition">
                  Back to Login
                </Link>
              </div>
            </>
          ) : (
            <div className="text-center py-4">
              <div className="w-16 h-16 bg-green-50 rounded-full flex items-center justify-center mx-auto mb-4">
                <CheckCircle className="w-8 h-8 text-green-500" />
              </div>
              <h2 className="text-xl font-bold text-gray-900 mb-2">Password Reset!</h2>
              <p className="text-gray-500 text-sm mb-6">Your password has been updated successfully.</p>
              <Button fullWidth onClick={() => navigate("/login")}>Continue to Login</Button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default ResetPassword;