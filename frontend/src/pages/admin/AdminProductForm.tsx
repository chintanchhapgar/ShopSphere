import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { ArrowLeft, Package, Save } from "lucide-react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { productApi } from "@/api/product.api";
import { categoryApi } from "@/api/category.api";
import { brandApi } from "@/api/brand.api";
import type { Category, Brand } from "@/types";
import Input from "@/components/ui/Input";
import Button from "@/components/ui/Button";
import Spinner from "@/components/ui/Spinner";
import toast from "react-hot-toast";

const schema = z.object({
  name:        z.string().min(2, "Name is required"),
  description: z.string().min(5, "Description is required"),
  sku:         z.string().min(2, "SKU is required"),
  basePrice:   z.coerce.number().min(1, "Price must be greater than 0"),
  costPrice:   z.coerce.number().optional(),
  categoryId:  z.string().min(1, "Category is required"),
  brandId:     z.string().min(1, "Brand is required"),
  slug:        z.string().min(2, "Slug is required"),
  barcode:     z.string().optional(),
  weight:      z.coerce.number().optional(),
});

type FormData = z.infer<typeof schema>;

const AdminProductForm = () => {
  const { id }   = useParams<{ id: string }>();
  const navigate = useNavigate();
  const isEdit   = !!id;

  const [categories, setCategories] = useState<Category[]>([]);
  const [brands, setBrands]         = useState<Brand[]>([]);
  const [isLoading, setIsLoading]   = useState(true);

  const {
    register,
    handleSubmit,
    reset,
    watch,
    setValue,
    formState: { errors, isSubmitting },
  } = useForm<FormData>({
    resolver: zodResolver(schema),
    defaultValues: {
      name: "", description: "", sku: "", basePrice: 0,
      categoryId: "", brandId: "", slug: "",
    },
  });

  const nameValue = watch("name");

  // Auto-generate slug from name
  useEffect(() => {
    if (!isEdit && nameValue) {
      const slug = nameValue
        .toLowerCase()
        .replace(/[^a-z0-9]+/g, "-")
        .replace(/(^-|-$)/g, "");
      setValue("slug", slug);
    }
  }, [nameValue, isEdit, setValue]);

  // ── Load Data ──────────────────────────────────────────────────────────────
  useEffect(() => {
    const load = async () => {
      setIsLoading(true);
      try {
        const [cats, brds] = await Promise.all([
          categoryApi.getAll(),
          brandApi.getAll(),
        ]);
        setCategories(cats);
        setBrands(brds);

        // Load product if editing
        if (id) {
          const product = await productApi.getById(id);
          reset({
            name:        product.name,
            description: product.description,
            sku:         product.sku,
            basePrice:   product.basePrice,
            costPrice:   product.costPrice || undefined,
            categoryId:  product.category?.id || "",
            brandId:     product.brand?.id || "",
            slug:        product.name.toLowerCase().replace(/[^a-z0-9]+/g, "-"),
          });
        }
      } catch (err) {
        toast.error("Failed to load data");
      } finally {
        setIsLoading(false);
      }
    };
    load();
  }, [id]);

  // ── Submit ─────────────────────────────────────────────────────────────────
  const onSubmit = async (data: FormData) => {
    try {
      if (isEdit && id) {
        await productApi.update(id, { ...data, id });
        toast.success("Product updated!");
      } else {
        await productApi.create(data);
        toast.success("Product created!");
      }
      navigate("/admin/products");
    } catch (err) {
      toast.error((err as Error).message || "Failed to save product");
    }
  };

  if (isLoading) {
    return (
      <div className="flex justify-center py-32">
        <Spinner size="lg" className="text-primary-600" />
      </div>
    );
  }

  return (
    <div className="max-w-3xl mx-auto px-4 py-8">

      {/* Back */}
      <button
        onClick={() => navigate(-1)}
        className="flex items-center gap-2 text-sm text-gray-500 hover:text-primary-600 mb-6 transition"
      >
        <ArrowLeft className="w-4 h-4" /> Back
      </button>

      {/* Header */}
      <div className="flex items-center gap-3 mb-8">
        <div className="w-10 h-10 bg-primary-100 rounded-xl flex items-center justify-center">
          <Package className="w-5 h-5 text-primary-600" />
        </div>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">
            {isEdit ? "Edit Product" : "Add New Product"}
          </h1>
          <p className="text-sm text-gray-500">
            {isEdit ? "Update product details" : "Create a new product"}
          </p>
        </div>
      </div>

      {/* Form */}
      <div className="bg-white rounded-2xl border border-gray-100 shadow-sm p-6">
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">

          {/* Name */}
          <Input
            label="Product Name"
            placeholder="iPhone 15 Pro"
            error={errors.name?.message}
            {...register("name")}
          />

          {/* Description */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1.5">
              Description
            </label>
            <textarea
              placeholder="Describe the product..."
              rows={4}
              className="w-full px-3 py-2.5 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500/20 focus:border-primary-500 resize-none transition"
              {...register("description")}
            />
            {errors.description && (
              <p className="mt-1.5 text-xs text-red-500">
                {errors.description.message}
              </p>
            )}
          </div>

          {/* SKU & Slug */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <Input
              label="SKU"
              placeholder="APL-IP15-001"
              error={errors.sku?.message}
              {...register("sku")}
            />
            <Input
              label="Slug"
              placeholder="iphone-15-pro"
              error={errors.slug?.message}
              {...register("slug")}
            />
          </div>

          {/* Category & Brand */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1.5">
                Category
              </label>
              <select
                className="w-full px-3 py-2.5 text-sm border border-gray-300 rounded-lg bg-white focus:outline-none focus:ring-2 focus:ring-primary-500/20 focus:border-primary-500 transition"
                {...register("categoryId")}
              >
                <option value="">Select Category</option>
                {categories.map((c) => (
                  <option key={c.id} value={c.id}>
                    {c.name}
                  </option>
                ))}
              </select>
              {errors.categoryId && (
                <p className="mt-1.5 text-xs text-red-500">
                  {errors.categoryId.message}
                </p>
              )}
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1.5">
                Brand
              </label>
              <select
                className="w-full px-3 py-2.5 text-sm border border-gray-300 rounded-lg bg-white focus:outline-none focus:ring-2 focus:ring-primary-500/20 focus:border-primary-500 transition"
                {...register("brandId")}
              >
                <option value="">Select Brand</option>
                {brands.map((b) => (
                  <option key={b.id} value={b.id}>
                    {b.name}
                  </option>
                ))}
              </select>
              {errors.brandId && (
                <p className="mt-1.5 text-xs text-red-500">
                  {errors.brandId.message}
                </p>
              )}
            </div>
          </div>

          {/* Price */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <Input
              label="Selling Price (₹)"
              type="number"
              placeholder="79999"
              error={errors.basePrice?.message}
              {...register("basePrice")}
            />
            <Input
              label="Cost Price (₹) - Optional"
              type="number"
              placeholder="65000"
              error={errors.costPrice?.message}
              {...register("costPrice")}
            />
          </div>

          {/* Barcode & Weight */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <Input
              label="Barcode (Optional)"
              placeholder="123456789012"
              {...register("barcode")}
            />
            <Input
              label="Weight in kg (Optional)"
              type="number"
              placeholder="0.5"
              step="0.01"
              {...register("weight")}
            />
          </div>

          {/* Actions */}
          <div className="flex gap-3 pt-2">
            <Button
              type="submit"
              size="lg"
              isLoading={isSubmitting}
              className="flex-1"
            >
              <Save className="w-4 h-4" />
              {isEdit ? "Update Product" : "Create Product"}
            </Button>
            <Button
              type="button"
              variant="outline"
              size="lg"
              onClick={() => navigate(-1)}
            >
              Cancel
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AdminProductForm;