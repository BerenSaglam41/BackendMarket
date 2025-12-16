import { create } from "zustand";
import { SellerPendingProductService } from "../../services/seller/SellerPendingProductService";

export const useSellerPendingProductStore = create((set, get) => ({
  items: [],
  loading: false,
  error: null,

  page: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 0,

  filters: { status: "all" },

  setFilters: (patch) =>
    set((s) => ({
      filters: { ...s.filters, ...patch },
      page: 1,
    })),

  setPage: (page) => set({ page }),

  fetchPendings: async () => {
    const { page, pageSize, filters } = get();
    set({ loading: true, error: null });

    try {
      const res = await SellerPendingProductService.getPendings({
        status: filters.status,
        page,
        pageSize,
      });

      set({
        items: res.data.data,
        totalCount: res.data.pagination?.totalCount ?? 0,
        totalPages: res.data.pagination?.totalPages ?? 0,
        loading: false,
      });
    } catch (err) {
      set({
        loading: false,
        error:
          err?.response?.data?.message ||
          err?.message ||
          "Ürün başvuruları getirilemedi.",
      });
    }
  },

  createPending: async (payload) => {
    set({ loading: true, error: null });
    try {
      await SellerPendingProductService.createPending(payload);
      await get().fetchPendings();
    } catch (err) {
      set({
        loading: false,
        error:
          err?.response?.data?.message ||
          err?.message ||
          "Ürün oluşturulamadı.",
      });
    }
  },

  updatePending: async (id, payload) => {
    set({ loading: true, error: null });
    try {
      await SellerPendingProductService.updatePending(id, payload);
      await get().fetchPendings();
    } catch (err) {
      set({
        loading: false,
        error:
          err?.response?.data?.message ||
          err?.message ||
          "Ürün güncellenemedi.",
      });
    }
  },

  deletePending: async (id) => {
    set({ loading: true, error: null });
    try {
      await SellerPendingProductService.deletePending(id);
      await get().fetchPendings();
    } catch (err) {
      set({
        loading: false,
        error:
          err?.response?.data?.message ||
          err?.message ||
          "Ürün silinemedi.",
      });
    }
  },
}));