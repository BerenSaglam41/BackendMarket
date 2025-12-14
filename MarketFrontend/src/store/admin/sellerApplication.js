import { create } from "zustand";
import { approveSellerApplicationApi, fetchSellerApplicationsApi, rejectSellerApplicationApi, requestUpdateSellerApplicationApi } from "../../services/admin/SellerApplicationService";

export const useSellerApplicationStore = create((set)=> ({
    applications : [],
    loading : false,
    error : null,

    // Fetch
    fetchApplications : async () => {
        set({ loading: true,  error: null });
        try {
            const response = await fetchSellerApplicationsApi();          
            set({ applications: response.data.data, loading: false });
        } catch (error) {
            set({ error: error.message, loading: false });
        } finally {
            set({ loading: false });
        }
    },
    // Actions
    approve : async (applicationId) => {
        await approveSellerApplicationApi(applicationId);
        await useSellerApplicationStore.getState().fetchApplications();
    },
    reject : async (applicationId,adminNote) => {
        await rejectSellerApplicationApi(applicationId,adminNote);
        await useSellerApplicationStore.getState().fetchApplications();
    },
    requestUpdate : async (applicationId,adminNote) => {
        await requestUpdateSellerApplicationApi(applicationId,adminNote);
        await useSellerApplicationStore.getState().fetchApplications();
    }
}))