import { create } from "zustand";
import { createProduct, deleteProduct, fetchAdminProductBySlug, fetchAdminProducts, updateProduct } from "../../services/admin/AdminProductService";


export const useAdminProductStore = create((set, get) => ({
  items: [],
  loading: false,
  selectedProduct : null,
  page: 1,
  pageSize: 20,
  totalCount: 0,

  filters: {
    search: "",
    brandId: "",
    categoryId: "",
    isActive : null
  },

  fetchProducts: async () => {
    const { page, pageSize, filters } = get();
    set({ loading: true });
    
    const res = await fetchAdminProducts({
      page,
      pageSize,
      search: filters.search || undefined,
      brandId: filters.brandId || undefined,
      categoryId: filters.categoryId || undefined,
      isActive: filters.isActive ,
    }); 
    
    
    set({
      items: res.data.data,
      totalCount: res.data.pagination.totalCount,
      loading: false,
    });
  },
  fetchProductBySlug: async (slug) => {
    set({ loading: true , selectedProduct: null});
        const res = await fetchAdminProductBySlug(slug);
        
        set({ selectedProduct: res.data.data, loading: false });
   
  },
  // Create
  createProduct : async (payload) => {
    set({loading:true});
    await createProduct(payload);
    set({page:1});
    await get().fetchProducts();
  },

  /* ======================
     UPDATE
     ====================== */
  updateProduct: async (id, payload) => {
    const res = await updateProduct(id, payload);
    
    await get().fetchProducts();
  },

  /* ======================
     DELETE
     ====================== */
  deleteProduct: async (id) => {
    await deleteProduct(id);
    set((state) => ({
      items: state.items.filter((p) => p.productId !== id),
      totalCount: state.totalCount - 1,
    }));
  },

  /* ======================
     UI HELPERS
     ====================== */
  setPage: (page) => set({ page }),
  setFilters: (filters) =>
    set((state) => ({
      filters: { ...state.filters, ...filters },
      page: 1,
    })),
}));