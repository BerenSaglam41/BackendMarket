import { EyeIcon } from "@heroicons/react/24/outline";
import { useOrderStore } from "../../../store/orderStore";

// --- HELPERS (Güvenli Hale Getirildi) ---

const formatCurrency = (val) => {
  if (val === undefined || val === null) return "-";
  return new Intl.NumberFormat("tr-TR", { 
    style: "currency", 
    currency: "TRY" 
  }).format(Number(val)); // String gelirse Number'a çevir
};

const formatDate = (dateStr) => {
  if (!dateStr) return "-";
  try {
    return new Date(dateStr).toLocaleDateString("tr-TR", {
      day: "numeric",
      month: "short",
      year: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  } catch (e) {
    return "Tarih Hatalı";
  }
};

// Renkleri dışarı aldık (Performans için her render'da tekrar tanımlanmasın)
const STATUS_STYLES = {
  // Sipariş Durumları
  Processing: "bg-blue-50 text-blue-700 ring-blue-600/20",
  Shipped: "bg-purple-50 text-purple-700 ring-purple-600/20",
  Delivered: "bg-emerald-50 text-emerald-700 ring-emerald-600/20",
  Cancelled: "bg-red-50 text-red-700 ring-red-600/20",
  AwaitingPayment: "bg-amber-50 text-amber-700 ring-amber-600/20",
  // Ödeme Durumları
  Paid: "bg-emerald-50 text-emerald-700 ring-emerald-600/20",
  Pending: "bg-amber-50 text-amber-700 ring-amber-600/20",
  Failed: "bg-red-50 text-red-700 ring-red-600/20",
};

const StatusBadge = ({ status }) => {
  // Eğer status listede yoksa gri (default) renk ver
  const className = STATUS_STYLES[status] || "bg-gray-50 text-gray-600 ring-gray-500/10";

  return (
    <span className={`inline-flex items-center rounded-md px-2 py-1 text-xs font-medium ring-1 ring-inset ${className}`}>
      {status || "-"}
    </span>
  );
};

export default function AdminOrderRow({ order }) {
  // Eğer store güncellenmediyse hata vermemesi için güvenli seçim
  const setSelectedOrder = useOrderStore((s) => s.setSelectedOrder);

  // Veri yoksa boş dön (Render hatasını önler)
  if (!order) return null;

  // Müşteri Gösterim Mantığı
  const getCustomerDisplay = () => {
    if (order.shippingAddress?.contactName) return order.shippingAddress.contactName;
    if (order.shippingAddress?.contactPhone) return order.shippingAddress.contactPhone;
    return `Müşteri #${order.orderId}`; // Hiçbiri yoksa ID göster
  };

  return (
    <tr className="border-b border-gray-50 hover:bg-gray-50/50 transition-colors group">
      
      {/* SİPARİŞ NO */}
      <td className="p-4">
        <div className="font-bold text-gray-900 text-sm font-mono">
          {order.orderNumber}
        </div>
        <div className="text-xs text-gray-400 mt-0.5">
          {formatDate(order.createdAt)}
        </div>
      </td>

      {/* MÜŞTERİ */}
      <td className="p-4">
        <div className="text-sm font-medium text-gray-700">
           {getCustomerDisplay()}
        </div>
        <div className="text-xs text-gray-400">
            {order.items?.length || 0} Ürün
        </div>
      </td>

      {/* TUTAR */}
      <td className="p-4 text-sm font-bold text-gray-900">
        {formatCurrency(order.totalAmount)}
      </td>

      {/* ÖDEME */}
      <td className="p-4">
        <StatusBadge status={order.paymentStatus} />
      </td>

      {/* DURUM */}
      <td className="p-4">
        <StatusBadge status={order.orderStatus} />
      </td>

      {/* AKSİYON */}
      <td className="p-4 text-right">
        <button
          onClick={() => {
            if (setSelectedOrder) {
                setSelectedOrder(order);
            } else {
                console.error("setSelectedOrder store içinde bulunamadı! Lütfen orderStore.js dosyasını güncelleyin.");
            }
          }}
          className="p-2 bg-white border border-gray-200 rounded-lg text-gray-500 hover:text-orange-600 hover:border-orange-200 hover:shadow-sm transition active:scale-95"
          title="Detayları Gör"
        >
          <EyeIcon className="w-4 h-4" />
        </button>
      </td>
    </tr>
  );
}