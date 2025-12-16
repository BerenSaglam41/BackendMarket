import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import {
  PencilSquareIcon,
  TrashIcon,
  EyeIcon,
  PhotoIcon,
  MagnifyingGlassIcon,
  PlusIcon,
  ShoppingBagIcon,
  XMarkIcon,
  CheckCircleIcon,
  NoSymbolIcon,
  ExclamationCircleIcon
} from "@heroicons/react/24/outline";

import { useSellerListingStore } from "../../store/seller/SellerListingStore";

export default function SellerProductsPage() {
  const {
    items,
    loading,
    error,
    fetchMyListings,
    deleteListing,
    page,
    totalPages,
    setPage,
  } = useSellerListingStore();

  const [searchTerm, setSearchTerm] = useState("");

  useEffect(() => {
    fetchMyListings();
  }, [page]);

  // Client-side Arama
  const filteredItems = useMemo(() => {
    const q = searchTerm.trim().toLowerCase();
    if (!q) return items;
    return items.filter((p) => 
        p.productName?.toLowerCase().includes(q) || 
        p.productSlug?.toLowerCase().includes(q)
    );
  }, [items, searchTerm]);

  return (
    <div className="min-h-screen bg-slate-50/50 p-6 lg:p-10 font-sans text-slate-800">
      
      {/* HEADER & ACTIONS */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 mb-8">
        <div>
          <h1 className="text-3xl font-bold text-slate-900 tracking-tight">
            Ürünlerim (Listem)
          </h1>
          <p className="text-slate-500 mt-1">
            Satışta olan veya pasife aldığınız ürünleri buradan yönetin.
          </p>
        </div>

        <Link
            to="/seller/pending-products/add"
            className="flex items-center gap-2 bg-orange-600 text-white px-5 py-2 rounded-xl hover:bg-orange-700 shadow-lg shadow-orange-600/20 transition active:scale-95 font-medium"
        >
            <PlusIcon className="w-5 h-5" />
            Yeni Ürün Ekle
        </Link>
      </div>

      {/* SEARCH BAR */}
      <div className="bg-white p-4 rounded-2xl border border-slate-200 shadow-sm mb-6">
        <div className="relative max-w-md">
          <MagnifyingGlassIcon className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
          <input
            type="text"
            placeholder="Ürün adı veya koduna göre ara..."
            className="w-full pl-10 pr-10 py-2.5 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-orange-500 focus:bg-white transition outline-none text-sm"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
          {searchTerm && (
            <button 
                onClick={() => setSearchTerm("")}
                className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400 hover:text-rose-500"
            >
                <XMarkIcon className="w-4 h-4" />
            </button>
          )}
        </div>
      </div>

      {/* ERROR MESSAGE */}
      {error && (
        <div className="mb-6 bg-rose-50 border border-rose-200 text-rose-700 rounded-2xl p-4 flex items-center gap-3">
          <ExclamationCircleIcon className="w-6 h-6 shrink-0"/>
          <span>{error}</span>
        </div>
      )}

      {/* TABLE */}
      <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
        {loading ? (
          <div className="p-12 flex justify-center">
            <div className="w-10 h-10 border-4 border-slate-200 border-t-orange-500 rounded-full animate-spin"></div>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full text-left border-collapse min-w-[900px]">
                <thead className="bg-slate-50 border-b border-slate-200 text-xs uppercase text-slate-500 font-semibold tracking-wider">
                <tr>
                    <th className="px-6 py-4">Ürün Bilgisi</th>
                    <th className="px-6 py-4">Fiyat</th>
                    <th className="px-6 py-4">Stok Durumu</th>
                    <th className="px-6 py-4">Kargo</th>
                    <th className="px-6 py-4 text-center">Durum</th>
                    <th className="px-6 py-4 text-right">İşlem</th>
                </tr>
                </thead>

                <tbody className="divide-y divide-slate-100">
                {filteredItems.map((p) => (
                    <tr
                    key={p.listingId}
                    className="group hover:bg-slate-50/60 transition-colors"
                    >
                    {/* PRODUCT INFO */}
                    <td className="px-6 py-4">
                        <div className="flex items-center gap-4">
                        <div className="w-12 h-12 rounded-lg bg-slate-100 border border-slate-200 flex items-center justify-center overflow-hidden shrink-0">
                            {p.productImageUrl ? (
                            <img
                                src={p.productImageUrl}
                                alt=""
                                className="w-full h-full object-cover"
                            />
                            ) : (
                            <PhotoIcon className="w-6 h-6 text-slate-300" />
                            )}
                        </div>
                        <div className="min-w-0 max-w-[220px]">
                            <div className="font-bold text-slate-900 truncate" title={p.productName}>
                                {p.productName}
                            </div>
                            <div className="text-xs text-slate-400 font-mono mt-0.5 truncate">
                                {p.productSlug}
                            </div>
                        </div>
                        </div>
                    </td>

                    {/* PRICE */}
                    <td className="px-6 py-4">
                        <div className="flex flex-col">
                            <span className="font-bold text-slate-900">
                                {p.unitPrice?.toLocaleString('tr-TR', { minimumFractionDigits: 2 })} ₺
                            </span>
                            {p.discountPercentage > 0 && (
                                <span className="text-xs text-emerald-600 bg-emerald-50 px-1.5 py-0.5 rounded w-fit mt-1">
                                    %{p.discountPercentage} İndirim
                                </span>
                            )}
                            {/* Eğer indirim varsa orijinal fiyatı çizili gösterelim (Backend'den geliyorsa) */}
                            {p.originalPrice > p.unitPrice && (
                                <span className="text-xs text-slate-400 line-through mt-0.5">
                                    {p.originalPrice} ₺
                                </span>
                            )}
                        </div>
                    </td>

                    {/* STOCK */}
                    <td className="px-6 py-4 text-sm">
                        {p.stock <= 0 ? (
                            <span className="text-rose-600 font-bold flex items-center gap-1">
                                <NoSymbolIcon className="w-4 h-4"/> Tükendi
                            </span>
                        ) : p.stock < 10 ? (
                            <span className="text-amber-600 font-medium flex items-center gap-1">
                                <ExclamationCircleIcon className="w-4 h-4"/> Son {p.stock} adet
                            </span>
                        ) : (
                            <span className="text-slate-600 font-medium">
                                {p.stock} adet
                            </span>
                        )}
                    </td>

                    {/* SHIPPING */}
                    <td className="px-6 py-4 text-sm text-slate-600">
                        <div className="flex flex-col">
                            <span>{p.shippingTimeInDays} günde kargo</span>
                            <span className="text-xs text-slate-400">
                                {p.shippingCost > 0 ? `${p.shippingCost} ₺` : "Ücretsiz"}
                            </span>
                        </div>
                    </td>

                    {/* STATUS */}
                    <td className="px-6 py-4 text-center">
                        <span
                        className={`inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-bold border ${
                            p.isActive
                            ? "bg-emerald-50 text-emerald-700 border-emerald-100"
                            : "bg-slate-100 text-slate-500 border-slate-200"
                        }`}
                        >
                        {p.isActive ? (
                            <CheckCircleIcon className="w-3.5 h-3.5" />
                        ) : (
                            <NoSymbolIcon className="w-3.5 h-3.5" />
                        )}
                        {p.isActive ? "Yayında" : "Pasif"}
                        </span>
                    </td>

                    {/* ACTIONS */}
                    <td className="px-6 py-4 text-right">
                        <div className="flex items-center justify-end gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                        <Link
                            to={`/seller/products/${p.productSlug}`} // Ürün detay linki varsa
                            className="p-2 text-slate-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition"
                            title="Ürünü Görüntüle"
                        >
                            <EyeIcon className="w-4 h-4" />
                        </Link>

                        <Link
                            to={`/seller/products/edit/${p.listingId}`}
                            className="p-2 text-slate-400 hover:text-orange-600 hover:bg-orange-50 rounded-lg transition"
                            title="Düzenle"
                        >
                            <PencilSquareIcon className="w-4 h-4" />
                        </Link>

                        <button
                            onClick={() => {
                            if (window.confirm("Bu ürünü satıştan kaldırmak istediğinize emin misiniz?")) {
                                deleteListing(p.listingId);
                            }
                            }}
                            className="p-2 text-slate-400 hover:text-rose-600 hover:bg-rose-50 rounded-lg transition"
                            title="Sil"
                        >
                            <TrashIcon className="w-4 h-4" />
                        </button>
                        </div>
                    </td>
                    </tr>
                ))}

                {filteredItems.length === 0 && !loading && (
                    <tr>
                    <td
                        colSpan="6"
                        className="px-6 py-16 text-center text-slate-400"
                    >
                        <div className="flex flex-col items-center justify-center">
                            <ShoppingBagIcon className="w-12 h-12 text-slate-300 mb-3" />
                            <p className="font-medium text-slate-600">Listenizde ürün bulunamadı.</p>
                            <p className="text-sm text-slate-400 mt-1">Arama kriterlerinizi değiştirin veya yeni ürün ekleyin.</p>
                        </div>
                    </td>
                    </tr>
                )}
                </tbody>
            </table>
          </div>
        )}

        {/* PAGINATION */}
        {!loading && totalPages > 1 && (
          <div className="flex items-center justify-between px-6 py-4 border-t border-slate-200 bg-white">
            <span className="text-sm text-slate-500">
              Toplam {totalPages} sayfa, şu an {page}. sayfadasınız
            </span>
            <div className="flex gap-2">
              <button
                onClick={() => setPage(Math.max(1, page - 1))}
                disabled={page <= 1}
                className="px-4 py-2 rounded-xl border border-slate-200 text-sm font-medium hover:bg-slate-50 disabled:opacity-50 disabled:cursor-not-allowed transition"
              >
                Önceki
              </button>
              <button
                onClick={() => setPage(Math.min(totalPages, page + 1))}
                disabled={page >= totalPages}
                className="px-4 py-2 rounded-xl border border-slate-200 text-sm font-medium hover:bg-slate-50 disabled:opacity-50 disabled:cursor-not-allowed transition"
              >
                Sonraki
              </button>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}