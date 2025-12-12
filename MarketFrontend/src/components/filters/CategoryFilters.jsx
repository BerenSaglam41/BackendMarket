export default function CategoryFilters({
  sortBy,
  setSortBy,
}) {
  return (
    <div className="bg-white border rounded-lg p-4 sticky top-24">
      <h3 className="font-semibold mb-3">Sırala</h3>

      <select
        value={sortBy}
        onChange={(e) => setSortBy(e.target.value)}
        className="w-full border rounded px-3 py-2 text-sm"
      >
        <option value="newest">En Yeniler</option>
        <option value="price_asc">Fiyat: Artan</option>
        <option value="price_desc">Fiyat: Azalan</option>
        <option value="discount">İndirim Oranı</option>
      </select>
    </div>
  );
}