import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import {
  ArrowLeft,
  X,
  ShoppingCart,
  Star,
  Check,
  GitCompare,
  Trash2,
  Heart,
} from "lucide-react";
import { useCompare } from "@/hooks/useCompare";
import { useCart } from "@/hooks/useCart";
import { productApi } from "@/api/product.api";
import type { ProductDetail } from "@/types";
import { formatPrice } from "@/utils/formatPrice";
import { cn } from "@/utils/cn";
import Spinner from "@/components/ui/Spinner";
import Button from "@/components/ui/Button";
import toast from "react-hot-toast";

const Compare = () => {
  const { items, removeFromCompare, clearCompare } = useCompare();
  const { addToCart } = useCart();

  const [details, setDetails]   = useState<Record<string, ProductDetail>>({});
  const [isLoading, setIsLoading] = useState(true);

  // ── Load full details ──────────────────────────────────────────────────────
  useEffect(() => {
    const load = async () => {
      setIsLoading(true);
      const detailsMap: Record<string, ProductDetail> = {};

      await Promise.allSettled(
        items.map(async (item) => {
          try {
            const detail = await productApi.getById(item.id);
            detailsMap[item.id] = detail;
          } catch {
            // Silent fail
          }
        })
      );

      setDetails(detailsMap);
      setIsLoading(false);
    };

    if (items.length > 0) load();
    else setIsLoading(false);
  }, [items]);

  const handleAddToCart = async (id: string, name: string) => {
    try {
      await addToCart(id, 1);
      toast.success(`${name} added to cart!`);
    } catch (err) {
      toast.error((err as Error).message || "Failed");
    }
  };

  // ── Empty State ────────────────────────────────────────────────────────────
  if (items.length === 0) {
    return (
      <div className="max-w-7xl mx-auto px-4 py-24 text-center">
        <GitCompare className="w-20 h-20 text-gray-200 mx-auto mb-6" />
        <h2 className="text-2xl font-bold text-gray-700 mb-3">
          No products to compare
        </h2>
        <p className="text-gray-500 mb-8">
          Add products to comparison from the product listing page
        </p>
        <Link to="/products">
          <Button size="lg">Browse Products</Button>
        </Link>
      </div>
    );
  }

  if (isLoading) {
    return (
      <div className="flex justify-center py-32">
        <Spinner size="lg" className="text-primary-600" />
      </div>
    );
  }

  // ── Comparison Rows ────────────────────────────────────────────────────────
  const rows = [
    {
      label: "Price",
      render: (p: ProductDetail) => (
        <span className="text-2xl font-bold text-primary-600">
          {formatPrice(p.basePrice)}
        </span>
      ),
      highlight: (values: number[]) => Math.min(...values),
      getValue: (p: ProductDetail) => p.basePrice,
    },
    {
      label: "Brand",
      render: (p: ProductDetail) => (
        <span className="font-medium text-gray-800 dark:text-gray-200">
          {p.brand?.name || "—"}
        </span>
      ),
    },
    {
      label: "Category",
      render: (p: ProductDetail) => (
        <span className="text-gray-600 dark:text-gray-400">
          {p.category?.name || "—"}
        </span>
      ),
    },
    {
      label: "SKU",
      render: (p: ProductDetail) => (
        <span className="font-mono text-xs text-gray-500">{p.sku}</span>
      ),
    },
    {
      label: "Description",
      render: (p: ProductDetail) => (
        <p className="text-sm text-gray-600 dark:text-gray-400 line-clamp-3">
          {p.description}
        </p>
      ),
    },
    {
      label: "Status",
      render: (p: ProductDetail) => (
        <span className={cn(
          "inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs font-semibold",
          p.isActive
            ? "bg-green-100 text-green-700"
            : "bg-red-100 text-red-700"
        )}>
          {p.isActive ? "✓ Available" : "✗ Unavailable"}
        </span>
      ),
    },
    {
      label: "Images",
      render: (p: ProductDetail) => (
        <span className="text-sm text-gray-600">
          {p.images?.length || 0} photos
        </span>
      ),
    },
  ];

  const allPrices = items
    .map((i) => details[i.id]?.basePrice)
    .filter(Boolean) as number[];
  const minPrice = allPrices.length ? Math.min(...allPrices) : 0;

  return (
    <div className="max-w-7xl mx-auto px-4 py-8">

      {/* Back */}
      <Link
        to="/products"
        className="inline-flex items-center gap-2 text-sm text-gray-500 hover:text-primary-600 mb-6 transition"
      >
        <ArrowLeft className="w-4 h-4" /> Back to Products
      </Link>

      {/* Header */}
      <div className="flex items-center justify-between mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 dark:text-gray-100 flex items-center gap-3">
            <GitCompare className="w-8 h-8 text-primary-600" />
            Compare Products
          </h1>
          <p className="text-gray-500 mt-1">
            Comparing {items.length} product{items.length !== 1 ? "s" : ""}
          </p>
        </div>
        <button
          onClick={() => {
            if (window.confirm("Clear all products?")) clearCompare();
          }}
          className="flex items-center gap-1 px-3 py-2 text-sm text-red-600 hover:bg-red-50 rounded-lg transition"
        >
          <Trash2 className="w-4 h-4" />
          Clear All
        </button>
      </div>

      {/* Comparison Table */}
      <div className="bg-white dark:bg-gray-800 rounded-2xl border border-gray-100 dark:border-gray-700 shadow-sm overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            {/* Product Headers */}
            <thead>
              <tr className="border-b border-gray-100 dark:border-gray-700">
                <th className="w-40 p-4 text-left text-xs font-semibold text-gray-500 uppercase tracking-wider bg-gray-50 dark:bg-gray-900">
                  Product
                </th>
                {items.map((item) => {
                  const p = details[item.id];
                  const name = 'name' in item ? item.name : '';
                  const imgSrc = p?.images?.[0]?.imageUrl ||
                    ('primaryImageUrl' in item && item.primaryImageUrl) ||
                    ('thumbnail' in item && item.thumbnail) ||
                    `https://placehold.co/200x200/e2e8f0/64748b?text=${name.charAt(0)}`;

                  return (
                    <th
                      key={item.id}
                      className="w-64 p-4 border-l border-gray-100 dark:border-gray-700 relative"
                    >
                      <button
                        onClick={() => removeFromCompare(item.id)}
                        className="absolute top-2 right-2 p-1 text-gray-400 hover:text-red-500 hover:bg-red-50 rounded-full transition"
                        title="Remove from comparison"
                      >
                        <X className="w-4 h-4" />
                      </button>
                      <Link
                        to={`/products/${item.id}`}
                        className="block group"
                      >
                        <img
                          src={imgSrc}
                          alt={name}
                          className="w-full aspect-square object-cover rounded-lg bg-gray-50 mb-3"
                        />
                        <h3 className="text-sm font-bold text-gray-800 dark:text-gray-200 group-hover:text-primary-600 line-clamp-2 min-h-[40px]">
                          {name}
                        </h3>
                      </Link>
                    </th>
                  );
                })}
              </tr>
            </thead>

            {/* Comparison Rows */}
            <tbody className="divide-y divide-gray-50 dark:divide-gray-700">
              {rows.map((row) => (
                <tr key={row.label} className="hover:bg-gray-50 dark:hover:bg-gray-900/50 transition">
                  <td className="p-4 text-sm font-semibold text-gray-600 dark:text-gray-400 bg-gray-50 dark:bg-gray-900">
                    {row.label}
                  </td>
                  {items.map((item) => {
                    const p = details[item.id];
                    if (!p) {
                      return (
                        <td key={item.id} className="p-4 border-l border-gray-100 dark:border-gray-700">
                          <Spinner size="sm" />
                        </td>
                      );
                    }

                    // Highlight best price
                    const isBestPrice = row.label === "Price" && p.basePrice === minPrice;

                    return (
                      <td
                        key={item.id}
                        className={cn(
                          "p-4 border-l border-gray-100 dark:border-gray-700",
                          isBestPrice && "bg-green-50 dark:bg-green-900/20"
                        )}
                      >
                        {row.render(p)}
                        {isBestPrice && (
                          <span className="inline-block mt-1 px-2 py-0.5 bg-green-500 text-white text-xs font-bold rounded-full">
                            🏆 Best Price
                          </span>
                        )}
                      </td>
                    );
                  })}
                </tr>
              ))}

              {/* Actions Row */}
              <tr className="bg-gray-50 dark:bg-gray-900">
                <td className="p-4 text-sm font-semibold text-gray-600 dark:text-gray-400">
                  Actions
                </td>
                {items.map((item) => {
                  const p = details[item.id];
                  const name = 'name' in item ? item.name : '';

                  return (
                    <td key={item.id} className="p-4 border-l border-gray-100 dark:border-gray-700">
                      <div className="flex flex-col gap-2">
                        <Button
                          size="sm"
                          fullWidth
                          onClick={() => handleAddToCart(item.id, name)}
                          disabled={!p?.isActive}
                        >
                          <ShoppingCart className="w-3.5 h-3.5" />
                          Add to Cart
                        </Button>
                        <Link to={`/products/${item.id}`}>
                          <Button size="sm" variant="outline" fullWidth>
                            View Details
                          </Button>
                        </Link>
                      </div>
                    </td>
                  );
                })}
              </tr>
            </tbody>
          </table>
        </div>
      </div>

      {/* Legend */}
      <div className="mt-4 flex items-center gap-4 text-xs text-gray-500">
        <div className="flex items-center gap-1">
          <span className="w-3 h-3 bg-green-500 rounded"></span>
          Best Price
        </div>
      </div>
    </div>
  );
};

export default Compare;