import { create } from "zustand";
import { fetchBrands } from "../services/BrandServices";
export const useBrandStore = create((set) => ({
    brands : [],
    loading: false,
    fetchBrands : async () => {
        set({ loading: true });
        const res = await fetchBrands();
        
        set({ brands: res.data.data, loading: false });
    },
}));