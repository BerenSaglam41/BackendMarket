import { useOrderStore } from "../../../store/orderStore";
import { FunnelIcon, ArrowPathIcon } from "@heroicons/react/24/outline";

export default function AdminOrderFilters() {
  const { filters, setFilters, fetchAdminOrders, loading } = useOrderStore();

  return (
    <div className="bg-white p-5 rounded-2xl border border-gray-100 shadow-sm flex flex-col md:flex-row gap-4 items-end justify-between">
      
      <div className="flex gap-4 w-full md:w-auto">
        {/* ORDER STATUS */}
        <div className="flex-1 md:w-48">
          <label className="block text-xs font-semibold text-gray-500 mb-1.5 ml-1">
            Sipariş Durumu
          </label>
          <div className="relative">
            <select
              value={filters.orderStatus}
              onChange={(e) => setFilters({ orderStatus: e.target.value })}
              className="w-full appearance-none bg-gray-50 border border-gray-200 text-gray-700 text-sm rounded-xl focus:ring-orange-500 focus:border-orange-500 block p-2.5 outline-none transition"
            >
              <option value="">Tümü</option>
              <option value="AwaitingPayment">Ödeme Bekliyor</option>
              <option value="Processing">Hazırlanıyor</option>
              <option value="Shipped">Kargoya Verildi</option>
              <option value="Delivered">Teslim Edildi</option>
              <option value="Cancelled">İptal Edildi</option>
            </select>
            <div className="pointer-events-none absolute inset-y-0 right-0 flex items-center px-2 text-gray-500">
              <svg className="fill-current h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"><path d="M9.293 12.95l.707.707L15.657 8l-1.414-1.414L10 10.828 5.757 6.586 4.343 8z"/></svg>
            </div>
          </div>
        </div>

        {/* PAYMENT STATUS */}
        <div className="flex-1 md:w-48">
          <label className="block text-xs font-semibold text-gray-500 mb-1.5 ml-1">
            Ödeme Durumu
          </label>
          <div className="relative">
            <select
              value={filters.paymentStatus}
              onChange={(e) => setFilters({ paymentStatus: e.target.value })}
              className="w-full appearance-none bg-gray-50 border border-gray-200 text-gray-700 text-sm rounded-xl focus:ring-orange-500 focus:border-orange-500 block p-2.5 outline-none transition"
            >
              <option value="">Tümü</option>
              <option value="Paid">Ödendi</option>
              <option value="Pending">Bekliyor</option>
              <option value="Failed">Başarısız</option>
            </select>
            <div className="pointer-events-none absolute inset-y-0 right-0 flex items-center px-2 text-gray-500">
              <svg className="fill-current h-4 w-4" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20"><path d="M9.293 12.95l.707.707L15.657 8l-1.414-1.414L10 10.828 5.757 6.586 4.343 8z"/></svg>
            </div>
          </div>
        </div>
      </div>

      <button
        onClick={() => fetchAdminOrders()}
        disabled={loading}
        className="flex items-center gap-2 bg-slate-900 text-white px-6 py-2.5 rounded-xl text-sm font-medium hover:bg-slate-800 transition active:scale-95 disabled:opacity-70"
      >
        {loading ? <ArrowPathIcon className="w-4 h-4 animate-spin"/> : <FunnelIcon className="w-4 h-4" />}
        Sonuçları Getir
      </button>
    </div>
  );
}