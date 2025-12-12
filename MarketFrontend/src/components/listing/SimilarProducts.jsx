import MiniListingCard from "./MiniListingCard";

export default function SimilarProducts({ products }) {
  if (!products || products.length === 0) return null;

  return (
    <div className="mt-14">
      {/* Başlık */}
      <h2 className="text-xl font-bold text-gray-900 mb-6">
        Benzer Ürünler
      </h2>

      {/* Grid */}
      <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-4">
        {products.map((item) => (
          <MiniListingCard
            key={item.listingId}
            listing={item}
          />
        ))}
      </div>
    </div>
  );
}