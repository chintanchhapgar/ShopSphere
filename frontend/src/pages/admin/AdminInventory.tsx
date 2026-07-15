import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import {
  Package,
  Search,
  AlertTriangle,
  BoxSelect,
  History,
  X,
  Save,
  ArrowUpDown,
  CheckCircle,
  TrendingUp,
  TrendingDown,
} from "lucide-react";
import { productApi } from "@/api/product.api";
import type { Product } from "@/types";
import { cn } from "@/utils/cn";
import Spinner from "@/components/ui/Spinner";
import Button from "@/components/ui/Button";
import toast from "react-hot-toast";

// ── Types matching API response ──────────────────────────────────────────────
interface InventoryData {
  productId:         string;
  quantityOnHand:    number;
  reservedQuantity:  number;
  availableQuantity: number;
  lowStockThreshold: number;
  isLowStock:        boolean;
}

interface InventoryTransaction {
  id:              string;
  quantity:        number;
  transactionType: number;
  reason:          string;
  reference:       string;
  createdAtUtc:    string;
  createdBy:       string;
}

// ── Transaction Type Labels ──────────────────────────────────────────────────
const TransactionTypeLabels: Record<number, string> = {
  1: "Initial Stock",
  2: "Adjustment",
  3: "Reservation",
  4: "Release",
  5: "Sale",
  6: "Return",
};

const TransactionTypeColors: Record<number, string> = {
  1: "bg-blue-100 text-blue-700",
  2: "bg-purple-100 text-purple-700",
  3: "bg-yellow-100 text-yellow-700",
  4: "bg-green-100 text-green-700",
  5: "bg-red-100 text-red-700",
  6: "bg-orange-100 text-orange-700",
};

const AdminInventory = () => {
  const [products, setProducts]       = useState<Product[]>([]);
  const [isLoading, setIsLoading]     = useState(true);
  const [searchQuery, setSearchQuery] = useState("");
  const [stockFilter, setStockFilter] = useState<"all" | "low" | "out">("all");

  // Inventory data keyed by productId
  const [inventoryMap, setInventoryMap] = useState<Record<string, InventoryData>>({});

  // ── Adjust Modal ───────────────────────────────────────────────────────────
  const [adjustProductId, setAdjustProductId]     = useState<string | null>(null);
  const [adjustProductName, setAdjustProductName] = useState("");
  const [adjustQuantity, setAdjustQuantity]       = useState("");
  const [adjustReason, setAdjustReason]           = useState("");
  const [adjustType, setAdjustType]               = useState<"add" | "remove">("add");
  const [adjusting, setAdjusting]                 = useState(false);

  // ── History Modal ──────────────────────────────────────────────────────────
  const [historyProductId, setHistoryProductId]     = useState<string | null>(null);
  const [historyProductName, setHistoryProductName] = useState("");
  const [history, setHistory]                       = useState<InventoryTransaction[]>([]);
  const [historyLoading, setHistoryLoading]         = useState(false);

  // ── Load All Data ──────────────────────────────────────────────────────────
  const loadAll = async () => {
    try {
      const prods = await productApi.getAll();
      setProducts(prods);

      const invMap: Record<string, InventoryData> = {};
      await Promise.allSettled(
        prods.map(async (p) => {
          try {
            const inv = await productApi.getInventory(p.id);
            if (inv) {
              invMap[p.id] = {
                productId:         p.id,
                quantityOnHand:    inv.quantityOnHand ?? 0,
                reservedQuantity:  inv.reservedQuantity ?? 0,
                availableQuantity: inv.availableQuantity ?? 0,
                lowStockThreshold: inv.lowStockThreshold ?? 10,
                isLowStock:        inv.isLowStock ?? false,
              };
            }
          } catch {
            invMap[p.id] = {
              productId: p.id, quantityOnHand: 0, reservedQuantity: 0,
              availableQuantity: 0, lowStockThreshold: 10, isLowStock: false,
            };
          }
        })
      );
      setInventoryMap(invMap);
    } catch {
      toast.error("Failed to load data");
    }
  };

  useEffect(() => {
    const load = async () => {
      setIsLoading(true);
      await loadAll();
      setIsLoading(false);
    };
    load();
  }, []);

  // ── Refresh Single ─────────────────────────────────────────────────────────
  const refreshInventory = async (productId: string) => {
    try {
      const inv = await productApi.getInventory(productId);
      setInventoryMap((prev) => ({
        ...prev,
        [productId]: {
          productId,
          quantityOnHand:    inv.quantityOnHand ?? 0,
          reservedQuantity:  inv.reservedQuantity ?? 0,
          availableQuantity: inv.availableQuantity ?? 0,
          lowStockThreshold: inv.lowStockThreshold ?? 10,
          isLowStock:        inv.isLowStock ?? false,
        },
      }));
    } catch {}
  };

  // ── Adjust ─────────────────────────────────────────────────────────────────
  const openAdjust = (product: Product) => {
    setAdjustProductId(product.id);
    setAdjustProductName(product.name);
    setAdjustQuantity("");
    setAdjustReason("");
    setAdjustType("add");
  };

  const handleAdjust = async () => {
    if (!adjustProductId) return;
    const qty = Number(adjustQuantity);
    if (!qty || qty <= 0) { toast.error("Enter valid quantity"); return; }
    if (!adjustReason.trim()) { toast.error("Reason is required"); return; }

    const finalQty = adjustType === "add" ? qty : -qty;

    setAdjusting(true);
    try {
      await productApi.adjustInventory(adjustProductId, {
        quantity: finalQty,
        reason:   adjustReason.trim(),
      });
      toast.success(`${adjustType === "add" ? "Added" : "Removed"} ${qty} units`);
      setAdjustProductId(null);
      await refreshInventory(adjustProductId);
    } catch (err) {
      toast.error((err as Error).message || "Failed to adjust");
    } finally {
      setAdjusting(false);
    }
  };

  // ── History ────────────────────────────────────────────────────────────────
  const openHistory = async (product: Product) => {
    setHistoryProductId(product.id);
    setHistoryProductName(product.name);
    setHistoryLoading(true);
    try {
      const data = await productApi.getInventoryHistory(product.id);
      setHistory(data);
    } catch {
      setHistory([]);
    } finally {
      setHistoryLoading(false);
    }
  };

  // ── Filter ─────────────────────────────────────────────────────────────────
  const filtered = products.filter((p) => {
    const inv = inventoryMap[p.id];
    const available = inv?.availableQuantity ?? 0;

    const matchSearch =
      !searchQuery ||
      p.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
      p.sku.toLowerCase().includes(searchQuery.toLowerCase());

    const matchStock =
      stockFilter === "all" ||
      (stockFilter === "out" && available <= 0) ||
      (stockFilter === "low" && inv?.isLowStock && available > 0);

    return matchSearch && matchStock;
  });

  // ── Stats ──────────────────────────────────────────────────────────────────
  const outOfStock = products.filter((p) => (inventoryMap[p.id]?.availableQuantity ?? 0) <= 0).length;
  const lowStock   = products.filter((p) => inventoryMap[p.id]?.isLowStock && (inventoryMap[p.id]?.availableQuantity ?? 0) > 0).length;
  const inStock    = products.length - outOfStock - lowStock;

  if (isLoading) {
    return <div className="flex justify-center py-32"><Spinner size="lg" className="text-primary-600" /></div>;
  }

  return (
    <div className="max-w-7xl mx-auto px-4 py-8">

      {/* Header */}
      <div className="mb-8">
        <h1 className="text-3xl font-bold text-gray-900 flex items-center gap-3">
          <Package className="w-8 h-8 text-primary-600" />
          Inventory Management
        </h1>
        <p className="text-gray-500 mt-1">Track and manage product stock levels</p>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-8">
        {[
          { label: "Total",      value: products.length, color: "bg-blue-50 text-blue-700",    icon: Package,       f: "all" as const },
          { label: "In Stock",   value: inStock,         color: "bg-green-50 text-green-700",   icon: CheckCircle,   f: "all" as const },
          { label: "Low Stock",  value: lowStock,        color: "bg-yellow-50 text-yellow-700", icon: AlertTriangle, f: "low" as const },
          { label: "Out of Stock", value: outOfStock,    color: "bg-red-50 text-red-700",       icon: BoxSelect,     f: "out" as const },
        ].map(({ label, value, color, icon: Icon, f }) => (
          <button key={label} onClick={() => setStockFilter(f)} className={cn("bg-white rounded-xl border border-gray-100 shadow-sm p-4 flex items-center gap-3 hover:shadow-md transition text-left", stockFilter === f && f !== "all" && "ring-2 ring-primary-500")}>
            <div className={cn("p-2.5 rounded-xl", color)}><Icon className="w-5 h-5" /></div>
            <div><p className="text-2xl font-bold text-gray-900">{value}</p><p className="text-xs text-gray-500">{label}</p></div>
          </button>
        ))}
      </div>

      {/* Search */}
      <div className="flex flex-col sm:flex-row gap-3 mb-6">
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
          <input type="text" value={searchQuery} onChange={(e) => setSearchQuery(e.target.value)} placeholder="Search by name or SKU..." className="w-full pl-10 pr-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-primary-500/20" />
        </div>
        <div className="flex gap-2">
          {(["all", "low", "out"] as const).map((v) => (
            <button key={v} onClick={() => setStockFilter(v)} className={cn("px-3 py-2 rounded-lg text-sm font-medium transition", stockFilter === v ? "bg-primary-600 text-white" : "bg-gray-100 text-gray-600 hover:bg-gray-200")}>
              {v === "all" ? "All" : v === "low" ? "Low Stock" : "Out of Stock"}
            </button>
          ))}
        </div>
      </div>

      {/* Table */}
      {filtered.length === 0 ? (
        <div className="text-center py-16 bg-gray-50 rounded-xl border border-gray-100">
          <Package className="w-14 h-14 text-gray-200 mx-auto mb-3" />
          <p className="text-gray-600 font-semibold">No products found</p>
        </div>
      ) : (
        <div className="bg-white rounded-xl border border-gray-100 shadow-sm overflow-hidden">
          <div className="overflow-x-auto">
            <table className="w-full">
              <thead className="bg-gray-50 border-b border-gray-100">
                <tr>
                  <th className="px-4 py-3 text-left text-xs font-semibold text-gray-500 uppercase">Product</th>
                  <th className="px-4 py-3 text-left text-xs font-semibold text-gray-500 uppercase">SKU</th>
                  <th className="px-4 py-3 text-center text-xs font-semibold text-gray-500 uppercase">On Hand</th>
                  <th className="px-4 py-3 text-center text-xs font-semibold text-gray-500 uppercase">Reserved</th>
                  <th className="px-4 py-3 text-center text-xs font-semibold text-gray-500 uppercase">Available</th>
                  <th className="px-4 py-3 text-center text-xs font-semibold text-gray-500 uppercase">Threshold</th>
                  <th className="px-4 py-3 text-center text-xs font-semibold text-gray-500 uppercase">Status</th>
                  <th className="px-4 py-3 text-center text-xs font-semibold text-gray-500 uppercase">Actions</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-50">
                {filtered.map((product) => {
                  const inv       = inventoryMap[product.id];
                  const onHand    = inv?.quantityOnHand ?? 0;
                  const reserved  = inv?.reservedQuantity ?? 0;
                  const available = inv?.availableQuantity ?? 0;
                  const threshold = inv?.lowStockThreshold ?? 10;
                  const isOut     = available <= 0;
                  const isLow     = inv?.isLowStock && available > 0;
                  const imgSrc    = product.primaryImageUrl || `https://placehold.co/40x40/e2e8f0/64748b?text=${product.name.charAt(0)}`;

                  return (
                    <tr key={product.id} className={cn("hover:bg-gray-50 transition", isOut && "bg-red-50/30")}>
                      <td className="px-4 py-3">
                        <div className="flex items-center gap-3">
                          <img src={imgSrc} alt="" onError={(e) => { (e.target as HTMLImageElement).src = `https://placehold.co/40x40/e2e8f0/64748b?text=${product.name.charAt(0)}`; }} className="w-10 h-10 rounded-lg object-cover bg-gray-100 shrink-0" />
                          <div className="min-w-0">
                            <Link to={`/products/${product.id}`} className="text-sm font-medium text-gray-800 hover:text-primary-600 truncate block max-w-[180px]">{product.name}</Link>
                            <p className="text-xs text-gray-400">{product.brandName}</p>
                          </div>
                        </div>
                      </td>
                      <td className="px-4 py-3"><span className="text-xs font-mono text-gray-600 bg-gray-100 px-2 py-0.5 rounded">{product.sku}</span></td>
                      <td className="px-4 py-3 text-center"><span className="text-sm font-bold text-gray-900">{onHand}</span></td>
                      <td className="px-4 py-3 text-center"><span className="text-sm text-gray-500">{reserved}</span></td>
                      <td className="px-4 py-3 text-center">
                        <span className={cn("text-sm font-bold", isOut ? "text-red-600" : isLow ? "text-yellow-600" : "text-green-600")}>{available}</span>
                      </td>
                      <td className="px-4 py-3 text-center"><span className="text-sm text-gray-500">{threshold}</span></td>
                      <td className="px-4 py-3 text-center">
                        {isOut ? (
                          <span className="inline-flex items-center gap-1 px-2 py-0.5 bg-red-100 text-red-700 text-xs font-semibold rounded-full"><BoxSelect className="w-3 h-3" /> Out</span>
                        ) : isLow ? (
                          <span className="inline-flex items-center gap-1 px-2 py-0.5 bg-yellow-100 text-yellow-700 text-xs font-semibold rounded-full"><AlertTriangle className="w-3 h-3" /> Low</span>
                        ) : (
                          <span className="inline-flex items-center gap-1 px-2 py-0.5 bg-green-100 text-green-700 text-xs font-semibold rounded-full"><CheckCircle className="w-3 h-3" /> OK</span>
                        )}
                      </td>
                      <td className="px-4 py-3">
                        <div className="flex items-center justify-center gap-1">
                          <button onClick={() => openAdjust(product)} className="p-1.5 text-gray-400 hover:text-primary-600 hover:bg-primary-50 rounded-lg transition" title="Adjust"><ArrowUpDown className="w-4 h-4" /></button>
                          <button onClick={() => openHistory(product)} className="p-1.5 text-gray-400 hover:text-purple-600 hover:bg-purple-50 rounded-lg transition" title="History"><History className="w-4 h-4" /></button>
                        </div>
                      </td>
                    </tr>
                  );
                })}
              </tbody>
            </table>
          </div>
          <div className="px-4 py-3 bg-gray-50 border-t border-gray-100 text-sm text-gray-500">
            Showing {filtered.length} of {products.length} products
          </div>
        </div>
      )}

      {/* ── Adjust Modal ──────────────────────────────────────────────────────── */}
      {adjustProductId && (
        <>
          <div className="fixed inset-0 bg-black/50 z-40" onClick={() => setAdjustProductId(null)} />
          <div className="fixed inset-0 flex items-center justify-center z-50 p-4">
            <div className="bg-white rounded-2xl shadow-xl w-full max-w-md p-6">
              <div className="flex items-center justify-between mb-4">
                <h2 className="text-lg font-bold text-gray-900">Adjust Stock</h2>
                <button onClick={() => setAdjustProductId(null)} className="p-1 text-gray-400 hover:text-gray-600"><X className="w-5 h-5" /></button>
              </div>

              <p className="text-sm font-semibold text-gray-800 mb-1">{adjustProductName}</p>
              <p className="text-xs text-gray-500 mb-4">
                On hand: <span className="font-bold">{inventoryMap[adjustProductId]?.quantityOnHand ?? 0}</span> •
                Available: <span className="font-bold">{inventoryMap[adjustProductId]?.availableQuantity ?? 0}</span>
              </p>

              {/* Type */}
              <div className="flex gap-2 mb-4">
                <button onClick={() => setAdjustType("add")} className={cn("flex-1 flex items-center justify-center gap-2 px-4 py-3 rounded-xl border-2 text-sm font-medium transition", adjustType === "add" ? "border-green-500 bg-green-50 text-green-700" : "border-gray-200 text-gray-500")}>
                  <TrendingUp className="w-4 h-4" /> Add Stock
                </button>
                <button onClick={() => setAdjustType("remove")} className={cn("flex-1 flex items-center justify-center gap-2 px-4 py-3 rounded-xl border-2 text-sm font-medium transition", adjustType === "remove" ? "border-red-500 bg-red-50 text-red-700" : "border-gray-200 text-gray-500")}>
                  <TrendingDown className="w-4 h-4" /> Remove Stock
                </button>
              </div>

              {/* Quantity */}
              <div className="mb-4">
                <label className="block text-sm font-medium text-gray-700 mb-1.5">Quantity</label>
                <input type="number" value={adjustQuantity} onChange={(e) => setAdjustQuantity(e.target.value)} placeholder="Enter quantity" min="1" className="w-full px-3 py-2.5 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500/20 transition" />
              </div>

              {/* Reason */}
              <div className="mb-4">
                <label className="block text-sm font-medium text-gray-700 mb-1.5">Reason <span className="text-red-500">*</span></label>
                <select value={adjustReason} onChange={(e) => setAdjustReason(e.target.value)} className="w-full px-3 py-2.5 text-sm border border-gray-300 rounded-lg bg-white focus:outline-none focus:ring-2 focus:ring-primary-500/20 transition">
                  <option value="">Select reason</option>
                  {adjustType === "add" ? (
                    <><option value="Stock refill">Stock refill</option><option value="New shipment received">New shipment</option><option value="Return processed">Return processed</option><option value="Inventory correction">Correction</option></>
                  ) : (
                    <><option value="Damaged goods">Damaged goods</option><option value="Lost in warehouse">Lost</option><option value="Inventory correction">Correction</option><option value="Expired products">Expired</option></>
                  )}
                </select>
              </div>

              {/* Preview */}
              {adjustQuantity && Number(adjustQuantity) > 0 && (
                <div className={cn("p-3 rounded-xl mb-4 text-sm", adjustType === "add" ? "bg-green-50 border border-green-200" : "bg-red-50 border border-red-200")}>
                  <p className={cn("font-medium", adjustType === "add" ? "text-green-700" : "text-red-700")}>
                    {adjustType === "add" ? "+" : "-"}{adjustQuantity} units →{" "}
                    New on hand: <span className="font-bold">{(inventoryMap[adjustProductId]?.quantityOnHand ?? 0) + (adjustType === "add" ? Number(adjustQuantity) : -Number(adjustQuantity))}</span>
                  </p>
                </div>
              )}

              <div className="flex gap-3">
                <Button onClick={handleAdjust} isLoading={adjusting} className="flex-1" variant={adjustType === "add" ? "primary" : "danger"}>
                  <Save className="w-4 h-4" /> {adjustType === "add" ? "Add Stock" : "Remove Stock"}
                </Button>
                <Button variant="outline" onClick={() => setAdjustProductId(null)}>Cancel</Button>
              </div>
            </div>
          </div>
        </>
      )}

      {/* ── History Modal ─────────────────────────────────────────────────────── */}
      {historyProductId && (
        <>
          <div className="fixed inset-0 bg-black/50 z-40" onClick={() => setHistoryProductId(null)} />
          <div className="fixed inset-0 flex items-center justify-center z-50 p-4">
            <div className="bg-white rounded-2xl shadow-xl w-full max-w-lg p-6 max-h-[80vh] overflow-y-auto">
              <div className="flex items-center justify-between mb-6">
                <div>
                  <h2 className="text-lg font-bold text-gray-900">Stock History</h2>
                  <p className="text-sm text-gray-500">{historyProductName}</p>
                </div>
                <button onClick={() => setHistoryProductId(null)} className="p-1 text-gray-400 hover:text-gray-600"><X className="w-5 h-5" /></button>
              </div>

              {historyLoading ? (
                <div className="flex justify-center py-8"><Spinner className="text-primary-600" /></div>
              ) : history.length === 0 ? (
                <div className="text-center py-8 bg-gray-50 rounded-xl">
                  <History className="w-12 h-12 text-gray-200 mx-auto mb-3" />
                  <p className="text-gray-600 font-medium">No history found</p>
                </div>
              ) : (
                <div className="space-y-3">
                  {history.map((txn, idx) => {
                    const isPositive = txn.quantity > 0;
                    const typeLabel  = TransactionTypeLabels[txn.transactionType] || `Type ${txn.transactionType}`;
                    const typeColor  = TransactionTypeColors[txn.transactionType] || "bg-gray-100 text-gray-700";

                    return (
                      <div key={txn.id || idx} className={cn("p-4 rounded-xl border", isPositive ? "border-green-100 bg-green-50/50" : "border-red-100 bg-red-50/50")}>
                        <div className="flex items-center justify-between mb-2">
                          <div className="flex items-center gap-2">
                            {isPositive ? <TrendingUp className="w-4 h-4 text-green-600" /> : <TrendingDown className="w-4 h-4 text-red-600" />}
                            <span className={cn("text-sm font-bold", isPositive ? "text-green-700" : "text-red-700")}>
                              {isPositive ? "+" : ""}{txn.quantity} units
                            </span>
                          </div>
                          <span className={cn("px-2 py-0.5 text-xs font-semibold rounded-full", typeColor)}>
                            {typeLabel}
                          </span>
                        </div>
                        {txn.reason && <p className="text-xs text-gray-600 mb-1">📝 {txn.reason}</p>}
                        {txn.reference && <p className="text-xs text-gray-400 font-mono">Ref: {txn.reference}</p>}
                        <div className="flex items-center justify-between mt-2 text-xs text-gray-400">
                          <span>By: {txn.createdBy || "System"}</span>
                          <span>
                            {new Date(txn.createdAtUtc).toLocaleDateString("en-IN", {
                              month: "short", day: "numeric", year: "numeric",
                              hour: "2-digit", minute: "2-digit",
                            })}
                          </span>
                        </div>
                      </div>
                    );
                  })}
                </div>
              )}

              <button onClick={() => setHistoryProductId(null)} className="w-full mt-4 px-4 py-2 border border-gray-200 text-gray-700 text-sm font-medium rounded-lg hover:bg-gray-50 transition">Close</button>
            </div>
          </div>
        </>
      )}
    </div>
  );
};

export default AdminInventory;