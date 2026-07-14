import { useEffect, useState, useRef } from "react";
import { useSearchParams } from "react-router-dom";
import {
  Search,
  SlidersHorizontal,
  X,
  Package,
  ChevronLeft,
  ChevronRight,
  IndianRupee,
} from "lucide-react";
import { productApi } from "@/api/product.api";
import { categoryApi } from "@/api/category.api";
import { brandApi } from "@/api/brand.api";
import { useAuth } from "@/hooks/useAuth";
import { useWishlist } from "@/hooks/useWishlist";
import type {
  ProductSearchItem,
  ProductSearchParams,
  ProductSortBy,
  Category,
  Brand,
} from "@/types";
import SearchProductCard from "@/components/features/SearchProductCard";
import Spinner from "@/components/ui/Spinner";
import { ProductSortLabels } from "@/types";

const PAGE_SIZE = 20;

const Products = () => {
  const [searchParams, setSearchParams] = useSearchParams();
  const { isAuthenticated } = useAuth();
  const { wishlistedIds, fetchWishlist } = useWishlist();

  const [products, setProducts]       = useState<ProductSearchItem[]>([]);
  const [categories, setCategories]   = useState<Category[]>([]);
  const [brands, setBrands]           = useState<Brand[]>([]);
  const [totalCount, setTotalCount]   = useState(0);
  const [totalPages, setTotalPages]   = useState(1);
  const [isLoading, setIsLoading]     = useState(true);
  const [error, setError]             = useState<string | null>(null);
  const [showFilters, setShowFilters] = useState(false);

  const searchQ   = searchParams.get("search")     || "";
  const catId     = searchParams.get("categoryId") || "";
  const brandId   = searchParams.get("brandId")    || "";
  const minPrice  = searchParams.get("minPrice")   || "";
  const maxPrice  = searchParams.get("maxPrice")   || "";
  const featured  = searchParams.get("featured")   || "";
  const inStock   = searchParams.get("inStock")    || "";
  const sortBy    = searchParams.get("sortBy")     || "0";
  const page      = Number(searchParams.get("page")) || 1;

  const [searchInput, setSearchInput]     = useState(searchQ);
  const [minPriceInput, setMinPriceInput] = useState(minPrice);
  const [maxPriceInput, setMaxPriceInput] = useState(maxPrice);

  const isFirstLoad = useRef(true);

  // ── Load Categories & Brands ───────────────────────────────────────────────
  useEffect(() => {
    const load = async () => {
      try {
        const [cats, brds] = await Promise.all([
          categoryApi.getAll(),
          brandApi.getAll(),
        ]);
        setCategories(cats.filter((c) => c.isActive));
        setBrands(brds.filter((b) => b.isActive));
      } catch {}
    };
    load();
  }, []);

  // ── Load Wishlist ──────────────────────────────────────────────────────────
  useEffect(() => {
    if (isAuthenticated) {
      fetchWishlist();
    }
  }, [isAuthenticated]);

  // ── Search Products ────────────────────────────────────────────────────────
  useEffect(() => {
    const search = async () => {
      setIsLoading(true);
      setError(null);
      try {
        const params: ProductSearchParams = {
          Page: page,
          PageSize: PAGE_SIZE,
          SortBy: Number(sortBy) as ProductSortBy,
          ...(searchQ  && { Search: searchQ }),
          ...(catId    && { CategoryId: catId }),
          ...(brandId  && { BrandId: brandId }),
          ...(minPrice && { MinPrice: Number(minPrice) }),
          ...(maxPrice && { MaxPrice: Number(maxPrice) }),
          ...(featured === "true" && { Featured: true }),
          ...(inStock  === "true" && { InStock: true }),
        };
        const res = await productApi.search(params);
        setProducts(res.items);
        setTotalCount(res.totalCount);
        setTotalPages(res.totalPages);
      } catch (err) {
        setError((err as Error).message);
        setProducts([]);
      } finally {
        setIsLoading(false);
        isFirstLoad.current = false;
      }
    };
    search();
  }, [searchQ, catId, brandId, minPrice, maxPrice, featured, inStock, sortBy, page]);

  // ── URL Helpers ────────────────────────────────────────────────────────────
  const updateParam = (key: string, value: string) => {
    const params = new URLSearchParams(searchParams);
    if (value) params.set(key, value);
    else params.delete(key);
    params.delete("page");
    setSearchParams(params);
  };

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    updateParam("search", searchInput.trim());
  };

  const handlePriceFilter = () => {
    const params = new URLSearchParams(searchParams);
    if (minPriceInput) params.set("minPrice", minPriceInput);
    else params.delete("minPrice");
    if (maxPriceInput) params.set("maxPrice", maxPriceInput);
    else params.delete("maxPrice");
    params.delete("page");
    setSearchParams(params);
  };

  const goToPage = (p: number) => {
    const params = new URLSearchParams(searchParams);
    params.set("page", String(p));
    setSearchParams(params);
    window.scrollTo({ top: 0, behavior: "smooth" });
  };

  const clearFilters = () => {
    setSearchInput("");
    setMinPriceInput("");
    setMaxPriceInput("");
    setSearchParams({});
  };

  const hasActiveFilters =
    searchQ || catId || brandId || minPrice || maxPrice || featured || inStock || sortBy !== "0";

  const filterCount = [searchQ, catId, brandId, minPrice, maxPrice, featured, inStock].filter(Boolean).length;

  return (
    <div className="max-w-7xl mx-auto px-4 py-8">
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900">All Products</h1>
        <p className="text-gray-500 mt-1">
          {isLoading ? "Searching..." : `${totalCount} product${totalCount !== 1 ? "s" : ""} found`}
        </p>
      </div>

      {/* Controls */}
      <div className="flex flex-col sm:flex-row gap-3 mb-6">
        <form onSubmit={handleSearch} className="flex-1 flex gap-2">
          <div className="relative flex-1">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
            <input type="text" value={searchInput} onChange={(e) => setSearchInput(e.target.value)} placeholder="Search products, brands..." className="w-full pl-10 pr-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-primary-500/20 focus:border-primary-400 transition" />
          </div>
          <button type="submit" className="px-5 py-2.5 bg-primary-600 text-white rounded-xl text-sm font-medium hover:bg-primary-700 transition">Search</button>
        </form>

        <select value={sortBy} onChange={(e) => updateParam("sortBy", e.target.value)} className="px-4 py-2.5 border border-gray-200 rounded-xl text-sm bg-white focus:outline-none focus:ring-2 focus:ring-primary-500/20 cursor-pointer">
          {Object.entries(ProductSortLabels).map(([value, label]) => (
            <option key={value} value={value}>{label}</option>
          ))}
        </select>

        <button onClick={() => setShowFilters(!showFilters)} className={`flex items-center gap-2 px-4 py-2.5 border rounded-xl text-sm transition ${showFilters ? "border-primary-400 bg-primary-50 text-primary-700" : "border-gray-200 hover:bg-gray-50 text-gray-700"}`}>
          <SlidersHorizontal className="w-4 h-4" />
          Filters
          {filterCount > 0 && <span className="bg-primary-600 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center font-medium">{filterCount}</span>}
        </button>

        {hasActiveFilters && (
          <button onClick={clearFilters} className="flex items-center gap-2 px-4 py-2.5 text-red-600 border border-red-200 rounded-xl text-sm hover:bg-red-50 transition">
            <X className="w-4 h-4" /> Clear All
          </button>
        )}
      </div>

      {/* Filters Panel */}
      {showFilters && (
        <div className="mb-6 p-5 bg-gray-50 rounded-xl border border-gray-100 space-y-5">
          <div>
            <p className="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-2">Category</p>
            <div className="flex flex-wrap gap-2">
              <button onClick={() => updateParam("categoryId", "")} className={`px-3 py-1.5 rounded-full text-sm font-medium transition ${!catId ? "bg-primary-600 text-white" : "bg-white border border-gray-200 text-gray-700 hover:border-primary-400"}`}>All</button>
              {categories.map((cat) => (
                <button key={cat.id} onClick={() => updateParam("categoryId", cat.id)} className={`px-3 py-1.5 rounded-full text-sm font-medium transition ${catId === cat.id ? "bg-primary-600 text-white" : "bg-white border border-gray-200 text-gray-700 hover:border-primary-400"}`}>{cat.name}</button>
              ))}
            </div>
          </div>
          <div>
            <p className="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-2">Brand</p>
            <div className="flex flex-wrap gap-2">
              <button onClick={() => updateParam("brandId", "")} className={`px-3 py-1.5 rounded-full text-sm font-medium transition ${!brandId ? "bg-primary-600 text-white" : "bg-white border border-gray-200 text-gray-700 hover:border-primary-400"}`}>All</button>
              {brands.map((brand) => (
                <button key={brand.id} onClick={() => updateParam("brandId", brand.id)} className={`px-3 py-1.5 rounded-full text-sm font-medium transition ${brandId === brand.id ? "bg-primary-600 text-white" : "bg-white border border-gray-200 text-gray-700 hover:border-primary-400"}`}>{brand.name}</button>
              ))}
            </div>
          </div>
          <div>
            <p className="text-xs font-semibold text-gray-500 uppercase tracking-wider mb-2">Price Range</p>
            <div className="flex items-center gap-3">
              <div className="relative flex-1">
                <IndianRupee className="absolute left-3 top-1/2 -translate-y-1/2 w-3.5 h-3.5 text-gray-400" />
                <input type="number" value={minPriceInput} onChange={(e) => setMinPriceInput(e.target.value)} placeholder="Min" className="w-full pl-8 pr-3 py-2 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary-500/20" />
              </div>
              <span className="text-gray-400 text-sm">to</span>
              <div className="relative flex-1">
                <IndianRupee className="absolute left-3 top-1/2 -translate-y-1/2 w-3.5 h-3.5 text-gray-400" />
                <input type="number" value={maxPriceInput} onChange={(e) => setMaxPriceInput(e.target.value)} placeholder="Max" className="w-full pl-8 pr-3 py-2 border border-gray-200 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-primary-500/20" />
              </div>
              <button onClick={handlePriceFilter} className="px-4 py-2 bg-primary-600 text-white rounded-lg text-sm font-medium hover:bg-primary-700 transition">Apply</button>
            </div>
          </div>
          <div className="flex flex-wrap gap-4">
            <label className="flex items-center gap-2 cursor-pointer">
              <input type="checkbox" checked={inStock === "true"} onChange={(e) => updateParam("inStock", e.target.checked ? "true" : "")} className="rounded border-gray-300 text-primary-600 focus:ring-primary-500" />
              <span className="text-sm text-gray-700">In Stock Only</span>
            </label>
            <label className="flex items-center gap-2 cursor-pointer">
              <input type="checkbox" checked={featured === "true"} onChange={(e) => updateParam("featured", e.target.checked ? "true" : "")} className="rounded border-gray-300 text-primary-600 focus:ring-primary-500" />
              <span className="text-sm text-gray-700">Featured Only</span>
            </label>
          </div>
        </div>
      )}

      {/* Filter Tags */}
      {hasActiveFilters && (
        <div className="flex flex-wrap gap-2 mb-6">
          {searchQ && <FilterTag label={`Search: "${searchQ}"`} onRemove={() => { setSearchInput(""); updateParam("search", ""); }} />}
          {catId && <FilterTag label={categories.find((c) => c.id === catId)?.name || "Category"} onRemove={() => updateParam("categoryId", "")} />}
          {brandId && <FilterTag label={brands.find((b) => b.id === brandId)?.name || "Brand"} onRemove={() => updateParam("brandId", "")} />}
          {minPrice && <FilterTag label={`Min: ₹${minPrice}`} onRemove={() => { setMinPriceInput(""); updateParam("minPrice", ""); }} />}
          {maxPrice && <FilterTag label={`Max: ₹${maxPrice}`} onRemove={() => { setMaxPriceInput(""); updateParam("maxPrice", ""); }} />}
          {inStock === "true" && <FilterTag label="In Stock" onRemove={() => updateParam("inStock", "")} />}
          {featured === "true" && <FilterTag label="Featured" onRemove={() => updateParam("featured", "")} />}
        </div>
      )}

      {/* Loading */}
      {isLoading && (
        <div className="flex flex-col items-center justify-center py-24 gap-4">
          <Spinner size="lg" className="text-primary-600" />
          <p className="text-gray-400 text-sm">Searching products...</p>
        </div>
      )}

      {/* Error */}
      {error && !isLoading && (
        <div className="text-center py-20">
          <div className="text-5xl mb-4">⚠️</div>
          <h3 className="text-xl font-semibold text-gray-700 mb-2">Something went wrong</h3>
          <p className="text-red-500 text-sm mb-6">{error}</p>
          <button onClick={() => window.location.reload()} className="px-6 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition">Try Again</button>
        </div>
      )}

      {/* Empty */}
      {!isLoading && !error && products.length === 0 && (
        <div className="text-center py-24">
          <Package className="w-16 h-16 text-gray-200 mx-auto mb-4" />
          <h3 className="text-xl font-semibold text-gray-700 mb-2">No products found</h3>
          <p className="text-gray-500 mb-6">Try different search terms or filters</p>
          <button onClick={clearFilters} className="px-6 py-2 bg-primary-600 text-white rounded-lg hover:bg-primary-700 transition">Clear Filters</button>
        </div>
      )}

      {/* ✅ Products Grid - passes wishlistedIds */}
      {!isLoading && !error && products.length > 0 && (
        <>
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
            {products.map((product) => (
              <SearchProductCard
                key={product.id}
                product={product}
                wishlistedIds={wishlistedIds}
                onWishlistChange={fetchWishlist}
              />
            ))}
          </div>

          {/* Pagination */}
          {totalPages > 1 && (
            <div className="flex items-center justify-center gap-2 mt-10">
              <button onClick={() => goToPage(page - 1)} disabled={page <= 1} className="p-2 border border-gray-200 rounded-lg disabled:opacity-40 hover:bg-gray-50 transition"><ChevronLeft className="w-4 h-4" /></button>
              {Array.from({ length: totalPages }, (_, i) => i + 1).filter((p) => p === 1 || p === totalPages || Math.abs(p - page) <= 2).map((p, idx, arr) => (
                <span key={p}>
                  {idx > 0 && arr[idx - 1] !== p - 1 && <span className="px-2 text-gray-400">...</span>}
                  <button onClick={() => goToPage(p)} className={`w-10 h-10 rounded-lg text-sm font-medium transition ${p === page ? "bg-primary-600 text-white shadow-sm" : "border border-gray-200 text-gray-700 hover:border-primary-400"}`}>{p}</button>
                </span>
              ))}
              <button onClick={() => goToPage(page + 1)} disabled={page >= totalPages} className="p-2 border border-gray-200 rounded-lg disabled:opacity-40 hover:bg-gray-50 transition"><ChevronRight className="w-4 h-4" /></button>
            </div>
          )}
          <p className="text-center text-sm text-gray-400 mt-4">Page {page} of {totalPages} • {totalCount} total products</p>
        </>
      )}
    </div>
  );
};

const FilterTag = ({ label, onRemove }: { label: string; onRemove: () => void }) => (
  <span className="inline-flex items-center gap-1 px-3 py-1 bg-primary-50 text-primary-700 text-sm rounded-full">
    {label}
    <button onClick={onRemove} className="hover:text-primary-900 transition"><X className="w-3 h-3" /></button>
  </span>
);

export default Products;