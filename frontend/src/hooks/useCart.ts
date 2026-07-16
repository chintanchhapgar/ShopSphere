import { useAppDispatch, useAppSelector } from "@/redux/store";
import {
  fetchCartThunk,
  addToCartThunk,
  updateCartItemThunk,
  removeFromCartThunk,
  clearCartThunk,
  applyCouponThunk,
  removeCouponThunk,
  resetCart,  // ✅ Import reset action
} from "@/redux/slices/cartSlice";

export const useCart = () => {
  const dispatch = useAppDispatch();
  const { data: cart, isLoading, error, couponCode } = useAppSelector((s) => s.cart);

  return {
    cart,
    isLoading,
    error,
    couponCode,
    totalItems:  cart?.items?.reduce((a, i) => a + i.quantity, 0) ?? 0,
    totalPrice:  cart?.total ?? 0,

    fetchCart:       ()                              => dispatch(fetchCartThunk()),
    addToCart:       (productId: string, qty = 1)    => dispatch(addToCartThunk({ productId, quantity: qty })),
    updateQuantity:  (itemId: string, qty: number)   => dispatch(updateCartItemThunk({ itemId, quantity: qty })),
    removeFromCart:  (itemId: string)                => dispatch(removeFromCartThunk(itemId)),
    clearCart:       ()                              => dispatch(clearCartThunk()),
    resetCart:       ()                              => dispatch(resetCart()),  // ✅ Local reset
    applyCoupon:     (code: string)                  => dispatch(applyCouponThunk(code)),
    removeCoupon:    ()                              => dispatch(removeCouponThunk()),
  };
};