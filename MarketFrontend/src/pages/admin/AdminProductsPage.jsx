import { useEffect, useState } from "react";
import { useAdminProductStore } from "../../store/admin/adminProductStore";
import {
  PlusIcon,
  PencilSquareIcon,
  TrashIcon,
  EyeIcon,
  MagnifyingGlassIcon,
  FunnelIcon,
  PhotoIcon,
  EllipsisHorizontalIcon
} from "@heroicons/react/24/outline";
import { Link } from "react-router-dom";

export default function AdminProductsPage() {
  const {
    items: products,
    loading,
    fetchProducts,
    deleteProduct,
    toggleProductStatus, // Store'da bu fonksiyonun olduğunu varsayıyoruz
    setFilters
  } = useAdminProductStore();

  const [searchTerm, setSearchTerm] = useState("");

  useEffect(() => {
    fetchProducts();
  }, []);

  // Basit bir Debounce simülasyonu veya Enter'a basınca arama
  const handleSearch = (e) => {
    e.preventDefault();
    setFilters({ search: searchTerm });
    fetchProducts();
  };

  return (
    <div className="min-h-screen bg-slate-50/50 p-6 lg:p-10 font-sans text-slate-800">
      
      {/* HEADER & ACTIONS */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 mb-8">
        <div>
          <h1 className="text-3xl font-bold text-slate-900 tracking-tight">Ürünler</h1>
          <p className="text-slate-500 mt-1">Mağazanızdaki tüm ürünleri buradan yönetin.</p>
        </div>
        
        <div className="flex gap-3">
            <button className="flex items-center gap-2 px-4 py-2 bg-white border border-slate-200 text-slate-600 rounded-xl hover:bg-slate-50 transition shadow-sm font-medium">
                <FunnelIcon className="w-5 h-5"/>
                Filtrele
            </button>
            <Link
            to="/admin/products/add"
            className="flex items-center gap-2 bg-orange-600 text-white px-5 py-2 rounded-xl hover:bg-orange-700 shadow-lg shadow-orange-600/20 transition active:scale-95 font-medium"
            >
            <PlusIcon className="w-5 h-5" />
            Ürün Ekle
            </Link>
        </div>
      </div>

      {/* SEARCH BAR */}
      <div className="bg-white p-4 rounded-2xl border border-slate-200 shadow-sm mb-6">
        <form onSubmit={handleSearch} className="relative max-w-md">
            <MagnifyingGlassIcon className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400"/>
            <input 
                type="text" 
                placeholder="Ürün adı, kodu veya kategori ara..." 
                className="w-full pl-10 pr-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-orange-500 focus:bg-white transition outline-none text-sm"
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
            />
        </form>
      </div>

      {/* TABLE */}
      <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-hidden">
        {loading ? (
             <div className="p-10 flex justify-center">
                 <div className="w-8 h-8 border-4 border-slate-200 border-t-orange-500 rounded-full animate-spin"></div>
             </div>
        ) : (
        <table className="w-full text-left border-collapse">
          <thead className="bg-slate-50 border-b border-slate-200 text-xs uppercase text-slate-500 font-semibold tracking-wider">
            <tr>
              <th className="px-6 py-4">Ürün Bilgisi</th>
              <th className="px-6 py-4">Kategori / Marka</th>
              <th className="px-6 py-4">Satıcı</th>
              <th className="px-6 py-4 text-center">Durum</th>
              <th className="px-6 py-4 text-right">İşlem</th>
            </tr>
          </thead>

          <tbody className="divide-y divide-slate-100">
            {products.map((p) => (
              <tr key={p.productId} className="group hover:bg-slate-50/80 transition-colors">
                
                {/* Product Name & Image */}
                <td className="px-6 py-4">
                  <div className="flex items-center gap-4">
                    <div className="w-12 h-12 rounded-lg bg-slate-100 border border-slate-200 flex items-center justify-center overflow-hidden shrink-0">
                        {p.imageUrl ? (
                            <img src={p.imageUrl} alt="" className="w-full h-full object-cover"/>
                        ) : (
                            <PhotoIcon className="w-6 h-6 text-slate-300" />
                        )}
                    </div>
                    <div>
                        <div className="font-bold text-slate-900 line-clamp-1">{p.name}</div>
                        <div className="text-xs text-slate-400 font-mono mt-0.5">{p.slug}</div>
                    </div>
                  </div>
                </td>

                <td className="px-6 py-4">
                    <div className="flex flex-col">
                        <span className="text-sm font-medium text-slate-700">{p.categoryName || "-"}</span>
                        <span className="text-xs text-slate-400">{p.brandName || "-"}</span>
                    </div>
                </td>

                <td className="px-6 py-4 text-sm text-slate-600">
                  {p.store ? (
                      <span className="flex items-center gap-1.5">
                          <span className="w-2 h-2 rounded-full bg-blue-500"></span>
                          {p.store.storeName}
                      </span>
                  ) : (
                      <span className="flex items-center gap-1.5">
                          <span className="w-2 h-2 rounded-full bg-orange-500"></span>
                          Admin
                      </span>
                  )}
                </td>

                <td className="px-6 py-4 text-center">
                  <button
                    // onClick={() => toggleProductStatus(p.productId)}
                    className={`inline-flex px-2.5 py-1 rounded-full text-xs font-bold border ${
                      p.isActive
                        ? "bg-emerald-50 text-emerald-700 border-emerald-100"
                        : "bg-slate-100 text-slate-600 border-slate-200"
                    }`}
                  >
                    {p.isActive ? "Yayında" : "Taslak"}
                  </button>
                </td>

                <td className="px-6 py-4 text-right">
                    <div className="flex items-center justify-end gap-2 opacity-0 group-hover:opacity-100 transition-opacity">
                        <Link
                            to={`/admin/products/${p.slug}`}
                            className="p-2 text-slate-400 hover:text-blue-600 hover:bg-blue-50 rounded-lg transition"
                            title="Görüntüle"
                        >
                            <EyeIcon className="w-4 h-4" />
                        </Link>

                        <Link
                            to={`/admin/products/edit/${p.slug}`}
                            className="p-2 text-slate-400 hover:text-orange-600 hover:bg-orange-50 rounded-lg transition"
                            title="Düzenle"
                        >
                            <PencilSquareIcon className="w-4 h-4" />
                        </Link>

                        <button
                            onClick={() => {
                                if(window.confirm("Bu ürünü silmek istediğinize emin misiniz?")) 
                                    deleteProduct(p.productId);
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
            {products.length === 0 && !loading && (
                <tr>
                    <td colSpan="5" className="px-6 py-12 text-center text-slate-400">
                        Aradığınız kriterlere uygun ürün bulunamadı.
                    </td>
                </tr>
            )}
          </tbody>
        </table>
        )}
      </div>
    </div>
  );
}