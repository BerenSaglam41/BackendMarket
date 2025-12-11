import { create } from 'zustand';
import api from '../lib/axios';

export const useCategoryStore = create((set) => ({
    tree : [],
    loading : false,
    error : null,
    fetchTree : async () => {
        try {
            set({loading: true, error: null});
            const response = await api.get('/category/tree');
            set({tree: response.data.data, loading: false, error: response.errors || null});
        }
        catch (err){
            set({
                loading: false,
                error: err.message || 'Kategori Yuklenemedi'
            })
        }
    }
}))