import { createSlice, createAsyncThunk, PayloadAction } from "@reduxjs/toolkit";
import { authApi } from "@/api/auth.api";
import type {
  User,
  LoginCommand,
  RegisterCommand,
  AuthResponse,
} from "@/types";

// ─── State ────────────────────────────────────────────────────────────────────
interface AuthState {
  user: User | null;
  token: string | null;
  isLoading: boolean;
  error: string | null;
  isInitialized: boolean;
}

const initialState: AuthState = {
  user: null,
  token: localStorage.getItem("token"),
  isLoading: false,
  error: null,
  isInitialized: false,
};

// ─── Thunks ───────────────────────────────────────────────────────────────────
export const loginThunk = createAsyncThunk<
  AuthResponse,
  LoginCommand,
  { rejectValue: string }
>(
  "auth/login",
  async (credentials, { rejectWithValue }) => {
    try {
      return await authApi.login(credentials);
    } catch (err) {
      return rejectWithValue((err as Error).message);
    }
  }
);

export const registerThunk = createAsyncThunk<
  AuthResponse,
  RegisterCommand,
  { rejectValue: string }
>(
  "auth/register",
  async (userData, { rejectWithValue }) => {
    try {
      return await authApi.register(userData);
    } catch (err) {
      return rejectWithValue((err as Error).message);
    }
  }
);

export const fetchMeThunk = createAsyncThunk<
  User,
  void,
  { rejectValue: string }
>(
  "auth/me",
  async (_, { rejectWithValue }) => {
    try {
      return await authApi.getMe();
    } catch (err) {
      return rejectWithValue((err as Error).message);
    }
  }
);

// ─── Slice ────────────────────────────────────────────────────────────────────
const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    logout(state) {
      state.user         = null;
      state.token        = null;
      state.isInitialized = false;
      state.error        = null;
      localStorage.removeItem("token");
    },
    clearError(state) {
      state.error = null;
    },
    setCredentials(state, action: PayloadAction<AuthResponse>) {
      state.user          = action.payload.user;
      state.token         = action.payload.token;
      state.isInitialized = true;
      localStorage.setItem("token", action.payload.token);
    },
  },
  extraReducers: (builder) => {
    // ── Login ────────────────────────────────────────────────────────────────
    builder
      .addCase(loginThunk.pending, (state) => {
        state.isLoading = true;
        state.error     = null;
      })
      .addCase(loginThunk.fulfilled, (state, { payload }) => {
        state.isLoading     = false;
        state.user          = payload.user;
        state.token         = payload.token;
        state.isInitialized = true;
        localStorage.setItem("token", payload.token);
      })
      .addCase(loginThunk.rejected, (state, { payload }) => {
        state.isLoading = false;
        state.error     = payload ?? "Login failed";
        state.token     = null;
      });
    // ── Register ─────────────────────────────────────────────────────────────
    builder
      .addCase(registerThunk.pending, (state) => {
        state.isLoading = true;
        state.error     = null;
      })
      .addCase(registerThunk.fulfilled, (state, { payload }) => {
        state.isLoading     = false;
        state.user          = payload.user;
        state.token         = payload.token;
        state.isInitialized = true;
        localStorage.setItem("token", payload.token);
      })
      .addCase(registerThunk.rejected, (state, { payload }) => {
        state.isLoading = false;
        state.error     = payload ?? "Registration failed";
        state.token     = null;
      });

    // ── Fetch Me ─────────────────────────────────────────────────────────────
    builder
      .addCase(fetchMeThunk.pending, (state) => {
        state.isLoading = true;
      })
      .addCase(fetchMeThunk.fulfilled, (state, { payload }) => {
        state.isLoading     = false;
        state.user          = payload;
        state.isInitialized = true;
      })
      .addCase(fetchMeThunk.rejected, (state) => {
        state.isLoading     = false;
        state.isInitialized = true;
        state.token         = null;
        localStorage.removeItem("token");
      });
  },
});

export const { logout, clearError, setCredentials } = authSlice.actions;
export default authSlice.reducer;