import api from "../../lib/axios";
export const SellerAvailableProductService = {
  getAvailableForListing: async ({
    page = 1,
    pageSize = 20,
    search = "",
  } = {}) => {
    return api.get("/product/available-for-listing", {
      params: {
        page,
        pageSize,
        search: search || undefined,
      },
      silent: true,
    });
  },
};