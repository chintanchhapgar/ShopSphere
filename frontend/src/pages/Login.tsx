import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Link, useNavigate, useLocation } from "react-router-dom";
import { Mail, Lock, Package, AlertTriangle } from "lucide-react";
import { useAppDispatch, useAppSelector } from "@/redux/store";
import { loginThunk, clearError } from "@/redux/slices/authSlice";
import Button from "@/components/ui/Button";
import Input from "@/components/ui/Input";
import toast from "react-hot-toast";
import { APP_NAME } from "@/utils/constants";
import { useEffect } from "react";

const loginSchema = z.object({
  email:    z.string().email("Invalid email address"),
  password: z.string().min(1, "Password is required"),
});

type LoginForm = z.infer<typeof loginSchema>;

const Login = () => {
  const dispatch = useAppDispatch();
  const navigate = useNavigate();
  const location = useLocation();

  const { isLoading, error, token } = useAppSelector((state) => state.auth);

  // ── Already logged in → redirect ──────────────────────────────────────────
  useEffect(() => {
    if (token) {
      const from = (location.state as { from?: string })?.from || "/products";
      navigate(from, { replace: true });
    }
  }, [token, navigate, location]);

  // ── Clear error on mount ──────────────────────────────────────────────────
  useEffect(() => {
    dispatch(clearError());
  }, [dispatch]);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginForm>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = async (data: LoginForm) => {
    // Clear previous error
    dispatch(clearError());

    const result = await dispatch(loginThunk(data));

    if (loginThunk.fulfilled.match(result)) {
      toast.success("Welcome back!");
      const from = (location.state as { from?: string })?.from || "/products";
      navigate(from, { replace: true });
    } else {
      // ✅ Show error as toast too
      const errorMsg = (result.payload as string) || "Login failed";
      toast.error(errorMsg);
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
          <p className="text-gray-500 mt-1">Sign in to your account</p>
        </div>

        {/* Card */}
        <div className="bg-white rounded-2xl shadow-sm border border-gray-100 p-8">
          <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">

            <Input
              label="Email Address"
              type="email"
              placeholder="you@example.com"
              leftIcon={<Mail className="w-4 h-4" />}
              error={errors.email?.message}
              {...register("email")}
            />

            <Input
              label="Password"
              type="password"
              placeholder="••••••••"
              leftIcon={<Lock className="w-4 h-4" />}
              error={errors.password?.message}
              {...register("password")}
            />

            {/* ✅ Server Error Display */}
            {error && (
              <div className="flex items-start gap-3 p-3 bg-red-50 border border-red-200 rounded-xl">
                <AlertTriangle className="w-5 h-5 text-red-500 shrink-0 mt-0.5" />
                <div>
                  <p className="text-sm font-medium text-red-700">
                    Login Failed
                  </p>
                  <p className="text-sm text-red-600 mt-0.5">{error}</p>
                </div>
              </div>
            )}

            <div className="flex items-center justify-between text-sm">
              <label className="flex items-center gap-2 text-gray-600 cursor-pointer">
                <input
                  type="checkbox"
                  className="rounded border-gray-300 text-primary-600 focus:ring-primary-500"
                />
                Remember me
              </label>
              <Link
                to="/forgot-password"
                className="text-primary-600 hover:text-primary-700 font-medium"
              >
                Forgot password?
              </Link>
            </div>

            <Button
              type="submit"
              fullWidth
              size="lg"
              isLoading={isLoading}
            >
              Sign In
            </Button>
          </form>

          <p className="text-center mt-6 text-sm text-gray-500">
            Don't have an account?{" "}
            <Link
              to="/register"
              className="text-primary-600 hover:text-primary-700 font-semibold"
            >
              Create one free
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default Login;