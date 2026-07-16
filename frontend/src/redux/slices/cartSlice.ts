import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { cartApi } from "@/api/cart.api";
import type { Cart, AddCartItemRequest } from "@/types";

interface CartState {
  data:       Cart | null;
  isLoading:  boolean;
  error:      string | null;
  couponCode: string | null;
}

const initialState: CartState = {
  data:       null,
  isLoading:  false,
  error:      null,
  couponCode: null,
};

// ═══════════════════════════════════════════════════════════════════════════
// Thunks
// ═══════════════════════════════════════════════════════════════════════════
export const fetchCartThunk = createAsyncThunk<Cart, void, { rejectValue: string }>(
  "cart/fetch",
  async (_, { rejectWithValue }) => {
    try {
      return await cartApi.getCart();
    } catch (err) {
      return rejectWithValue((err as Error).message);
    }
  }
);

export const addToCartThunk = createAsyncThunk<Cart, AddCartItemRequest, { rejectValue: string }>(
  "cart/addItem",
  async (data, { rejectWithValue }) => {
    try {
      await cartApi.addItem(data);
      return await cartApi.getCart();
    } catch (err) {
      return rejectWithValue((err as Error).message);
    }
  }
);

export const updateCartItemThunk = createAsyncThunk<
  Cart,
  { itemId: string; quantity: number },
  { rejectValue: string }
>(
  "cart/updateItem",
  async ({ itemId, quantity }, { rejectWithValue }) => {
    try {
      await cartApi.updateItem(itemId, { quantity });
      return await cartApi.getCart();
    } catch (err) {
      return rejectWithValue((err as Error).message);
    }
  }
);

export const removeFromCartThunk = createAsyncThunk<Cart, string, { rejectValue: string }>(
  "cart/removeItem",
  async (itemId, { rejectWithValue }) => {
    try {
      await cartApi.removeItem(itemId);
      return await cartApi.getCart();
    } catch (err) {
      return rejectWithValue((err as Error).message);
    }
  }
);

export const clearCartThunk = createAsyncThunk<void, void, { rejectValue: string }>(
  "cart/clear",
  async (_, { rejectWithValue }) => {
    try {
      await cartApi.clearCart();
    } catch (err) {
      return rejectWithValue((err as Error).message);
    }
  }
);

export const applyCouponThunk = createAsyncThunk<
  { cart: Cart; code: string },
  string,
  { rejectValue: string }
>(
  "cart/applyCoupon",
  async (couponCode, { rejectWithValue }) => {
    try {
      await cartApi.applyCoupon({ couponCode });
      const cart = await cartApi.getCart();
      return { cart, code: couponCode };
    } catch (err) {
      return rejectWithValue((err as Error).message);
    }
  }
);

export const removeCouponThunk = createAsyncThunk<Cart, void, { rejectValue: string }>(
  "cart/removeCoupon",
  async (_, { rejectWithValue }) => {
    try {
      await cartApi.removeCoupon();
      return await cartApi.getCart();
    } catch (err) {
      return rejectWithValue((err as Error).message);
    }
  }
);

// ═══════════════════════════════════════════════════════════════════════════
// Slice
// ═══════════════════════════════════════════════════════════════════════════
const cartSlice = createSlice({
  name: "cart",
  initialState,
  reducers: {
    // ✅ Reset cart locally (after order placed)
    resetCart(state) {
      state.data       = null;
      state.error      = null;
      state.couponCode = null;
    },
  },
  extraReducers: (builder) => {
    const setLoading = (state: CartState) => {
      state.isLoading = true;
      state.error     = null;
    };

    const setCart = (state: CartState, { payload }: { payload: Cart }) => {
      state.isLoading  = false;
      state.data       = payload;
      // Also update coupon code from cart if present
      if (payload.couponCode) state.couponCode = payload.couponCode;
    };

    const setError = (state: CartState, { payload }: { payload: unknown }) => {
      state.isLoading = false;
      state.error     = payload as string;
    };

    builder
      // Fetch
      .addCase(fetchCartThunk.pending, setLoading)
      .addCase(fetchCartThunk.fulfilled, setCart)
      .addCase(fetchCartThunk.rejected, setError)

      // Add
      .addCase(addToCartThunk.pending, setLoading)
      .addCase(addToCartThunk.fulfilled, setCart)
      .addCase(addToCartThunk.rejected, setError)

      // Update
      .addCase(updateCartItemThunk.pending, setLoading)
      .addCase(updateCartItemThunk.fulfilled, setCart)
      .addCase(updateCartItemThunk.rejected, setError)

      // Remove
      .addCase(removeFromCartThunk.pending, setLoading)
      .addCase(removeFromCartThunk.fulfilled, setCart)
      .addCase(removeFromCartThunk.rejected, setError)

      // Clear
      .addCase(clearCartThunk.fulfilled, (state) => {
        state.data       = null;
        state.isLoading  = false;
        state.couponCode = null;
      })

      // Apply Coupon
      .addCase(applyCouponThunk.pending, setLoading)
      .addCase(applyCouponThunk.fulfilled, (state, { payload }) => {
        state.isLoading  = false;
        state.data       = payload.cart;
        state.couponCode = payload.code;
      })
      .addCase(applyCouponThunk.rejected, setError)

      // Remove Coupon
      .addCase(removeCouponThunk.pending, setLoading)
      .addCase(removeCouponThunk.fulfilled, (state, { payload }) => {
        state.isLoading  = false;
        state.data       = payload;
        state.couponCode = null;
      })
      .addCase(removeCouponThunk.rejected, setError);
  },
});

export const { resetCart } = cartSlice.actions;
export default cartSlice.reducer;