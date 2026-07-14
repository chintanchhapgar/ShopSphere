import { configureStore } from "@reduxjs/toolkit";
import { TypedUseSelectorHook, useDispatch, useSelector } from "react-redux";
import authReducer from "./slices/authSlice";
import cartReducer from "./slices/cartSlice";   // ✅ default import
import uiReducer   from "./slices/uiSlice";

export const store = configureStore({
  reducer: {
    auth: authReducer,
    cart: cartReducer,
    ui:   uiReducer,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: false,
    }),
});

export type RootState  = ReturnType<typeof store.getState>;
export type AppDispatch = typeof store.dispatch;

// ── Typed Hooks ───────────────────────────────────────────────────────────────
export const useAppDispatch = () => useDispatch<AppDispatch>();
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;