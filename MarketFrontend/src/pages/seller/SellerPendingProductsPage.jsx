import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import {
  PlusIcon,
  PencilSquareIcon,
  TrashIcon,
  EyeIcon,
  MagnifyingGlassIcon,
  FunnelIcon,
  PhotoIcon,
  ExclamationTriangleIcon,
  ClockIcon,
  CheckCircleIcon,
  XMarkIcon,
  ArrowPathIcon,
  InboxIcon
} from "@heroicons/react/24/outline";
import { useSellerPendingProductStore } from "../../store/seller/SellerPendingProductStore";

// Durum Seçenekleri
const STATUS_OPTIONS = [
  { value: "all", label: "Tümü" },
  { value: "Waiting", label: "Beklemede" },
  { value: "NeedsUpdate", label: "Güncelleme İstendi" },
  { value: "Rejected", label: "Reddedildi" },
];

// Badge Bileşeni
function StatusBadge({ status }) {
  const map = {
    Waiting: {
      text: "Beklemede",
      cls: "bg-orange-50 text-orange-700 border-orange-100",
      Icon: ClockIcon,
    },
    NeedsUpdate: {
      text: "Güncelleme İsteniyor",
      cls: "bg-amber-50 text-amber-700 border-amber-100",
      Icon: ExclamationTriangleIcon,
    },
    Approved: {
      text: "Onaylandı",
      cls: "bg-emerald-50 text-emerald-700 border-emerald-100",
      Icon: CheckCircleIcon,
    },
    Rejected: {
      text: "Reddedildi",
      cls: "bg-rose-50 text-rose-700 border-rose-100",
      Icon: ExclamationTriangleIcon,
    },
  };

  const item = map[status] || {
    text: status || "-",
    cls: "bg-slate-100 text-slate-600 border-slate-200",
    Icon: ClockIcon,
  };

  const Icon = item.Icon;

  return (
    <span className={`inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-bold border ${item.cls}`}>
      <Icon className="w-4 h-4" />
      {item.text}
    </span>
  );
}

export default function SellerPendingProductsPage() {
  const {
    items,
    loading,
    error,
    fetchPendings,
    deletePending,
    filters,
    setFilters,
    page,
    totalPages,
    setPage,
  } = useSellerPendingProductStore();

  const [searchTerm, setSearchTerm] = useState("");
  const [showFilters, setShowFilters] = useState(false);

  useEffect(() => {
    fetchPendings();
  }, [filters.status, page]);

  // Client-side search logic (Backend destekleyene kadar)
  const filteredItems = useMemo(() => {
    const q = searchTerm.trim().toLowerCase();
    if (!q) return items;
    return items.filter((p) => {
      return (
        (p.name || "").toLowerCase().includes(q) ||
        (p.slug || "").toLowerCase().includes(q) ||
        (p.categoryName || "").toLowerCase().includes(q) ||
        (p.brandName || "").toLowerCase().includes(q)
      );
    });
  }, [items, searchTerm]);

  // Yetki Kontrolleri
  const canEdit = (status) => status === "Waiting" || status === "NeedsUpdate";
  const canDelete = (status) => status === "Waiting" || status === "NeedsUpdate";

  // Filtre Temizleme
  const clearFilters = () => {
      setSearchTerm("");
      setFilters({ status: "all" });
  };

  return (
    <div className="min-h-screen bg-slate-50/50 p-6 lg:p-10 font-sans text-slate-800">
      
      {/* HEADER & ACTIONS */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 mb-8">
        <div>
          <h1 className="text-3xl font-bold text-slate-900 tracking-tight">Ürün Başvurularım</h1>
          <p className="text-slate-500 mt-1">
            Satışa açılmasını istediğiniz ürünleri buradan yönetin.
          </p>
        </div>

        <div className="flex gap-3">
          <button
            onClick={() => setShowFilters((v) => !v)}
            className={`flex items-center gap-2 px-4 py-2 border rounded-xl transition shadow-sm font-medium ${
                showFilters 
                ? "bg-orange-50 border-orange-200 text-orange-700" 
                : "bg-white border-slate-200 text-slate-600 hover:bg-slate-50"
            }`}
          >
            <FunnelIcon className="w-5 h-5" />
            Filtrele
          </button>

          <Link
            to="/seller/pending-products/add"
            className="flex items-center gap-2 bg-orange-600 text-white px-5 py-2 rounded-xl hover:bg-orange-700 shadow-lg shadow-orange-600/20 transition active:scale-95 font-medium"
          >
            <PlusIcon className="w-5 h-5" />
            Ürün Başvurusu Yap
          </Link>
        </div>
      </div>

      {/* FILTER PANEL */}
      {showFilters && (
        <div className="bg-white p-5 rounded-2xl border border-orange-100 shadow-sm mb-6 animate-in slide-in-from-top-2 duration-200">
           <div className="flex flex-col md:flex-row md:items-end gap-4">
              <div className="flex-1 max-w-xs">
                <label className="block text-xs font-semibold text-slate-500 mb-1.5 uppercase">Başvuru Durumu</label>
                <select
                  value={filters.status}
                  onChange={(e) => setFilters({ status: e.target.value })}
                  className="w-full px-3 py-2.5 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-orange-500 outline-none text-sm"
                >
                  {STATUS_OPTIONS.map((o) => (
                    <option key={o.value} value={o.value}>
                      {o.label}
                    </option>
                  ))}
                </select>
              </div>

              <button 
                onClick={clearFilters}
                className="px-4 py-2.5 border border-slate-200 text-slate-500 rounded-xl hover:bg-slate-50 hover:text-rose-600 transition flex items-center gap-2 text-sm font-medium"
              >
                <ArrowPathIcon className="w-4 h-4" />
                Temizle
              </button>
           </div>
           <div className="mt-3 text-xs text-slate-400 bg-slate-50 p-2 rounded-lg inline-block">
             Not: Arama işlemi şimdilik sadece görüntülenen sayfada çalışır.
           </div>
        </div>
      )}

      {/* SEARCH BAR */}
      <div className="bg-white p-4 rounded-2xl border border-slate-200 shadow-sm mb-6">
        <div className="relative max-w-md">
          <MagnifyingGlassIcon className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400" />
          <input
            type="text"
            placeholder="Ürün adı, slug, kategori veya marka ara..."
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
          <ExclamationTriangleIcon className="w-6 h-6 shrink-0"/>
          <span className="text-sm font-medium">{error}</span>
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
            <table className="w-full text-left border-collapse min-w-[800px]">
              <thead className="bg-slate-50 border-b border-slate-200 text-xs uppercase text-slate-500 font-semibold tracking-wider">
                <tr>
                  <th className="px-6 py-4">Ürün Bilgisi</th>
                  <th className="px-6 py-4">Kategori / Marka</th>
                  <th className="px-6 py-4">Fiyat / Stok</th>
                  <th className="px-6 py-4 text-center">Durum</th>
                  <th className="px-6 py-4 text-right">İşlem</th>
                </tr>
              </thead>

              <tbody className="divide-y divide-slate-100">
                {filteredItems.map((p) => (
                  <tr key={p.productPendingId} className="group hover:bg-slate-50/60 transition-colors">
                    
                    {/* Ürün & Resim */}
                    <td className="px-6 py-4">
                      <div className="flex items-center gap-4">
                        <div className="w-12 h-12 rounded-lg bg-slate-100 border border-slate-200 flex items-center justify-center overflow-hidden shrink-0">
                          {p.imageUrl ? (
                            <img src={p.imageUrl} alt="" className="w-full h-full object-cover" />
                          ) : (
                            <PhotoIcon className="w-6 h-6 text-slate-300" />
                          )}
                        </div>
                        <div className="min-w-0 max-w-[200px]">
                          <div className="font-bold text-slate-900 truncate" title={p.name}>{p.name}</div>
                          <div className="text-xs text-slate-400 font-mono mt-0.5 truncate">{p.slug}</div>

                          {/* Admin Notu Varsa Göster */}
                          {(p.status === "Rejected" || p.status === "NeedsUpdate") && p.adminNote && (
                            <div className="mt-2 text-xs text-rose-600 bg-rose-50 border border-rose-100 rounded-lg p-1.5 px-2 line-clamp-1 group-hover:line-clamp-none transition-all">
                              <span className="font-semibold">Admin:</span> {p.adminNote}
                            </div>
                          )}
                        </div>
                      </div>
                    </td>

                    {/* Kategori */}
                    <td className="px-6 py-4">
                      <div className="flex flex-col">
                        <span className="text-sm font-medium text-slate-700">
                          {p.categoryName || p.sellerCategorySuggestion || "-"}
                        </span>
                        <span className="text-xs text-slate-400">{p.brandName || "-"}</span>
                      </div>
                    </td>

                    {/* Fiyat Stok */}
                    <td className="px-6 py-4 text-sm text-slate-600">
                      <div className="flex flex-col">
                        <span className="font-semibold text-slate-900">
                            {p.proposedPrice ? `${p.proposedPrice} ₺` : "-"}
                        </span>
                        <span className="text-xs text-slate-400">
                          Stok: {p.proposedStock ?? "-"} • {p.shippingTimeInDays ?? "-"} gün
                        </span>
                      </div>
                    </td>

                    {/* Durum */}
                    <td className="px-6 py-4 text-center">
                      <StatusBadge status={p.status} />
                    </td>

                    {/* Aksiyonlar */}
                    <td className="px-6 py-4 text-right">
                      <div className="flex items-center justify-end gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                        <Link
                          to={`/seller/pending-products/${p.productPendingId}`}
                          className="p-2 text-slate-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition"
                          title="Detay Görüntüle"
                        >
                          <EyeIcon className="w-4 h-4" />
                        </Link>

                        <Link
                          to={`/seller/pending-products/edit/${p.productPendingId}`}
                          className={`p-2 rounded-lg transition ${
                            canEdit(p.status)
                              ? "text-slate-400 hover:text-orange-600 hover:bg-orange-50"
                              : "text-slate-200 cursor-not-allowed"
                          }`}
                          title={canEdit(p.status) ? "Düzenle" : "Bu statüde düzenlenemez"}
                          onClick={(e) => {
                            if (!canEdit(p.status)) e.preventDefault();
                          }}
                        >
                          <PencilSquareIcon className="w-4 h-4" />
                        </Link>

                        <button
                          onClick={() => {
                            if (!canDelete(p.status)) return;
                            if (window.confirm("Bu başvuruyu silmek istediğinize emin misiniz?")) {
                              deletePending(p.productPendingId);
                            }
                          }}
                          className={`p-2 rounded-lg transition ${
                            canDelete(p.status)
                              ? "text-slate-400 hover:text-rose-600 hover:bg-rose-50"
                              : "text-slate-200 cursor-not-allowed"
                          }`}
                          title={canDelete(p.status) ? "Sil" : "Bu statüde silinemez"}
                        >
                          <TrashIcon className="w-4 h-4" />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}

                {filteredItems.length === 0 && !loading && (
                  <tr>
                    <td colSpan="5" className="px-6 py-16 text-center text-slate-400">
                      <div className="flex flex-col items-center justify-center">
                          <InboxIcon className="w-12 h-12 text-slate-300 mb-3" />
                          <p>Kriterlere uygun ürün başvurusu bulunamadı.</p>
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