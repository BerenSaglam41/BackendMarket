import { Link } from "react-router-dom";
import { HeartIcon, ShoppingCartIcon } from "@heroicons/react/24/outline";
import { HeartIcon as HeartSolid } from "@heroicons/react/24/solid";
import { useState } from "react";

const formatPrice = (price) =>
  new Intl.NumberFormat("tr-TR", {
    style: "currency",
    currency: "TRY",
    minimumFractionDigits: 2,
  }).format(price);

export default function MiniListingCard({ listing }) {
  const [isFavorite, setIsFavorite] = useState(false);
  const hasDiscount = listing.discountPercentage > 0;

  return (
    <div className="group bg-white border border-gray-200 rounded-xl p-3 hover:shadow-lg transition relative flex flex-col">
      
      {/* İNDİRİM ROZETİ */}
      {hasDiscount && (
        <div className="absolute top-2 left-2 bg-red-600 text-white text-[11px] font-bold px-2 py-0.5 rounded z-10">
          %{listing.discountPercentage}
        </div>
      )}

      {/* FAVORİ */}
      <button
        onClick={(e) => {
          e.preventDefault();
          setIsFavorite(!isFavorite);
        }}
        className="absolute top-2 right-2 bg-white rounded-full p-1.5 shadow hover:scale-110 transition z-10"
      >
        {isFavorite ? (
          <HeartSolid className="w-4 h-4 text-orange-500" />
        ) : (
          <HeartIcon className="w-4 h-4 text-gray-500 group-hover:text-orange-500" />
        )}
      </button>

      {/* GÖRSEL */}
      <Link
        to={`/listing/${listing.slug}`}
        className="aspect-square bg-gray-50 rounded-lg flex items-center justify-center mb-3 overflow-hidden"
      >
        <img
          src={listing.productImageUrl}
          alt={listing.productName}
          className="object-contain max-h-full group-hover:scale-105 transition-transform duration-300"
        />
      </Link>

      {/* ÜRÜN ADI */}
      <Link
        to={`/listing/${listing.slug}`}
        className="text-sm font-medium text-gray-800 line-clamp-2 mb-1 hover:text-orange-600 transition"
        title={listing.productName}
      >
        {listing.productName}
      </Link>

      {/* FİYAT + SEPET */}
      <div className="mt-auto flex items-center justify-between gap-2 pt-2">
        
        {/* FİYAT */}
        <div className="flex flex-col">
          {hasDiscount && (
            <span className="text-[11px] text-gray-400 line-through">
              {formatPrice(listing.originalPrice)}
            </span>
          )}
          <span className="text-sm font-bold text-gray-900">
            {formatPrice(listing.unitPrice)}
          </span>
        </div>

        {/* SEPET */}
        <button
          disabled={listing.stock === 0}
          className={`p-2 rounded-full transition active:scale-95
            ${
              listing.stock === 0
                ? "bg-gray-100 text-gray-400 cursor-not-allowed"
                : "bg-orange-500 text-white hover:bg-orange-600"
            }
          `}
          title="Sepete ekle"
        >
          <ShoppingCartIcon className="w-4 h-4" />
        </button>
      </div>
    </div>
  );
}