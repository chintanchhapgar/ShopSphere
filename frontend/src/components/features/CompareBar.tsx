import { Link } from "react-router-dom";
import { X, GitCompare, ArrowRight } from "lucide-react";
import { useCompare } from "@/hooks/useCompare";
import { formatPrice } from "@/utils/formatPrice";
import { cn } from "@/utils/cn";
import toast from "react-hot-toast";

const CompareBar = () => {
  const { items, removeFromCompare, clearCompare } = useCompare();

  if (items.length === 0) return null;

  const handleClear = () => {
    if (window.confirm("Clear all products from comparison?")) {
      clearCompare();
      toast.success("Comparison cleared");
    }
  };

  return (
    <div className="fixed bottom-0 left-0 right-0 bg-white dark:bg-gray-800 border-t-2 border-primary-500 shadow-2xl z-40 animate-fadeIn">
      <div className="max-w-7xl mx-auto px-4 py-3">
        <div className="flex items-center justify-between gap-4">

          {/* Left: Product Chips */}
          <div className="flex items-center gap-3 flex-1 overflow-x-auto">
            <div className="flex items-center gap-2 shrink-0">
              <GitCompare className="w-5 h-5 text-primary-600" />
              <span className="text-sm font-bold text-gray-800 dark:text-gray-100">
                Compare ({items.length}/4)
              </span>
            </div>

            <div className="flex gap-2 items-center">
              {items.map((item) => {
                const name = 'name' in item ? item.name : '';
                const price = 'basePrice' in item
                  ? item.basePrice
                  : (item as any).price || 0;
                const imgSrc =
                  ('primaryImageUrl' in item && item.primaryImageUrl) ||
                  ('thumbnail' in item && item.thumbnail) ||
                  `https://placehold.co/48x48/e2e8f0/64748b?text=${name.charAt(0)}`;

                return (
                  <div
                    key={item.id}
                    className="relative flex items-center gap-2 px-2 py-1.5 bg-gray-50 dark:bg-gray-700 rounded-lg group"
                  >
                    <img
                      src={imgSrc}
                      alt={name}
                      className="w-8 h-8 rounded object-cover bg-white"
                    />
                    <div className="hidden sm:block">
                      <p className="text-xs font-medium text-gray-800 dark:text-gray-200 max-w-[100px] truncate">
                        {name}
                      </p>
                      <p className="text-xs text-primary-600 font-bold">
                        {formatPrice(price as number)}
                      </p>
                    </div>
                    <button
                      onClick={() => removeFromCompare(item.id)}
                      className="p-0.5 text-gray-400 hover:text-red-500 hover:bg-red-50 rounded transition"
                    >
                      <X className="w-3 h-3" />
                    </button>
                  </div>
                );
              })}

              {/* Empty Slots */}
              {[...Array(4 - items.length)].map((_, idx) => (
                <div
                  key={`empty-${idx}`}
                  className="w-16 h-11 border-2 border-dashed border-gray-200 dark:border-gray-700 rounded-lg flex items-center justify-center"
                >
                  <span className="text-xs text-gray-400">+</span>
                </div>
              ))}
            </div>
          </div>

          {/* Right: Actions */}
          <div className="flex items-center gap-2 shrink-0">
            <button
              onClick={handleClear}
              className="hidden sm:block px-3 py-2 text-xs text-red-600 hover:bg-red-50 rounded-lg transition"
            >
              Clear All
            </button>
            {items.length >= 2 ? (
              <Link
                to="/compare"
                className="flex items-center gap-1 px-4 py-2 bg-primary-600 text-white text-sm font-medium rounded-lg hover:bg-primary-700 transition"
              >
                Compare Now
                <ArrowRight className="w-4 h-4" />
              </Link>
            ) : (
              <span className="text-xs text-gray-500">
                Add {2 - items.length} more to compare
              </span>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default CompareBar;