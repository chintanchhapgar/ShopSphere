export const APP_NAME = import.meta.env.VITE_APP_NAME || "ShopSphere";
export const API_URL = import.meta.env.VITE_API_URL || "http://localhost:5000/api";

export const CATEGORIES = [
  "All",
  "Electronics",
  "Clothing",
  "Books",
  "Home & Garden",
  "Sports",
  "Beauty",
  "Toys",
];

export const SORT_OPTIONS = [
  { label: "Newest", value: "-createdAt" },
  { label: "Price: Low to High", value: "price" },
  { label: "Price: High to Low", value: "-price" },
  { label: "Top Rated", value: "-rating" },
];

export const ITEMS_PER_PAGE = 12;