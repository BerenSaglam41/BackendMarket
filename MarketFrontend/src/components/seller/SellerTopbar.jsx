import { useLocation } from "react-router-dom";
import { useAuthStore } from "../../store/authStore";

export default function SellerTopbar() {
  const { user } = useAuthStore();
  const location = useLocation();

  const getPageTitle = () => {
    const path = location.pathname;
    if (path === "/seller") return "Satıcı Paneli";
    if (path.includes("/products/add")) return "Yeni Ürün Ekle";
    if (path.includes("/products")) return "Ürünlerim";
    if (path.includes("/orders")) return "Siparişler";
    return "Satıcı Paneli";
  };

  const today = new Date().toLocaleDateString("tr-TR", {
    day: "numeric",
    month: "long",
    weekday: "long",
  });

  return (
    <header className="h-20 bg-white border-b border-gray-200 px-8 flex items-center justify-between sticky top-0 z-40 shadow-sm/50">
      {/* SOL */}
      <div className="flex flex-col">
        <h1 className="text-xl font-bold text-gray-800 tracking-tight">
          {getPageTitle()}
        </h1>
        <span className="text-xs text-gray-500 font-medium mt-0.5">
          {today}
        </span>
      </div>

      {/* SAĞ */}
      <div className="flex items-center gap-4">
        <div className="hidden md:flex flex-col items-end">
          <span className="text-sm font-bold text-gray-800">
            {user?.store?.storeName || "Mağaza"}
          </span>
          <span className="text-[10px] text-gray-500 uppercase font-semibold bg-gray-100 px-2 py-0.5 rounded">
            Satıcı
          </span>
        </div>

        <div className="w-10 h-10 rounded-full bg-gradient-to-br from-orange-500 to-orange-700 flex items-center justify-center text-white font-bold shadow-md ring-2 ring-white">
          {user?.store?.storeName?.charAt(0).toUpperCase() || "S"}
        </div>
      </div>
    </header>
  );
}