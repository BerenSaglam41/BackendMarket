import { Link } from "react-router-dom";
import { StarIcon } from "@heroicons/react/20/solid";

export default function ProductSummary({ listing }) {
  return (
    <div className="flex flex-col gap-4">
      
      {/* Marka Linki */}
      <Link
        to={`/brand/${listing.brandSlug}`}
        className="text-orange-600 font-bold text-sm uppercase tracking-wide hover:underline w-fit"
      >
        {listing.brandName}
      </Link>

      {/* Ürün Başlığı */}
      <h1 className="text-2xl md:text-3xl font-semibold text-gray-900 leading-tight">
        {listing.productName}
      </h1>

      {/* Puanlama (Statik Örnek) */}
      <div className="flex items-center gap-2">
        <div className="flex text-yellow-400">
          {[...Array(5)].map((_, i) => (
            <StarIcon key={i} className="w-5 h-5" />
          ))}
        </div>
        <span className="text-sm text-gray-500 font-medium">4.6 (24 Değerlendirme)</span>
      </div>

      {/* Açıklama */}
      <div className="mt-2">
        <h3 className="text-sm font-bold text-gray-900 mb-2">Ürün Hakkında:</h3>
        <p className="text-gray-600 text-sm leading-relaxed line-clamp-6">
          {listing.productDescription}
        </p>
      </div>

      {/* Ek Özellikler / Varyasyonlar (Örnek) */}
      <div className="pt-4 border-t border-gray-200 mt-2">
         <span className="text-xs text-gray-400">Kategori: </span>
         <Link to={`/category/${listing.categorySlug}`} className="text-xs text-gray-600 font-medium hover:text-orange-500">
            {listing.categoryName}
         </Link>
      </div>
    </div>
  );
}