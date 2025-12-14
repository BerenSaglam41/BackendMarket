import AdminOrderRow from "./AdminOrderRow";
import { InboxIcon } from "@heroicons/react/24/outline";

export default function AdminOrderTable({ orders, loading }) {
  if (loading && orders.length === 0) {
    return (
      <div className="bg-white rounded-2xl border border-gray-100 p-12 text-center shadow-sm">
        <div className="w-12 h-12 border-4 border-orange-100 border-t-orange-500 rounded-full animate-spin mx-auto mb-4"></div>
        <p className="text-gray-500 text-sm font-medium">Siparişler yükleniyor...</p>
      </div>
    );
  }

  if (!loading && orders.length === 0) {
    return (
      <div className="bg-white rounded-2xl border border-gray-100 p-16 text-center shadow-sm flex flex-col items-center">
        <div className="bg-gray-50 p-4 rounded-full mb-4">
          <InboxIcon className="w-8 h-8 text-gray-400" />
        </div>
        <h3 className="text-gray-900 font-bold mb-1">Kayıt Bulunamadı</h3>
        <p className="text-gray-500 text-sm">Seçilen filtrelere uygun sipariş yok.</p>
      </div>
    );
  }

  return (
    <div className="bg-white border border-gray-200 rounded-2xl shadow-sm overflow-hidden">
      <div className="overflow-x-auto">
        <table className="w-full text-left border-collapse">
          <thead className="bg-gray-50/50 border-b border-gray-100">
            <tr>
              <th className="p-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">Sipariş No</th>
              <th className="p-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">Müşteri / Ürün</th>
              <th className="p-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">Tutar</th>
              <th className="p-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">Ödeme</th>
              <th className="p-4 text-xs font-semibold text-gray-500 uppercase tracking-wider">Durum</th>
              <th className="p-4 text-xs font-semibold text-gray-500 uppercase tracking-wider text-right">İşlem</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-50">
            {orders.map((order) => (
              <AdminOrderRow key={order.orderId} order={order} />
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}