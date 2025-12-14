import { useEffect } from "react";
import { useOrderStore } from "../../store/orderStore";
import AdminOrderFilters from "../../components/admin/orders/AdminOrderFilters";
import AdminOrderTable from "../../components/admin/orders/AdminOrderTable";
import AdminOrderDetailsModal from "../../components/admin/orders/AdminOrderDetailsModal"; // Yeni Modal

export default function AdminOrdersPage() {
  const {
    orders,
    loading,
    fetchAdminOrders,
    page,
    pageSize,
    totalCount,
    setPage,
  } = useOrderStore();

  useEffect(() => {
    fetchAdminOrders();
  }, [page]); // Filters değiştiğinde store zaten page'i 1 yapıyor, bu yeterli.

  return (
    <div className="p-6 md:p-8 max-w-[1600px] mx-auto space-y-6">
      
      {/* PAGE HEADER */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold text-gray-900 tracking-tight">Sipariş Yönetimi</h1>
          <p className="text-slate-500 mt-1">
            Toplam <span className="font-bold text-slate-900">{totalCount}</span> sipariş listeleniyor.
          </p>
        </div>
        {/* Opsiyonel: Dışa Aktar butonu vb. buraya gelebilir */}
      </div>

      {/* FILTERS */}
      <AdminOrderFilters />

      {/* TABLE */}
      <AdminOrderTable
        orders={orders}
        loading={loading}
      />

      {/* PAGINATION */}
      {!loading && totalCount > pageSize && (
        <div className="flex justify-between items-center bg-white p-4 rounded-xl border border-gray-200 shadow-sm">
           <span className="text-sm text-gray-500">
              Sayfa {page} / {Math.ceil(totalCount / pageSize)}
           </span>
           <div className="flex gap-2">
            <button
                disabled={page === 1}
                onClick={() => setPage(page - 1)}
                className="px-4 py-2 text-sm border rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition"
            >
                Önceki
            </button>
            <button
                disabled={page * pageSize >= totalCount}
                onClick={() => setPage(page + 1)}
                className="px-4 py-2 text-sm border rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition"
            >
                Sonraki
            </button>
          </div>
        </div>
      )}

      {/* DETAILS MODAL */}
      <AdminOrderDetailsModal />
      
    </div>
  );
}