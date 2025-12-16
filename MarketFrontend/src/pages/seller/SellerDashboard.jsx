import { useEffect } from "react";
import { Link } from "react-router-dom";
import {
  BanknotesIcon,
  ShoppingBagIcon,
  CubeIcon,
  ClockIcon,
  ArrowPathIcon,
  PlusIcon,
  ListBulletIcon,
  ExclamationTriangleIcon,
  CheckCircleIcon,
  XCircleIcon,
  ArrowRightIcon,
  EllipsisHorizontalIcon
} from "@heroicons/react/24/outline";

import { useSellerDashboardStore } from "../../store/seller/SellerDashboardStore";

// --- CUSTOM ICONS ---
const TurkishLiraIcon = ({ className }) => (
  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className={className}>
    <path strokeLinecap="round" strokeLinejoin="round" d="M8 7h9M8 11h9M9.5 20V4.5" />
    <path strokeLinecap="round" strokeLinejoin="round" d="M6 5l3.5-1" />
  </svg>
);

// --- HELPERS ---
const formatDate = (dateString) => {
    if (!dateString) return "-";
    return new Date(dateString).toLocaleDateString("tr-TR", { month: "short", day: "numeric", hour: "2-digit", minute: "2-digit" });
};

const formatCurrency = (amount) => new Intl.NumberFormat("tr-TR", { style: "currency", currency: "TRY" }).format(amount || 0);

// --- SUB COMPONENTS ---

const PremiumStatCard = ({ title, value, icon: Icon, color, subText }) => {
    const colorClasses = {
        blue: "text-blue-600 bg-blue-50/50",
        orange: "text-orange-600 bg-orange-50/50",
        emerald: "text-emerald-600 bg-emerald-50/50",
        amber: "text-amber-600 bg-amber-50/50",
        rose: "text-rose-600 bg-rose-50/50",
    };

    return (
        <div className="group bg-white rounded-2xl p-5 border border-slate-100 shadow-[0_2px_10px_-4px_rgba(0,0,0,0.05)] hover:shadow-[0_8px_30px_-12px_rgba(0,0,0,0.1)] transition-all duration-300">
            <div className="flex justify-between items-start mb-4">
                <div className={`p-3 rounded-xl ${colorClasses[color] || "text-gray-600 bg-gray-50"} transition-colors`}>
                    <Icon className="w-6 h-6" />
                </div>
            </div>
            <div>
                <h4 className="text-slate-500 text-sm font-medium mb-1">{title}</h4>
                <div className="text-3xl font-bold text-slate-800 tracking-tight">{value}</div>
                {subText && <div className="text-xs text-slate-400 mt-2 font-medium">{subText}</div>}
            </div>
        </div>
    );
};

const ActionCard = ({ title, count, icon: Icon, color, link, label }) => {
    const gradients = {
        orange: "from-orange-500 to-amber-500",
        red: "from-rose-500 to-pink-500",
        blue: "from-blue-500 to-indigo-500",
    };

    return (
        <Link to={link} className="relative overflow-hidden bg-white rounded-2xl p-6 border border-slate-100 shadow-sm hover:shadow-md transition-all group">
            <div className="relative z-10 flex items-center justify-between">
                <div>
                    <p className="text-slate-500 text-sm font-medium">{title}</p>
                    <h3 className="text-2xl font-bold text-slate-800 mt-1 group-hover:text-transparent group-hover:bg-clip-text group-hover:bg-gradient-to-r group-hover:from-gray-800 group-hover:to-gray-600 transition-all">
                        {count} <span className="text-base font-normal text-slate-400">{label}</span>
                    </h3>
                </div>
                <div className={`w-12 h-12 rounded-full flex items-center justify-center text-white bg-gradient-to-br ${gradients[color]} shadow-lg group-hover:scale-110 transition-transform`}>
                    <Icon className="w-6 h-6" />
                </div>
            </div>
            <div className={`absolute -right-6 -bottom-6 w-24 h-24 bg-gradient-to-br ${gradients[color]} opacity-10 rounded-full blur-2xl group-hover:opacity-20 transition-opacity`} />
        </Link>
    );
};

const StatusBadge = ({ status }) => {
    const styles = {
        Paid: "bg-emerald-100 text-emerald-700 border-emerald-200",
        Processing: "bg-blue-100 text-blue-700 border-blue-200",
        Shipped: "bg-purple-100 text-purple-700 border-purple-200",
        Waiting: "bg-amber-100 text-amber-700 border-amber-200",
        Approved: "bg-emerald-100 text-emerald-700 border-emerald-200",
        Rejected: "bg-rose-100 text-rose-700 border-rose-200",
        NeedsUpdate: "bg-orange-100 text-orange-700 border-orange-200",
    };

    // İkon seçimi
    let Icon = CheckCircleIcon;
    if (status === 'Rejected' || status === 'NeedsUpdate') Icon = ExclamationTriangleIcon;
    if (status === 'Waiting') Icon = ClockIcon;

    return (
        <span className={`inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-bold border ${styles[status] || "bg-slate-100 text-slate-600 border-slate-200"}`}>
            <Icon className="w-3.5 h-3.5" />
            {status}
        </span>
    );
};

const EmptyState = ({ message }) => (
    <div className="p-10 flex flex-col items-center justify-center text-slate-300">
        <ListBulletIcon className="w-10 h-10 mb-2 opacity-50" />
        <span className="text-sm font-medium">{message || "Veri bulunamadı"}</span>
    </div>
);

// --- MAIN PAGE ---

export default function SellerDashboardPage() {
  const { data, loading, error, fetchDashboard } = useSellerDashboardStore();

  useEffect(() => {
    fetchDashboard();
  }, []);

  if (loading) {
    return (
        <div className="flex flex-col items-center justify-center h-screen bg-slate-50 gap-4">
            <div className="relative">
                <div className="w-12 h-12 border-4 border-slate-200 rounded-full"></div>
                <div className="w-12 h-12 border-4 border-orange-500 rounded-full border-t-transparent animate-spin absolute top-0 left-0"></div>
            </div>
            <p className="text-slate-500 font-medium animate-pulse">Mağaza verileri yükleniyor...</p>
        </div>
    );
  }

  if (error) {
    return (
      <div className="p-10 flex justify-center">
          <div className="bg-rose-50 border border-rose-200 text-rose-700 rounded-xl p-6 max-w-lg text-center">
             <ExclamationTriangleIcon className="w-10 h-10 mx-auto mb-3 opacity-50"/>
             <h3 className="font-bold">Bir Hata Oluştu</h3>
             <p className="text-sm mt-1">{error}</p>
             <button onClick={() => fetchDashboard()} className="mt-4 text-sm underline hover:text-rose-900">Tekrar Dene</button>
          </div>
      </div>
    );
  }

  if (!data) return null;

  // Veri Kontrolü (Fallback)
  const recentOrders = data.recentOrders || [];
  const recentPending = data.recentPendingProducts || [];
  
  // Stok Uyarısı Mesajı Oluşturma
  const stockAlertCount = (data.outOfStockListings || 0) + (data.lowStockListings || 0);
  const stockMessage = stockAlertCount > 0 
    ? `${data.outOfStockListings} ürün stokta yok, ${data.lowStockListings} ürün kritik seviyede.` 
    : "Stok durumunuz gayet iyi görünüyor.";

  return (
    <div className="min-h-screen bg-slate-50/50 p-6 lg:p-10 space-y-10 font-sans text-slate-800">
      
      {/* WELCOME BANNER */}
      <div className="relative rounded-3xl bg-slate-900 overflow-hidden shadow-2xl shadow-slate-200">
         {/* Decorative Gradients */}
         <div className="absolute top-0 right-0 w-[500px] h-[500px] bg-emerald-500/20 rounded-full blur-[100px] -translate-y-1/2 translate-x-1/3 mix-blend-screen"></div>
         <div className="absolute bottom-0 left-0 w-[300px] h-[300px] bg-orange-500/20 rounded-full blur-[80px] translate-y-1/2 -translate-x-1/3 mix-blend-screen"></div>

         <div className="relative z-10 px-8 py-10 md:py-12 flex flex-col md:flex-row justify-between items-start md:items-center gap-6">
            <div>
                <h1 className="text-3xl md:text-4xl font-bold text-white tracking-tight">
                    Satıcı Paneli
                </h1>
                <p className="text-slate-400 mt-2 max-w-xl text-lg flex items-center gap-2">
                    {stockAlertCount > 0 ? <ExclamationTriangleIcon className="w-5 h-5 text-amber-500" /> : <CheckCircleIcon className="w-5 h-5 text-emerald-500" />}
                    {stockMessage}
                </p>
            </div>
            <div className="flex items-center gap-3">
                <button 
                    onClick={() => fetchDashboard()} 
                    className="flex items-center gap-2 px-5 py-2.5 bg-white/10 hover:bg-white/20 text-white rounded-xl backdrop-blur-md border border-white/10 transition-all active:scale-95"
                >
                    <ArrowPathIcon className="w-5 h-5" />
                    <span>Yenile</span>
                </button>
                <Link to="/seller/pending-products/add" className="flex items-center gap-2 px-5 py-2.5 bg-orange-600 hover:bg-orange-700 text-white font-semibold rounded-xl shadow-lg shadow-orange-600/30 transition-all active:scale-95">
                    <PlusIcon className="w-5 h-5" />
                    <span>Yeni Başvuru</span>
                </Link>
            </div>
         </div>
      </div>

      {/* STATS GRID */}
      <div>
        <h3 className="text-lg font-bold text-slate-800 mb-5 flex items-center gap-2">
            <ArrowRightIcon className="w-5 h-5 text-orange-600" />
            Performans Özeti
        </h3>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            <PremiumStatCard 
                title="Toplam Ciro" 
                value={formatCurrency(data.totalRevenue)} 
                icon={TurkishLiraIcon} 
                color="emerald"
                subText="Tüm zamanlar"
            />
            <PremiumStatCard 
                title="Toplam Sipariş" 
                value={data.totalOrders} 
                icon={ShoppingBagIcon} 
                color="blue"
                subText="Adet sipariş alındı"
            />
            <PremiumStatCard 
                title="Aktif Ürün" 
                value={data.activeListings} 
                icon={CubeIcon} 
                color="orange"
                subText="Şu an satışta"
            />
            <PremiumStatCard 
                title="Bekleyen Başvuru" 
                value={data.pendingProducts} 
                icon={ClockIcon} 
                color="amber"
                subText="Onay bekliyor"
            />
        </div>
      </div>

      {/* ACTION & ALERTS */}
      {(stockAlertCount > 0 || data.pendingProducts > 0) && (
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
             {stockAlertCount > 0 && (
                 <ActionCard 
                    title="Stok Uyarısı" 
                    count={stockAlertCount} 
                    icon={ExclamationTriangleIcon} 
                    color="red" 
                    link="/seller/products"
                    label="Üründe Sorun Var"
                 />
             )}
             {data.pendingProducts > 0 && (
                 <ActionCard 
                    title="Başvuru Takibi" 
                    count={data.pendingProducts} 
                    icon={ClockIcon} 
                    color="orange" 
                    link="/seller/pending-products"
                    label="Başvuru İnceleniyor"
                 />
             )}
          </div>
      )}

      {/* TABLES LAYOUT */}
      <div className="grid grid-cols-1 xl:grid-cols-2 gap-8">
          
          {/* RECENT ORDERS */}
          <div className="bg-white rounded-3xl border border-slate-100 shadow-sm overflow-hidden flex flex-col h-full">
            <div className="px-8 py-6 border-b border-slate-50 flex justify-between items-center bg-white">
                <div>
                    <h3 className="font-bold text-slate-800 text-lg">Son Siparişler</h3>
                    <p className="text-slate-400 text-sm mt-0.5">Mağazanızdan yapılan son işlemler</p>
                </div>
                <Link to="/seller/orders" className="p-2 hover:bg-slate-50 rounded-full text-slate-400 hover:text-orange-600 transition">
                    <EllipsisHorizontalIcon className="w-6 h-6" />
                </Link>
            </div>
            <div className="flex-1 overflow-x-auto">
                 {recentOrders.length === 0 ? <EmptyState message="Henüz sipariş almadınız." /> : (
                    <table className="w-full text-left border-collapse">
                        <tbody className="divide-y divide-slate-50">
                            {recentOrders.map((order, idx) => (
                                <tr key={idx} className="group hover:bg-slate-50/80 transition-colors">
                                    <td className="px-8 py-5">
                                        <div className="flex flex-col">
                                            <span className="font-mono text-xs font-semibold text-slate-400 mb-1">{order.orderNumber}</span>
                                            <span className="font-bold text-slate-800">{formatCurrency(order.totalPrice)}</span>
                                        </div>
                                    </td>
                                    <td className="px-8 py-5">
                                        <div className="text-sm text-slate-500">
                                            {formatDate(order.createdAt)}
                                        </div>
                                    </td>
                                    <td className="px-8 py-5 text-right">
                                         <StatusBadge status={order.orderStatus} />
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                 )}
            </div>
          </div>

          {/* RECENT PENDING PRODUCTS */}
          <div className="bg-white rounded-3xl border border-slate-100 shadow-sm overflow-hidden flex flex-col h-full">
            <div className="px-8 py-6 border-b border-slate-50 flex justify-between items-center bg-white">
                <div>
                    <h3 className="font-bold text-slate-800 text-lg">Son Başvurular</h3>
                    <p className="text-slate-400 text-sm mt-0.5">Onaya gönderdiğiniz son ürünler</p>
                </div>
                <Link to="/seller/pending-products" className="p-2 hover:bg-slate-50 rounded-full text-slate-400 hover:text-orange-600 transition">
                    <EllipsisHorizontalIcon className="w-6 h-6" />
                </Link>
            </div>
            <div className="flex-1 overflow-x-auto">
                {recentPending.length === 0 ? <EmptyState message="Bekleyen başvuru yok." /> : (
                    <table className="w-full text-left border-collapse">
                        <tbody className="divide-y divide-slate-50">
                            {recentPending.map((p) => (
                                <tr key={p.productPendingId} className="group hover:bg-slate-50/80 transition-colors">
                                    <td className="px-8 py-5">
                                        <div className="flex flex-col">
                                            <span className="font-bold text-slate-800 line-clamp-1">{p.name}</span>
                                            <span className="text-xs text-slate-400">{formatDate(p.createdAt)}</span>
                                            
                                            {/* Admin Notu Varsa Göster */}
                                            {p.adminNote && (
                                                <div className="mt-1.5 flex items-start gap-1.5 text-xs text-rose-600 bg-rose-50 p-2 rounded-lg max-w-xs">
                                                    <ExclamationTriangleIcon className="w-4 h-4 shrink-0" />
                                                    <span>{p.adminNote}</span>
                                                </div>
                                            )}
                                        </div>
                                    </td>
                                    <td className="px-8 py-5 text-right align-top">
                                        <StatusBadge status={p.status} />
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                )}
            </div>
          </div>

      </div>
    </div>
  );
}