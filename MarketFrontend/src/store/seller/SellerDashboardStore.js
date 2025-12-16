import { create } from "zustand";
import { SellerDashboardService } from "../../services/seller/SellerDashboardService";

export const useSellerDashboardStore = create((set,get) => ({
    data : null,
    loading : false,
    errorr : null,
    fetchDashboard : async () => {
        set({loading:true, errorr:null});
        try{
            const res = await SellerDashboardService.getDashboard();            
            set({
                data : res.data,
                loading : false
            });
        }
        catch(err){
            set({
                loading : false,
                errorr : err?.response?.data?.message || err?.message || "Dashboard yüklenemedi.",
            })
        }
    },
    clearDashboard : () => set({data:null, errorr:null,loading:false})
}))