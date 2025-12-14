import { useLocation } from "react-router-dom";
import { useAuthStore } from "../../store/authStore";

export default function AdminTopbar() {
  const { user } = useAuthStore();
  const location = useLocation();

  // Dinamik Başlık Mantığı
  const getPageTitle = () => {
    const path = location.pathname;
    if (path === "/admin") return "Genel Bakış (Dashboard)";
    if (path.includes("/products")) return "Ürün Yönetimi";
    if (path.includes("/orders")) return "Sipariş Takibi";
    if (path.includes("/seller-applications")) return "Satıcı Başvuruları";
    if (path.includes("/coupons")) return "Kupon Yönetimi";
    return "Yönetim Paneli";
  };

  // Tarih Bilgisi
  const today = new Date().toLocaleDateString("tr-TR", {
    day: "numeric",
    month: "long",
    weekday: "long",
  });

  return (
    <header className="h-20 bg-white border-b border-gray-200 px-8 flex items-center justify-between sticky top-0 z-40 shadow-sm/50">
      
      {/* SOL: Başlık ve Tarih */}
      <div className="flex flex-col">
        <h1 className="text-xl font-bold text-gray-800 tracking-tight">
          {getPageTitle()}
        </h1>
        <span className="text-xs text-gray-500 font-medium mt-0.5">
          {today}
        </span>
      </div>

      {/* SAĞ: Profil Alanı */}
      <div className="flex items-center">
        
        {/* Profil Kartı */}
        <div className="flex items-center gap-3 cursor-default group p-2 rounded-lg hover:bg-gray-50 transition">
          
          {/* İsim ve Rol */}
          <div className="hidden md:flex flex-col items-end">
            <span className="text-sm font-bold text-gray-800 group-hover:text-orange-600 transition">
              {user?.firstName} {user?.lastName}
            </span>
            <span className="text-[10px] text-gray-500 uppercase font-semibold bg-gray-100 px-2 py-0.5 rounded">
              Yönetici
            </span>
          </div>

          {/* Avatar (İsim Baş Harfleri) */}
          <div className="w-10 h-10 rounded-full bg-gradient-to-br from-gray-700 to-gray-900 flex items-center justify-center text-white font-bold shadow-md ring-2 ring-white group-hover:ring-orange-200 transition">
            {user?.firstName?.charAt(0).toUpperCase()}
            {user?.lastName?.charAt(0).toUpperCase()}
          </div>
        </div>

      </div>
    </header>
  );
}