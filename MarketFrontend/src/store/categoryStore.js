import { create } from "zustand";
import { fetchCategories, fetchCategoryTree } from "../services/CategoryService";
import { data } from "react-router-dom";

export const useCategoryStore = create((set) => ({
  categories : [],
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
  fetchCategories : async () => {
    set({ loading: true });
    const res = await fetchCategories();
    
    set({ categories: res.data.data, loading: false });
  }
}));