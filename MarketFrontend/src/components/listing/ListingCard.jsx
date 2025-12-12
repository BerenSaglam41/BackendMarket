import { Link } from "react-router-dom";
import { HeartIcon, ShoppingCartIcon } from "@heroicons/react/24/outline";
import { useUIStore } from "../../store/uiStore";
import { useAuthStore } from "../../store/authStore";
import { use } from "react";
import { useCartStore } from "../../store/cartStore";
// Para birimi formatlayıcı
const formatPrice = (price) => {
  return new Intl.NumberFormat("tr-TR", {
    style: "currency",
    currency: "TRY",
    minimumFractionDigits: 2,
  }).format(price);
};

export default function ListingCard({ listing }) {
  const {
    slug,
    productName,
    productImageUrl,
    brandName,
    sellerName,
    originalPrice,
    unitPrice,
    discountPercentage,
    stock,
  } = listing;  
  const addToCart = useCartStore((s) => s.addToCart);
  const openCart = useUIStore((s) => s.openCart);
  const hasDiscount = discountPercentage > 0;

  return (
<div className="
  h-full flex flex-col bg-white
  border rounded-xl shadow-sm
  text-center md:text-left
">      {/* ===== GÖRSEL ALANI ===== */}
      <div className="relative h-56 p-4 flex items-center justify-center bg-gray-50 overflow-hidden">
        {/* İndirim Rozeti */}
        {hasDiscount && (
          <div className="absolute top-3 left-3 bg-red-600 text-white text-xs font-bold px-2 py-1 rounded shadow-sm z-10">
            %{discountPercentage}
          </div>
        )}

        {/* Favori Butonu (şimdilik UI only) */}
        <button
          type="button"
          onClick={(e) => {
            e.preventDefault();
            e.stopPropagation();
            console.log("favorite clicked", listing.listingId);
          }}
          className="absolute top-3 right-3 z-10 bg-white/90 backdrop-blur rounded-full p-2 shadow-sm border border-gray-100 hover:scale-110 hover:shadow transition"
        >
          <HeartIcon className="w-5 h-5 text-gray-500 group-hover:text-orange-500 transition-colors" />
        </button>

        <Link
          to={`/listing/${slug}`}
          className="w-full h-full flex justify-center items-center"
        >
          <img
            src={productImageUrl || "/images/placeholder.png"}
            alt={productName}
            onError={(e) => {
              e.currentTarget.src = "/images/placeholder.png";
            }}
            className={`object-contain max-h-full max-w-full transition-transform duration-500 group-hover:scale-105 ${
              stock === 0 ? "opacity-60" : ""
            }`}
          />
        </Link>
      </div>

      {/* ===== İÇERİK ===== */}
      <div className="flex flex-col flex-1 p-4">
        {/* Marka & Stok */}
        <div className="flex justify-between items-start mb-1">
          <span className="text-xs font-bold text-orange-600 uppercase tracking-wide">
            {brandName}
          </span>

          {stock <= 5 && stock > 0 && (
            <span className="text-[10px] bg-red-100 text-red-600 px-1.5 py-0.5 rounded font-medium">
              Son {stock}
            </span>
          )}
        </div>

        {/* Ürün İsmi */}
        <Link
          to={`/listing/${slug}`}
          className="text-sm font-medium text-gray-800 leading-snug line-clamp-2 hover:text-orange-600 transition-colors mb-2 min-h-[40px]"
          title={productName}
        >
          {productName}
        </Link>

        {/* Satıcı */}
        <div className="text-xs text-gray-400 mb-4">
          Satıcı:{" "}
          <span className="text-gray-600 font-medium hover:underline cursor-pointer">
            {sellerName}
          </span>
          {/* TODO: seller page */}
        </div>

        {/* ===== ALT KISIM ===== */}
        <div className="mt-auto pt-3 border-t border-gray-100 md:text-left">
          {/* Fiyat */}
          <div className="flex items-end gap-2 mb-3">
            <div className="flex flex-col">
              {hasDiscount && (
                <span className="text-xs text-gray-400 line-through decoration-red-400">
                  {formatPrice(originalPrice)}
                </span>
              )}
              <span className="text-xl font-bold text-gray-900">
                {formatPrice(unitPrice)}
              </span>
            </div>
          </div>

          {/* Sepete Ekle */}
          <button
            disabled={stock === 0}
            onClick={(e) => {
              e.preventDefault();
              addToCart(listing, 1);
              openCart();
            }}
            className={`w-full flex items-center justify-center gap-2 py-2.5 rounded-lg text-sm font-semibold transition-all active:scale-95
    ${
      stock === 0
        ? "bg-gray-100 text-gray-400 cursor-not-allowed"
        : "bg-orange-500 text-white hover:bg-orange-600 hover:shadow-md"
    }
  `}
          >
            {stock === 0 ? (
              "Stokta Yok"
            ) : (
              <>
                <ShoppingCartIcon className="w-4 h-4" />
                Sepete Ekle
              </>
            )}
          </button>
        </div>
      </div>
    </div>
  );
}
