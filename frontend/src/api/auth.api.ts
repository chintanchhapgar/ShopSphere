import axiosInstance from "./axiosInstance";
import type {
  AuthResponse,
  LoginCommand,
  RegisterCommand,
  EmailVerificationCommand,
  ForgotPasswordCommand,
  ResetPasswordCommand,
  User,
} from "@/types";

interface LoginData {
  accessToken: string;
  expiresAt: string;
}

export const authApi = {
  // POST /api/auth/login
  login: async (data: LoginCommand): Promise<AuthResponse> => {
    const res = await axiosInstance.post("/auth/login", data);

    const raw   = res.data;
    const token = raw?.data?.accessToken;
    if (!token) throw new Error("Invalid response from server");

    const user = decodeJwtUser(token);
    return { token, user };
  },

  // POST /api/auth/register
  register: async (data: RegisterCommand): Promise<AuthResponse> => {
    const res = await axiosInstance.post("/auth/register", data);

    const raw   = res.data;
    const token = raw?.data?.accessToken;
    if (!token) {
      throw new Error(raw?.message || "Registration successful. Please verify your email.");
    }

    const user = decodeJwtUser(token);
    return { token, user };
  },

  // GET /api/auth/me
  // Response: { data: { id, email, firstName, lastName, roles: [...] } }
  getMe: async (): Promise<User> => {
    const res = await axiosInstance.get("/auth/me");
    console.log("=== ME RAW ===", res.data);

    const data = res.data?.data ?? res.data?.value ?? res.data;

    return {
      id:        data.id        || "",
      firstName: data.firstName || "",
      lastName:  data.lastName  || "",
      email:     data.email     || "",
      role:      Array.isArray(data.roles) ? data.roles[0] || "Customer" : data.role || "Customer",
      roles:     Array.isArray(data.roles) ? data.roles : [data.role || "Customer"],
    };
  },

  // POST /api/auth/verify-email
  verifyEmail: async (data: EmailVerificationCommand): Promise<void> => {
    await axiosInstance.post("/auth/verify-email", data);
  },

  // POST /api/auth/forgot-password
  forgotPassword: async (data: ForgotPasswordCommand): Promise<void> => {
    await axiosInstance.post("/auth/forgot-password", data);
  },

  // POST /api/auth/reset-password
  resetPassword: async (data: ResetPasswordCommand): Promise<void> => {
    await axiosInstance.post("/auth/reset-password", data);
  },
};

// ─── Decode JWT ──────────────────────────────────────────────────────────────
export const decodeJwtUser = (token: string): User => {
  try {
    const base64Payload = token.split(".")[1];
    const decoded = JSON.parse(atob(base64Payload));

    const role =
      decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
      decoded.role ||
      "Customer";

    return {
      id:        decoded.sub         || "",
      firstName: decoded.firstName   ||
                 decoded.given_name  ||
                 decoded.name?.split(" ")[0] || "",
      lastName:  decoded.lastName    ||
                 decoded.family_name ||
                 decoded.name?.split(" ")[1] || "",
      email:     decoded.email       || "",
      role:      Array.isArray(role) ? role[0] : role,
      roles:     Array.isArray(role) ? role : [role],
    };
  } catch {
    return {
      id: "", firstName: "", lastName: "", email: "", role: "Customer", roles: ["Customer"],
    };
  }
};