import { useEffect, useState } from "react";
import {
  Tag,
  Plus,
  Search,
  Edit,
  Trash2,
  CheckCircle,
  XCircle,
  Save,
  X,
  Calendar,
  Percent,
  IndianRupee,
  Copy,
} from "lucide-react";
import axiosInstance from "@/api/axiosInstance";
import type { Coupon } from "@/types";
import { formatPrice } from "@/utils/formatPrice";
import { cn } from "@/utils/cn";
import Spinner from "@/components/ui/Spinner";
import Button from "@/components/ui/Button";
import Input from "@/components/ui/Input";
import toast from "react-hot-toast";

// ── Coupon API ───────────────────────────────────────────────────────────────
const couponApi = {
  getAll: async (): Promise<Coupon[]> => {
    const res = await axiosInstance.get("/coupons");
    const data = res.data?.data ?? res.data?.value ?? res.data;
    if (Array.isArray(data)) return data;
    if (Array.isArray(data?.items)) return data.items;
    return [];
  },

  getById: async (id: string): Promise<Coupon> => {
    const res = await axiosInstance.get(`/coupons/${id}`);
    return res.data?.data ?? res.data?.value ?? res.data;
  },

  create: async (data: any): Promise<Coupon> => {
    const res = await axiosInstance.post("/coupons", data);
    return res.data?.data ?? res.data?.value ?? res.data;
  },

  update: async (id: string, data: any): Promise<Coupon> => {
    const res = await axiosInstance.put(`/coupons/${id}`, data);
    return res.data?.data ?? res.data?.value ?? res.data;
  },

  delete: async (id: string): Promise<void> => {
    await axiosInstance.delete(`/coupons/${id}`);
  },
};

// ── Discount Type Labels ─────────────────────────────────────────────────────
const DiscountTypeLabels: Record<number, string> = {
  1: "Percentage",
  2: "Fixed Amount",
};

const AdminCoupons = () => {
  const [coupons, setCoupons]         = useState<Coupon[]>([]);
  const [isLoading, setIsLoading]     = useState(true);
  const [searchQuery, setSearchQuery] = useState("");
  const [statusFilter, setStatusFilter] = useState<"all" | "active" | "expired">("all");

  // ── Form State ─────────────────────────────────────────────────────────────
  const [showForm, setShowForm]       = useState(false);
  const [editId, setEditId]           = useState<string | null>(null);
  const [saving, setSaving]           = useState(false);

  const [formCode, setFormCode]                   = useState("");
  const [formName, setFormName]                   = useState("");
  const [formDescription, setFormDescription]     = useState("");
  const [formDiscountType, setFormDiscountType]   = useState(1);
  const [formDiscountValue, setFormDiscountValue] = useState("");
  const [formMinOrder, setFormMinOrder]           = useState("");
  const [formMaxDiscount, setFormMaxDiscount]     = useState("");
  const [formStartDate, setFormStartDate]         = useState("");
  const [formEndDate, setFormEndDate]             = useState("");
  const [formUsageLimit, setFormUsageLimit]       = useState("");
  const [formIsActive, setFormIsActive]           = useState(true);

  // ── Load ───────────────────────────────────────────────────────────────────
  const loadCoupons = async () => {
    try {
      const data = await couponApi.getAll();
      setCoupons(data);
    } catch (err) {
      toast.error((err as Error).message || "Failed to load coupons");
    }
  };

  useEffect(() => {
    const load = async () => {
      setIsLoading(true);
      await loadCoupons();
      setIsLoading(false);
    };
    load();
  }, []);

  // ── Form Helpers ───────────────────────────────────────────────────────────
  const resetForm = () => {
    setEditId(null);
    setFormCode("");
    setFormName("");
    setFormDescription("");
    setFormDiscountType(1);
    setFormDiscountValue("");
    setFormMinOrder("");
    setFormMaxDiscount("");
    setFormStartDate("");
    setFormEndDate("");
    setFormUsageLimit("");
    setFormIsActive(true);
  };

  const openAddForm = () => {
    resetForm();
    setShowForm(true);
  };

  const openEditForm = (coupon: Coupon) => {
    setEditId(coupon.id);
    setFormCode(coupon.code);
    setFormName(coupon.name);
    setFormDescription(coupon.description || "");
    setFormDiscountType(coupon.discountType);
    setFormDiscountValue(String(coupon.discountValue));
    setFormMinOrder(String(coupon.minimumOrderAmount));
    setFormMaxDiscount(coupon.maximumDiscountAmount ? String(coupon.maximumDiscountAmount) : "");
    setFormStartDate(coupon.startsAtUtc?.slice(0, 16) || "");
    setFormEndDate(coupon.expiresAtUtc?.slice(0, 16) || "");
    setFormUsageLimit(String(coupon.usageLimit));
    setFormIsActive(coupon.isActive);
    setShowForm(true);
  };

  const closeForm = () => {
    setShowForm(false);
    resetForm();
  };

  // ── Save ───────────────────────────────────────────────────────────────────
  const handleSave = async () => {
    if (!formCode.trim() || !formName.trim()) {
      toast.error("Code and name are required");
      return;
    }
    if (!formDiscountValue || Number(formDiscountValue) <= 0) {
      toast.error("Discount value must be greater than 0");
      return;
    }
    if (!formStartDate || !formEndDate) {
      toast.error("Start and end dates are required");
      return;
    }

    const payload = {
      code:                 formCode.trim().toUpperCase(),
      name:                 formName.trim(),
      description:          formDescription.trim() || undefined,
      discountType:         formDiscountType,
      discountValue:        Number(formDiscountValue),
      minimumOrderAmount:   Number(formMinOrder) || 0,
      maximumDiscountAmount: formMaxDiscount ? Number(formMaxDiscount) : undefined,
      startsAtUtc:          new Date(formStartDate).toISOString(),
      expiresAtUtc:         new Date(formEndDate).toISOString(),
      usageLimit:           Number(formUsageLimit) || 100,
      ...(editId && { id: editId, isActive: formIsActive }),
    };

    setSaving(true);
    try {
      if (editId) {
        await couponApi.update(editId, payload);
        toast.success("Coupon updated!");
      } else {
        await couponApi.create(payload);
        toast.success("Coupon created!");
      }
      closeForm();
      await loadCoupons();
    } catch (err) {
      toast.error((err as Error).message || "Failed to save");
    } finally {
      setSaving(false);
    }
  };

  // ── Delete ─────────────────────────────────────────────────────────────────
  const handleDelete = async (coupon: Coupon) => {
    if (!window.confirm(`Delete coupon "${coupon.code}"?`)) return;
    try {
      await couponApi.delete(coupon.id);
      toast.success("Coupon deleted");
      await loadCoupons();
    } catch (err) {
      toast.error((err as Error).message || "Failed");
    }
  };

  // ── Copy Code ──────────────────────────────────────────────────────────────
  const copyCode = (code: string) => {
    navigator.clipboard.writeText(code);
    toast.success(`Copied: ${code}`);
  };

  // ── Filter ─────────────────────────────────────────────────────────────────
  const now = new Date();
  const filtered = coupons.filter((c) => {
    const matchSearch =
      !searchQuery ||
      c.code.toLowerCase().includes(searchQuery.toLowerCase()) ||
      c.name.toLowerCase().includes(searchQuery.toLowerCase());

    const isExpired = new Date(c.expiresAtUtc) < now;
    const matchStatus =
      statusFilter === "all" ||
      (statusFilter === "active" && c.isActive && !isExpired) ||
      (statusFilter === "expired" && (isExpired || !c.isActive));

    return matchSearch && matchStatus;
  });

  const activeCoupons  = coupons.filter((c) => c.isActive && new Date(c.expiresAtUtc) >= now).length;
  const expiredCoupons = coupons.length - activeCoupons;

  if (isLoading) {
    return (
      <div className="flex justify-center py-32">
        <Spinner size="lg" className="text-primary-600" />
      </div>
    );
  }

  return (
    <div className="max-w-6xl mx-auto px-4 py-8">

      {/* ── Header ──────────────────────────────────────────────────────────── */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 flex items-center gap-3">
            <Tag className="w-8 h-8 text-primary-600" />
            Manage Coupons
          </h1>
          <p className="text-gray-500 mt-1">
            {coupons.length} coupons • {activeCoupons} active • {expiredCoupons} expired
          </p>
        </div>
        <Button onClick={openAddForm}>
          <Plus className="w-4 h-4" /> Create Coupon
        </Button>
      </div>

      {/* ── Stats ───────────────────────────────────────────────────────────── */}
      <div className="grid grid-cols-3 gap-4 mb-6">
        {[
          { label: "Total",   value: coupons.length, color: "bg-blue-50 text-blue-700",  filter: "all" as const },
          { label: "Active",  value: activeCoupons,  color: "bg-green-50 text-green-700", filter: "active" as const },
          { label: "Expired", value: expiredCoupons, color: "bg-red-50 text-red-700",    filter: "expired" as const },
        ].map(({ label, value, color, filter }) => (
          <button
            key={label}
            onClick={() => setStatusFilter(filter)}
            className={cn(
              "bg-white rounded-xl border border-gray-100 shadow-sm p-4 flex items-center gap-3 hover:shadow-md transition text-left",
              statusFilter === filter && "ring-2 ring-primary-500"
            )}
          >
            <div className={cn("p-2 rounded-lg", color)}>
              <Tag className="w-4 h-4" />
            </div>
            <div>
              <p className="text-xl font-bold text-gray-900">{value}</p>
              <p className="text-xs text-gray-500">{label}</p>
            </div>
          </button>
        ))}
      </div>

      {/* ── Search ──────────────────────────────────────────────────────────── */}
      <div className="relative mb-6">
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
        <input
          type="text"
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          placeholder="Search by code or name..."
          className="w-full pl-10 pr-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-primary-500/20"
        />
      </div>

      {/* ── Form Modal ──────────────────────────────────────────────────────── */}
      {showForm && (
        <>
          <div className="fixed inset-0 bg-black/50 z-40" onClick={closeForm} />
          <div className="fixed inset-0 flex items-center justify-center z-50 p-4">
            <div className="bg-white rounded-2xl shadow-xl w-full max-w-lg p-6 max-h-[90vh] overflow-y-auto">
              <div className="flex items-center justify-between mb-6">
                <h2 className="text-lg font-bold text-gray-900">
                  {editId ? "Edit Coupon" : "Create Coupon"}
                </h2>
                <button onClick={closeForm} className="p-1 text-gray-400 hover:text-gray-600">
                  <X className="w-5 h-5" />
                </button>
              </div>

              <div className="space-y-4">
                {/* Code & Name */}
                <div className="grid grid-cols-2 gap-4">
                  <Input
                    label="Coupon Code"
                    value={formCode}
                    onChange={(e) => setFormCode(e.target.value.toUpperCase())}
                    placeholder="SAVE20"
                  />
                  <Input
                    label="Coupon Name"
                    value={formName}
                    onChange={(e) => setFormName(e.target.value)}
                    placeholder="Summer Sale"
                  />
                </div>

                {/* Description */}
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1.5">Description</label>
                  <textarea
                    value={formDescription}
                    onChange={(e) => setFormDescription(e.target.value)}
                    placeholder="Get 20% off on all products"
                    rows={2}
                    className="w-full px-3 py-2.5 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500/20 resize-none transition"
                  />
                </div>

                {/* Discount Type & Value */}
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1.5">Discount Type</label>
                    <select
                      value={formDiscountType}
                      onChange={(e) => setFormDiscountType(Number(e.target.value))}
                      className="w-full px-3 py-2.5 text-sm border border-gray-300 rounded-lg bg-white focus:outline-none focus:ring-2 focus:ring-primary-500/20 transition"
                    >
                      <option value={1}>Percentage (%)</option>
                      <option value={2}>Fixed Amount (₹)</option>
                    </select>
                  </div>
                  <Input
                    label={formDiscountType === 1 ? "Discount (%)" : "Discount (₹)"}
                    type="number"
                    value={formDiscountValue}
                    onChange={(e) => setFormDiscountValue(e.target.value)}
                    placeholder={formDiscountType === 1 ? "20" : "500"}
                  />
                </div>

                {/* Min Order & Max Discount */}
                <div className="grid grid-cols-2 gap-4">
                  <Input
                    label="Min Order Amount (₹)"
                    type="number"
                    value={formMinOrder}
                    onChange={(e) => setFormMinOrder(e.target.value)}
                    placeholder="1000"
                  />
                  <Input
                    label="Max Discount (₹)"
                    type="number"
                    value={formMaxDiscount}
                    onChange={(e) => setFormMaxDiscount(e.target.value)}
                    placeholder="500"
                    helperText="Optional"
                  />
                </div>

                {/* Dates */}
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1.5">Start Date</label>
                    <input
                      type="datetime-local"
                      value={formStartDate}
                      onChange={(e) => setFormStartDate(e.target.value)}
                      className="w-full px-3 py-2.5 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500/20 transition"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1.5">End Date</label>
                    <input
                      type="datetime-local"
                      value={formEndDate}
                      onChange={(e) => setFormEndDate(e.target.value)}
                      className="w-full px-3 py-2.5 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500/20 transition"
                    />
                  </div>
                </div>

                {/* Usage Limit & Active */}
                <div className="grid grid-cols-2 gap-4">
                  <Input
                    label="Usage Limit"
                    type="number"
                    value={formUsageLimit}
                    onChange={(e) => setFormUsageLimit(e.target.value)}
                    placeholder="100"
                  />
                  {editId && (
                    <div>
                      <label className="block text-sm font-medium text-gray-700 mb-1.5">Status</label>
                      <select
                        value={formIsActive ? "true" : "false"}
                        onChange={(e) => setFormIsActive(e.target.value === "true")}
                        className="w-full px-3 py-2.5 text-sm border border-gray-300 rounded-lg bg-white focus:outline-none focus:ring-2 focus:ring-primary-500/20 transition"
                      >
                        <option value="true">Active</option>
                        <option value="false">Inactive</option>
                      </select>
                    </div>
                  )}
                </div>

                {/* Actions */}
                <div className="flex gap-3 pt-2">
                  <Button onClick={handleSave} isLoading={saving} className="flex-1">
                    <Save className="w-4 h-4" /> {editId ? "Update Coupon" : "Create Coupon"}
                  </Button>
                  <Button variant="outline" onClick={closeForm}>Cancel</Button>
                </div>
              </div>
            </div>
          </div>
        </>
      )}

      {/* ── Coupons List ────────────────────────────────────────────────────── */}
      {filtered.length === 0 ? (
        <div className="text-center py-16 bg-gray-50 rounded-xl border border-gray-100">
          <Tag className="w-14 h-14 text-gray-200 mx-auto mb-3" />
          <p className="text-gray-600 font-semibold">No coupons found</p>
          <p className="text-sm text-gray-400 mt-1">Create your first coupon</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          {filtered.map((coupon) => {
            const isExpired   = new Date(coupon.expiresAtUtc) < now;
            const isNotStarted = new Date(coupon.startsAtUtc) > now;
            const isActive    = coupon.isActive && !isExpired && !isNotStarted;

            return (
              <div
                key={coupon.id}
                className={cn(
                  "bg-white rounded-xl border shadow-sm overflow-hidden hover:shadow-md transition",
                  isActive ? "border-gray-100" : "border-red-100 opacity-80"
                )}
              >
                {/* Top Bar */}
                <div className={cn(
                  "px-5 py-3 flex items-center justify-between",
                  isActive ? "bg-primary-50" : "bg-red-50"
                )}>
                  <div className="flex items-center gap-2">
                    <button
                      onClick={() => copyCode(coupon.code)}
                      className="flex items-center gap-1.5 text-sm font-bold font-mono bg-white px-3 py-1 rounded-lg border border-gray-200 hover:border-primary-400 transition"
                    >
                      {coupon.code}
                      <Copy className="w-3 h-3 text-gray-400" />
                    </button>
                    {isActive && (
                      <span className="px-2 py-0.5 bg-green-100 text-green-700 text-xs font-semibold rounded-full flex items-center gap-1">
                        <CheckCircle className="w-3 h-3" /> Active
                      </span>
                    )}
                    {isExpired && (
                      <span className="px-2 py-0.5 bg-red-100 text-red-700 text-xs font-semibold rounded-full flex items-center gap-1">
                        <XCircle className="w-3 h-3" /> Expired
                      </span>
                    )}
                    {isNotStarted && (
                      <span className="px-2 py-0.5 bg-yellow-100 text-yellow-700 text-xs font-semibold rounded-full">
                        Scheduled
                      </span>
                    )}
                    {!coupon.isActive && !isExpired && (
                      <span className="px-2 py-0.5 bg-gray-100 text-gray-600 text-xs font-semibold rounded-full">
                        Disabled
                      </span>
                    )}
                  </div>

                  {/* Actions */}
                  <div className="flex items-center gap-1">
                    <button
                      onClick={() => openEditForm(coupon)}
                      className="p-1.5 text-gray-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition"
                    >
                      <Edit className="w-4 h-4" />
                    </button>
                    <button
                      onClick={() => handleDelete(coupon)}
                      className="p-1.5 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition"
                    >
                      <Trash2 className="w-4 h-4" />
                    </button>
                  </div>
                </div>

                {/* Content */}
                <div className="px-5 py-4">
                  <h3 className="text-sm font-bold text-gray-800 mb-1">{coupon.name}</h3>
                  {coupon.description && (
                    <p className="text-xs text-gray-500 mb-3">{coupon.description}</p>
                  )}

                  {/* Discount */}
                  <div className="flex items-center gap-4 mb-3">
                    <div className="flex items-center gap-1">
                      {coupon.discountType === 1 ? (
                        <Percent className="w-4 h-4 text-primary-600" />
                      ) : (
                        <IndianRupee className="w-4 h-4 text-primary-600" />
                      )}
                      <span className="text-lg font-bold text-primary-600">
                        {coupon.discountType === 1
                          ? `${coupon.discountValue}%`
                          : formatPrice(coupon.discountValue)}
                      </span>
                      <span className="text-xs text-gray-500">off</span>
                    </div>
                    {coupon.maximumDiscountAmount && (
                      <span className="text-xs text-gray-400">
                        Max: {formatPrice(coupon.maximumDiscountAmount)}
                      </span>
                    )}
                  </div>

                  {/* Details */}
                  <div className="grid grid-cols-2 gap-2 text-xs">
                    <div className="flex items-center gap-1 text-gray-500">
                      <IndianRupee className="w-3 h-3" />
                      Min: {formatPrice(coupon.minimumOrderAmount)}
                    </div>
                    <div className="flex items-center gap-1 text-gray-500">
                      Uses: {coupon.usageLimit}
                    </div>
                    <div className="flex items-center gap-1 text-gray-500">
                      <Calendar className="w-3 h-3" />
                      {new Date(coupon.startsAtUtc).toLocaleDateString("en-IN", { month: "short", day: "numeric" })}
                    </div>
                    <div className="flex items-center gap-1 text-gray-500">
                      <Calendar className="w-3 h-3" />
                      {new Date(coupon.expiresAtUtc).toLocaleDateString("en-IN", { month: "short", day: "numeric", year: "numeric" })}
                    </div>
                  </div>
                </div>
              </div>
            );
          })}
        </div>
      )}

      <p className="text-center text-sm text-gray-400 mt-6">
        {filtered.length} of {coupons.length} coupons
      </p>
    </div>
  );
};

export default AdminCoupons;