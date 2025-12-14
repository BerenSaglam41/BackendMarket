// src/store/orderStore.js
import { create } from "zustand";
import {
  fetchAdminOrdersApi,
  fetchMyOrdersApi,
  fetchSellerOrdersApi,
} from "../services/OrderService";

export const useOrderStore = create((set, get) => ({
  orders: [],
  selectedOrder: null, // Modal için ekledik
  page: 1,
  pageSize: 20,
  totalCount: 0,
  loading: false,

  filters: {
    orderStatus: "",
    paymentStatus: "",
  },

  /* ======================
     ADMIN (DÜZELTİLDİ)
     ====================== */
  fetchAdminOrders: async () => {
    const { page, pageSize, filters } = get();

    set({ loading: true });

    try {
      const res = await fetchAdminOrdersApi({
        page,
        pageSize,
        orderStatus: filters.orderStatus || undefined,
        paymentStatus: filters.paymentStatus || undefined,
      });
      
      set({
        orders: res.data || [], 
        totalCount: res.pagination?.totalCount ?? 0,
        loading: false,
      });
    } catch (err) {
      set({ loading: false });
      throw err;
    }
  },

  /* ======================
     USER
     ====================== */
  fetchMyOrders: async () => {
    const { page, pageSize } = get();

    set({ loading: true });

    try {
      const res = await fetchMyOrdersApi(page, pageSize);

      set({
        orders: res.data.data || [],
        totalCount: res.data.pagination?.totalCount ?? 0,
        loading: false,
      });
    } catch (err) {
      set({ loading: false });
      throw err;
    }
  },

  /* ======================
     SELLER
     ====================== */
  fetchSellerOrders: async () => {
    const { page, pageSize } = get();

    set({ loading: true });

    try {
      const res = await fetchSellerOrdersApi(page, pageSize);

      set({
        orders: res.data.data || [],
        totalCount: res.data.pagination?.totalCount ?? 0,
        loading: false,
      });
    } catch (err) {
      set({ loading: false });
      throw err;
    }
  },

  /* ======================
     UI HELPERS
     ====================== */
  setPage: (page) => set({ page }),

  setPageSize: (pageSize) => set({ pageSize, page: 1 }),

  // Modal'ı açmak için gerekli helper
  setSelectedOrder: (order) => set({ selectedOrder: order }),

  setFilters: (filters) =>
    set((state) => ({
      filters: { ...state.filters, ...filters },
      page: 1,
    })),

  resetOrders: () =>
    set({
      orders: [],
      selectedOrder: null,
      page: 1,
      pageSize: 20,
      totalCount: 0,
      loading: false,
      filters: {
        orderStatus: "",
        paymentStatus: "",
      },
    }),
}));