import axiosInstance from "./axiosInstance";
import type {
  Product,
  ProductSearchParams,
  ProductSearchResponse,
  CreateProductCommand,
  UpdateProductCommand,
  ChangeProductStatusRequest,
  ProductImage,
  UpdateProductImageDisplayOrderRequest,
  ApiWrapper,
} from "@/types";

export const productApi = {
  // GET /api/products → returns ApiWrapper<Product[]>
  getAll: async (): Promise<Product[]> => {
    const res = await axiosInstance.get("/products");

    console.log("=== RAW PRODUCTS RESPONSE ===", res.data);

    // Handle ApiWrapper<Product[]>
    const data = res.data;

    if (Array.isArray(data)) return data;
    if (Array.isArray(data?.data)) return data.data;
    if (Array.isArray(data?.data?.data)) return data.data.data;

    console.warn("Unexpected products response shape:", data);
    return [];
  },

  // GET /api/products/search → returns ApiWrapper<ProductSearchResponse>
  search: async (params: ProductSearchParams): Promise<ProductSearchResponse> => {
    const res = await axiosInstance.get<ApiWrapper<ProductSearchResponse>>(
      "/products/search",
      { params }
    );
    console.log("=== SEARCH RESPONSE ===", res.data);
    return (
      res.data?.data ?? {
        items: [],
        totalCount: 0,
        page: 1,
        pageSize: 20,
        totalPages: 1,
      }
    );
  },

  // GET /api/products/:id
  getById: async (id: string): Promise<Product> => {
    const res = await axiosInstance.get<ApiWrapper<Product>>(`/products/${id}`);
    return res.data?.data;
  },

  // POST /api/products
  create: async (data: CreateProductCommand): Promise<Product> => {
    const res = await axiosInstance.post<ApiWrapper<Product>>("/products", data);
    return res.data?.data;
  },

  // PUT /api/products/:id
  update: async (id: string, data: UpdateProductCommand): Promise<Product> => {
    const res = await axiosInstance.put<ApiWrapper<Product>>(
      `/products/${id}`,
      data
    );
    return res.data?.data;
  },

  // DELETE /api/products/:id
  delete: async (id: string): Promise<void> => {
    await axiosInstance.delete(`/products/${id}`);
  },

  // PATCH /api/products/:id/status
  changeStatus: async (
    id: string,
    data: ChangeProductStatusRequest
  ): Promise<void> => {
    await axiosInstance.patch(`/products/${id}/status`, data);
  },

  // ── Images ────────────────────────────────────────────────────────────────
  // POST /api/products/:productId/images
  uploadImage: async (
    productId: string,
    file: File,
    isPrimary: boolean
  ): Promise<ProductImage> => {
    const formData = new FormData();
    formData.append("file", file);
    const res = await axiosInstance.post<ApiWrapper<ProductImage>>(
      `/products/${productId}/images?isPrimary=${isPrimary}`,
      formData,
      { headers: { "Content-Type": "multipart/form-data" } }
    );
    return res.data?.data;
  },

  // GET /api/products/:productId/images
  getImages: async (productId: string): Promise<ProductImage[]> => {
    const res = await axiosInstance.get<ApiWrapper<ProductImage[]>>(
      `/products/${productId}/images`
    );
    return res.data?.data ?? [];
  },

  // PATCH /api/products/:productId/images/:imageId/primary
  setPrimaryImage: async (
    productId: string,
    imageId: string
  ): Promise<void> => {
    await axiosInstance.patch(
      `/products/${productId}/images/${imageId}/primary`
    );
  },

  // DELETE /api/products/:productId/images/:imageId
  deleteImage: async (productId: string, imageId: string): Promise<void> => {
    await axiosInstance.delete(`/products/${productId}/images/${imageId}`);
  },

  // PATCH /api/products/:productId/images/:imageId/display-order
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
};