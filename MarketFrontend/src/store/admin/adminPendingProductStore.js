import { create } from 'zustand';
import { approvePendingProductApi, fetchAdminPendingProducts, fetchAdminPendingProductsByIdApi, rejectPendingProductApi, requestUpdatePendingProductApi } from '../../services/admin/PendingProduct';

export const useAdminPendingProductStore = create((set, get) => ({
    items: [],
    selected: null,
    page: 1,
    pageSize: 20,
    totalCount: 0,
    loading: false,
    filters: {
        status: '',
        sellerId: null,
    },
    fetchPendingProduct: async () => {
        const { page, pageSize, filters } = get(); 
        set({ loading: true });
        try {
            const res = await fetchAdminPendingProducts({
                page,
                pageSize,
                status: filters.status || undefined,
                sellerId: filters.sellerId || undefined,
            });            
            set({
                items: res.data.data,
                totalCount: res.data.pagination.totalCount,
                loading: false,
            });
        } catch (error) {
            console.error("Ürünler alınırken hata oluştu:", error);
            set({ loading: false });
        }
    },
    // Detail
    fetchPendingProductById: async (id) => {
        set({ loading: true });
        try {
            const res = await fetchAdminPendingProductsByIdApi(id);
            set({
                selected: res.data,
                loading: false,
            });
            
        } catch (error) {
            console.error("Ürün detayı alınırken hata oluştu:", error);
            set({ loading: false });
        }
    },
    clearSelected: () => set({ selected: null }),
    // Actions
    approveProduct: async (id, payload) => {
        set({ loading: true });
        try {
            console.log("sa");
            
            await approvePendingProductApi(id, payload);
            await get().fetchPendingProduct();
        } catch (error) {
            console.error("Ürün onaylanırken hata oluştu:", error);
        } finally {
            set({ loading: false });
        }
    },
    rejectProduct: async (id, payload) => {
        set({ loading: true });
        try {
            await rejectPendingProductApi(id, payload);
            await get().fetchPendingProduct(); // Yazım hatası düzeltildi
        } catch (error) {
            console.error("Ürün reddedilirken hata oluştu:", error);
        } finally {
            set({ loading: false });
        }
    },
    requestUpdate: async (id, payload) => {
        set({ loading: true });
        try {
            await requestUpdatePendingProductApi(id, payload);
            await get().fetchPendingProduct();
        } catch (error) {
            console.error("Güncelleme isteği gönderilirken hata oluştu:", error);
        } finally {
            set({ loading: false });
        }
    },
    // UI
    setPage: (page) => set({ page }),
    setFilters: (filters) =>
        set((state) => ({
            filters: { ...state.filters, ...filters },
            page: 1,
        })),
    reset: () =>
        set({
            items: [],
            selected: null,
            page: 1,
            totalCount: 0,
            filters: { status: "", sellerId: "" },
        }),
}));