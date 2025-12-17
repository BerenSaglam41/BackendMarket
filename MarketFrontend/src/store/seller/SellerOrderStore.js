import { create } from "zustand";
import { SellerOrderService } from "../../services/seller/SellerOrderService";

export const useSellerOrderStore = create((set, get) => ({
    items: [],
    loading: false,
    error: null,
    
    // Pagination
    page: 1,
    pageSize: 20,
    totalCount: 0,
    totalPages: 0,

    // Sayfa değiştirme
    setPage: (page) => set({ page }),

    // Siparişleri Çek
    fetchOrders: async () => {
        const { page, pageSize } = get();
        set({ loading: true, error: null });
        
        try {
            const res = await SellerOrderService.getOrders({ page, pageSize });
            
            // Backend PagedApiResponse yapısına göre set ediyoruz
            set({
                items: res.data, 
                totalCount: res.pagination?.totalCount || 0,
                totalPages: res.pagination?.totalPages || 0,
                loading: false
            });
        } catch (err) {
            set({
                loading: false,
                error: err.response?.data?.message || err.message || "Siparişler alınırken bir hata oluştu."
            });
        }
    },

    // Durum Güncelleme (Backend'deki logic'i tetikler)
    updateOrderStatus: async (orderId, payload) => {
        set({ loading: true, error: null });
        try {
            await SellerOrderService.updateStatus(orderId, payload);
            
            await get().fetchOrders(); 
            
            return true; 
        } catch (err) {
            const errorMessage = err.response?.data?.message || err.message || "Güncelleme başarısız.";
            set({
                loading: false,
                error: errorMessage
            });
            return false; 
        }
    }
}));