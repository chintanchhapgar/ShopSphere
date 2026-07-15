import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Link, useNavigate } from "react-router-dom";
import { Mail, Lock, User, Package, Eye, EyeOff, CheckCircle, XCircle } from "lucide-react";
import { useAppDispatch, useAppSelector } from "@/redux/store";
import { registerThunk } from "@/redux/slices/authSlice";
import Button from "@/components/ui/Button";
import Input from "@/components/ui/Input";
import toast from "react-hot-toast";
import { APP_NAME } from "@/utils/constants";
import { useEffect, useState } from "react";
import { cn } from "@/utils/cn";

// ── Password Rules ───────────────────────────────────────────────────────────
const PASSWORD_RULES = [
  { label: "At least 8 characters",        test: (v: string) => v.length >= 8 },
  { label: "One uppercase letter (A-Z)",   test: (v: string) => /[A-Z]/.test(v) },
  { label: "One lowercase letter (a-z)",   test: (v: string) => /[a-z]/.test(v) },
  { label: "One number (0-9)",             test: (v: string) => /[0-9]/.test(v) },
  { label: "One special character (!@#$)", test: (v: string) => /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(v) },
];

// ── Schema ───────────────────────────────────────────────────────────────────
const registerSchema = z
  .object({
    firstName: z.string().min(2, "First name must be at least 2 characters"),
    lastName:  z.string().min(2, "Last name must be at least 2 characters"),
    email:     z.string().email("Invalid email address"),
    password:  z
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
  .refine((data) => data.password === data.confirmPassword, {
    message: "Passwords don't match",
    path: ["confirmPassword"],
  });

type RegisterForm = z.infer<typeof registerSchema>;

const Register = () => {
  const dispatch  = useAppDispatch();
  const navigate  = useNavigate();
  const { isLoading, error, token } = useAppSelector((state) => state.auth);

  const [showPassword, setShowPassword]   = useState(false);
  const [showConfirm, setShowConfirm]     = useState(false);

  useEffect(() => {
    if (token) navigate("/products", { replace: true });
  }, [token, navigate]);

  const {
    register,
    handleSubmit,
    watch,
    formState: { errors },
  } = useForm<RegisterForm>({
    resolver: zodResolver(registerSchema),
  });

  const passwordValue = watch("password") || "";

  const onSubmit = async (data: RegisterForm) => {
    const { confirmPassword, ...payload } = data;
    const result = await dispatch(registerThunk(payload));
    if (registerThunk.fulfilled.match(result)) {
      toast.success("Account created! Please verify your email.");
      navigate("/login", { replace: true });
    } else {
      toast.error((result.payload as string) || "Registration failed");
    }
  };

  return (
    <div className="min-h-screen bg-gradient-to-br from-gray-50 to-primary-50 flex items-center justify-center px-4 py-10">
      <div className="w-full max-w-md">

        {/* Logo */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center w-12 h-12 bg-primary-600 text-white rounded-xl mb-4">
            <Package className="w-6 h-6" />
          </div>
          <h1 className="text-2xl font-bold text-gray-900">Create Account</h1>
          <p className="text-gray-500 mt-1">Join {APP_NAME} today</p>
        </div>

        {/* Card */}
        <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-8">
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">

            {/* Name */}
            <div className="grid grid-cols-2 gap-4">
              <Input
                label="First Name"
                placeholder="John"
                leftIcon={<User className="w-4 h-4" />}
                error={errors.firstName?.message}
                {...register("firstName")}
              />
              <Input
                label="Last Name"
                placeholder="Doe"
                leftIcon={<User className="w-4 h-4" />}
                error={errors.lastName?.message}
                {...register("lastName")}
              />
            </div>

            {/* Email */}
            <Input
              label="Email Address"
              type="email"
              placeholder="you@example.com"
              leftIcon={<Mail className="w-4 h-4" />}
              error={errors.email?.message}
              {...register("email")}
            />

            {/* Password */}
            <Input
              label="Password"
              type={showPassword ? "text" : "password"}
              placeholder="••••••••"
              leftIcon={<Lock className="w-4 h-4" />}
              rightIcon={
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="hover:text-gray-600 transition"
                >
                  {showPassword ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                </button>
              }
              error={errors.password?.message}
              {...register("password")}
            />

            {/* Password Strength Indicator */}
            {passwordValue.length > 0 && (
              <div className="space-y-1.5 p-3 bg-gray-50 rounded-xl border border-gray-100">
                <p className="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-2">
                  Password Requirements
                </p>
                {PASSWORD_RULES.map(({ label, test }) => {
                  const passed = test(passwordValue);
                  return (
                    <div
                      key={label}
                      className={cn(
                        "flex items-center gap-2 text-xs transition",
                        passed ? "text-green-600" : "text-gray-400"
                      )}
                    >
                      {passed ? (
                        <CheckCircle className="w-3.5 h-3.5 shrink-0" />
                      ) : (
                        <XCircle className="w-3.5 h-3.5 shrink-0" />
                      )}
                      {label}
                    </div>
                  );
                })}

                {/* Strength Bar */}
                <div className="mt-2">
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
                      <div>
                        <div className="flex justify-between mb-1">
                          <span className="text-xs text-gray-500">Strength</span>
                          <span className={cn(
                            "text-xs font-semibold",
                            score <= 2 ? "text-red-600" :
                            score <= 3 ? "text-yellow-600" :
                            "text-green-600"
                          )}>
                            {label}
                          </span>
                        </div>
                        <div className="w-full h-1.5 bg-gray-200 rounded-full overflow-hidden">
                          <div
                            className={cn("h-full rounded-full transition-all duration-300", color)}
                            style={{ width: `${pct}%` }}
                          />
                        </div>
                      </div>
                    );
                  })()}
                </div>
              </div>
            )}

            {/* Confirm Password */}
            <Input
              label="Confirm Password"
              type={showConfirm ? "text" : "password"}
              placeholder="••••••••"
              leftIcon={<Lock className="w-4 h-4" />}
              rightIcon={
                <button
                  type="button"
                  onClick={() => setShowConfirm(!showConfirm)}
                  className="hover:text-gray-600 transition"
                >
                  {showConfirm ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                </button>
              }
              error={errors.confirmPassword?.message}
              {...register("confirmPassword")}
            />

            {/* Server Error */}
            {error && (
              <div className="p-3 bg-red-50 border border-red-100 rounded-lg">
                <p className="text-sm text-red-600">{error}</p>
              </div>
            )}

            <Button type="submit" fullWidth size="lg" isLoading={isLoading} className="mt-2">
              Create Account
            </Button>
          </form>

          <p className="text-center mt-6 text-sm text-gray-500">
            Already have an account?{" "}
            <Link to="/login" className="text-primary-600 hover:text-primary-700 font-semibold">
              Sign in
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default Register;