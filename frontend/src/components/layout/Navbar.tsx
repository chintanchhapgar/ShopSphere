import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import {
  ShoppingCart,
  Search,
  User,
  Menu,
  X,
  LogOut,
  Package,
  MessageSquare,
  LayoutDashboard,
  Heart,
  Settings,
  FolderTree,
  Award,
  ShoppingBag,
  MapPin,
  Tag,
  Warehouse,
  Sun,
  Moon,
} from "lucide-react";
import { useAuth } from "@/hooks/useAuth";
import { useCart } from "@/hooks/useCart";
import { cn } from "@/utils/cn";
import { APP_NAME } from "@/utils/constants";
import { waitForDebugger } from "inspector";
import { useTheme } from "@/hooks/useTheme";
import NotificationsDropdown from "@/components/features/NotificationsDropdown";

const Navbar = () => {
  const { isAuthenticated, user, logout } = useAuth();
  const { totalItems } = useCart();
  const navigate = useNavigate();
  const [isMobileOpen, setIsMobileOpen]     = useState(false);
  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false);
  const [searchValue, setSearchValue]       = useState("");
  const { theme, toggleTheme } = useTheme();

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    if (searchValue.trim()) {
      navigate(`/products?search=${encodeURIComponent(searchValue)}`);
      setSearchValue("");
      setIsMobileOpen(false);
    }
  };

  const handleLogout = () => {
    logout();
    setIsUserMenuOpen(false);
    setIsMobileOpen(false);
    navigate("/");
  };

  // ─── Helpers ──────────────────────────────────────────────────────────────
  const userInitial  = user?.firstName?.charAt(0).toUpperCase() ?? "U";
  const userFullName = user ? `${user.firstName} ${user.lastName}`.trim() : "";
  const userDisplay  = user?.firstName ?? "";
  const isAdmin      = user?.role === "Admin" || user?.roles?.includes("Admin");

  // ─── Menu Items ───────────────────────────────────────────────────────────
  const customerMenuItems = [
    { to: "/profile",  label: "My Profile", icon: User },
    { to: "/orders",   label: "My Orders",  icon: Package },
    { to: "/wishlist", label: "Wishlist",    icon: Heart },
  ];

const adminMenuItems = [
  { to: "/admin/dashboard",  label: "Dashboard",       icon: LayoutDashboard },
  { to: "/admin/orders",     label: "Manage Orders",   icon: ShoppingBag },
  { to: "/admin/products",   label: "Manage Products", icon: Settings },
  { to: "/admin/inventory",  label: "Manage Inventory",       icon: Warehouse },
  { to: "/admin/categories", label: "Manage Categories",      icon: FolderTree },
  { to: "/admin/brands",     label: "Manage Brands",          icon: Award },
  { to: "/admin/coupons",    label: "Manage Coupons",         icon: Tag },         
  { to: "/admin/reviews",    label: "Pending Reviews", icon: MessageSquare },
];

  return (
    <header className="sticky top-0 z-50 bg-white/95 dark:bg-gray-900/95 backdrop-blur border-b border-gray-200 dark:border-gray-800">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex items-center justify-between h-16">

          {/* ── Logo ─────────────────────────────────────────────────────── */}
          <Link
            to="/"
            className="flex items-center gap-2 text-primary-600 font-bold text-xl shrink-0"
          >
            <Package className="w-6 h-6" />
            <span>{APP_NAME}</span>
          </Link>

          {/* ── Search Bar Desktop ────────────────────────────────────────── */}
          <form
            onSubmit={handleSearch}
            className="hidden md:flex flex-1 max-w-md mx-8"
          >
            <div className="relative w-full">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
              <input
                type="text"
                value={searchValue}
                onChange={(e) => setSearchValue(e.target.value)}
                placeholder="Search products..."
                className="w-full pl-10 pr-4 py-2 text-sm border border-gray-200 rounded-full bg-gray-50 focus:outline-none focus:ring-2 focus:ring-primary-500/20 focus:border-primary-400 transition"
              />
            </div>
          </form>

          {/* ── Desktop Nav ───────────────────────────────────────────────── */}
          <nav className="hidden md:flex items-center gap-1">
            <Link
              to="/products"
              className="px-4 py-2 text-sm text-gray-600 hover:text-primary-600 font-medium transition rounded-lg hover:bg-gray-50"
            >
              Products
            </Link>

            <button
              onClick={toggleTheme}
              className="p-2 text-gray-600 hover:text-primary-600 hover:bg-primary-50 dark:text-gray-300 dark:hover:bg-gray-800 rounded-lg transition"
              title={theme === "light" ? "Switch to dark mode" : "Switch to light mode"}
            >
              {theme === "light" ? (
                <Moon className="w-5 h-5" />
              ) : (
                <Sun className="w-5 h-5" />
              )}
            </button>

            <NotificationsDropdown />

            {/* Cart */}
            <Link
              to="/cart"
              className="relative p-2 text-gray-600 hover:text-primary-600 hover:bg-primary-50 rounded-lg transition"
            >
              <ShoppingCart className="w-5 h-5" />
              {totalItems > 0 && (
                <span className="absolute -top-1 -right-1 bg-primary-600 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center font-medium leading-none">
                  {totalItems > 9 ? "9+" : totalItems}
                </span>
              )}
            </Link>

            {/* Wishlist */}
            {isAuthenticated && (
              <Link
                to="/wishlist"
                className="p-2 text-gray-600 hover:text-red-500 hover:bg-red-50 rounded-lg transition"
              >
                <Heart className="w-5 h-5" />
              </Link>
            )}

            {/* Auth Section */}
            {isAuthenticated ? (
              <div className="relative ml-1">
                <button
                  onClick={() => setIsUserMenuOpen(!isUserMenuOpen)}
                  className="flex items-center gap-2 px-3 py-2 text-sm text-gray-700 hover:bg-gray-100 rounded-lg transition"
                >
                  <div className="w-7 h-7 bg-primary-600 text-white rounded-full flex items-center justify-center text-xs font-bold shrink-0">
                    {userInitial}
                  </div>
                  <span className="font-medium max-w-[100px] truncate">
                    {userDisplay}
                  </span>
                </button>

                {/* ── Desktop Dropdown ─────────────────────────────────────── */}
                {isUserMenuOpen && (
                  <>
                    <div
                      className="fixed inset-0 z-10"
                      onClick={() => setIsUserMenuOpen(false)}
                    />
                    <div className="absolute right-0 top-full mt-1 w-56 bg-white rounded-xl border border-gray-100 shadow-lg py-1 z-20 max-h-[80vh] overflow-y-auto">

                      {/* User Info */}
                      <div className="px-4 py-3 border-b border-gray-50">
                        <p className="text-sm font-semibold text-gray-800 truncate">
                          {userFullName}
                        </p>
                        <p className="text-xs text-gray-500 truncate">
                          {user?.email}
                        </p>
                        <span
                          className={cn(
                            "inline-block mt-1 px-2 py-0.5 text-xs rounded-full font-medium",
                            isAdmin
                              ? "bg-red-50 text-red-700"
                              : "bg-primary-50 text-primary-700"
                          )}
                        >
                          {user?.role}
                        </span>
                      </div>

                      {/* Customer Menu */}
                      {customerMenuItems.map(({ to, label, icon: Icon }) => (
                        <Link
                          key={to}
                          to={to}
                          className="flex items-center gap-2 px-4 py-2 text-sm text-gray-700 hover:bg-gray-50 transition"
                          onClick={() => setIsUserMenuOpen(false)}
                        >
                          <Icon className="w-4 h-4 text-gray-400" />
                          {label}
                          {label === "My Cart" && totalItems > 0 && (
                            <span className="ml-auto bg-primary-100 text-primary-700 text-xs rounded-full px-2 py-0.5 font-medium">
                              {totalItems}
                            </span>
                          )}
                        </Link>
                      ))}

                      <Link
                        to="/cart"
                        className="flex items-center gap-2 px-4 py-2 text-sm text-gray-700 hover:bg-gray-50 transition"
                        onClick={() => setIsUserMenuOpen(false)}
                      >
                        <ShoppingCart className="w-4 h-4 text-gray-400" />
                        My Cart
                        {totalItems > 0 && (
                          <span className="ml-auto bg-primary-100 text-primary-700 text-xs rounded-full px-2 py-0.5 font-medium">
                            {totalItems}
                          </span>
                        )}
                      </Link>

                      {/* Admin Menu */}
                      {isAdmin && (
                        <>
                          <hr className="my-1 border-gray-100" />
                          <div className="px-4 py-1">
                            <p className="text-xs font-semibold text-gray-400 uppercase tracking-wider">
                              Admin
                            </p>
                          </div>
                          {adminMenuItems.map(({ to, label, icon: Icon }) => (
                            <Link
                              key={to}
                              to={to}
                              className="flex items-center gap-2 px-4 py-2 text-sm text-gray-700 hover:bg-gray-50 transition"
                              onClick={() => setIsUserMenuOpen(false)}
                            >
                              <Icon className="w-4 h-4 text-gray-400" />
                              {label}
                            </Link>
                          ))}
                        </>
                      )}

                      {/* Logout */}
                      <hr className="my-1 border-gray-100" />
                      <button
                        onClick={handleLogout}
                        className="flex items-center gap-2 px-4 py-2 text-sm text-red-600 hover:bg-red-50 w-full transition"
                      >
                        <LogOut className="w-4 h-4" />
                        Logout
                      </button>
                    </div>
                  </>
                )}
              </div>
            ) : (
              <div className="flex items-center gap-2 ml-1">
                <Link
                  to="/login"
                  className="px-4 py-2 text-sm font-medium text-gray-700 hover:text-primary-600 transition rounded-lg hover:bg-gray-50"
                >
                  Login
                </Link>
                <Link
                  to="/register"
                  className="px-4 py-2 text-sm font-medium bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition shadow-sm"
                >
                  Sign Up
                </Link>
              </div>
            )}
          </nav>

          {/* ── Mobile Toggle ─────────────────────────────────────────────── */}
          <button
            className="md:hidden p-2 text-gray-600 hover:bg-gray-100 rounded-lg transition"
            onClick={() => setIsMobileOpen(!isMobileOpen)}
          >
            {isMobileOpen ? <X className="w-5 h-5" /> : <Menu className="w-5 h-5" />}
          </button>
        </div>

        {/* ══════════════════════════════════════════════════════════════════ */}
        {/* ── Mobile Menu ──────────────────────────────────────────────────── */}
        {/* ══════════════════════════════════════════════════════════════════ */}
        {isMobileOpen && (
          <div className="md:hidden py-4 border-t border-gray-100 space-y-1 max-h-[80vh] overflow-y-auto">

            {/* Mobile Search */}
            <form onSubmit={handleSearch} className="mb-3">
              <div className="relative">
                <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
                <input
                  type="text"
                  value={searchValue}
                  onChange={(e) => setSearchValue(e.target.value)}
                  placeholder="Search products..."
                  className="w-full pl-10 pr-4 py-2 text-sm border border-gray-200 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500/20"
                />
              </div>
            </form>

            {/* Mobile User Info */}
            {isAuthenticated && (
              <div className="flex items-center gap-3 px-3 py-3 bg-gray-50 rounded-lg mb-2">
                <div className="w-9 h-9 bg-primary-600 text-white rounded-full flex items-center justify-center font-bold shrink-0">
                  {userInitial}
                </div>
                <div className="overflow-hidden">
                  <p className="text-sm font-semibold text-gray-800 truncate">
                    {userFullName}
                  </p>
                  <div className="flex items-center gap-2">
                    <p className="text-xs text-gray-500 truncate">{user?.email}</p>
                    <span
                      className={cn(
                        "px-1.5 py-0.5 text-xs rounded-full font-medium shrink-0",
                        isAdmin
                          ? "bg-red-50 text-red-700"
                          : "bg-primary-50 text-primary-700"
                      )}
                    >
                      {user?.role}
                    </span>
                  </div>
                </div>
              </div>
            )}

            {/* ── Mobile Nav Links ─────────────────────────────────────────── */}
            <Link
              to="/products"
              className="flex items-center gap-2 px-3 py-2 text-gray-700 hover:bg-gray-50 rounded-lg transition"
              onClick={() => setIsMobileOpen(false)}
            >
              <Package className="w-4 h-4 text-gray-400" />
              Products
            </Link>

            <Link
              to="/cart"
              className="flex items-center justify-between px-3 py-2 text-gray-700 hover:bg-gray-50 rounded-lg transition"
              onClick={() => setIsMobileOpen(false)}
            >
              <div className="flex items-center gap-2">
                <ShoppingCart className="w-4 h-4 text-gray-400" />
                Cart
              </div>
              {totalItems > 0 && (
                <span className="bg-primary-100 text-primary-700 text-xs rounded-full px-2 py-0.5 font-medium">
                  {totalItems}
                </span>
              )}
            </Link>

            {isAuthenticated ? (
              <>
                {/* Customer Links */}
                {customerMenuItems.map(({ to, label, icon: Icon }) => (
                  <Link
                    key={to}
                    to={to}
                    className="flex items-center gap-2 px-3 py-2 text-gray-700 hover:bg-gray-50 rounded-lg transition"
                    onClick={() => setIsMobileOpen(false)}
                  >
                    <Icon className="w-4 h-4 text-gray-400" />
                    {label}
                  </Link>
                ))}

                <Link
                  to="/addresses/new"
                  className="flex items-center gap-2 px-3 py-2 text-gray-700 hover:bg-gray-50 rounded-lg transition"
                  onClick={() => setIsMobileOpen(false)}
                >
                  <MapPin className="w-4 h-4 text-gray-400" />
                  Addresses
                </Link>

                {/* ── Admin Section (Mobile) ────────────────────────────────── */}
                {isAdmin && (
                  <>
                    <hr className="my-2 border-gray-100" />
                    <div className="px-3 py-1">
                      <p className="text-xs font-semibold text-gray-400 uppercase tracking-wider">
                        Admin Panel
                      </p>
                    </div>
                    {adminMenuItems.map(({ to, label, icon: Icon }) => (
                      <Link
                        key={to}
                        to={to}
                        className="flex items-center gap-2 px-3 py-2 text-gray-700 hover:bg-gray-50 rounded-lg transition"
                        onClick={() => setIsMobileOpen(false)}
                      >
                        <Icon className="w-4 h-4 text-gray-400" />
                        {label}
                      </Link>
                    ))}
                  </>
                )}

                {/* Logout */}
                <hr className="border-gray-100 my-1" />
                <button
                  onClick={handleLogout}
                  className="flex items-center gap-2 w-full px-3 py-2 text-red-600 hover:bg-red-50 rounded-lg transition"
                >
                  <LogOut className="w-4 h-4" />
                  Logout
                </button>
              </>
            ) : (
              <>
                <hr className="border-gray-100 my-1" />
                <Link
                  to="/login"
                  className="block px-3 py-2 text-gray-700 hover:bg-gray-50 rounded-lg transition"
                  onClick={() => setIsMobileOpen(false)}
                >
                  Login
                </Link>
                <Link
                  to="/register"
                  className="block px-3 py-2 bg-primary-600 text-white rounded-lg text-center font-medium hover:bg-primary-700 transition"
                  onClick={() => setIsMobileOpen(false)}
                >
                  Sign Up
                </Link>
              </>
            )}
          </div>
        )}
      </div>
    </header>
  );
};

export default Navbar;