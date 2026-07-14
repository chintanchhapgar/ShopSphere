import axios, {
  AxiosError,
  AxiosResponse,
  InternalAxiosRequestConfig,
} from "axios";

const axiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_URL || "https://localhost:7065/api",
  timeout: 15000,
  headers: {
    "Content-Type": "application/json",
  },
});

// ─── Request Interceptor ──────────────────────────────────────────────────────
axiosInstance.interceptors.request.use(
  (config: InternalAxiosRequestConfig) => {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error: AxiosError) => Promise.reject(error)
);

// ─── Response Interceptor ─────────────────────────────────────────────────────
axiosInstance.interceptors.response.use(
  (response: AxiosResponse) => {
    const data = response.data;
    if (data?.success === false || data?.isSuccess === false || data?.isFailure === true) {
      const message = extractErrorMessage(data);
      return Promise.reject(new Error(message));
    }
    return response;
  },
  (error: AxiosError) => {
    if (error.response?.status === 401) {
      // ✅ Only redirect if NOT on public pages
      const publicPaths = ["/", "/products", "/login", "/register", "/verify-email"];
      const currentPath = window.location.pathname;
      const isPublicPage = publicPaths.some((p) => currentPath === p || currentPath.startsWith("/products/"));

      if (!isPublicPage) {
        localStorage.removeItem("token");
        window.location.href = "/login";
      }

      return Promise.reject(new Error("Authentication required"));
    }

    const data = error.response?.data as any;
    if (data) {
      const message = extractErrorMessage(data);
      return Promise.reject(new Error(message));
    }

    if (error.code === "ECONNABORTED") {
      return Promise.reject(new Error("Request timed out. Please try again."));
    }

    if (!error.response) {
      return Promise.reject(new Error("Network error. Please check your connection."));
    }

    return Promise.reject(new Error(error.message || "Something went wrong"));
  }
);

// ─── Extract Error Message from any .NET response shape ──────────────────────
function extractErrorMessage(data: any): string {
  // Shape 1: { success: false, message: "...", errors: [{ description: "..." }] }
  if (data?.message && data?.message !== "Success") {
    return data.message;
  }

  // Shape 2: { isSuccess: false, message: "...", error: "..." }
  if (data?.error && typeof data.error === "string") {
    return data.error;
  }

  // Shape 3: errors array [{ code, description, field }]
  if (Array.isArray(data?.errors) && data.errors.length > 0) {
    return data.errors
      .map((e: any) => e.description || e.message || e.code)
      .filter(Boolean)
      .join(". ");
  }

  // Shape 4: .NET validation errors { errors: { Email: ["..."], Password: ["..."] } }
  if (data?.errors && typeof data.errors === "object" && !Array.isArray(data.errors)) {
    return Object.values(data.errors as Record<string, string[]>)
      .flat()
      .join(". ");
  }

  // Shape 5: { title: "..." }
  if (data?.title) {
    return data.title;
  }

  // Shape 6: { detail: "..." }
  if (data?.detail) {
    return data.detail;
  }

  return "Something went wrong";
}

export default axiosInstance;