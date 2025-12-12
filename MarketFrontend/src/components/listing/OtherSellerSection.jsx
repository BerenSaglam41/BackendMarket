import { useState } from "react";
import OtherSellerRow from "./OtherSellerRow";
import { ChevronDownIcon, ChevronUpIcon } from "@heroicons/react/24/outline";

export default function OtherSellersSection({ sellers }) {
  const [showAll, setShowAll] = useState(false);

  if (!sellers || sellers.length === 0) return null;

  // Backend zaten fiyata göre sıralı gönderiyor ama garanti olsun
  const sortedSellers = [...sellers].sort(
    (a, b) => a.unitPrice - b.unitPrice
  );

  const visibleSellers = showAll
    ? sortedSellers
    : sortedSellers.slice(0, 3);

  const hiddenCount = Math.max(0, sortedSellers.length - 3);

  return (
    <div className="mt-10 bg-gray-50 border border-gray-200 rounded-xl p-6">
      
      <h2 className="text-lg font-bold text-gray-900 mb-4">
        Diğer Satıcılar{" "}
        <span className="text-sm font-normal text-gray-500">
          ({sellers.length} alternatif teklif)
        </span>
      </h2>

      <div className="flex flex-col gap-3">
        {visibleSellers.map((seller) => (
          <OtherSellerRow
            key={seller.listingId}
            seller={seller}
          />
        ))}
      </div>

      {sellers.length > 3 && (
        <button
          onClick={() => setShowAll(!showAll)}
          className="w-full mt-4 flex items-center justify-center gap-2 text-sm font-medium text-gray-600 hover:text-orange-600 hover:bg-gray-100 py-3 rounded-lg transition border border-dashed border-gray-300"
        >
          {showAll ? (
            <>
              <ChevronUpIcon className="w-4 h-4" />
              Daha Az Göster
            </>
          ) : (
            <>
              <ChevronDownIcon className="w-4 h-4" />
              Diğer {hiddenCount} Satıcıyı Göster
            </>
          )}
        </button>
      )}
    </div>
  );
}