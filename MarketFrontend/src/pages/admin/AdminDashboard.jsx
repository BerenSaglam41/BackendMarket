import { useEffect } from "react";
import { Link } from "react-router-dom";
import { 
  UsersIcon, 
  BuildingStorefrontIcon, 
  TagIcon, 
  ShoppingBagIcon, 
  ListBulletIcon, 
  ExclamationTriangleIcon, 
  ArrowPathIcon, 
  CheckBadgeIcon, 
  ArrowTrendingUpIcon, 
  ArrowTrendingDownIcon, 
  EllipsisHorizontalIcon, 
  PlusIcon,
  ClockIcon,
  CubeIcon
} from "@heroicons/react/24/outline";
import { useAdminDashboardStore } from "../../store/admin/adminDashboardStore";

// --- CUSTOM ICONS ---
const TurkishLiraIcon = ({ className }) => (
  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className={className}>
    <path strokeLinecap="round" strokeLinejoin="round" d="M8 7h9M8 11h9M9.5 20V4.5" />
    <path strokeLinecap="round" strokeLinejoin="round" d="M6 5l3.5-1" />
  </svg>
);

// --- HELPERS ---
const formatDate = (dateString) => {
    if(!dateString) return "-";
    return new Date(dateString).toLocaleDateString("tr-TR", { month: "short", day: "numeric", hour: "2-digit", minute: "2-digit" });
};
const formatCurrency = (amount) => new Intl.NumberFormat("tr-TR", { style: "currency", currency: "TRY" }).format(amount || 0);

// --- COMPONENTS ---
const PremiumStatCard = ({ title, value, icon: Icon, trend, trendValue, color }) => {
    const colorClasses = {
        blue: "text-blue-600 bg-blue-50/50",
        purple: "text-purple-600 bg-purple-50/50",
        orange: "text-orange-600 bg-orange-50/50",
        green: "text-emerald-600 bg-emerald-50/50",
    };
    const isPositive = trend === "up";

    return (
        <div className="group bg-white rounded-2xl p-5 border border-slate-100 shadow-[0_2px_10px_-4px_rgba(0,0,0,0.05)] hover:shadow-[0_8px_30px_-12px_rgba(0,0,0,0.1)] transition-all duration-300">
            <div className="flex justify-between items-start mb-4">
                <div className={`p-3 rounded-xl ${colorClasses[color] || "text-gray-600 bg-gray-50"} transition-colors`}>
                    <Icon className="w-6 h-6" />
                </div>
                {trend && (
                    <div className={`flex items-center gap-1 text-xs font-bold px-2 py-1 rounded-full ${isPositive ? 'text-emerald-700 bg-emerald-50' : 'text-rose-700 bg-rose-50'}`}>
                        {isPositive ? <ArrowTrendingUpIcon className="w-3 h-3" /> : <ArrowTrendingDownIcon className="w-3 h-3" />}
                        {trendValue}
                    </div>
                )}
            </div>
            <div>
                <h4 className="text-slate-500 text-sm font-medium mb-1">{title}</h4>
                <div className="text-3xl font-bold text-slate-800 tracking-tight">{value}</div>
            </div>
        </div>
    );
};

const ActionCard = ({ title, count, icon: Icon, color, link }) => {
    const gradients = {
        orange: "from-orange-500 to-amber-500",
        blue: "from-blue-500 to-indigo-500",
        red: "from-rose-500 to-pink-500",
    };

    return (
        <Link to={link} className="relative overflow-hidden bg-white rounded-2xl p-6 border border-slate-100 shadow-sm hover:shadow-md transition-all group">
            <div className="relative z-10 flex items-center justify-between">
                <div>
                    <p className="text-slate-500 text-sm font-medium">{title}</p>
                    <h3 className="text-2xl font-bold text-slate-800 mt-1 group-hover:text-transparent group-hover:bg-clip-text group-hover:bg-gradient-to-r group-hover:from-gray-800 group-hover:to-gray-600 transition-all">
                        {count || 0} <span className="text-base font-normal text-slate-400">Bekleyen</span>
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

export default function AdminDashboard() {
    const { data, fetchDashboard, loading } = useAdminDashboardStore();

    useEffect(() => {
        fetchDashboard();
    }, []);

    // 1. Loading
    if (loading) {
        return (
            <div className="flex flex-col items-center justify-center h-screen bg-slate-50 gap-4">
                <div className="relative">
                    <div className="w-12 h-12 border-4 border-slate-200 rounded-full"></div>
                    <div className="w-12 h-12 border-4 border-orange-500 rounded-full border-t-transparent animate-spin absolute top-0 left-0"></div>
                </div>
                <p className="text-slate-500 font-medium animate-pulse">Panel hazırlanıyor...</p>
            </div>
        );
    }

    // 2. Data Check
    if (!data) return null;

    // Veri Güvenliği (Defaults)
    const recentOrders = data.recentOrders || [];
    const recentApplications = data.recentSellerApplications || [];

    return (
        <div className="min-h-screen bg-slate-50/50 p-6 lg:p-10 space-y-10 font-sans text-slate-800">
          
          {/* WELCOME BANNER */}
          <div className="relative rounded-3xl bg-gray-900 overflow-hidden shadow-2xl shadow-gray-200">
             <div className="absolute top-0 right-0 w-[500px] h-[500px] bg-orange-500/20 rounded-full blur-[100px] -translate-y-1/2 translate-x-1/3 mix-blend-screen"></div>
             <div className="absolute bottom-0 left-0 w-[300px] h-[300px] bg-blue-500/20 rounded-full blur-[80px] translate-y-1/2 -translate-x-1/3 mix-blend-screen"></div>

             <div className="relative z-10 px-8 py-10 md:py-12 flex flex-col md:flex-row justify-between items-start md:items-center gap-6">
                <div>
                    <h1 className="text-3xl md:text-4xl font-bold text-white tracking-tight">
                        Admin Paneli
                    </h1>
                    <p className="text-slate-400 mt-2 max-w-xl text-lg">
                        Platform durumu stabil. {data.totalPendingSellerApplications + data.totalPendingProducts > 0 
                            ? `İlgilenmen gereken ${data.totalPendingSellerApplications + data.totalPendingProducts} yeni işlem var.` 
                            : "Şu an bekleyen kritik bir işlem yok."}
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
                    <Link to="/admin/products/add" className="flex items-center gap-2 px-5 py-2.5 bg-orange-500 hover:bg-orange-600 text-white font-semibold rounded-xl shadow-lg shadow-orange-500/30 transition-all active:scale-95">
                        <PlusIcon className="w-5 h-5" />
                        <span>Yeni Ürün</span>
                    </Link>
                </div>
             </div>
          </div>

          {/* KEY METRICS */}
          <div>
            <h3 className="text-lg font-bold text-slate-800 mb-5 flex items-center gap-2">
                <ArrowTrendingUpIcon className="w-5 h-5 text-orange-500" />
                Genel İstatistikler
            </h3>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                {/* 1. Ciro (Backend'de totalRevenue yoksa geçici çözüm) */}
                <PremiumStatCard 
                    title="Toplam Satış (Ciro)" 
                    // Backend bu veriyi göndermediği için şimdilik recentOrders toplamı veya 0 gösteriyoruz
                    value={formatCurrency(recentOrders.reduce((acc, curr) => acc + curr.totalAmount, 0))} 
                    icon={TurkishLiraIcon} 
                    // trend="up" trendValue="%12.5" // Bu veri backend'den gelmeli
                    color="green"
                />
                
                {/* 2. Kullanıcılar */}
                <PremiumStatCard 
                    title="Toplam Kullanıcı" 
                    value={data.totalUsers} 
                    icon={UsersIcon} 
                    trend="up" 
                    trendValue="Aktif"
                    color="blue"
                />

                {/* 3. Ürünler */}
                <PremiumStatCard 
                    title="Toplam Ürün" 
                    value={data.totalProducts} 
                    icon={CubeIcon} 
                    trend="up" 
                    trendValue={`${data.totalCategories} Kategori`}
                    color="orange"
                />

                {/* 4. Satıcılar */}
                <PremiumStatCard 
                    title="Satıcı Sayısı" 
                    value={data.totalSellers} 
                    icon={BuildingStorefrontIcon} 
                    color="purple"
                />
            </div>
          </div>

          {/* ACTION CENTER */}
          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <ActionCard title="Satıcı Başvurusu" count={data.totalPendingSellerApplications} icon={CheckBadgeIcon} color="orange" link="/admin/seller-applications" />
            <ActionCard title="Ürün Onayı" count={data.totalPendingProducts} icon={TagIcon} color="blue" link="/admin/pending-products" />
            <ActionCard title="Şikayet/Rapor" count={data.totalReportedReviews} icon={ExclamationTriangleIcon} color="red" link="/admin/reviews" />
          </div>

          {/* TABLES */}
          <div className="grid grid-cols-1 xl:grid-cols-2 gap-8">
              
              {/* Recent Sellers Table */}
              <div className="bg-white rounded-3xl border border-slate-100 shadow-sm overflow-hidden flex flex-col">
                <div className="px-8 py-6 border-b border-slate-50 flex justify-between items-center">
                    <div>
                        <h3 className="font-bold text-slate-800 text-lg">Son Satıcı Başvuruları</h3>
                        <p className="text-slate-400 text-sm mt-0.5">Mağaza açmak isteyen son adaylar</p>
                    </div>
                    <Link to="/admin/seller-applications" className="p-2 hover:bg-slate-50 rounded-full text-slate-400 hover:text-orange-500 transition">
                        <EllipsisHorizontalIcon className="w-6 h-6" />
                    </Link>
                </div>
                <div className="flex-1 overflow-x-auto">
                     {recentApplications.length === 0 ? <EmptyState /> : (
                        <table className="w-full text-left border-collapse">
                            <tbody className="divide-y divide-slate-50">
                                {recentApplications.map((app) => (
                                    <tr key={app.sellerApplicationId} className="group hover:bg-slate-50/80 transition-colors">
                                        <td className="px-8 py-5">
                                            <div className="flex items-center gap-4">
                                                <div className="w-10 h-10 rounded-xl bg-orange-100 text-orange-600 flex items-center justify-center font-bold text-lg border border-orange-200">
                                                    {(app.storeName || "X").charAt(0)}
                                                </div>
                                                <div>
                                                    <div className="font-bold text-slate-800">{app.storeName}</div>
                                                    <div className="text-xs text-slate-400 font-medium">{formatDate(app.createdAt)}</div>
                                                </div>
                                            </div>
                                        </td>
                                        <td className="px-8 py-5">
                                            <StatusBadge status={app.status} />
                                        </td>
                                        <td className="px-8 py-5 text-right">
                                            <Link to={`/admin/seller-applications`} className="text-sm font-semibold text-orange-600 hover:text-orange-700 opacity-0 group-hover:opacity-100 transition-opacity">
                                                Detay
                                            </Link>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                     )}
                </div>
              </div>

              {/* Recent Orders Table */}
              <div className="bg-white rounded-3xl border border-slate-100 shadow-sm overflow-hidden flex flex-col">
                <div className="px-8 py-6 border-b border-slate-50 flex justify-between items-center">
                    <div>
                        <h3 className="font-bold text-slate-800 text-lg">Son Siparişler</h3>
                        <p className="text-slate-400 text-sm mt-0.5">Platform üzerinden geçen son işlemler</p>
                    </div>
                    <button className="p-2 hover:bg-slate-50 rounded-full text-slate-400 hover:text-orange-500 transition">
                        <EllipsisHorizontalIcon className="w-6 h-6" />
                    </button>
                </div>
                <div className="flex-1 overflow-x-auto">
                    {recentOrders.length === 0 ? <EmptyState /> : (
                        <table className="w-full text-left border-collapse">
                            <tbody className="divide-y divide-slate-50">
                                {recentOrders.map((order) => (
                                    <tr key={order.orderId} className="group hover:bg-slate-50/80 transition-colors">
                                        <td className="px-8 py-5">
                                            <div className="flex flex-col">
                                                <span className="font-mono text-xs font-semibold text-slate-400 mb-1">{order.orderNumber}</span>
                                                <span className="font-bold text-slate-800">{formatCurrency(order.totalAmount)}</span>
                                            </div>
                                        </td>
                                        <td className="px-8 py-5">
                                            <div className="flex items-center gap-2 text-sm text-slate-500">
                                                <ClockIcon className="w-4 h-4" />
                                                {formatDate(order.createdAt)}
                                            </div>
                                        </td>
                                        <td className="px-8 py-5 text-right">
                                             <StatusBadge status={order.status} />
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

// --- SUB COMPONENTS ---

const StatusBadge = ({ status }) => {
    const styles = {
        Approved: "bg-emerald-100 text-emerald-700 border-emerald-200",
        Paid: "bg-emerald-100 text-emerald-700 border-emerald-200",
        Rejected: "bg-rose-100 text-rose-700 border-rose-200",
        Pending: "bg-amber-100 text-amber-700 border-amber-200",
        Processing: "bg-blue-100 text-blue-700 border-blue-200",
        NeedsUpdate: "bg-orange-100 text-orange-700 border-orange-200",
    };

    return (
        <span className={`inline-flex px-3 py-1 rounded-full text-xs font-bold border ${styles[status] || "bg-slate-100 text-slate-600 border-slate-200"}`}>
            {status || "Bilinmiyor"}
        </span>
    );
};

const EmptyState = () => (
    <div className="p-10 flex flex-col items-center justify-center text-slate-300">
        <ListBulletIcon className="w-10 h-10 mb-2 opacity-50" />
        <span className="text-sm font-medium">Veri bulunamadı</span>
    </div>
);