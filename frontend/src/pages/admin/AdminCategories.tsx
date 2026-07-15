import { useEffect, useState } from "react";
import {
  FolderTree,
  Plus,
  Search,
  Edit,
  Trash2,
  CheckCircle,
  XCircle,
  Save,
  X,
} from "lucide-react";
import { categoryApi } from "@/api/category.api";
import type { Category } from "@/types";
import { cn } from "@/utils/cn";
import Spinner from "@/components/ui/Spinner";
import Button from "@/components/ui/Button";
import Input from "@/components/ui/Input";
import toast from "react-hot-toast";

const AdminCategories = () => {
  const [categories, setCategories] = useState<Category[]>([]);
  const [isLoading, setIsLoading]   = useState(true);
  const [searchQuery, setSearchQuery] = useState("");

  // ── Form State ─────────────────────────────────────────────────────────────
  const [showForm, setShowForm]     = useState(false);
  const [editId, setEditId]         = useState<string | null>(null);
  const [formName, setFormName]     = useState("");
  const [formDesc, setFormDesc]     = useState("");
  const [formParent, setFormParent] = useState("");
  const [saving, setSaving]         = useState(false);

  // ── Load ───────────────────────────────────────────────────────────────────
  const loadCategories = async () => {
    try {
      const data = await categoryApi.getAll();
      setCategories(data);
    } catch (err) {
      toast.error((err as Error).message || "Failed to load categories");
    }
  };

  useEffect(() => {
    const load = async () => {
      setIsLoading(true);
      await loadCategories();
      setIsLoading(false);
    };
    load();
  }, []);

  // ── Open Form ──────────────────────────────────────────────────────────────
  const openAddForm = () => {
    setEditId(null);
    setFormName("");
    setFormDesc("");
    setFormParent("");
    setShowForm(true);
  };

  const openEditForm = (cat: Category) => {
    setEditId(cat.id);
    setFormName(cat.name);
    setFormDesc(cat.description || "");
    setFormParent(cat.parentCategoryId || "");
    setShowForm(true);
  };

  const closeForm = () => {
    setShowForm(false);
    setEditId(null);
    setFormName("");
    setFormDesc("");
    setFormParent("");
  };

  // ── Save ───────────────────────────────────────────────────────────────────
  const handleSave = async () => {
    if (!formName.trim()) {
      toast.error("Category name is required");
      return;
    }

    setSaving(true);
    try {
      if (editId) {
        await categoryApi.update(editId, {
          id:               editId,
          name:             formName.trim(),
          description:      formDesc.trim() || undefined,
          parentCategoryId: formParent || undefined,
        });
        toast.success("Category updated!");
      } else {
        await categoryApi.create({
          name:             formName.trim(),
          description:      formDesc.trim() || undefined,
          parentCategoryId: formParent || undefined,
        });
        toast.success("Category created!");
      }
      closeForm();
      await loadCategories();
    } catch (err) {
      toast.error((err as Error).message || "Failed to save");
    } finally {
      setSaving(false);
    }
  };

  // ── Toggle Status ──────────────────────────────────────────────────────────
  const handleToggleStatus = async (cat: Category) => {
    try {
      await categoryApi.changeStatus(cat.id, { isActive: !cat.isActive });
      toast.success(`${cat.name} ${!cat.isActive ? "activated" : "deactivated"}`);
      await loadCategories();
    } catch (err) {
      toast.error((err as Error).message || "Failed");
    }
  };

  // ── Delete ─────────────────────────────────────────────────────────────────
  const handleDelete = async (cat: Category) => {
    if (!window.confirm(`Delete "${cat.name}"? This cannot be undone.`)) return;
    try {
      await categoryApi.delete(cat.id);
      toast.success(`${cat.name} deleted`);
      await loadCategories();
    } catch (err) {
      toast.error((err as Error).message || "Failed to delete");
    }
  };

  // ── Filter ─────────────────────────────────────────────────────────────────
  const filtered = categories.filter(
    (c) =>
      !searchQuery ||
      c.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
      c.description?.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const activeCount   = categories.filter((c) => c.isActive).length;
  const inactiveCount = categories.length - activeCount;

  if (isLoading) {
    return (
      <div className="flex justify-center py-32">
        <Spinner size="lg" className="text-primary-600" />
      </div>
    );
  }

  return (
    <div className="max-w-5xl mx-auto px-4 py-8">

      {/* ── Header ──────────────────────────────────────────────────────────── */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-8">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 flex items-center gap-3">
            <FolderTree className="w-8 h-8 text-primary-600" />
            Manage Categories
          </h1>
          <p className="text-gray-500 mt-1">
            {categories.length} categories • {activeCount} active • {inactiveCount} inactive
          </p>
        </div>
        <Button onClick={openAddForm}>
          <Plus className="w-4 h-4" /> Add Category
        </Button>
      </div>

      {/* ── Search ──────────────────────────────────────────────────────────── */}
      <div className="relative mb-6">
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" />
        <input
          type="text"
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          placeholder="Search categories..."
          className="w-full pl-10 pr-4 py-2.5 border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-primary-500/20 focus:border-primary-400 transition"
        />
      </div>

      {/* ── Add/Edit Form Modal ─────────────────────────────────────────────── */}
      {showForm && (
        <>
          <div className="fixed inset-0 bg-black/50 z-40" onClick={closeForm} />
          <div className="fixed inset-0 flex items-center justify-center z-50 p-4">
            <div className="bg-white rounded-2xl shadow-xl w-full max-w-md p-6">
              <div className="flex items-center justify-between mb-6">
                <h2 className="text-lg font-bold text-gray-900">
                  {editId ? "Edit Category" : "Add Category"}
                </h2>
                <button onClick={closeForm} className="p-1 text-gray-400 hover:text-gray-600 transition">
                  <X className="w-5 h-5" />
                </button>
              </div>

              <div className="space-y-4">
                <Input
                  label="Category Name"
                  value={formName}
                  onChange={(e) => setFormName(e.target.value)}
                  placeholder="Electronics"
                />

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1.5">
                    Description (optional)
                  </label>
                  <textarea
                    value={formDesc}
                    onChange={(e) => setFormDesc(e.target.value)}
                    placeholder="Describe this category..."
                    rows={3}
                    className="w-full px-3 py-2.5 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500/20 focus:border-primary-500 resize-none transition"
                  />
                </div>

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1.5">
                    Parent Category (optional)
                  </label>
                  <select
                    value={formParent}
                    onChange={(e) => setFormParent(e.target.value)}
                    className="w-full px-3 py-2.5 text-sm border border-gray-300 rounded-lg bg-white focus:outline-none focus:ring-2 focus:ring-primary-500/20 focus:border-primary-500 transition"
                  >
                    <option value="">None (Top Level)</option>
                    {categories
                      .filter((c) => c.id !== editId)
                      .map((c) => (
                        <option key={c.id} value={c.id}>
                          {c.name}
                        </option>
                      ))}
                  </select>
                </div>

                <div className="flex gap-3 pt-2">
                  <Button
                    onClick={handleSave}
                    isLoading={saving}
                    className="flex-1"
                  >
                    <Save className="w-4 h-4" />
                    {editId ? "Update" : "Create"}
                  </Button>
                  <Button variant="outline" onClick={closeForm}>
                    Cancel
                  </Button>
                </div>
              </div>
            </div>
          </div>
        </>
      )}

      {/* ── Categories List ─────────────────────────────────────────────────── */}
      {filtered.length === 0 ? (
        <div className="text-center py-16 bg-gray-50 rounded-xl border border-gray-100">
          <FolderTree className="w-14 h-14 text-gray-200 mx-auto mb-3" />
          <p className="text-gray-600 font-semibold">No categories found</p>
        </div>
      ) : (
        <div className="bg-white rounded-xl border border-gray-100 shadow-sm overflow-hidden">
          <table className="w-full">
            <thead className="bg-gray-50 border-b border-gray-100">
              <tr>
                <th className="px-5 py-3 text-left text-xs font-semibold text-gray-500 uppercase">Name</th>
                <th className="px-5 py-3 text-left text-xs font-semibold text-gray-500 uppercase">Description</th>
                <th className="px-5 py-3 text-left text-xs font-semibold text-gray-500 uppercase">Parent</th>
                <th className="px-5 py-3 text-center text-xs font-semibold text-gray-500 uppercase">Status</th>
                <th className="px-5 py-3 text-center text-xs font-semibold text-gray-500 uppercase">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-50">
              {filtered.map((cat) => {
                const parent = categories.find((c) => c.id === cat.parentCategoryId);
                return (
                  <tr key={cat.id} className="hover:bg-gray-50 transition">
                    <td className="px-5 py-4">
                      <div className="flex items-center gap-3">
                        <div className="w-8 h-8 bg-primary-50 text-primary-600 rounded-lg flex items-center justify-center text-sm font-bold">
                          {cat.name.charAt(0)}
                        </div>
                        <span className="text-sm font-semibold text-gray-800">{cat.name}</span>
                      </div>
                    </td>
                    <td className="px-5 py-4">
                      <p className="text-sm text-gray-500 truncate max-w-[200px]">
                        {cat.description || "—"}
                      </p>
                    </td>
                    <td className="px-5 py-4">
                      <span className="text-sm text-gray-500">
                        {parent?.name || "—"}
                      </span>
                    </td>
                    <td className="px-5 py-4 text-center">
                      <button
                        onClick={() => handleToggleStatus(cat)}
                        className={cn(
                          "inline-flex items-center gap-1 px-2.5 py-1 rounded-full text-xs font-semibold transition cursor-pointer",
                          cat.isActive
                            ? "bg-green-100 text-green-700 hover:bg-green-200"
                            : "bg-red-100 text-red-700 hover:bg-red-200"
                        )}
                      >
                        {cat.isActive ? (
                          <><CheckCircle className="w-3 h-3" /> Active</>
                        ) : (
                          <><XCircle className="w-3 h-3" /> Inactive</>
                        )}
                      </button>
                    </td>
                    <td className="px-5 py-4">
                      <div className="flex items-center justify-center gap-1">
                        <button
                          onClick={() => openEditForm(cat)}
                          className="p-1.5 text-gray-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition"
                          title="Edit"
                        >
                          <Edit className="w-4 h-4" />
                        </button>
                        <button
                          onClick={() => handleDelete(cat)}
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
          <div className="px-5 py-3 bg-gray-50 border-t border-gray-100 text-sm text-gray-500">
            Showing {filtered.length} of {categories.length} categories
          </div>
        </div>
      )}
    </div>
  );
};

export default AdminCategories;