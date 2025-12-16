import api from "../../lib/axios";

export const SellerDashboardService = {
    getDashboard : async () => {
        const res =await api.get("/seller/dashboard",{ silent: true });
        return res.data;
    }
}