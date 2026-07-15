import axiosInstance from "./axiosInstance";
import type {
  Product,
  ProductDetail,
  ProductSearchParams,
  ProductSearchItem,
  PaginatedResponse,
  CreateProductCommand,
  UpdateProductCommand,
  ChangeProductStatusRequest,
  ProductImage,
  UpdateProductImageDisplayOrderRequest,
} from "@/types";

// ── Backend URL for resolving relative image paths ───────────────────────────
const BACKEND_URL =
  import.meta.env.VITE_API_URL?.replace("/api", "") ||
  "https://localhost:7065";

// ── Helper: Convert relative URLs to absolute ────────────────────────────────
const resolveImageUrl = (url: string | null | undefined): string | null => {
  if (!url) return null;
  if (url.startsWith("http")) return url;
  if (url.startsWith("/")) return `${BACKEND_URL}${url}`;
  return url;
};

// ── Helper: Unwrap API response ──────────────────────────────────────────────
const unwrap = <T>(resData: any): T => {
  return resData?.data ?? resData?.value ?? resData;
};

// ── Helper: Unwrap array response ────────────────────────────────────────────
const unwrapArray = (resData: any): any[] => {
  const data = resData?.data ?? resData?.value ?? resData;
  if (Array.isArray(data)) return data;
  if (Array.isArray(data?.items)) return data.items;
  if (Array.isArray(data?.products)) return data.products;
  return [];
};

// ── Helper: Resolve product image URLs ───────────────────────────────────────
const resolveProduct = (p: any): Product => ({
  ...p,
  primaryImageUrl: resolveImageUrl(p.primaryImageUrl),
});

const resolveProductDetail = (p: any): ProductDetail => ({
  ...p,
  images: Array.isArray(p?.images)
    ? p.images.map((img: any) => ({
        ...img,
        imageUrl: resolveImageUrl(img.imageUrl) ?? "",
      }))
    : [],
});

const resolveSearchItem = (p: any): ProductSearchItem => ({
  ...p,
  thumbnail: resolveImageUrl(p.thumbnail),
});

const resolveImage = (img: any, productId: string): ProductImage => ({
  id:           img.id,
  productId:    img.productId ?? productId,
  imageUrl:     resolveImageUrl(img.imageUrl) ?? "",
  isPrimary:    img.isPrimary ?? false,
  displayOrder: img.displayOrder ?? 0,
});

// ─── Product API ─────────────────────────────────────────────────────────────
export const productApi = {
  // ── GET /api/products ────────────────────────────────────────────────────
  getAll: async (): Promise<Product[]> => {
    const res = await axiosInstance.get("/products");
    const items = unwrapArray(res.data);
    return items.map(resolveProduct);
  },

  // ── GET /api/products/search ─────────────────────────────────────────────
  search: async (
    params: ProductSearchParams
  ): Promise<PaginatedResponse<ProductSearchItem>> => {
    const res = await axiosInstance.get("/products/search", { params });
    const data = unwrap<any>(res.data);

    if (data?.items && Array.isArray(data.items)) {
      return {
        ...data,
        items: data.items.map(resolveSearchItem),
      };
    }

    if (Array.isArray(data)) {
      return {
        items:      data.map(resolveSearchItem),
        totalCount: data.length,
        page:       1,
        pageSize:   20,
        totalPages: 1,
      };
    }

    return {
      items:      [],
      totalCount: 0,
      page:       1,
      pageSize:   20,
      totalPages: 1,
    };
  },

  // ── GET /api/products/:id ────────────────────────────────────────────────
  getById: async (id: string): Promise<ProductDetail> => {
    const res = await axiosInstance.get(`/products/${id}`);
    const data = unwrap<any>(res.data);
    return resolveProductDetail(data);
  },

  // ── POST /api/products ───────────────────────────────────────────────────
  create: async (data: CreateProductCommand): Promise<Product> => {
    const res = await axiosInstance.post("/products", data);
    return resolveProduct(unwrap(res.data));
  },

  // ── PUT /api/products/:id ────────────────────────────────────────────────
  update: async (id: string, data: UpdateProductCommand): Promise<Product> => {
    const res = await axiosInstance.put(`/products/${id}`, data);
    return resolveProduct(unwrap(res.data));
  },

  // ── DELETE /api/products/:id ─────────────────────────────────────────────
  delete: async (id: string): Promise<void> => {
    await axiosInstance.delete(`/products/${id}`);
  },

  // ── PATCH /api/products/:id/status ───────────────────────────────────────
  changeStatus: async (
    id: string,
    data: ChangeProductStatusRequest
  ): Promise<void> => {
    await axiosInstance.patch(`/products/${id}/status`, data);
  },

  // ═══════════════════════════════════════════════════════════════════════════
  // ── IMAGES ─────────────────────────────────────────────────────────────────
  // ═══════════════════════════════════════════════════════════════════════════

  // ── GET /api/products/:productId/images ──────────────────────────────────
  getImages: async (productId: string): Promise<ProductImage[]> => {
    const res = await axiosInstance.get(`/products/${productId}/images`);

    const raw = res.data;
    // Handle both wrappers:
    // { data: [...] } (ApiWrapper)
    // { value: [...] } (ResultWrapper)
    const data = raw?.value ?? raw?.data ?? raw;

    let items: any[] = [];
    if (Array.isArray(data)) items = data;
    else if (Array.isArray(data?.items)) items = data.items;
    else if (Array.isArray(data?.images)) items = data.images;

    return items.map((img) => resolveImage(img, productId));
  },

  // ── POST /api/products/:productId/images?isPrimary=true ──────────────────
  uploadImage: async (
    productId: string,
    file: File,
    isPrimary: boolean
  ): Promise<ProductImage> => {
    const formData = new FormData();
    formData.append("file", file);

    const res = await axiosInstance.post(
      `/products/${productId}/images?isPrimary=${isPrimary}`,
      formData,
      { headers: { "Content-Type": "multipart/form-data" } }
    );

    const data = unwrap<any>(res.data);
    return resolveImage(data ?? {}, productId);
  },

  // ── PATCH /api/products/:productId/images/:imageId/primary ───────────────
  setPrimaryImage: async (
    productId: string,
    imageId: string
  ): Promise<void> => {
    await axiosInstance.patch(
      `/products/${productId}/images/${imageId}/primary`
    );
  },

  // ── DELETE /api/products/:productId/images/:imageId ──────────────────────
  deleteImage: async (
    productId: string,
    imageId: string
  ): Promise<void> => {
    await axiosInstance.delete(
      `/products/${productId}/images/${imageId}`
    );
  },

  // ── PATCH /api/products/:productId/images/:imageId/display-order ─────────
  updateImageDisplayOrder: async (
    productId: string,
    imageId: string,
    data: UpdateProductImageDisplayOrderRequest
  ): Promise<void> => {
    await axiosInstance.patch(
      `/products/${productId}/images/${imageId}/display-order`,
      data
    );
  },

  // ═══════════════════════════════════════════════════════════════════════════
  // ── INVENTORY ──────────────────────────────────────────────────────────────
  // ═══════════════════════════════════════════════════════════════════════════

  // GET /api/products/:productId/inventory
  getInventory: async (productId: string): Promise<any> => {
    const res = await axiosInstance.get(`/products/${productId}/inventory`);
    // Response: { value: { quantityOnHand, reservedQuantity, ... }, isSuccess }
    return res.data?.value ?? res.data?.data ?? res.data;
  },

  // POST /api/products/:productId/inventory/adjust
  adjustInventory: async (
    productId: string,
    data: { quantity: number; reason?: string }
  ): Promise<void> => {
    await axiosInstance.post(`/products/${productId}/inventory/adjust`, data);
  },

  // GET /api/products/:productId/inventory/history
  getInventoryHistory: async (productId: string): Promise<any[]> => {
    const res = await axiosInstance.get(`/products/${productId}/inventory/history`);
    // Response: { value: [...], isSuccess }
    const data = res.data?.value ?? res.data?.data ?? res.data;
    if (Array.isArray(data)) return data;
    if (Array.isArray(data?.items)) return data.items;
    return [];
  },
};

