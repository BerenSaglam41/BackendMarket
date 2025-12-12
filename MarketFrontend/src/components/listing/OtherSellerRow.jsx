import { ShoppingCartIcon, TruckIcon } from "@heroicons/react/24/outline";
import { Link } from "react-router-dom";

const formatPrice = (price) =>
  new Intl.NumberFormat("tr-TR", {
    style: "currency",
    currency: "TRY",
    minimumFractionDigits: 2,
  }).format(price);

export default function OtherSellerRow({ seller }) {
  const hasDiscount = seller.discountPercentage > 0;

  return (
    <Link
      to={`/listing/${seller.slug}`}
      className="block"
    >
      <div className="bg-white border border-gray-200 rounded-lg p-4 hover:border-orange-300 hover:shadow-md transition-all cursor-pointer">
        
        <div className="grid grid-cols-1 md:grid-cols-12 gap-4 items-center">

          {/* SATIŞCI */}
          <div className="md:col-span-4 flex items-center gap-3">
            <div className="w-10 h-10 rounded-full bg-gray-100 flex items-center justify-center text-gray-600 font-bold uppercase">
              {seller.sellerName?.charAt(0)}
            </div>

            <div className="text-sm font-semibold text-gray-900">
              {seller.sellerName}
            </div>
          </div>

          {/* KARGO */}
          <div className="md:col-span-3 text-xs text-gray-600">
            <div className="flex items-center gap-1.5">
              <TruckIcon className="w-4 h-4 text-gray-400" />
              {seller.shippingCost === 0 ? (
                <span className="text-green-600 font-medium">Ücretsiz Kargo</span>
              ) : (
                <span>+{seller.shippingCost}₺ Kargo</span>
              )}
            </div>
            <div className="text-gray-400 pl-5">
              {seller.shippingTimeInDays} gün içinde
            </div>
          </div>

          {/* FİYAT */}
          <div className="md:col-span-3 md:text-right">
            {hasDiscount && (
              <div className="text-xs text-gray-400 line-through">
                {formatPrice(seller.originalPrice)}
              </div>
            )}
            <div className="text-xl font-bold text-gray-900">
              {formatPrice(seller.unitPrice)}
            </div>
          </div>

          {/* BUTON */}
          <div className="md:col-span-2">
            <button
              onClick={(e) => {
                e.preventDefault();
                e.stopPropagation(); // 👈 ÖNEMLİ
                // burada ileride addToCart()
              }}
              disabled={seller.stock === 0}
              className={`w-full flex items-center justify-center gap-2 py-2 rounded-md font-semibold text-sm transition
                ${
                  seller.stock === 0
                    ? "bg-gray-100 text-gray-400 cursor-not-allowed"
                    : "bg-orange-500 text-white hover:bg-orange-600 active:scale-95"
                }
              `}
            >
              <ShoppingCartIcon className="w-4 h-4" />
              {seller.stock === 0 ? "Tükendi" : "Sepete Ekle"}
            </button>
          </div>

        </div>
      </div>
    </Link>
  );
}