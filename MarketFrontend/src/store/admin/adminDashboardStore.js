import { create } from "zustand";
import { fetchAdminDashboardApi } from "../../services/admin/adminDashboard";

export const useAdminDashboardStore = create((set, get) => ({
    data : null,
    loading : false,

    fetchDashboard : async () => {
        set({loading:true});
        
        const res = await fetchAdminDashboardApi();
        
        set({data: res.data.data, loading:false});
    }
}));