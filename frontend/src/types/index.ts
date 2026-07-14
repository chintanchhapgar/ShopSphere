// ─── API Wrapper - matches your .NET response shape ───────────────────────────
// Shape 1: ApiWrapper<T> (used by products, auth)
export interface ApiWrapper<T> {
  data: T;
  success: boolean;
  message: string;
  errors: ApiError[] | null;
}

// Shape 2: Result<T> (used by cart, etc.)
export interface ResultWrapper<T> {
  value: T;
  isSuccess: boolean;
  isFailure: boolean;
  message: string;
  error: string | null;
}

export interface ApiError {
  code: string;
  description: string;
  field: string | null;
}

// ─── Auth ─────────────────────────────────────────────────────────────────────
export interface LoginCommand {
  email: string;
  password: string;
}

export interface RegisterCommand {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
}

export interface EmailVerificationCommand {
  email: string;
  token: string;
}

export interface ForgotPasswordCommand {
  email: string;
}

export interface ResetPasswordCommand {
  email: string;
  token: string;
  newPassword: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

// ─── Product (from GET /api/products) ────────────────────────────────────────
export interface Product {
  id: string;
  name: string;
  description: string;
  sku: string;
  basePrice: number;
  costPrice?: number;
  categoryId: string;
  categoryName: string;
  brandId: string;
  brandName: string;
  isActive: boolean;
  primaryImageUrl: string | null;
}

// ─── Product Search (from GET /api/products/search) ──────────────────────────
export enum ProductSortBy {
  Newest      = 0,
  PriceAsc    = 1,
  PriceDesc   = 2,
  NameAsc     = 3,
  NameDesc    = 4,
  Rating      = 5,
  BestSelling = 6,
}

export const ProductSortLabels: Record<ProductSortBy, string> = {
  [ProductSortBy.Newest]:      "Newest",
  [ProductSortBy.PriceAsc]:    "Price: Low to High",
  [ProductSortBy.PriceDesc]:   "Price: High to Low",
  [ProductSortBy.NameAsc]:     "Name: A to Z",
  [ProductSortBy.NameDesc]:    "Name: Z to A",
  [ProductSortBy.Rating]:      "Top Rated",
  [ProductSortBy.BestSelling]: "Best Selling",
};

export interface ProductSearchParams {
  Search?:     string;
  CategoryId?: string;
  BrandId?:    string;
  MinPrice?:   number;
  MaxPrice?:   number;
  Featured?:   boolean;
  InStock?:    boolean;
  SortBy?:     ProductSortBy;
  Page?:       number;
  PageSize?:   number;
}

// ─── Matches ProductSearchResponse from .NET ──────────────────────────────────
export interface ProductSearchItem {
  id:            string;
  name:          string;
  sku:           string;
  price:         number;
  category:      string;
  brand:         string;
  stock:         number;
  isFeatured:    boolean;
  averageRating: number;
  totalReviews:  number;
  thumbnail:     string | null;
}

export interface PaginatedResponse<T> {
  items:      T[];
  totalCount: number;
  page:       number;
  pageSize:   number;
  totalPages: number;
}

// ─── Product Images ───────────────────────────────────────────────────────────
export interface ProductImage {
  id:           string;
  productId:    string;
  imageUrl:     string;
  isPrimary:    boolean;
  displayOrder: number;
}

export interface UpdateProductImageDisplayOrderRequest {
  displayOrder: number;
}

// ─── Category ─────────────────────────────────────────────────────────────────
export interface Category {
  id:               string;
  name:             string;
  description?:     string;
  parentCategoryId?: string;
  isActive:         boolean;
}

export interface CreateCategoryCommand {
  name:              string;
  description?:      string;
  parentCategoryId?: string;
}

export interface UpdateCategoryCommand extends CreateCategoryCommand {
  id: string;
}

export interface ChangeCategoryStatusRequest {
  isActive: boolean;
}

// ─── Brand ────────────────────────────────────────────────────────────────────
export interface Brand {
  id:           string;
  name:         string;
  description?: string;
  isActive:     boolean;
}

export interface CreateBrandCommand {
  name:         string;
  description?: string;
}

export interface UpdateBrandCommand extends CreateBrandCommand {
  id: string;
}

export interface ChangeBrandStatusRequest {
  isActive: boolean;
}

// ─── Product Commands ─────────────────────────────────────────────────────────
export interface CreateProductCommand {
  name:         string;
  description:  string;
  sku:          string;
  basePrice:    number;
  costPrice?:   number;
  categoryId:   string;
  brandId:      string;
  slug:         string;
  barcode?:     string;
  weight?:      number;
}

export interface UpdateProductCommand extends CreateProductCommand {
  id: string;
}

export interface ChangeProductStatusRequest {
  isActive: boolean;
}

// ─── Cart ─────────────────────────────────────────────────────────────────────
export interface Cart {
  id:              string;
  items:           CartItem[];
  totalPrice:      number;
  discountAmount?: number;
  finalPrice?:     number;
  couponCode?:     string;
  total: number;
}

export interface CartItem {
  id:         string;
  productId:  string;
  product?:   Product;
  quantity:   number;
  unitPrice:  number;
  totalPrice: number;
}

export interface AddCartItemRequest {
  productId: string;
  quantity:  number;
}

export interface UpdateCartItemRequest {
  quantity: number;
}

export interface ApplyCouponCommand {
  couponCode: string;
}

// ─── Address ──────────────────────────────────────────────────────────────────
export interface Address {
  id:            string;
  fullName:      string;
  phoneNumber:   string;
  addressLine1:  string;
  addressLine2?: string;
  city:          string;
  state:         string;
  postalCode:    string;
  country:       string;
  isDefault:     boolean;
}

export interface CreateAddressCommand {
  fullName:      string;
  phoneNumber:   string;
  addressLine1:  string;
  addressLine2?: string;
  city:          string;
  state:         string;
  postalCode:    string;
  country:       string;
  isDefault:     boolean;
}

export interface UpdateAddressCommand
  extends Omit<CreateAddressCommand, "isDefault"> {
  addressId: string;
}

// ─── Order ────────────────────────────────────────────────────────────────────
export enum OrderStatus {
  Pending    = 1,
  Confirmed  = 2,
  Processing = 3,
  Shipped    = 4,
  Delivered  = 5,
  Cancelled  = 6,
  Completed   = 7,
}

export const OrderStatusLabels: Record<OrderStatus, string> = {
  [OrderStatus.Pending]:    "Pending",
  [OrderStatus.Confirmed]:  "Confirmed",
  [OrderStatus.Processing]: "Processing",
  [OrderStatus.Shipped]:    "Shipped",
  [OrderStatus.Delivered]:  "Delivered",
  [OrderStatus.Cancelled]:  "Cancelled",
  [OrderStatus.Completed]:   "Completed",
};


export interface Order {
  id:         string;
  status:     OrderStatus;
  totalPrice: number;
  items:      OrderItem[];
  address?:   Address;
  createdAt:  string;
}

export interface OrderItem {
  id:         string;
  productId:  string;
  product?:   Product;
  quantity:   number;
  unitPrice:  number;
  totalPrice: number;
}

export interface CreateOrderCommand {
  addressId: string;
}

export interface UpdateOrderStatusRequest {
  status: OrderStatus;
}

// ─── Payment ──────────────────────────────────────────────────────────────────
export enum PaymentMethod {
  CreditCard = 1,
  DebitCard  = 2,
  PayPal     = 3,
  Cash       = 4,
}

export const PaymentMethodLabels: Record<PaymentMethod, string> = {
  [PaymentMethod.CreditCard]: "Credit Card",
  [PaymentMethod.DebitCard]:  "Debit Card",
  [PaymentMethod.PayPal]:     "PayPal",
  [PaymentMethod.Cash]:       "Cash on Delivery",
};

// ─── Payment Status ──────────────────────────────────────────────────────────
export const PaymentStatusLabels: Record<number, string> = {
  1: "Pending",
  2: "Paid",
  3: "Failed",
  4: "Refunded",
};

export const PaymentStatusColors: Record<number, string> = {
  1: "bg-yellow-100 text-yellow-700",
  2: "bg-green-100 text-green-700",
  3: "bg-red-100 text-red-700",
  4: "bg-gray-100 text-gray-700",
};


export interface PaymentSucceededRequest {
  transactionId:    string;
  gatewayReference?: string;
}

export interface PaymentFailedRequest {
  reason: string;
}

export interface PaymentWebhookRequest {
  transactionId:    string;
  event:            string;
  paymentReference: string;
}

// ─── Coupon ───────────────────────────────────────────────────────────────────
export enum DiscountType {
  Percentage = 1,
  Fixed      = 2,
}

export interface Coupon {
  id:                     string;
  code:                   string;
  name:                   string;
  description?:           string;
  discountType:           DiscountType;
  discountValue:          number;
  minimumOrderAmount:     number;
  maximumDiscountAmount?: number;
  startsAtUtc:            string;
  expiresAtUtc:           string;
  usageLimit:             number;
  isActive:               boolean;
}

export interface CreateCouponCommand {
  code:                   string;
  name:                   string;
  description?:           string;
  discountType:           DiscountType;
  discountValue:          number;
  minimumOrderAmount:     number;
  maximumDiscountAmount?: number;
  startsAtUtc:            string;
  expiresAtUtc:           string;
  usageLimit:             number;
}

export interface UpdateCouponCommand extends CreateCouponCommand {
  id:       string;
  isActive: boolean;
}

// ─── Inventory ────────────────────────────────────────────────────────────────
export interface Inventory {
  productId:           string;
  quantity:            number;
  reservedQuantity:    number;
  availableQuantity:   number;
}

export interface AdjustInventoryRequest {
  quantity: number;
  reason?:  string;
}

// ─── Shipment ─────────────────────────────────────────────────────────────────
export enum ShipmentStatus {
  Preparing      = 1,
  Packed         = 2,
  Dispatched     = 3,
  InTransit      = 4,
  OutForDelivery = 5,
  Delivered      = 6,
  Failed         = 7,
  Returned       = 8,
}

export const ShipmentStatusLabels: Record<ShipmentStatus, string> = {
  [ShipmentStatus.Preparing]:      "Preparing",
  [ShipmentStatus.Packed]:         "Packed",
  [ShipmentStatus.Dispatched]:     "Dispatched",
  [ShipmentStatus.InTransit]:      "In Transit",
  [ShipmentStatus.OutForDelivery]: "Out for Delivery",
  [ShipmentStatus.Delivered]:      "Delivered",
  [ShipmentStatus.Failed]:         "Failed",
  [ShipmentStatus.Returned]:       "Returned",
};

export interface Shipment {
  id:             string;
  orderId:        string;
  status:         ShipmentStatus;
  trackingNumber?: string;
  carrier?:       string;
}

export interface UpdateShipmentStatusRequest {
  status:          ShipmentStatus;
  trackingNumber?: string;
  carrier?:        string;
}

// ─── Review ───────────────────────────────────────────────────────────────────

export interface AddReviewRequest {
  rating:   number;
  comment?: string;
}

// ─── Wishlist ─────────────────────────────────────────────────────────────────
export interface WishlistItem {
  id:        string;
  productId: string;
  product?:  Product;
}

export interface AddToWishlistCommand {
  productId: string;
}

// ─── Admin Dashboard ──────────────────────────────────────────────────────────
export interface DashboardStats {
  totalRevenue:  number;
  totalOrders:   number;
  totalProducts: number;
  totalUsers:    number;
  pendingOrders: number;
}

export interface SalesAnalytics {
  date:       string;
  revenue:    number;
  orderCount: number;
}

export interface ProductDetail {
  id:          string;
  name:        string;
  description: string;
  sku:         string;
  basePrice:   number;
  costPrice?:  number;
  isActive:    boolean;
  category: {
    id:   string;
    name: string;
  };
  brand: {
    id:   string;
    name: string;
  };
  images: ProductImage[];
}

export interface ReviewStatistics {
  averageRating: number;
  totalReviews:  number;
  fiveStar:      number;
  fourStar:      number;
  threeStar:     number;
  twoStar:       number;
  oneStar:       number;
}

export interface Review {
  id:          string;
  productId:   string;
  userId:      string;
  userName?:   string;
  rating:      number;
  comment?:    string | null;
  isApproved?: boolean;
  createdAt:   string;
}

export interface ReviewsResponse {
  statistics: ReviewStatistics;
  reviews:    Review[];
}



// ─── Order List Item (from GET /api/orders/my) ───────────────────────────────
export interface OrderListItem {
  id:           string;
  orderNumber:  string;
  orderDate:    string;
  status:       string;
  totalAmount:  number;
  totalItems:   number;
}


// ─── Order List (GET /api/orders/my) ─────────────────────────────────────────
export interface OrderListItem {
  id:           string;
  orderNumber:  string;
  orderDate:    string;
  status:       string;
  totalAmount:  number;
  totalItems:   number;
}

// ─── Order Detail (GET /api/orders/:id) ──────────────────────────────────────
export interface OrderDetail {
  id:              string;
  orderNumber:     string;
  orderDate:       string;
  status:          string;
  subTotal:        number;
  taxAmount:       number;
  shippingAmount:  number;
  discountAmount:  number;
  totalAmount:     number;
  // Flat shipping address
  shippingName:    string;
  phoneNumber:     string;
  addressLine1:    string;
  addressLine2?:   string;
  city:            string;
  state:           string;
  postalCode:      string;
  country:         string;
  // Items
  items:           OrderDetailItem[];
}

export interface OrderDetailItem {
  productId:     string;
  productName:   string;
  productSku:    string;
  productImage:  string | null;
  unitPrice:     number;
  quantity:      number;
  lineTotal:     number;
}
export interface WishlistItem {
  productId: string;
  name:      string;
  sku:       string;
  price:     number;
  imageUrl:  string | null;
  inStock:   boolean;
}

export interface AddToWishlistCommand {
  productId: string;
}

export interface User {
  id:        string;
  firstName: string;
  lastName:  string;
  email:     string;
  role:      string;   // decoded from JWT (single)
  roles?:    string[]; // from /api/auth/me (array)
}


export interface Payment {
  id:            string;
  orderId:       string;
  amount:        number;
  status:        number;
  method:        number;
  transactionId: string | null;
  createdAtUtc:  string;
}

export interface PaymentRequest {
  paymentMethod: number;
}


// ─── Order Status Colors (string keys from API) ─────────────────────────────
export const OrderStatusColors: Record<string, string> = {
  Pending:    "bg-yellow-100 text-yellow-700",
  Confirmed:  "bg-blue-100 text-blue-700",
  Processing: "bg-purple-100 text-purple-700",
  Shipped:    "bg-indigo-100 text-indigo-700",
  Delivered:  "bg-green-100 text-green-700",
  Cancelled:  "bg-red-100 text-red-700",
  Completed:  "bg-emerald-100 text-emerald-700",
};

// ── Order Status Enum (numeric for API calls) ────────────────────────────────
export const OrderStatusEnum: Record<string, number> = {
  Pending:    1,
  Confirmed:  2,
  Processing: 3,
  Shipped:    4,
  Delivered:  5,
  Cancelled:  6,
  Completed:  7,
};

// ── Admin allowed status transitions ─────────────────────────────────────────
export const AdminStatusTransitions: Record<string, string[]> = {
  Pending:    ["Confirmed", "Cancelled"],
  Confirmed:  ["Processing", "Cancelled"],
  Processing: ["Shipped", "Cancelled"],
  Shipped:    ["Delivered"],
  Delivered:  ["Completed"],
  Cancelled:  [],
  Completed:  [],
};