// src/pages/seller/SellerOrdersPage.jsx
import { useEffect } from "react";
import { useSellerOrderStore } from "../../store/seller/SellerOrderStore";
import {
  TruckIcon,
  ClockIcon,
  CheckCircleIcon,
} from "@heroicons/react/24/outline";

const StatusBadge = ({ status }) => {
  const map = {
    AwaitingPayment: "bg-slate-100 text-slate-700",
    Processing: "bg-blue-50 text-blue-700",
    Shipped: "bg-orange-50 text-orange-700",
    Delivered: "bg-emerald-50 text-emerald-700",
    Cancelled: "bg-rose-50 text-rose-700",
  };

  return (
    <span
      className={`px-3 py-1 rounded-full text-xs font-bold ${
        map[status] || "bg-gray-100"
      }`}
    >
      {status}
    </span>
  );
};

export default function SellerOrdersPage() {
  const {
    items,
    loading,
    error,
    fetchOrders,
    page,
    totalPages,
    setPage,
    updateOrderStatus,
  } = useSellerOrderStore();

  useEffect(() => {
    fetchOrders();
  }, [page]);

  return (
    <div className="min-h-screen bg-slate-50/50 p-6 lg:p-10">
      <h1 className="text-3xl font-bold mb-6">Siparişler</h1>

      {error && (
        <div className="mb-4 bg-rose-50 border border-rose-200 text-rose-700 p-3 rounded-xl">
          {error}
        </div>
      )}

      <div className="bg-white rounded-2xl border shadow-sm overflow-hidden">
        {loading ? (
          <div className="p-10 text-center text-slate-500">
            Siparişler yükleniyor...
          </div>
        ) : (
          <table className="w-full">
            <thead className="bg-slate-50 border-b text-xs text-slate-500 uppercase">
              <tr>
                <th className="px-6 py-4">Sipariş</th>
                <th className="px-6 py-4">Tutar</th>
                <th className="px-6 py-4">Durum</th>
                <th className="px-6 py-4">Tarih</th>
                <th className="px-6 py-4 text-right">İşlem</th>
              </tr>
            </thead>

            <tbody className="divide-y">
              {items.map((o) => (
                <tr key={o.orderId} className="hover:bg-slate-50">
                  <td className="px-6 py-4 font-semibold">
                    {o.orderNumber}
                  </td>

                  <td className="px-6 py-4">
                    {o.totalAmount.toFixed(2)} ₺
                  </td>

                  <td className="px-6 py-4">
                    <StatusBadge status={o.orderStatus} />
                  </td>

                  <td className="px-6 py-4 text-sm text-slate-500">
                    {new Date(o.createdAt).toLocaleDateString("tr-TR")}
                  </td>

                  <td className="px-6 py-4 text-right">
                    {o.orderStatus === "Processing" && (
                      <button
                        onClick={() =>
                          updateOrderStatus(o.orderId, {
                            newStatus: "Shipped",
                          })
                        }
                        className="inline-flex items-center gap-2 px-4 py-2 bg-orange-600 text-white rounded-xl hover:bg-orange-700"
                      >
                        <TruckIcon className="w-4 h-4" />
                        Kargoya Ver
                      </button>
                    )}

                    {o.orderStatus === "Shipped" && (
                      <span className="inline-flex items-center gap-1 text-sm text-orange-600">
                        <ClockIcon className="w-4 h-4" />
                        Yolda
                      </span>
                    )}

                    {o.orderStatus === "Delivered" && (
                      <span className="inline-flex items-center gap-1 text-sm text-emerald-600">
                        <CheckCircleIcon className="w-4 h-4" />
                        Teslim Edildi
                      </span>
                    )}
                  </td>
                </tr>
              ))}

              {items.length === 0 && (
                <tr>
                  <td
                    colSpan="5"
                    className="px-6 py-10 text-center text-slate-400"
                  >
                    Sipariş bulunamadı.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        )}

        {!loading && totalPages > 1 && (
          <div className="flex justify-between px-6 py-4 border-t">
            <button
              disabled={page === 1}
              onClick={() => setPage(page - 1)}
              className="px-3 py-2 border rounded-lg disabled:opacity-50"
            >
              Geri
            </button>

            <span className="text-sm text-slate-500">
              Sayfa {page} / {totalPages}
            </span>

            <button
              disabled={page === totalPages}
              onClick={() => setPage(page + 1)}
              className="px-3 py-2 border rounded-lg disabled:opacity-50"
            >
              İleri
            </button>
          </div>
        )}
      </div>
    </div>
  );
}