import { useEffect, useState } from "react";
import {
  Award,
  Plus,
  Search,
  Edit,
  Trash2,
  CheckCircle,
  XCircle,
  Save,
  X,
} from "lucide-react";
import { brandApi } from "@/api/brand.api";
import type { Brand } from "@/types";
import { cn } from "@/utils/cn";
import Spinner from "@/components/ui/Spinner";
import Button from "@/components/ui/Button";
import Input from "@/components/ui/Input";
import toast from "react-hot-toast";

const AdminBrands = () => {
  const [brands, setBrands]           = useState<Brand[]>([]);
  const [isLoading, setIsLoading]     = useState(true);
  const [searchQuery, setSearchQuery] = useState("");

  const [showForm, setShowForm] = useState(false);
  const [editId, setEditId]     = useState<string | null>(null);
  const [formName, setFormName] = useState("");
  const [formDesc, setFormDesc] = useState("");
  const [saving, setSaving]     = useState(false);

  const loadBrands = async () => {
    try {
      const data = await brandApi.getAll();
      setBrands(data);
    } catch (err) {
      toast.error((err as Error).message || "Failed to load brands");
    }
  };

  useEffect(() => {
    const load = async () => {
      setIsLoading(true);
      await loadBrands();
      setIsLoading(false);
    };
    load();
  }, []);

  const openAddForm = () => {
    setEditId(null);
    setFormName("");
    setFormDesc("");
    setShowForm(true);
  };

  const openEditForm = (brand: Brand) => {
    setEditId(brand.id);
    setFormName(brand.name);
    setFormDesc(brand.description || "");
    setShowForm(true);
  };

  const closeForm = () => {
    setShowForm(false);
    setEditId(null);
    setFormName("");
    setFormDesc("");
  };

  const handleSave = async () => {
    if (!formName.trim()) {
      toast.error("Brand name is required");
      return;
    }
    setSaving(true);
    try {
      if (editId) {
        await brandApi.update(editId, {
          id:          editId,
          name:        formName.trim(),
          description: formDesc.trim() || undefined,
        });
        toast.success("Brand updated!");
      } else {
        await brandApi.create({
          name:        formName.trim(),
          description: formDesc.trim() || undefined,
        });
        toast.success("Brand created!");
      }
      closeForm();
      await loadBrands();
    } catch (err) {
      toast.error((err as Error).message || "Failed to save");
    } finally {
      setSaving(false);
    }
  };

  const handleToggleStatus = async (brand: Brand) => {
    try {
      await brandApi.changeStatus(brand.id, { isActive: !brand.isActive });
      toast.success(`${brand.name} ${!brand.isActive ? "activated" : "deactivated"}`);
      await loadBrands();
    } catch (err) {
      toast.error((err as Error).message || "Failed");
    }
  };

  const handleDelete = async (brand: Brand) => {
    if (!window.confirm(`Delete "${brand.name}"?`)) return;
    try {
      await brandApi.delete(brand.id);
      toast.success(`${brand.name} deleted`);
      await loadBrands();
    } catch (err) {
      toast.error((err as Error).message || "Failed to delete");
    }
  };

  const filtered = brands.filter(
    (b) =>
      !searchQuery ||
      b.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
      b.description?.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const activeCount   = brands.filter((b) => b.isActive).length;
  const inactiveCount = brands.length - activeCount;

  if (isLoading) {
    return (
      <div className="flex justify-center py-32">
        <Spinner size="lg" className="text-primary-600" />
      </div>
    );
  }

  return (
    <div className="max-w-5xl mx-auto px-4 py-8">

      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 flex items-center gap-3">
            <Award className="w-8 h-8 text-primary-600" />
            Manage Brands
          </h1>
          <p className="text-gray-500 mt-1">
            {brands.length} brands • {activeCount} active • {inactiveCount} inactive
          </p>
        </div>
        <Button onClick={openAddForm}>
          <Plus className="w-4 h-4" /> Add Brand
        </Button>
      </div>

      <div className="relative mb-6">
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
        <input type="text" value={searchQuery} onChange={(e) => setSearchQuery(e.target.value)} placeholder="Search brands..." className="w-full pl-10 pr-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-primary-500/20 focus:border-primary-400 transition" />
      </div>

      {/* ── Form Modal ──────────────────────────────────────────────────────── */}
      {showForm && (
        <>
          <div className="fixed inset-0 bg-black/50 z-40" onClick={closeForm} />
          <div className="fixed inset-0 flex items-center justify-center z-50 p-4">
            <div className="bg-white rounded-2xl shadow-xl w-full max-w-md p-6">
              <div className="flex items-center justify-between mb-6">
                <h2 className="text-lg font-bold text-gray-900">{editId ? "Edit Brand" : "Add Brand"}</h2>
                <button onClick={closeForm} className="p-1 text-gray-400 hover:text-gray-600"><X className="w-5 h-5" /></button>
              </div>
              <div className="space-y-4">
                <Input label="Brand Name" value={formName} onChange={(e) => setFormName(e.target.value)} placeholder="Apple" />
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1.5">Description (optional)</label>
                  <textarea value={formDesc} onChange={(e) => setFormDesc(e.target.value)} placeholder="Describe this brand..." rows={3} className="w-full px-3 py-2.5 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500/20 focus:border-primary-500 resize-none transition" />
                </div>
                <div className="flex gap-3 pt-2">
                  <Button onClick={handleSave} isLoading={saving} className="flex-1">
                    <Save className="w-4 h-4" /> {editId ? "Update" : "Create"}
                  </Button>
                  <Button variant="outline" onClick={closeForm}>Cancel</Button>
                </div>
              </div>
            </div>
          </div>
        </>
      )}

      {/* ── Brands Grid ─────────────────────────────────────────────────────── */}
      {filtered.length === 0 ? (
        <div className="text-center py-16 bg-gray-50 rounded-xl border border-gray-100">
          <Award className="w-14 h-14 text-gray-200 mx-auto mb-3" />
          <p className="text-gray-600 font-semibold">No brands found</p>
        </div>
      ) : (
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4">
          {filtered.map((brand) => (
            <div key={brand.id} className={cn("bg-white rounded-xl border shadow-sm p-5 hover:shadow-md transition relative group", brand.isActive ? "border-gray-100" : "border-red-100 bg-red-50/30")}>
              <div className="flex items-start justify-between mb-3">
                <div className="flex items-center gap-3">
                  <div className="w-10 h-10 bg-gradient-to-br from-primary-500 to-primary-700 text-white rounded-xl flex items-center justify-center text-lg font-bold">
                    {brand.name.charAt(0)}
                  </div>
                  <div>
                    <h3 className="text-sm font-bold text-gray-800">{brand.name}</h3>
                    <button
                      onClick={() => handleToggleStatus(brand)}
                      className={cn("inline-flex items-center gap-1 px-2 py-0.5 rounded-full text-xs font-semibold mt-1 transition cursor-pointer", brand.isActive ? "bg-green-100 text-green-700 hover:bg-green-200" : "bg-red-100 text-red-700 hover:bg-red-200")}
                    >
                      {brand.isActive ? <><CheckCircle className="w-3 h-3" /> Active</> : <><XCircle className="w-3 h-3" /> Inactive</>}
                    </button>
                  </div>
                </div>
                <div className="flex items-center gap-0.5 opacity-0 group-hover:opacity-100 transition">
                  <button onClick={() => openEditForm(brand)} className="p-1.5 text-gray-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition"><Edit className="w-4 h-4" /></button>
                  <button onClick={() => handleDelete(brand)} className="p-1.5 text-gray-400 hover:text-red-600 hover:bg-red-50 rounded-lg transition"><Trash2 className="w-4 h-4" /></button>
                </div>
              </div>
              <p className="text-xs text-gray-500 line-clamp-2">{brand.description || "No description"}</p>
            </div>
          ))}
        </div>
      )}

      <p className="text-center text-sm text-gray-400 mt-6">
        {filtered.length} of {brands.length} brands
      </p>
    </div>
  );
};

export default AdminBrands;