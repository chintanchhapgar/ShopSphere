import { useAppDispatch, useAppSelector } from "@/redux/store";
import { loginThunk, registerThunk, logout } from "@/redux/slices/authSlice";
import type { LoginPayload, RegisterPayload } from "@/types";

export const useAuth = () => {
  const dispatch = useAppDispatch();
  const { user, token, isLoading, error } = useAppSelector((state) => state.auth);

  const login = async (credentials: LoginPayload) => {
    return dispatch(loginThunk(credentials));
  };

  const register = async (userData: RegisterPayload) => {
    return dispatch(registerThunk(userData));
  };

  const logoutUser = () => {
    dispatch(logout());
  };

  return {
    user,
    token,
    isLoading,
    error,
    isAuthenticated: !!token,
    login,
    register,
    logout: logoutUser,
  };
};