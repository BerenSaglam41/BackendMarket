import api from "../lib/axios";

export const fetchListingsByCategory = async ({
    categorySlug,
    page =1,
    pageSize = 20,
}) => {
    const res = await api.get(`/listing/category/${categorySlug}`, {
      params: {
        page,
        pageSize,
      },
    });
    return res.data;
};

export const fetchListings = async ({
    categoryId,
    brandId,
    searchTerm,
    minPrice,
    maxPrice,
    sortBy =  "newest",
    page = 1,
    pageSize = 20,
}) => {
    const res = await api.get("/listing", {
        params : {
            categoryId,
            brandId,
            searchTerm,
            minPrice,
            maxPrice,
            sortBy,
            page,
            pageSize,
        },
    });
    return res.data;
}
export const fetchListingDetail = async (slugOrId) => {
    const res = await api.get(`/listing/${slugOrId}`);
    return res.data;
}
   