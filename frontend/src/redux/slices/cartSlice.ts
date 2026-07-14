import { createSlice, createAsyncThunk } from "@reduxjs/toolkit";
import { cartApi } from "@/api/cart.api";
import type { Cart, AddCartItemRequest } from "@/types";

// ── State ─────────────────────────────────────────────────────────────────────
interface CartState {
  data: Cart | null;
  isLoading: boolean;
  error: string | null;
}

const initialState: CartState = {
  data: null,
  isLoading: false,
  error: null,
};

// ── Thunks ────────────────────────────────────────────────────────────────────
export const fetchCartThunk = createAsyncThunk<
  Cart,
  void,
  { rejectValue: string }
>(
  "cart/fetch",
  async (_, { rejectWithValue }) => {
    try {
      return await cartApi.getCart();
    } catch (err) {
      return rejectWithValue((err as Error).message);
    }
  }
);

export const addToCartThunk = createAsyncThunk<
  Cart,
  AddCartItemRequest,
  { rejectValue: string }
>(
  "cart/addItem",
  async (data, { rejectWithValue }) => {
    try {
      await cartApi.addItem(data);
      // Add item response doesn't return full cart → re-fetch
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

export const removeFromCartThunk = createAsyncThunk<
  Cart,
  string,
  { rejectValue: string }
>(
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

export const clearCartThunk = createAsyncThunk<
  void,
  void,
  { rejectValue: string }
>(
  "cart/clear",
  async (_, { rejectWithValue }) => {
    try {
      await cartApi.clearCart();
    } catch (err) {
      return rejectWithValue((err as Error).message);
    }
  }
);

// ── Slice ─────────────────────────────────────────────────────────────────────
const cartSlice = createSlice({
  name: "cart",
  initialState,
  reducers: {
    resetCart(state) {
      state.data  = null;
      state.error = null;
    },
  },
  extraReducers: (builder) => {
    // ── Fetch ────────────────────────────────────────────────────────────────
    builder
      .addCase(fetchCartThunk.pending, (state) => {
        state.isLoading = true;
        state.error     = null;
      })
      .addCase(fetchCartThunk.fulfilled, (state, { payload }) => {
        state.isLoading = false;
        state.data      = payload;
      })
      .addCase(fetchCartThunk.rejected, (state, { payload }) => {
        state.isLoading = false;
        state.error     = payload ?? "Failed to fetch cart";
      });

    // ── Add Item ─────────────────────────────────────────────────────────────
    builder
      .addCase(addToCartThunk.pending, (state) => {
        state.isLoading = true;
        state.error     = null;
      })
      .addCase(addToCartThunk.fulfilled, (state, { payload }) => {
        state.isLoading = false;
        state.data      = payload;
      })
      .addCase(addToCartThunk.rejected, (state, { payload }) => {
        state.isLoading = false;
        state.error     = payload ?? "Failed to add item";
      });

    // ── Update Item ───────────────────────────────────────────────────────────
    builder
      .addCase(updateCartItemThunk.pending, (state) => {
        state.isLoading = true;
        state.error     = null;
      })
      .addCase(updateCartItemThunk.fulfilled, (state, { payload }) => {
        state.isLoading = false;
        state.data      = payload;
      })
      .addCase(updateCartItemThunk.rejected, (state, { payload }) => {
        state.isLoading = false;
        state.error     = payload ?? "Failed to update item";
      });

    // ── Remove Item ───────────────────────────────────────────────────────────
    builder
      .addCase(removeFromCartThunk.pending, (state) => {
        state.isLoading = true;
        state.error     = null;
      })
      .addCase(removeFromCartThunk.fulfilled, (state, { payload }) => {
        state.isLoading = false;
        state.data      = payload;
      })
      .addCase(removeFromCartThunk.rejected, (state, { payload }) => {
        state.isLoading = false;
        state.error     = payload ?? "Failed to remove item";
      });

    // ── Clear Cart ────────────────────────────────────────────────────────────
    builder
      .addCase(clearCartThunk.fulfilled, (state) => {
        state.isLoading = false;
        state.data      = null;
        state.error     = null;
      });
  },
});

export const { resetCart } = cartSlice.actions;

// ✅ Default export - this was missing!
export default cartSlice.reducer;