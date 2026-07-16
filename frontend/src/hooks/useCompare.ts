import { useState, useEffect, useCallback } from "react";
import type { Product, ProductSearchItem } from "@/types";

const STORAGE_KEY = "product-compare";
const MAX_COMPARE = 4;
const EVENT_NAME = "compare-updated";

type CompareProduct = Product | ProductSearchItem;

// ── Helper functions ──────────────────────────────────────────────────────────
const getStoredItems = (): CompareProduct[] => {
  if (typeof window === "undefined") return [];
  try {
    const stored = localStorage.getItem(STORAGE_KEY);
    return stored ? JSON.parse(stored) : [];
  } catch {
    return [];
  }
};

const saveItems = (items: CompareProduct[]) => {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(items));
  // ✅ Dispatch custom event to notify all components
  window.dispatchEvent(new CustomEvent(EVENT_NAME, { detail: items }));
};

// ═══════════════════════════════════════════════════════════════════════════
// Hook
// ═══════════════════════════════════════════════════════════════════════════
export const useCompare = () => {
  const [items, setItems] = useState<CompareProduct[]>(getStoredItems);

  // ── Listen for changes from other components ──────────────────────────────
  useEffect(() => {
    // Handle custom event (same tab)
    const handleCustomEvent = (e: Event) => {
      const customEvent = e as CustomEvent<CompareProduct[]>;
      setItems(customEvent.detail);
    };

    // Handle storage event (different tabs)
    const handleStorageEvent = (e: StorageEvent) => {
      if (e.key === STORAGE_KEY) {
        setItems(getStoredItems());
      }
    };

    window.addEventListener(EVENT_NAME, handleCustomEvent);
    window.addEventListener("storage", handleStorageEvent);

    return () => {
      window.removeEventListener(EVENT_NAME, handleCustomEvent);
      window.removeEventListener("storage", handleStorageEvent);
    };
  }, []);

  // ── Add to compare ────────────────────────────────────────────────────────
  const addToCompare = useCallback((product: CompareProduct) => {
    const current = getStoredItems();

    if (current.length >= MAX_COMPARE) return;
    if (current.some((p) => p.id === product.id)) return;

    const updated = [...current, product];
    saveItems(updated);
    setItems(updated);
  }, []);

  // ── Remove from compare ───────────────────────────────────────────────────
  const removeFromCompare = useCallback((id: string) => {
    const current = getStoredItems();
    const updated = current.filter((p) => p.id !== id);
    saveItems(updated);
    setItems(updated);
  }, []);

  // ── Clear all ─────────────────────────────────────────────────────────────
  const clearCompare = useCallback(() => {
    saveItems([]);
    setItems([]);
  }, []);

  // ── Check if in compare ───────────────────────────────────────────────────
  const isInCompare = useCallback(
    (id: string) => items.some((p) => p.id === id),
    [items]
  );

  const isFull = items.length >= MAX_COMPARE;

  return {
    items,
    count: items.length,
    max: MAX_COMPARE,
    isFull,
    addToCompare,
    removeFromCompare,
    clearCompare,
    isInCompare,
  };
};