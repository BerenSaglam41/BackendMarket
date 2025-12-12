import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { useListingStore } from "../store/listingStore";
import ListingGrid from "../components/listing/ListingGrid";
import CategoryFilters from "../components/filters/CategoryFilters";

export default function CategoryPage() {
  const { slug } = useParams();

  const {
    listings,
    loading,
    error,
    fetchByCategory,
  } = useListingStore();

  const [sortBy, setSortBy] = useState("newest");

  useEffect(() => {
    if (slug) {
      fetchByCategory({
        categorySlug: slug,
        page: 1,
        pageSize: 20,
        sortBy,
      });
    }
  }, [slug, sortBy]);

  return (
    <div className="max-w-7xl mx-auto px-4 py-6">

      {/* TITLE */}
      <h1 className="text-2xl font-bold mb-6 capitalize text-center md:text-left">
          {slug.replace(/-/g, " ")}
      </h1>

      <div className="grid grid-cols-12 gap-6">

        {/* FILTERS */}
        <aside className="hidden md:block col-span-3">
          <CategoryFilters
            sortBy={sortBy}
            setSortBy={setSortBy}
          />
        </aside>

        {/* LISTINGS */}
        <section className="col-span-12 md:col-span-9">

          {loading && (
            <div className="text-center py-20 text-gray-500">
              Ürünler yükleniyor...
            </div>
          )}

          {error && (
            <div className="text-center py-20 text-red-500">
              {error}
            </div>
          )}

          {!loading && listings.length === 0 && (
            <div className="text-center py-20 text-gray-500">
              Bu kategoride ürün bulunamadı
            </div>
          )}

          {!loading && listings.length > 0 && (
            <ListingGrid listings={listings} />
          )}

        </section>
      </div>
    </div>
  );
}