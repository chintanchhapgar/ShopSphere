import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import {
  User,
  Mail,
  Calendar,
  Package,
  Heart,
  ShoppingCart,
  MapPin,
  Trash2,
  CheckCircle,
  LogOut,
  Shield,
  Plus,
  Edit,
} from "lucide-react";
import { authApi } from "@/api/auth.api";
import { orderApi } from "@/api/order.api";
import { wishlistApi } from "@/api/wishlist.api";
import { addressApi } from "@/api/address.api";
import { useAuth } from "@/hooks/useAuth";
import type { User as UserType, OrderListItem, WishlistItem, Address } from "@/types";
import { OrderStatusColors } from "@/types";
import { formatPrice } from "@/utils/formatPrice";
import { cn } from "@/utils/cn";
import Spinner from "@/components/ui/Spinner";
import Button from "@/components/ui/Button";
import toast from "react-hot-toast";

type ActiveTab = "overview" | "orders" | "addresses" | "wishlist";

const Profile = () => {
  const { user, logout, isAuthenticated } = useAuth();
  const navigate = useNavigate();

  const [profile, setProfile]       = useState<UserType | null>(null);
  const [orders, setOrders]         = useState<OrderListItem[]>([]);
  const [wishlist, setWishlist]     = useState<WishlistItem[]>([]);
  const [addresses, setAddresses]   = useState<Address[]>([]);
  const [isLoading, setIsLoading]   = useState(true);
  const [activeTab, setActiveTab]   = useState<ActiveTab>("overview");

  // ── Load All Data ──────────────────────────────────────────────────────────
  useEffect(() => {
    if (!isAuthenticated) return;
    const load = async () => {
      setIsLoading(true);
      try {
        const [profileRes, ordersRes, wishlistRes, addressRes] =
          await Promise.allSettled([
            authApi.getMe(),
            orderApi.getMyOrders(),
            wishlistApi.getWishlist(),
            addressApi.getAddresses(),
          ]);

        if (profileRes.status  === "fulfilled") setProfile(profileRes.value);
        if (ordersRes.status   === "fulfilled") setOrders(ordersRes.value);
        if (wishlistRes.status === "fulfilled") setWishlist(wishlistRes.value);
        if (addressRes.status  === "fulfilled") setAddresses(addressRes.value);
      } catch {
        // silent
      } finally {
        setIsLoading(false);
      }
    };
    load();
  }, [isAuthenticated]);

  // ── Address Handlers ───────────────────────────────────────────────────────
  const handleSetDefault = async (id: string) => {
    try {
      await addressApi.setDefaultAddress(id);
      const updated = await addressApi.getAddresses();
      setAddresses(updated);
      toast.success("Default address updated");
    } catch (err) {
      toast.error((err as Error).message || "Failed");
    }
  };

  const handleDeleteAddress = async (id: string) => {
    if (!window.confirm("Delete this address?")) return;
    try {
      await addressApi.deleteAddress(id);
      setAddresses((prev) => prev.filter((a) => a.id !== id));
      toast.success("Address deleted");
    } catch (err) {
      toast.error((err as Error).message || "Failed");
    }
  };

  const handleLogout = () => {
    logout();
    navigate("/");
  };

  // ── Not Authenticated ──────────────────────────────────────────────────────
  if (!isAuthenticated) {
    return (
      <div className="max-w-7xl mx-auto px-4 py-24 text-center">
        <User className="w-20 h-20 text-gray-200 mx-auto mb-6" />
        <h2 className="text-2xl font-bold text-gray-700 mb-3">Login to view your profile</h2>
        <Link to="/login" state={{ from: "/profile" }}>
          <Button size="lg">Login</Button>
        </Link>
      </div>
    );
  }

  // ── Loading ────────────────────────────────────────────────────────────────
  if (isLoading) {
    return (
      <div className="flex justify-center py-32">
        <Spinner size="lg" className="text-primary-600" />
      </div>
    );
  }

  // ── Computed ────────────────────────────────────────────────────────────────
  const displayName = profile
    ? `${profile.firstName} ${profile.lastName}`.trim()
    : user
    ? `${user.firstName} ${user.lastName}`.trim()
    : "";

  const displayEmail = profile?.email || user?.email || "";

  // ✅ Handle roles array
  const displayRoles = profile?.roles || user?.roles || [user?.role || "Customer"];
  const primaryRole  = displayRoles[0] || "Customer";
  const isAdmin      = displayRoles.includes("Admin");

  const initials = displayName
    ? displayName.split(" ").map((n) => n[0]).join("").toUpperCase().slice(0, 2)
    : "U";

  const totalOrders    = orders.length;
  const totalWishlist  = wishlist.length;
  const totalAddresses = addresses.length;
  const totalSpent     = orders.reduce((acc, o) => acc + o.totalAmount, 0);

  const TABS: { key: ActiveTab; label: string; icon: React.ElementType; count?: number }[] = [
    { key: "overview",  label: "Overview",  icon: User },
    { key: "orders",    label: "Orders",    icon: Package,  count: totalOrders },
    { key: "addresses", label: "Addresses", icon: MapPin,   count: totalAddresses },
    { key: "wishlist",  label: "Wishlist",  icon: Heart,    count: totalWishlist },
  ];

  return (
    <div className="max-w-7xl mx-auto px-4 py-8">

      {/* ── Profile Header ────────────────────────────────────────────────────── */}
      <div className="bg-gradient-to-r from-primary-600 to-primary-800 rounded-2xl p-8 mb-8 text-white">
        <div className="flex flex-col sm:flex-row items-center gap-6">
          {/* Avatar */}
          <div className="w-20 h-20 bg-white/20 backdrop-blur rounded-2xl flex items-center justify-center text-3xl font-bold shrink-0">
            {initials}
          </div>

          {/* Info */}
          <div className="text-center sm:text-left flex-1">
            <h1 className="text-2xl font-bold">{displayName}</h1>
            <div className="flex flex-col sm:flex-row items-center gap-3 mt-2 text-primary-100">
              <span className="flex items-center gap-1.5 text-sm">
                <Mail className="w-4 h-4" />
                {displayEmail}
              </span>

              {/* ✅ Show all roles as badges */}
              <div className="flex items-center gap-2">
                {displayRoles.map((role) => (
                  <span
                    key={role}
                    className={cn(
                      "px-2.5 py-0.5 rounded-full text-xs font-semibold",
                      role === "Admin"
                        ? "bg-red-500/30 text-red-100"
                        : "bg-white/20 text-white"
                    )}
                  >
                    <Shield className="w-3 h-3 inline mr-1" />
                    {role}
                  </span>
                ))}
              </div>
            </div>
          </div>

          {/* Stats */}
          <div className="flex gap-6 text-center shrink-0">
            <div>
              <p className="text-2xl font-bold">{totalOrders}</p>
              <p className="text-xs text-primary-200">Orders</p>
            </div>
            <div>
              <p className="text-2xl font-bold">{formatPrice(totalSpent)}</p>
              <p className="text-xs text-primary-200">Spent</p>
            </div>
            <div>
              <p className="text-2xl font-bold">{totalWishlist}</p>
              <p className="text-xs text-primary-200">Wishlist</p>
            </div>
          </div>
        </div>
      </div>

      {/* ── Tabs ──────────────────────────────────────────────────────────────── */}
      <div className="flex gap-1 border-b border-gray-200 mb-8 overflow-x-auto">
        {TABS.map(({ key, label, icon: Icon, count }) => (
          <button
            key={key}
            onClick={() => setActiveTab(key)}
            className={cn(
              "flex items-center gap-2 px-5 py-3 text-sm font-medium border-b-2 transition whitespace-nowrap -mb-px",
              activeTab === key
                ? "border-primary-600 text-primary-600"
                : "border-transparent text-gray-500 hover:text-gray-700"
            )}
          >
            <Icon className="w-4 h-4" />
            {label}
            {count != null && count > 0 && (
              <span className={cn(
                "px-2 py-0.5 text-xs rounded-full font-medium",
                activeTab === key ? "bg-primary-100 text-primary-700" : "bg-gray-100 text-gray-500"
              )}>
                {count}
              </span>
            )}
          </button>
        ))}
      </div>

      {/* ── Overview Tab ──────────────────────────────────────────────────────── */}
      {activeTab === "overview" && (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">

          {/* Personal Info */}
          <div className="bg-white rounded-xl border border-gray-100 shadow-sm p-6">
            <h3 className="text-lg font-semibold text-gray-800 mb-4 flex items-center gap-2">
              <User className="w-5 h-5 text-gray-400" />
              Personal Information
            </h3>
            <div className="space-y-4">
              {[
                { label: "First Name", value: profile?.firstName || user?.firstName || "–" },
                { label: "Last Name",  value: profile?.lastName  || user?.lastName  || "–" },
                { label: "Email",      value: displayEmail },
                { label: "Role",       value: displayRoles.join(", ") },
                { label: "User ID",    value: profile?.id || user?.id || "–", mono: true },
              ].map(({ label, value, mono }) => (
                <div key={label} className="flex justify-between items-start">
                  <span className="text-sm text-gray-500">{label}</span>
                  <span className={cn("text-sm font-medium text-gray-800 text-right max-w-[60%] break-all", mono && "font-mono text-xs text-gray-600")}>
                    {value}
                  </span>
                </div>
              ))}
            </div>
          </div>

          {/* Quick Stats */}
          <div className="space-y-4">
            {[
              { label: "Total Orders",  value: String(totalOrders),   icon: Package,      color: "bg-blue-50 text-blue-600",   tab: "orders"    as ActiveTab },
              { label: "Total Spent",   value: formatPrice(totalSpent), icon: ShoppingCart, color: "bg-green-50 text-green-600", tab: "orders"    as ActiveTab },
              { label: "Wishlist Items", value: String(totalWishlist), icon: Heart,         color: "bg-red-50 text-red-600",     tab: "wishlist"  as ActiveTab },
              { label: "Addresses",     value: String(totalAddresses), icon: MapPin,       color: "bg-purple-50 text-purple-600", tab: "addresses" as ActiveTab },
            ].map(({ label, value, icon: Icon, color, tab }) => (
              <button
                key={label}
                onClick={() => setActiveTab(tab)}
                className="w-full bg-white rounded-xl border border-gray-100 shadow-sm p-4 flex items-center gap-4 hover:shadow-md transition text-left"
              >
                <div className={cn("p-3 rounded-xl", color)}>
                  <Icon className="w-5 h-5" />
                </div>
                <div>
                  <p className="text-sm text-gray-500">{label}</p>
                  <p className="text-xl font-bold text-gray-900">{value}</p>
                </div>
              </button>
            ))}
          </div>

          {/* Recent Orders */}
          <div className="bg-white rounded-xl border border-gray-100 shadow-sm p-6 md:col-span-2">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg font-semibold text-gray-800 flex items-center gap-2">
                <Package className="w-5 h-5 text-gray-400" />
                Recent Orders
              </h3>
              <Link to="/orders" className="text-sm text-primary-600 hover:underline">View All</Link>
            </div>

            {orders.length === 0 ? (
              <div className="text-center py-8">
                <Package className="w-12 h-12 text-gray-200 mx-auto mb-2" />
                <p className="text-gray-400 text-sm">No orders yet</p>
                <Link to="/products" className="text-sm text-primary-600 hover:underline mt-2 inline-block">Start Shopping</Link>
              </div>
            ) : (
              <div className="space-y-3">
                {orders.slice(0, 5).map((order) => (
                  <Link
                    key={order.id}
                    to={`/orders/${order.id}`}
                    className="flex items-center justify-between p-3 bg-gray-50 rounded-lg hover:bg-gray-100 transition"
                  >
                    <div>
                      <p className="text-sm font-medium text-gray-800">{order.orderNumber}</p>
                      <p className="text-xs text-gray-500">
                        {new Date(order.orderDate).toLocaleDateString("en-IN", { month: "short", day: "numeric", year: "numeric" })}
                        {" • "}{order.totalItems} item{order.totalItems !== 1 ? "s" : ""}
                      </p>
                    </div>
                    <div className="flex items-center gap-3">
                      <span className={cn("px-2.5 py-0.5 rounded-full text-xs font-semibold", OrderStatusColors[order.status] || "bg-gray-100 text-gray-700")}>
                        {order.status}
                      </span>
                      <span className="text-sm font-bold text-gray-900">{formatPrice(order.totalAmount)}</span>
                    </div>
                  </Link>
                ))}
              </div>
            )}
          </div>
        </div>
      )}

      {/* ── Orders Tab ────────────────────────────────────────────────────────── */}
      {activeTab === "orders" && (
        <div>
          {orders.length === 0 ? (
            <div className="text-center py-16 bg-gray-50 rounded-xl border border-gray-100">
              <Package className="w-14 h-14 text-gray-200 mx-auto mb-3" />
              <p className="text-gray-600 font-semibold">No orders yet</p>
              <Link to="/products" className="text-sm text-primary-600 hover:underline mt-2 inline-block">Start Shopping</Link>
            </div>
          ) : (
            <div className="space-y-3">
              {orders.map((order) => (
                <Link
                  key={order.id}
                  to={`/orders/${order.id}`}
                  className="flex flex-col sm:flex-row sm:items-center justify-between p-5 bg-white rounded-xl border border-gray-100 shadow-sm hover:shadow-md transition gap-3"
                >
                  <div>
                    <div className="flex items-center gap-2 flex-wrap">
                      <p className="text-sm font-bold text-gray-900">{order.orderNumber}</p>
                      <span className={cn("px-2.5 py-0.5 rounded-full text-xs font-semibold", OrderStatusColors[order.status] || "bg-gray-100 text-gray-700")}>
                        {order.status}
                      </span>
                    </div>
                    <p className="text-xs text-gray-500 mt-1">
                      {new Date(order.orderDate).toLocaleDateString("en-IN", { year: "numeric", month: "long", day: "numeric" })}
                      {" • "}{order.totalItems} item{order.totalItems !== 1 ? "s" : ""}
                    </p>
                  </div>
                  <p className="text-lg font-bold text-gray-900">{formatPrice(order.totalAmount)}</p>
                </Link>
              ))}
            </div>
          )}
        </div>
      )}

      {/* ── Addresses Tab ─────────────────────────────────────────────────────── */}
        {activeTab === "addresses" && (
        <div>
            <div className="flex items-center justify-between mb-6">
            <h3 className="text-lg font-semibold text-gray-800">
                Saved Addresses ({addresses.length})
            </h3>
            <Link to="/addresses/new">
                <Button size="md" variant="primary">
                <Plus className="w-4 h-4" /> Add Address
                </Button>
            </Link>
            </div>

            {addresses.length === 0 ? (
            <div className="text-center py-16 bg-gray-50 rounded-xl border border-gray-100">
                <MapPin className="w-14 h-14 text-gray-200 mx-auto mb-3" />
                <p className="text-gray-600 font-semibold">No addresses saved</p>
                <p className="text-sm text-gray-400 mt-1 mb-4">
                Add an address for faster checkout
                </p>
                <Link to="/addresses/new">
                <Button size="md">
                    <Plus className="w-4 h-4" /> Add Your First Address
                </Button>
                </Link>
            </div>
            ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                {addresses.map((addr) => (
                <div
                    key={addr.id}
                    className={cn(
                    "bg-white rounded-xl border shadow-sm p-5 relative group",
                    addr.isDefault
                        ? "border-primary-300 ring-1 ring-primary-100"
                        : "border-gray-100 hover:border-gray-200"
                    )}
                >
                    {/* Default Badge */}
                    {addr.isDefault && (
                    <span className="absolute top-3 right-3 px-2.5 py-1 bg-primary-50 text-primary-700 text-xs font-semibold rounded-full flex items-center gap-1">
                        <CheckCircle className="w-3 h-3" /> Default
                    </span>
                    )}

                    {/* Address Content */}
                    <div className="pr-20">
                    <p className="font-semibold text-gray-800 text-sm">
                        {addr.fullName}
                    </p>
                    <div className="text-sm text-gray-600 mt-2 space-y-0.5">
                        <p>{addr.addressLine1}</p>
                        {addr.addressLine2 && <p>{addr.addressLine2}</p>}
                        <p>
                        {addr.city}, {addr.state} {addr.postalCode}
                        </p>
                        <p>{addr.country}</p>
                    </div>
                    {addr.phoneNumber && (
                        <p className="text-sm text-gray-500 mt-2 flex items-center gap-1.5">
                        📞 {addr.phoneNumber}
                        </p>
                    )}
                    </div>

                    {/* Action Buttons */}
                    <div className="flex flex-wrap gap-2 mt-4 pt-4 border-t border-gray-100">
                    {!addr.isDefault && (
                        <button
                        onClick={() => handleSetDefault(addr.id)}
                        className="px-3 py-1.5 text-xs font-medium text-primary-600 border border-primary-200 rounded-lg hover:bg-primary-50 transition flex items-center gap-1"
                        >
                        <CheckCircle className="w-3 h-3" />
                        Set Default
                        </button>
                    )}
                    <Link
                        to={`/addresses/${addr.id}/edit`}
                        className="px-3 py-1.5 text-xs font-medium text-gray-600 border border-gray-200 rounded-lg hover:bg-gray-50 transition flex items-center gap-1"
                    >
                        <Edit className="w-3 h-3" />
                        Edit
                    </Link>
                    <button
                        onClick={() => handleDeleteAddress(addr.id)}
                        className="px-3 py-1.5 text-xs font-medium text-red-600 border border-red-200 rounded-lg hover:bg-red-50 transition flex items-center gap-1"
                    >
                        <Trash2 className="w-3 h-3" />
                        Delete
                    </button>
                    </div>
                </div>
                ))}
            </div>
            )}
        </div>
        )}

      {/* ── Wishlist Tab ──────────────────────────────────────────────────────── */}
      {activeTab === "wishlist" && (
        <div>
          {wishlist.length === 0 ? (
            <div className="text-center py-16 bg-gray-50 rounded-xl border border-gray-100">
              <Heart className="w-14 h-14 text-gray-200 mx-auto mb-3" />
              <p className="text-gray-600 font-semibold">Wishlist is empty</p>
              <Link to="/products" className="text-sm text-primary-600 hover:underline mt-2 inline-block">Browse Products</Link>
            </div>
          ) : (
            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-4">
              {wishlist.map((item) => {
                const imgSrc = item.imageUrl || `https://placehold.co/200x200/e2e8f0/64748b?text=${encodeURIComponent(item.name.charAt(0))}`;
                return (
                  <Link
                    key={item.productId}
                    to={`/products/${item.productId}`}
                    className="bg-white rounded-xl border border-gray-100 shadow-sm hover:shadow-md transition overflow-hidden flex flex-col"
                  >
                    <div className="relative bg-gray-50 aspect-square">
                      <img src={imgSrc} alt={item.name} onError={(e) => { (e.target as HTMLImageElement).src = `https://placehold.co/200x200/e2e8f0/64748b?text=${item.name.charAt(0)}`; }} className="w-full h-full object-cover" />
                      {!item.inStock && (
                        <div className="absolute inset-0 bg-black/40 flex items-center justify-center">
                          <span className="bg-red-500 text-white text-xs font-medium px-3 py-1 rounded-full">Out of Stock</span>
                        </div>
                      )}
                    </div>
                    <div className="p-3">
                      <p className="text-xs text-gray-400 font-mono">{item.sku}</p>
                      <h3 className="text-sm font-semibold text-gray-800 line-clamp-2 mt-1">{item.name}</h3>
                      <p className="text-lg font-bold text-gray-900 mt-2">{formatPrice(item.price)}</p>
                    </div>
                  </Link>
                );
              })}
            </div>
          )}
        </div>
      )}

      {/* ── Logout ────────────────────────────────────────────────────────────── */}
      <div className="mt-12 pt-8 border-t border-gray-100">
        <button
          onClick={handleLogout}
          className="flex items-center gap-2 px-4 py-2 text-sm text-red-600 border border-red-200 rounded-lg hover:bg-red-50 transition"
        >
          <LogOut className="w-4 h-4" />
          Logout
        </button>
      </div>
    </div>
  );
};

export default Profile;