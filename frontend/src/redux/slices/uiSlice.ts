import { createSlice, PayloadAction } from "@reduxjs/toolkit";

interface UiState {
  isMobileMenuOpen: boolean;
  isCartOpen:       boolean;
  searchQuery:      string;
}

const initialState: UiState = {
  isMobileMenuOpen: false,
  isCartOpen:       false,
  searchQuery:      "",
};

const uiSlice = createSlice({
  name: "ui",
  initialState,
  reducers: {
    toggleMobileMenu(state) {
      state.isMobileMenuOpen = !state.isMobileMenuOpen;
    },
    toggleCart(state) {
      state.isCartOpen = !state.isCartOpen;
    },
    setSearchQuery(state, action: PayloadAction<string>) {
      state.searchQuery = action.payload;
    },
  },
});

export const { toggleMobileMenu, toggleCart, setSearchQuery } = uiSlice.actions;

// ✅ Default export
export default uiSlice.reducer;