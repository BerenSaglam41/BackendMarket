import { useEffect } from "react";
import { useParams, Link } from "react-router-dom";
import { ChevronRightIcon } from "@heroicons/react/24/solid";

import { useListingStore } from "../store/listingStore";
import { useCartStore } from "../store/cartStore";
import { useUIStore } from "../store/uiStore";

import ProductGallery from "../components/listing/ProductGallery";
import ProductSummary from "../components/listing/ProductSummary";
import PriceBox from "../components/listing/PriceBox";
import SimilarProducts from "../components/listing/SimilarProducts";
import OtherSellersSection from "../components/listing/OtherSellerSection";

/* ===== PRICE FORMATTER ===== */
const formatPrice = (price) =>
  new Intl.NumberFormat("tr-TR", {
    style: "currency",
    currency: "TRY",
    minimumFractionDigits: 2,
  }).format(price);

export default function ListingDetailPage() {
  const { slug } = useParams();

  const {
    listingDetail,
    loading,
    error,
    fetchDetail,
    clearDetail,
  } = useListingStore();

  const addToCart = useCartStore((s) => s.addToCart);
  const openCart = useUIStore((s) => s.openCart);

  useEffect(() => {
    if (slug) fetchDetail(slug);
    return () => clearDetail();
  }, [slug]);

  if (loading)
    return (
      <div className="p-10 text-center text-gray-500">
        Ürün detayları yükleniyor...
      </div>
    );

  if (error)
    return (
      <div className="p-10 text-center text-red-500">
        {error}
      </div>
    );

  if (!listingDetail) return null;

  return (
    <div className="min-h-screen bg-gray-50 pb-28 lg:pb-12">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-6">

        {/* ===== BREADCRUMB ===== */}
        <nav className="flex items-center text-sm text-gray-500 mb-6 gap-2">
          <Link to="/" className="hover:text-orange-500">
            Anasayfa
          </Link>
          <ChevronRightIcon className="w-4 h-4" />
          <Link
            to={`/category/${listingDetail.categorySlug}`}
            className="hover:text-orange-500"
          >
            {listingDetail.categoryName}
          </Link>
          <ChevronRightIcon className="w-4 h-4" />
          <span className="text-gray-900 font-medium truncate max-w-[240px]">
            {listingDetail.productName}
          </span>
        </nav>

        {/* ===== MAIN GRID ===== */}
        <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 lg:gap-12">

          {/* LEFT – IMAGE */}
          <div className="lg:col-span-5">
            <ProductGallery image={listingDetail.productImageUrl} />
          </div>

          {/* CENTER – PRODUCT INFO */}
          <div className="lg:col-span-4">
            <ProductSummary listing={listingDetail} />
          </div>

          {/* RIGHT – PRICE BOX (DESKTOP) */}
          <div className="lg:col-span-3 hidden lg:block">
            <PriceBox listing={listingDetail} />
          </div>
        </div>

        {/* ===== SIMILAR PRODUCTS ===== */}
        <SimilarProducts products={listingDetail.similarProducts} />

        {/* ===== OTHER SELLERS ===== */}
        <OtherSellersSection sellers={listingDetail.otherSellers} />
      </div>

      {/* ===== MOBILE FIXED ADD TO CART BAR ===== */}
      <div className="lg:hidden fixed bottom-0 left-0 right-0 bg-white border-t z-50 px-4 py-3 shadow-lg">
        <div className="flex items-center justify-between gap-3">
          <div className="flex flex-col">
            <span className="text-xs text-gray-500">Fiyat</span>
            <span className="text-lg font-bold text-gray-900">
              {formatPrice(listingDetail.unitPrice)}
            </span>
          </div>

          <button
            disabled={listingDetail.stock === 0}
            onClick={() => {
              addToCart(listingDetail, 1);
              openCart();
            }}
            className={`flex-1 font-bold py-3 rounded-lg transition active:scale-95
              ${
                listingDetail.stock === 0
                  ? "bg-gray-200 text-gray-400 cursor-not-allowed"
                  : "bg-orange-600 text-white hover:bg-orange-700"
              }
            `}
          >
            {listingDetail.stock === 0 ? "Stokta Yok" : "Sepete Ekle"}
          </button>
        </div>
      </div>
    </div>
  );
}