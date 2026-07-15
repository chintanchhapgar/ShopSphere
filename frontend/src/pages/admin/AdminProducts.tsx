import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import {
  Package,
  Plus,
  Search,
  Edit,
  Trash2,
  Eye,
  EyeOff,
  Image as ImageIcon,
  Filter,
  MoreVertical,
  CheckCircle,
  XCircle,
} from "lucide-react";
import { productApi } from "@/api/product.api";
import { categoryApi } from "@/api/category.api";
import { brandApi } from "@/api/brand.api";
import type { Product, Category, Brand } from "@/types";
import { formatPrice } from "@/utils/formatPrice";
import { cn } from "@/utils/cn";
import Spinner from "@/components/ui/Spinner";
import Button from "@/components/ui/Button";
import toast from "react-hot-toast";

const AdminProducts = () => {
  const [products, setProducts]         = useState<Product[]>([]);
  const [categories, setCategories]     = useState<Category[]>([]);
  const [brands, setBrands]             = useState<Brand[]>([]);
  const [isLoading, setIsLoading]       = useState(true);
  const [searchQuery, setSearchQuery]   = useState("");
  const [categoryFilter, setCategoryFilter] = useState("");
  const [brandFilter, setBrandFilter]   = useState("");
  const [statusFilter, setStatusFilter] = useState<"all" | "active" | "inactive">("all");
  const [showFilters, setShowFilters]   = useState(false);

  // ── Action Menu State ──────────────────────────────────────────────────────
  const [activeMenuId, setActiveMenuId] = useState<string | null>(null);

  // ── Load Data ──────────────────────────────────────────────────────────────
  const loadProducts = async () => {
    try {
      const data = await productApi.getAll();
      setProducts(data);
    } catch (err) {
      toast.error((err as Error).message || "Failed to load products");
    }
  };

  useEffect(() => {
    const load = async () => {
      setIsLoading(true);
      try {
        const [prods, cats, brds] = await Promise.allSettled([
          productApi.getAll(),
          categoryApi.getAll(),
          brandApi.getAll(),
        ]);
        if (prods.status === "fulfilled") setProducts(prods.value);
        if (cats.status === "fulfilled") setCategories(cats.value);
        if (brds.status === "fulfilled") setBrands(brds.value);
      } catch {}
      finally { setIsLoading(false); }
    };
    load();
  }, []);

  // ── Toggle Status ──────────────────────────────────────────────────────────
  const handleToggleStatus = async (product: Product) => {
    try {
      await productApi.changeStatus(product.id, {
        isActive: !product.isActive,
      });
      toast.success(
        `${product.name} ${!product.isActive ? "activated" : "deactivated"}`
      );
      await loadProducts();
    } catch (err) {
      toast.error((err as Error).message || "Failed to update status");
    }
  };

  // ── Delete Product ─────────────────────────────────────────────────────────
  const handleDelete = async (product: Product) => {
    if (!window.confirm(`Delete "${product.name}"? This cannot be undone.`))
      return;
    try {
      await productApi.delete(product.id);
      toast.success(`${product.name} deleted`);
      await loadProducts();
    } catch (err) {
      toast.error((err as Error).message || "Failed to delete");
    }
  };

  // ── Filter Products ────────────────────────────────────────────────────────
  const filtered = products.filter((p) => {
    const matchSearch =
      !searchQuery ||
      p.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
      p.sku.toLowerCase().includes(searchQuery.toLowerCase()) ||
      p.brandName?.toLowerCase().includes(searchQuery.toLowerCase());

    const matchCategory = !categoryFilter || p.categoryId === categoryFilter;
    const matchBrand    = !brandFilter || p.brandId === brandFilter;
    const matchStatus   =
      statusFilter === "all" ||
      (statusFilter === "active" && p.isActive) ||
      (statusFilter === "inactive" && !p.isActive);

    return matchSearch && matchCategory && matchBrand && matchStatus;
  });

  // ── Stats ──────────────────────────────────────────────────────────────────
  const totalProducts = products.length;
  const activeProducts = products.filter((p) => p.isActive).length;
  const inactiveProducts = totalProducts - activeProducts;

  if (isLoading) {
    return (
      <div className="flex justify-center py-32">
        <Spinner size="lg" className="text-primary-600" />
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 py-8">

      {/* ── Header ──────────────────────────────────────────────────────────── */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Manage Products</h1>
          <p className="text-gray-500 mt-1">{totalProducts} total products</p>
        </div>
        <Link to="/admin/products/new">
          <Button>
            <Plus className="w-4 h-4" /> Add Product
          </Button>
        </Link>
      </div>

      {/* ── Stats ───────────────────────────────────────────────────────────── */}
      <div className="grid grid-cols-3 gap-4 mb-6">
        {[
          { label: "Total",    value: totalProducts,    color: "bg-blue-50 text-blue-700",  icon: Package },
          { label: "Active",   value: activeProducts,   color: "bg-green-50 text-green-700", icon: CheckCircle },
          { label: "Inactive", value: inactiveProducts, color: "bg-red-50 text-red-700",    icon: XCircle },
        ].map(({ label, value, color, icon: Icon }) => (
          <button
            key={label}
            onClick={() =>
              setStatusFilter(
                label === "Total" ? "all" : label === "Active" ? "active" : "inactive"
              )
            }
            className={cn(
              "bg-white rounded-xl border border-gray-100 shadow-sm p-4 flex items-center gap-3 hover:shadow-md transition text-left",
              statusFilter ===
                (label === "Total" ? "all" : label.toLowerCase()) &&
                "ring-2 ring-primary-500"
            )}
          >
            <div className={cn("p-2 rounded-lg", color)}>
              <Icon className="w-4 h-4" />
            </div>
            <div>
              <p className="text-xl font-bold text-gray-900">{value}</p>
              <p className="text-xs text-gray-500">{label}</p>
            </div>
          </button>
        ))}
      </div>

      {/* ── Search & Filters ────────────────────────────────────────────────── */}
      <div className="flex flex-col sm:flex-row gap-3 mb-4">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
          <input
            type="text"
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            placeholder="Search by name, SKU, brand..."
            className="w-full pl-10 pr-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-primary-500/20"
          />
        </div>
        <button
          onClick={() => setShowFilters(!showFilters)}
          className={cn(
            "flex items-center gap-2 px-4 py-2.5 border rounded-xl text-sm transition",
            showFilters
              ? "border-primary-400 bg-primary-50 text-primary-700"
              : "border-gray-200 text-gray-700 hover:bg-gray-50"
          )}
        >
          <Filter className="w-4 h-4" />
          Filters
          {(categoryFilter || brandFilter) && (
            <span className="w-2 h-2 bg-primary-600 rounded-full" />
          )}
        </button>
      </div>

      {/* ── Filter Panel ────────────────────────────────────────────────────── */}
      {showFilters && (
        <div className="mb-6 p-4 bg-gray-50 rounded-xl border border-gray-100 flex flex-wrap gap-4">
          <div>
            <label className="text-xs font-semibold text-gray-500 uppercase mb-1 block">
              Category
            </label>
            <select
              value={categoryFilter}
              onChange={(e) => setCategoryFilter(e.target.value)}
              className="px-3 py-2 border border-gray-200 rounded-lg text-sm bg-white focus:outline-none focus:ring-2 focus:ring-primary-500/20"
            >
              <option value="">All Categories</option>
              {categories.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
            </select>
          </div>
          <div>
            <label className="text-xs font-semibold text-gray-500 uppercase mb-1 block">
              Brand
            </label>
            <select
              value={brandFilter}
              onChange={(e) => setBrandFilter(e.target.value)}
              className="px-3 py-2 border border-gray-200 rounded-lg text-sm bg-white focus:outline-none focus:ring-2 focus:ring-primary-500/20"
            >
              <option value="">All Brands</option>
              {brands.map((b) => (
                <option key={b.id} value={b.id}>
                  {b.name}
                </option>
              ))}
            </select>
          </div>
          {(categoryFilter || brandFilter) && (
            <button
              onClick={() => {
                setCategoryFilter("");
                setBrandFilter("");
              }}
              className="self-end px-3 py-2 text-sm text-red-600 hover:bg-red-50 rounded-lg transition"
            >
              Clear
            </button>
          )}
        </div>
      )}

      {/* ── Products Table ──────────────────────────────────────────────────── */}
      {filtered.length === 0 ? (
        <div className="text-center py-16 bg-gray-50 rounded-xl border border-gray-100">
          <Package className="w-14 h-14 text-gray-200 mx-auto mb-3" />
          <p className="text-gray-600 font-semibold">No products found</p>
          <p className="text-sm text-gray-400 mt-1">Try different filters</p>
        </div>
      ) : (
        <div className="bg-white rounded-xl border border-gray-100 shadow-sm overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gray-50 border-b border-gray-100">
                <tr>
                  <th className="px-4 py-3 text-left text-xs font-semibold text-gray-500 uppercase">
                    Product
                  </th>
                  <th className="px-4 py-3 text-left text-xs font-semibold text-gray-500 uppercase">
                    SKU
                  </th>
                  <th className="px-4 py-3 text-left text-xs font-semibold text-gray-500 uppercase">
                    Category
                  </th>
                  <th className="px-4 py-3 text-left text-xs font-semibold text-gray-500 uppercase">
                    Brand
                  </th>
                  <th className="px-4 py-3 text-right text-xs font-semibold text-gray-500 uppercase">
                    Price
                  </th>
                  <th className="px-4 py-3 text-center text-xs font-semibold text-gray-500 uppercase">
                    Status
                  </th>
                  <th className="px-4 py-3 text-center text-xs font-semibold text-gray-500 uppercase">
                    Actions
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-50">
                {filtered.map((product) => {
                  const imgSrc =
                    product.primaryImageUrl ||
                    `https://placehold.co/40x40/e2e8f0/64748b?text=${encodeURIComponent(
                      product.name.charAt(0)
                    )}`;

                  return (
                    <tr
                      key={product.id}
                      className="hover:bg-gray-50 transition"
                    >
                      {/* Product */}
                      <td className="px-4 py-3">
                        <div className="flex items-center gap-3">
                          <img
                            src={imgSrc}
                            alt={product.name}
                            onError={(e) => {
                              (e.target as HTMLImageElement).src = `https://placehold.co/40x40/e2e8f0/64748b?text=${product.name.charAt(0)}`;
                            }}
                            className="w-10 h-10 rounded-lg object-cover bg-gray-100 border border-gray-100 shrink-0"
                          />
                          <div className="min-w-0">
                            <Link
                              to={`/products/${product.id}`}
                              className="text-sm font-medium text-gray-800 hover:text-primary-600 truncate block max-w-[200px] transition"
                            >
                              {product.name}
                            </Link>
                            <p className="text-xs text-gray-400 truncate max-w-[200px]">
                              {product.description}
                            </p>
                          </div>
                        </div>
                      </td>

                      {/* SKU */}
                      <td className="px-4 py-3">
                        <span className="text-xs font-mono text-gray-600 bg-gray-100 px-2 py-0.5 rounded">
                          {product.sku}
                        </span>
                      </td>

                      {/* Category */}
                      <td className="px-4 py-3">
                        <span className="text-sm text-gray-600">
                          {product.categoryName}
                        </span>
                      </td>

                      {/* Brand */}
                      <td className="px-4 py-3">
                        <span className="text-sm text-gray-600">
                          {product.brandName}
                        </span>
                      </td>

                      {/* Price */}
                      <td className="px-4 py-3 text-right">
                        <p className="text-sm font-bold text-gray-900">
                          {formatPrice(product.basePrice)}
                        </p>
                        {product.costPrice && (
                          <p className="text-xs text-gray-400">
                            Cost: {formatPrice(product.costPrice)}
                          </p>
                        )}
                      </td>

                      {/* Status */}
                      <td className="px-4 py-3 text-center">
                        <button
                          onClick={() => handleToggleStatus(product)}
                          className={cn(
                            "inline-flex items-center gap-1 px-2.5 py-1 rounded-full text-xs font-semibold transition cursor-pointer",
                            product.isActive
                              ? "bg-green-100 text-green-700 hover:bg-green-200"
                              : "bg-red-100 text-red-700 hover:bg-red-200"
                          )}
                        >
                          {product.isActive ? (
                            <>
                              <CheckCircle className="w-3 h-3" /> Active
                            </>
                          ) : (
                            <>
                              <XCircle className="w-3 h-3" /> Inactive
                            </>
                          )}
                        </button>
                      </td>

                      {/* Actions */}
                      <td className="px-4 py-3">
                        <div className="flex items-center justify-center gap-1">
                          <Link
                            to={`/products/${product.id}`}
                            className="p-1.5 text-gray-400 hover:text-primary-600 hover:bg-primary-50 rounded-lg transition"
                            title="View"
                          >
                            <Eye className="w-4 h-4" />
                          </Link>
                          <Link
                            to={`/admin/products/${product.id}/edit`}
                            className="p-1.5 text-gray-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition"
                            title="Edit"
                          >
                            <Edit className="w-4 h-4" />
                          </Link>
                          <Link
                            to={`/admin/products/${product.id}/images`}
                            className="p-1.5 text-gray-400 hover:text-purple-600 hover:bg-purple-50 rounded-lg transition"
                            title="Images"
                          >
                            <ImageIcon className="w-4 h-4" />
                          </Link>
                          <button
                            onClick={() => handleDelete(product)}
                            className="p-1.5 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition"
                            title="Delete"
                          >
                            <Trash2 className="w-4 h-4" />
                          </button>
                        </div>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>

          {/* Footer */}
          <div className="px-4 py-3 bg-gray-50 border-t border-gray-100 text-sm text-gray-500">
            Showing {filtered.length} of {products.length} products
          </div>
        </div>
      )}
    </div>
  );
};

export default AdminProducts;