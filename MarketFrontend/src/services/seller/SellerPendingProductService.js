import api from "../../lib/axios";

export const SellerPendingProductService = {
  getPendings: async ({ status, page = 1, pageSize = 20 }) => {
    const params = { page, pageSize };
    if (status && status !== "all") params.status = status;

    return api.get("/seller/products", {
      params,
      silent: true,
    });
  },

  getPendingById: async (id) => {
    return api.get(`/seller/products/${id}`, { silent: true });
  },

  createPending: async (payload) => {
    return api.post("/seller/products", payload);
  },

  updatePending: async (id, payload) => {
    return api.put(`/seller/products/${id}`, payload);
  },

  deletePending: async (id) => {
    return api.delete(`/seller/products/${id}`);
  },
};