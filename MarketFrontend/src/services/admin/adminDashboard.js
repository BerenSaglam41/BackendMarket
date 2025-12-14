import api from "../../lib/axios";

export const fetchAdminDashboardApi = () => {
    return api.get('/admin/dashboard',{silent:true});
}