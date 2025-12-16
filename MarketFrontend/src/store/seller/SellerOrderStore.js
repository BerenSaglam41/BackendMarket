import { create } from "zustand";
import { SellerOrderService } from "../../services/seller/SellerOrderService";

export const useSellerOrderStore = create((set,get) => ({
    items : [],
    loading : false,
    error: null,
    page:1,
    pageSize:20,
    totalCount:0,
    totalPages : 0,
    setPage : (page) => set({page}),
    fetchOrders : async () => {
        const {page, pageSize} = get();
        set({loading:true, error:null});
        try{
            const res = await SellerOrderService.getOrders({page, pageSize});
            set({
                items: res.data,
                totalCount: res.pagination.totalCount,
                totalPages: res.pagination.totalPages,
                loading:false
            });
        }catch (err){
            set({
                loading:false,
                error: err.message || "Siparişler alınırken bir hata oluştu."
            })
        }
    },
    updateOrderStatus : async (orderId,payload) => {
        set({loading:true, error:null});
        try{
            await SellerOrderService.updateStatus(orderId,payload);
            // Yeniden siparişleri getir
            await get().fetchOrders();
        }catch (err){
            set({
                loading:false,
                error: err.message || "Sipariş durumu güncellenirken bir hata oluştu."
            })
        }
    }
}))