import { useEffect, useState, useRef } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  ArrowLeft,
  Upload,
  Trash2,
  Star,
  Image as ImageIcon,
} from "lucide-react";
import { productApi } from "@/api/product.api";
import type { ProductImage, ProductDetail } from "@/types";
import { cn } from "@/utils/cn";
import Spinner from "@/components/ui/Spinner";
import Button from "@/components/ui/Button";
import toast from "react-hot-toast";

const AdminProductImages = () => {
  const { id }   = useParams<{ id: string }>();
  const navigate = useNavigate();
  const fileInputRef = useRef<HTMLInputElement>(null);

  const [product, setProduct]     = useState<ProductDetail | null>(null);
  const [images, setImages]       = useState<ProductImage[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [uploading, setUploading] = useState(false);

  // ── Load ───────────────────────────────────────────────────────────────────
  const loadData = async () => {
    if (!id) return;
    try {
      // Load product detail
      const prod = await productApi.getById(id);
      setProduct(prod);

      // Load images from dedicated endpoint (already resolved URLs)
      const imgs = await productApi.getImages(id);
      setImages(imgs);
    } catch (err) {
      console.error("Load error:", err);
      toast.error("Failed to load product");
    }
  };

  useEffect(() => {
    const load = async () => {
      setIsLoading(true);
      await loadData();
      setIsLoading(false);
    };
    load();
  }, [id]);

  // ── Upload ─────────────────────────────────────────────────────────────────
  const handleUploadClick = () => fileInputRef.current?.click();

  const handleFileChange = async (e: React.ChangeEvent<HTMLInputElement>) => {
    if (!id || !e.target.files?.length) return;
    const file = e.target.files[0];

    if (!file.type.startsWith("image/")) {
      toast.error("Please select an image file");
      return;
    }
    if (file.size > 5 * 1024 * 1024) {
      toast.error("Image must be less than 5MB");
      return;
    }

    setUploading(true);
    try {
      const isPrimary = images.length === 0;
      await productApi.uploadImage(id, file, isPrimary);
      toast.success("Image uploaded!");
      await loadData();
    } catch (err) {
      toast.error((err as Error).message || "Upload failed");
    } finally {
      setUploading(false);
      if (fileInputRef.current) fileInputRef.current.value = "";
    }
  };

  const handleSetPrimary = async (imageId: string) => {
    if (!id) return;
    try {
      await productApi.setPrimaryImage(id, imageId);
      toast.success("Primary image updated");
      await loadData();
    } catch (err) {
      toast.error((err as Error).message || "Failed");
    }
  };

  const handleDelete = async (imageId: string) => {
    if (!id || !window.confirm("Delete this image?")) return;
    try {
      await productApi.deleteImage(id, imageId);
      toast.success("Image deleted");
      await loadData();
    } catch (err) {
      toast.error((err as Error).message || "Failed");
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
    <div className="max-w-4xl mx-auto px-4 py-8">

      <button onClick={() => navigate("/admin/products")} className="flex items-center gap-2 text-sm text-gray-500 hover:text-primary-600 mb-6 transition">
        <ArrowLeft className="w-4 h-4" /> Back to Products
      </button>

      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 mb-8">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Product Images</h1>
          <p className="text-gray-500 text-sm mt-1">{product?.name}</p>
          <p className="text-xs text-gray-400 mt-0.5">{images.length} image{images.length !== 1 ? "s" : ""}</p>
        </div>
        <input ref={fileInputRef} type="file" accept="image/*" onChange={handleFileChange} className="hidden" />
        <Button onClick={handleUploadClick} isLoading={uploading} disabled={uploading}>
          <Upload className="w-4 h-4" /> {uploading ? "Uploading..." : "Upload Image"}
        </Button>
      </div>

      {images.length === 0 ? (
        <div className="text-center py-20 bg-gray-50 rounded-xl border-2 border-dashed border-gray-200">
          <ImageIcon className="w-16 h-16 text-gray-200 mx-auto mb-4" />
          <p className="text-gray-600 font-semibold text-lg mb-1">No images yet</p>
          <p className="text-sm text-gray-400 mb-6">Upload your first product image</p>
          <Button onClick={handleUploadClick}><Upload className="w-4 h-4" /> Choose Image</Button>
        </div>
      ) : (
        <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
          {images.map((img) => (
            <div key={img.id} className={cn("relative rounded-xl overflow-hidden border-2 group", img.isPrimary ? "border-primary-500 ring-2 ring-primary-100" : "border-gray-100 hover:border-gray-200")}>
              <img
                src={img.imageUrl}
                alt={product?.name || "Product"}
                className="w-full aspect-square object-cover"
                onError={(e) => {
                  (e.target as HTMLImageElement).src = "https://placehold.co/300x300/fecaca/dc2626?text=Load+Error";
                }}
              />
              {img.isPrimary && (
                <span className="absolute top-2 left-2 px-2.5 py-1 bg-primary-600 text-white text-xs font-semibold rounded-full flex items-center gap-1 shadow-sm">
                  <Star className="w-3 h-3" /> Primary
                </span>
              )}
              <div className="absolute bottom-0 inset-x-0 bg-gradient-to-t from-black/70 via-black/40 to-transparent p-3 flex justify-between items-end opacity-0 group-hover:opacity-100 transition-opacity">
                {!img.isPrimary ? (
                  <button onClick={() => handleSetPrimary(img.id)} className="flex items-center gap-1 px-2.5 py-1.5 bg-white text-xs font-medium rounded-lg hover:bg-primary-50 transition">
                    <Star className="w-3 h-3" /> Set Primary
                  </button>
                ) : (
                  <span className="text-xs text-white/70">Primary</span>
                )}
                <button onClick={() => handleDelete(img.id)} className="p-2 bg-red-500 text-white rounded-lg hover:bg-red-600 transition">
                  <Trash2 className="w-3.5 h-3.5" />
                </button>
              </div>
            </div>
          ))}

          <button onClick={handleUploadClick} disabled={uploading} className="aspect-square rounded-xl border-2 border-dashed border-gray-200 hover:border-primary-400 hover:bg-primary-50 flex flex-col items-center justify-center gap-2 transition cursor-pointer disabled:opacity-50">
            {uploading ? <Spinner className="text-primary-600" /> : <><Upload className="w-6 h-6 text-gray-400" /><span className="text-sm text-gray-500 font-medium">Add More</span></>}
          </button>
        </div>
      )}

      <div className="mt-6 p-4 bg-gray-50 rounded-xl border border-gray-100">
        <p className="text-xs text-gray-500">💡 Supported: JPG, PNG, WebP • Max: 5MB • First image auto-set as primary</p>
      </div>
    </div>
  );
};

export default AdminProductImages;