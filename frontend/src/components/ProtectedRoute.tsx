import { Navigate, useLocation } from "react-router-dom";
import { useAppSelector } from "@/redux/store";

interface ProtectedRouteProps {
  children: React.ReactNode;
}

const ProtectedRoute = ({ children }: ProtectedRouteProps) => {
  const { token, isLoading } = useAppSelector((state) => state.auth);
  const location = useLocation();

  // ✅ Wait until auth is checked
  if (isLoading) {
    return (
      <div className="flex justify-center items-center min-h-screen">
        <div className="w-8 h-8 border-4 border-primary-600 border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  // ✅ Check token from Redux (which reads from localStorage)
  if (!token) {
    return (
      <Navigate
        to="/login"
        state={{ from: location.pathname }}
        replace
      />
    );
  }

  return <>{children}</>;
};

export default ProtectedRoute;