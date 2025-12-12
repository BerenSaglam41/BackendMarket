import { ShoppingCartIcon, TruckIcon, ShieldCheckIcon } from "@heroicons/react/24/outline";

const formatPrice = (price) =>
  new Intl.NumberFormat("tr-TR", {
    style: "currency",
    currency: "TRY",
    minimumFractionDigits: 2
  }).format(price);

export default function PriceBox({ listing }) {
  const hasDiscount = listing.discountPercentage > 0;

  return (
    <div className="bg-white border border-gray-200 rounded-xl shadow-lg p-6 sticky top-24 flex flex-col gap-5">
      
      {/* Fiyat Alanı */}
      <div>
        <div className="text-sm text-gray-500 mb-1">Bugun Sepette</div>
        <div className="flex items-end gap-3">
          <span className="text-3xl font-bold text-gray-900">
            {formatPrice(listing.unitPrice)}
          </span>
          {hasDiscount && (
            <span className="text-lg text-gray-400 line-through mb-1">
              {formatPrice(listing.originalPrice)}
            </span>
          )}
        </div>
        {hasDiscount && (
            <div className="text-green-600 text-xs font-bold mt-1 bg-green-50 w-fit px-2 py-1 rounded">
                %{listing.discountPercentage} İndirim
            </div>
        )}
      </div>

      {/* Aksiyon Butonu */}
      <button
        disabled={listing.stock === 0}
        className={`w-full flex items-center justify-center gap-2 py-3.5 rounded-lg font-semibold text-base transition shadow-md active:scale-95
          ${
            listing.stock === 0
              ? "bg-gray-200 text-gray-400 cursor-not-allowed"
              : "bg-orange-600 text-white hover:bg-orange-700 hover:shadow-orange-200"
          }
        `}
      >
        {listing.stock === 0 ? "Stokta Yok" : (
            <>
                <ShoppingCartIcon className="w-6 h-6" />
                Sepete Ekle
            </>
        )}
      </button>

      {/* Kargo & Stok Bilgisi */}
      <div className="space-y-3 pt-4 border-t border-gray-100">
        
        {/* Kargo */}
        <div className="flex items-start gap-3">
            <div className="bg-blue-50 p-2 rounded-full text-blue-600">
                <TruckIcon className="w-5 h-5" />
            </div>
            <div>
                <span className="block text-sm font-semibold text-gray-800">
                    {listing.shippingCost === 0 ? "Kargo Bedava" : `${listing.shippingCost} ₺ Kargo`}
                </span>
                <span className="text-xs text-gray-500">
                    Tahmini teslimat: {listing.shippingTimeInDays} gün
                </span>
            </div>
        </div>

        {/* Güvenlik */}
        <div className="flex items-start gap-3">
            <div className="bg-green-50 p-2 rounded-full text-green-600">
                <ShieldCheckIcon className="w-5 h-5" />
            </div>
            <div>
                <span className="block text-sm font-semibold text-gray-800">
                    Güvenli Ödeme
                </span>
                <span className="text-xs text-gray-500">
                    256-bit SSL koruması
                </span>
            </div>
        </div>

      </div>

      {/* Satıcı Bilgisi */}
      <div className="text-xs text-center text-gray-400 mt-2">
        Satıcı: <span className="text-gray-700 font-medium underline cursor-pointer">{listing.sellerName}</span>
      </div>

        {/* Kritik Stok Uyarısı */}
        {listing.stock <= 5 && listing.stock > 0 && (
            <div className="text-center text-red-600 text-xs font-bold animate-pulse">
                Acele et! Son {listing.stock} ürün kaldı.
            </div>
        )}
    </div>
  );
}