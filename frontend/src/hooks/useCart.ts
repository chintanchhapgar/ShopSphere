import { useAppDispatch, useAppSelector } from "@/redux/store";
import {
  fetchCartThunk,
  addToCartThunk,
  updateCartItemThunk,
  removeFromCartThunk,
  clearCartThunk,
} from "@/redux/slices/cartSlice";

export const useCart = () => {
  const dispatch = useAppDispatch();
  const { data: cart, isLoading, error } = useAppSelector((state) => state.cart);

  const fetchCart = () => dispatch(fetchCartThunk());

  const addToCart = (productId: string, quantity = 1) =>
    dispatch(addToCartThunk({ productId, quantity }));

  const updateQuantity = (itemId: string, quantity: number) =>
    dispatch(updateCartItemThunk({ itemId, quantity }));

  const removeFromCart = (itemId: string) =>
    dispatch(removeFromCartThunk(itemId));

  const clearCart = () => dispatch(clearCartThunk());

  // ✅ Use cart.total from API, items for count
  const totalItems =
    cart?.items?.reduce((acc, item) => acc + item.quantity, 0) ?? 0;

  const totalPrice = cart?.total ?? 0;

  return {
    cart,
    isLoading,
    error,
    totalItems,
    totalPrice,
    fetchCart,
    addToCart,
    updateQuantity,
    removeFromCart,
    clearCart,
  };
};