import { create } from "zustand";
import { SellerListingService } from "../../services/seller/SellerListingService";

export const useSellerListingStore = create((set, get) => ({
  items: [],
  loading: false,
  error: null,

  page: 1,
  pageSize: 20,
  totalCount: 0,
  totalPages: 0,

  setPage: (page) => set({ page }),
  createListing: async (payload) => {
    set({ loading: true, error: null });
    try {
      await SellerListingService.createListing(payload);
      set({ page: 1 });
      await get().fetchMyListings();
      set({ loading: false });
    } catch (err) {
      set({
        loading: false,
        error:
          err?.response?.data?.message ||
          err?.message ||
          "Satış oluşturulamadı.",
      });
    }
},
  fetchMyListings: async ({ isActive } = {}) => {
    const { page, pageSize } = get();
    set({ loading: true, error: null });

    try {
      const res = await SellerListingService.getMyListings({
        page,
        pageSize,
        isActive,
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
          "Satışlar getirilemedi.",
      });
    }
  },

  updateListing: async (id, payload) => {
    set({ loading: true, error: null });
    try {
      await SellerListingService.updateListing(id, payload);
      await get().fetchMyListings();
    } catch (err) {
      set({
        loading: false,
        error:
          err?.response?.data?.message ||
          err?.message ||
          "Satış güncellenemedi.",
      });
    }
  },

  deleteListing: async (id) => {
    set({ loading: true, error: null });
    try {
      await SellerListingService.deleteListing(id);
      await get().fetchMyListings();
    } catch (err) {
      set({
        loading: false,
        error:
          err?.response?.data?.message ||
          err?.message ||
          "Satış silinemedi.",
      });
    }
  },
}));