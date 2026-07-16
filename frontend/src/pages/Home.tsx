import { Link } from "react-router-dom";
import { ArrowRight, Shield, Truck, RefreshCw, Star } from "lucide-react";
import { useEffect, useState } from "react";
import { productApi } from "@/api/product.api";
import { categoryApi } from "@/api/category.api";
import { brandApi } from "@/api/brand.api";
import { useAuth } from "@/hooks/useAuth";
import type { Product, Category, Brand } from "@/types";
import ProductCard from "@/components/features/ProductCard";
import Spinner from "@/components/ui/Spinner";
import { useTranslation } from "react-i18next";

// ── Feature icons (labels come from translations) ────────────────────────────
const FEATURES = [
  { key: "free_shipping",  icon: Truck,     color: "text-blue-600 bg-blue-50" },
  { key: "secure_payment", icon: Shield,    color: "text-green-600 bg-green-50" },
  { key: "easy_returns",   icon: RefreshCw, color: "text-purple-600 bg-purple-50" },
  { key: "top_rated",      icon: Star,      color: "text-yellow-600 bg-yellow-50" },
] as const;

const getCategoryEmoji = (name: string): string => {
  const map: Record<string, string> = {
    Electronics:       "💻",
    Books:             "📚",
    "Home Appliances": "🏠",
    Furniture:         "🪑",
    Sports:            "⚽",
    Toys:              "🧸",
    Automotive:        "🚗",
    Clothing:          "👕",
    Beauty:            "💄",
    Health:            "💊",
    Garden:            "🌱",
    Food:              "🍕",
    Music:             "🎵",
    Gaming:            "🎮",
    Office:            "📎",
    Jewelry:           "💎",
    Pets:              "🐾",
    Baby:              "👶",
    Travel:            "✈️",
  };
  return map[name] || name.charAt(0);
};

const Home = () => {
  const { isAuthenticated } = useAuth();
  const { t } = useTranslation();

  const [featuredProducts, setFeaturedProducts] = useState<Product[]>([]);
  const [categories, setCategories]             = useState<Category[]>([]);
  const [brands, setBrands]                     = useState<Brand[]>([]);
  const [isLoading, setIsLoading]               = useState(true);

  useEffect(() => {
    const loadAll = async () => {
      setIsLoading(true);

      try {
        const products = await productApi.getAll();
        setFeaturedProducts(Array.isArray(products) ? products.slice(0, 8) : []);
      } catch {
        setFeaturedProducts([]);
      }

      try {
        const cats = await categoryApi.getAll();
        setCategories(Array.isArray(cats) ? cats.filter((c) => c.isActive !== false) : []);
      } catch {
        setCategories([]);
      }

      try {
        const brds = await brandApi.getAll();
        setBrands(Array.isArray(brds) ? brds.filter((b) => b.isActive !== false) : []);
      } catch {
        setBrands([]);
      }

      setIsLoading(false);
    };

    loadAll();
  }, [isAuthenticated]);

  return (
    <div>
      {/* ── Hero ──────────────────────────────────────────────────────────────── */}
      <section className="relative bg-gradient-to-br from-primary-700 via-primary-600 to-primary-800 text-white overflow-hidden">
        <div className="absolute inset-0 bg-[url('https://images.unsplash.com/photo-1607082348824-0a96f2a4b9da?w=1200')] bg-cover bg-center opacity-10" />
        <div className="relative max-w-7xl mx-auto px-4 py-28 text-center">
          <span className="inline-block px-4 py-1.5 bg-white/20 rounded-full text-sm font-medium mb-6 backdrop-blur-sm">
            🎉 {t("home.new_arrivals")}
          </span>
          <h1 className="text-5xl md:text-7xl font-bold mb-6 leading-tight">
            {t("home.hero_title")}
          </h1>
          <p className="text-xl text-primary-100 mb-10 max-w-2xl mx-auto">
            {t("home.hero_subtitle")}
          </p>
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <Link
              to="/products"
              className="inline-flex items-center gap-2 px-8 py-4 bg-white text-primary-700 font-semibold rounded-xl hover:bg-primary-50 transition text-lg shadow-lg"
            >
              {t("home.shop_now")} <ArrowRight className="w-5 h-5" />
            </Link>
            {!isAuthenticated && (
              <Link
                to="/register"
                className="inline-flex items-center gap-2 px-8 py-4 border-2 border-white/40 text-white font-semibold rounded-xl hover:bg-white/10 transition text-lg backdrop-blur-sm"
              >
                {t("home.create_account")}
              </Link>
            )}
          </div>
        </div>
      </section>

      {/* ── Features ──────────────────────────────────────────────────────────── */}
      <section className="max-w-7xl mx-auto px-4 py-16">
        <div className="grid grid-cols-2 md:grid-cols-4 gap-6">
          {FEATURES.map(({ key, icon: Icon, color }) => (
            <div
              key={key}
              className="flex flex-col items-center text-center p-6 bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 shadow-sm hover:shadow-md transition"
            >
              <div className={`p-3 rounded-xl mb-4 ${color}`}>
                <Icon className="w-6 h-6" />
              </div>
              <h3 className="font-semibold text-gray-800 dark:text-gray-100 mb-1">
                {t(`home.features.${key}`)}
              </h3>
              <p className="text-sm text-gray-500 dark:text-gray-400">
                {t(`home.features.${key}_desc`)}
              </p>
            </div>
          ))}
        </div>
      </section>

      {/* ── Shop by Category ──────────────────────────────────────────────────── */}
      {categories.length > 0 && (
        <section className="max-w-7xl mx-auto px-4 pb-16">
          <div className="flex items-center justify-between mb-8">
            <div>
              <h2 className="text-3xl font-bold text-gray-900 dark:text-gray-100">
                {t("home.shop_by_category")}
              </h2>
              <p className="text-gray-500 dark:text-gray-400 mt-1">
                {t("home.browse_categories")}
              </p>
            </div>
            <Link
              to="/products"
              className="flex items-center gap-2 text-primary-600 font-medium hover:text-primary-700 transition"
            >
              {t("home.view_all")} <ArrowRight className="w-4 h-4 rtl-flip" />
            </Link>
          </div>
          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-4">
            {categories.map((category) => (
              <Link
                key={category.id}
                to={`/products?categoryId=${category.id}`}
                className="group flex flex-col items-center p-6 bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 shadow-sm hover:shadow-md hover:border-primary-200 hover:-translate-y-1 transition-all duration-200"
              >
                <div className="w-14 h-14 bg-primary-50 dark:bg-primary-900/30 text-primary-600 dark:text-primary-400 rounded-2xl flex items-center justify-center mb-3 group-hover:bg-primary-100 dark:group-hover:bg-primary-900/50 transition text-2xl font-bold">
                  {getCategoryEmoji(category.name)}
                </div>
                <h3 className="text-sm font-semibold text-gray-800 dark:text-gray-200 text-center group-hover:text-primary-600 transition">
                  {category.name}
                </h3>
              </Link>
            ))}
          </div>
        </section>
      )}

      {/* ── Top Brands ────────────────────────────────────────────────────────── */}
      {brands.length > 0 && (
        <section className="bg-gray-50 dark:bg-gray-900 py-16">
          <div className="max-w-7xl mx-auto px-4">
            <div className="flex items-center justify-between mb-8">
              <div>
                <h2 className="text-3xl font-bold text-gray-900 dark:text-gray-100">
                  {t("home.top_brands")}
                </h2>
                <p className="text-gray-500 dark:text-gray-400 mt-1">
                  {t("home.trusted_brands")}
                </p>
              </div>
              <Link
                to="/products"
                className="flex items-center gap-2 text-primary-600 font-medium hover:text-primary-700 transition"
              >
                {t("home.view_all")} <ArrowRight className="w-4 h-4 rtl-flip" />
              </Link>
            </div>
            <div className="grid grid-cols-3 sm:grid-cols-4 md:grid-cols-6 lg:grid-cols-8 gap-3">
              {brands.map((brand) => (
                <Link
                  key={brand.id}
                  to={`/products?brandId=${brand.id}`}
                  className="group flex flex-col items-center justify-center p-4 bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 hover:border-primary-200 hover:shadow-md hover:-translate-y-0.5 transition-all duration-200"
                >
                  <div className="w-12 h-12 bg-gradient-to-br from-primary-500 to-primary-700 text-white rounded-xl flex items-center justify-center text-lg font-bold mb-2 group-hover:scale-110 transition-transform">
                    {brand.name.charAt(0)}
                  </div>
                  <p className="text-xs font-semibold text-gray-700 dark:text-gray-300 text-center truncate w-full group-hover:text-primary-600 transition">
                    {brand.name}
                  </p>
                </Link>
              ))}
            </div>
          </div>
        </section>
      )}

      {/* ── Featured Products ─────────────────────────────────────────────────── */}
      <section className="max-w-7xl mx-auto px-4 py-16">
        <div className="flex items-center justify-between mb-8">
          <div>
            <h2 className="text-3xl font-bold text-gray-900 dark:text-gray-100">
              {t("home.featured_products")}
            </h2>
            <p className="text-gray-500 dark:text-gray-400 mt-1">
              {t("home.handpicked")}
            </p>
          </div>
          <Link
            to="/products"
            className="flex items-center gap-2 text-primary-600 font-medium hover:text-primary-700 transition"
          >
            {t("home.view_all")} <ArrowRight className="w-4 h-4 rtl-flip" />
          </Link>
        </div>

        {isLoading ? (
          <div className="flex justify-center py-20">
            <Spinner size="lg" className="text-primary-600" />
          </div>
        ) : featuredProducts.length === 0 ? (
          <div className="text-center py-20 bg-gray-50 dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700">
            <p className="text-gray-400 dark:text-gray-500 mb-2">
              {isAuthenticated ? t("home.no_products") : t("home.login_to_see")}
            </p>
            {!isAuthenticated && (
              <Link
                to="/login"
                className="text-sm text-primary-600 hover:underline"
              >
                {t("home.login_browse")}
              </Link>
            )}
          </div>
        ) : (
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
            {featuredProducts.map((product) => (
              <ProductCard key={product.id} product={product} />
            ))}
          </div>
        )}
      </section>

      {/* ── Newsletter ────────────────────────────────────────────────────────── */}
      <section className="bg-gradient-to-r from-primary-600 to-primary-800 text-white py-16">
        <div className="max-w-3xl mx-auto px-4 text-center">
          <h2 className="text-3xl font-bold mb-3">
            {t("home.newsletter.title")}
          </h2>
          <p className="text-primary-100 mb-8">
            {t("home.newsletter.subtitle")}
          </p>
          <form
            onSubmit={(e) => e.preventDefault()}
            className="flex flex-col sm:flex-row gap-3 max-w-lg mx-auto"
          >
            <input
              type="email"
              placeholder={t("home.newsletter.placeholder")}
              className="flex-1 px-5 py-3 rounded-xl text-gray-900 placeholder:text-gray-400 focus:outline-none focus:ring-2 focus:ring-white/30"
            />
            <button
              type="submit"
              className="px-8 py-3 bg-white text-primary-700 font-semibold rounded-xl hover:bg-primary-50 transition shadow-md"
            >
              {t("home.newsletter.subscribe")}
            </button>
          </form>
        </div>
      </section>
    </div>
  );
};

export default Home;