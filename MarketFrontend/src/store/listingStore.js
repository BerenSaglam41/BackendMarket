import {
  fetchListingDetail,
  fetchListings,
  fetchListingsByCategory,
} from "../services/ListingService";
import { create } from "zustand";

export const useListingStore = create((set) => ({
  listings: [],
  listingDetail: null,
  loading: false,
  error: null,
  pagination: {
    currentPage: 1,
    totalPages: 0,
    pageSize: 20,
    totalCount: 0,
    hasNext: false,
    hasPrevious: false,
  },

  fetchByCategory: async ({ categorySlug, page = 1, pageSize = 20 }) => {
    try {
      set({ loading: true, error: null });
      const res = await fetchListingsByCategory({
        categorySlug,
        page,
        pageSize,
      });
      set({
        listings: res.data,
        pagination: res.pagination,
        loading: false,
      });
    } catch (error) {
      set({
        loading: false,
        error: error.message || "Listingler yuklenemedi",
      });
    }
  },
  fetchAll: async (filters) => {
    try {
      set({ loading: true, error: null });
      const res = await fetchListings(filters);
      set({
        listings: res.data,
        pagination: res.pagination,
        loading: false,
      });
    } catch (error) {
      set({
        loading: false,
        error: error.message || "Listingler yuklenemedi",
      });
    }
  },
  fetchDetail: async (slugOrId) => {
    try {
      set({ loading: true, error: null });
      const res = await fetchListingDetail(slugOrId);
      set({
        listingDetail: res.data,
        loading: false,
      });
    } catch (error) {
      set({
        loading: false,
        error: error.message || "Listing detayi yuklenemedi",
      });
    }
  },
  clearListings: () => {
    set({
      listings: [],
      pagination: {
        currentPage: 1,
        pageSize: 20,
        totalCount: 0,
        totalPages: 0,
        hasNext: false,
        hasPrevious: false,
      },
      error: null,
    });
  },

  clearDetail: () => {
    set({ listingDetail: null, error: null });
  },
}));
