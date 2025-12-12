import { create } from "zustand";
import { fetchCategoryTree } from "../services/CategoryService";

export const useCategoryStore = create((set) => ({
  tree: [],
  loading: false,
  error: null,

  fetchTree: async () => {
    set({ loading: true, error: null });
    try {
      const data = await fetchCategoryTree();
      set({ tree: data, loading: false });
    } catch (err) {
      set({
        loading: false,
        error: err?.response?.data?.message || "Kategori yüklenemedi",
      });
    }
  },
}));