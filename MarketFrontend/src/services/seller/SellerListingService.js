import api from "../../lib/axios";

export const SellerListingService = {

  getMyListings: async ({ page = 1, pageSize = 20, isActive }) => {
    const params = { page, pageSize };
    if (isActive !== undefined) params.isActive = isActive;

    return api.get("/seller/my-products", {
      params,
      silent: true,
    });
  },
  createListing : async (payload) => {
    return api.post(`/seller/listings`, payload);
  },
  
  updateListing: async (id, payload) => {
    return api.put(`/seller/listings/${id}`, payload);
  },

  deleteListing: async (id) => {
    return api.delete(`/seller/listings/${id}`);
  },
};