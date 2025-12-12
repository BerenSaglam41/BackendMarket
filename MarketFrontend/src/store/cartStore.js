import { create } from "zustand";
import { toast } from "react-toastify";
import { addToCartApi, fetchCartApi } from "../services/CartService";
import { useAuthStore } from "./authStore";
import api from "../lib/axios";
const CART_STORAGE_KEY = "market_cart";

/* ===== LOCAL STORAGE (GUEST) ===== */
const loadCartFromStorage = () => {
  try {
    const data = localStorage.getItem(CART_STORAGE_KEY);
    return data ? JSON.parse(data) : [];
  } catch {
    return [];
  }
};

const saveCartToStorage = (items) => {
  localStorage.setItem(CART_STORAGE_KEY, JSON.stringify(items));
};

export const useCartStore = create((set, get) => ({
  items: loadCartFromStorage(),
  summary: null,

  /* ===== DERIVED ===== */
  totalQuantity: () =>
    get().items.reduce((sum, item) => sum + item.quantity, 0),

  totalPrice: () =>
    get()
      .items.filter((i) => i.isSelectedForCheckout)
      .reduce((sum, item) => sum + item.totalPrice, 0),

  /* =========================
     TEK GİRİŞ NOKTASI
     ========================= */

  addToCart: async (listing, quantity = 1) => {
    const { isAuthenticated } = useAuthStore.getState();

    // ===== AUTH KULLANICI =====
    if (isAuthenticated) {
      await addToCartApi(listing.listingId, quantity);
      await get().fetchCartFromBackend();
      toast.success(`${listing.productName} sepete eklendi`);
      return;
    }

    // ===== GUEST =====
    const items = [...get().items];
    const existing = items.find((i) => i.listingId === listing.listingId);

    if (existing) {
      existing.quantity = Math.min(
        existing.quantity + quantity,
        existing.availableStock
      );
      existing.totalPrice = existing.quantity * existing.unitPrice;
    } else {
      items.push({
        cartItemId: null,
        productId: listing.productId,
        productName: listing.productName,
        productImage: listing.productImageUrl,
        listingId: listing.listingId,
        storeName: listing.sellerName,
        unitPrice: listing.unitPrice,
        quantity,
        totalPrice: listing.unitPrice * quantity,
        availableStock: listing.stock,
        isOutOfStock: listing.stock === 0,
        isSelectedForCheckout: true,
        appliedCouponCode: null,
        discountApplied: 0,
      });
    }

    set({ items });
    saveCartToStorage(items);
    toast.success(`${listing.productName} sepete eklendi`);
  },
  updateQuantity: async (listingId, newQuantity) => {
    const { isAuthenticated } = useAuthStore.getState();

    // min 1 kuralı
    if (newQuantity < 1) return;

    if (isAuthenticated) {
      // backend cartItemId ile çalışıyor
      const item = get().items.find((i) => i.listingId === listingId);
      if (!item) return;

      await api.put(`/cart/${item.cartItemId}`, {
        quantity: newQuantity,
      });

      await get().fetchCartFromBackend();
      return;
    }

    // ===== GUEST =====
    const items = [...get().items];
    const item = items.find((i) => i.listingId === listingId);
    if (!item) return;

    item.quantity = Math.min(newQuantity, item.availableStock);
    item.totalPrice = item.quantity * item.unitPrice;

    set({ items });
    saveCartToStorage(items);
  },
  removeFromCart: async (listingId) => {
    const { isAuthenticated } = useAuthStore.getState();

    if (isAuthenticated) {
      const item = get().items.find((i) => i.listingId === listingId);
      if (!item) return;

      await api.delete(`/cart/${item.cartItemId}`);
      await get().fetchCartFromBackend();
      return;
    }
    const items = get().items.filter((i) => i.listingId !== listingId);
    set({ items });
    saveCartToStorage(items);
  },

  /* =========================
     BACKEND
     ========================= */

  fetchCartFromBackend: async () => {
    const res = await fetchCartApi();
    set({
      items: res.data.data.items,
      summary: res.data.data.summary,
    });
  },

  syncLocalCartToBackend: async () => {
    const items = get().items;

    for (const item of items) {
      await addToCartApi(item.listingId, item.quantity);
    }

    localStorage.removeItem(CART_STORAGE_KEY);
    await get().fetchCartFromBackend();
  },

  clearCart: () => {
    set({ items: [], summary: null });
    localStorage.removeItem(CART_STORAGE_KEY);
  },
}));
