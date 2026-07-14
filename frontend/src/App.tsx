import { Routes, Route } from "react-router-dom";
import Layout from "@/components/layout/Layout";
import ProtectedRoute from "@/components/ProtectedRoute";

import Home from "@/pages/Home";
import Login from "@/pages/Login";
import Register from "@/pages/Register";
import Products from "@/pages/Products";
import ProductDetail from "@/pages/ProductDetail";
import Cart from "@/pages/Cart";
import Checkout from "@/pages/Checkout";
import PaymentPage from "@/pages/Payment";
import Orders from "@/pages/Orders";
import OrderDetail from "@/pages/OrderDetail";
import Wishlist from "@/pages/Wishlist";
import Profile from "@/pages/Profile";
import AddressForm from "@/pages/AddressForm";
import VerifyEmail from "@/pages/VerifyEmail";
import NotFound from "@/pages/NotFound";

import AdminDashboard from "@/pages/admin/AdminDashboard";
import AdminOrders from "@/pages/admin/AdminOrders";
import AdminReviews from "@/pages/admin/AdminReviews";

const App = () => {
  return (
    <Routes>
      <Route element={<Layout />}>
        {/* Public */}
        <Route path="/" element={<Home />} />
        <Route path="/products" element={<Products />} />
        <Route path="/products/:id" element={<ProductDetail />} />
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/verify-email" element={<VerifyEmail />} />

        {/* Protected */}
        <Route path="/cart" element={<ProtectedRoute><Cart /></ProtectedRoute>} />
        <Route path="/checkout" element={<ProtectedRoute><Checkout /></ProtectedRoute>} />
        <Route path="/orders" element={<ProtectedRoute><Orders /></ProtectedRoute>} />
        <Route path="/orders/:id" element={<ProtectedRoute><OrderDetail /></ProtectedRoute>} />
        <Route path="/orders/:orderId/payment" element={<ProtectedRoute><PaymentPage /></ProtectedRoute>} />
        <Route path="/wishlist" element={<ProtectedRoute><Wishlist /></ProtectedRoute>} />
        <Route path="/profile" element={<ProtectedRoute><Profile /></ProtectedRoute>} />
        <Route path="/addresses/new" element={<ProtectedRoute><AddressForm /></ProtectedRoute>} />
        <Route path="/addresses/:id/edit" element={<ProtectedRoute><AddressForm /></ProtectedRoute>} />

        {/* Admin */}
        <Route path="/admin/dashboard" element={<ProtectedRoute><AdminDashboard /></ProtectedRoute>} />
        <Route path="/admin/orders" element={<ProtectedRoute><AdminOrders /></ProtectedRoute>} />
        <Route path="/admin/reviews" element={<ProtectedRoute><AdminReviews /></ProtectedRoute>} />

        {/* 404 */}
        <Route path="*" element={<NotFound />} />
      </Route>
    </Routes>
  );
};

export default App;