import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { Link, useNavigate } from "react-router-dom";
import { Mail, Lock, User, Package } from "lucide-react";
import { useAppDispatch, useAppSelector } from "@/redux/store";
import { registerThunk } from "@/redux/slices/authSlice";
import Button from "@/components/ui/Button";
import Input from "@/components/ui/Input";
import toast from "react-hot-toast";
import { APP_NAME } from "@/utils/constants";
import { useEffect } from "react";

// ─── Schema ───────────────────────────────────────────────────────────────────
const registerSchema = z
  .object({
    firstName:       z.string().min(2, "First name must be at least 2 characters"),
    lastName:        z.string().min(2, "Last name must be at least 2 characters"),
    email:           z.string().email("Invalid email address"),
    password:        z.string().min(6, "Password must be at least 6 characters"),
    confirmPassword: z.string(),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "Passwords don't match",
    path: ["confirmPassword"],
  });

type RegisterForm = z.infer<typeof registerSchema>;

// ─── Component ────────────────────────────────────────────────────────────────
const Register = () => {
  const dispatch  = useAppDispatch();
  const navigate  = useNavigate();
  const { isLoading, error, token } = useAppSelector((state) => state.auth);

  // Already logged in → redirect
  useEffect(() => {
    if (token) navigate("/products", { replace: true });
  }, [token, navigate]);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<RegisterForm>({
    resolver: zodResolver(registerSchema),
  });

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

            {/* First & Last Name */}
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
              type="password"
              placeholder="••••••••"
              leftIcon={<Lock className="w-4 h-4" />}
              error={errors.password?.message}
              helperText="Minimum 6 characters"
              {...register("password")}
            />

            {/* Confirm Password */}
            <Input
              label="Confirm Password"
              type="password"
              placeholder="••••••••"
              leftIcon={<Lock className="w-4 h-4" />}
              error={errors.confirmPassword?.message}
              {...register("confirmPassword")}
            />

            {/* Server Error */}
            {error && (
              <div className="p-3 bg-red-50 border border-red-100 rounded-lg">
                <p className="text-sm text-red-600">{error}</p>
              </div>
            )}

            {/* Submit */}
            <Button
              type="submit"
              fullWidth
              size="lg"
              isLoading={isLoading}
              className="mt-2"
            >
              Create Account
            </Button>
          </form>

          {/* Login Link */}
          <p className="text-center mt-6 text-sm text-gray-500">
            Already have an account?{" "}
            <Link
              to="/login"
              className="text-primary-600 hover:text-primary-700 font-semibold"
            >
              Sign in
            </Link>
          </p>
        </div>

      </div>
    </div>
  );
};

export default Register;