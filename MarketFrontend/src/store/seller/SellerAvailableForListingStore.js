import { create } from "zustand";
import { SellerAvailableProductService } from "../../services/seller/SellerAvailableForListingService";

export const useSellerAvailableForListingStore = create ((set,get) => ({
    items : [],
    loading : false,
    error : null,

    page : 1,
    pageSize : 20,
    totalCount : 0,
    totalPages : 0,
    search : "",

    setPage : (page) => set ({page}),
    setSearch : (search) => set ({search}),

    fetchAvailableForListing : async () => {
        const {page , pageSize, search} = get();
        set ({loading:true, error:null});
        try{
            const res = await SellerAvailableProductService.getAvailableForListing({
                page,
                pageSize,
                search,
            });            
            set({
                items : res.data.data,
                totalCount : res.data.pagination?.totalCount ?? 0,
                totalPages : res.data.pagination?.totalPages ?? 0,
                loading : false,
            })
        } catch (error) {
            set({
                loading : false,
                error : error?.response?.data?.message || error?.message || "Listeleme için uygun ürünler alınamadı.",
            })
        }
    },
    reset : () => set ({
        items : [],
        page : 1,
        totalCount : 0,
        totalPages : 0,
        search : "",
        error : null
    })
}))