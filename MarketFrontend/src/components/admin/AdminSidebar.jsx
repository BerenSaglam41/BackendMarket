import { useState } from "react";
import { NavLink, Link } from "react-router-dom";
import {
  HomeIcon,
  UserGroupIcon,
  CubeIcon,
  ShoppingBagIcon,
  TicketIcon,
  ArrowTopRightOnSquareIcon,
  ChevronLeftIcon,
  ChevronRightIcon,
  Squares2X2Icon // Logo ikonu olarak
} from "@heroicons/react/24/outline";

// Menü konfigürasyonu
const MENU_ITEMS = [
  { name: "Dashboard", path: "/admin", icon: HomeIcon, end: true },
  { name: "Satıcı Başvuruları", path: "/admin/seller-applications", icon: UserGroupIcon },
  { name: "Ürün Yönetimi", path: "/admin/products", icon: CubeIcon },
  { name: "Siparişler", path: "/admin/orders", icon: ShoppingBagIcon },
  { name: "Kuponlar", path: "/admin/coupons", icon: TicketIcon },
  { name: "Bekleyen Ürünler", path: "/admin/pending-products", icon: ShoppingBagIcon },
];

export default function AdminSidebar() {
  // Sidebar açık mı kapalı mı durumunu tutan state
  const [isCollapsed, setIsCollapsed] = useState(false);

  return (
    <aside 
      className={`
        bg-white border-r border-gray-200 shadow-sm hidden md:flex flex-col h-screen sticky top-0
        transition-all duration-300 ease-in-out relative
        ${isCollapsed ? "w-20" : "w-72"} 
      `}
    >
      
      {/* --- TOGGLE BUTONU (Çizgi Üzerindeki Yuvarlak Buton) --- */}
      <button
        onClick={() => setIsCollapsed(!isCollapsed)}
        className="absolute -right-3 top-9 z-50 bg-white border border-gray-200 rounded-full p-1.5 shadow-sm hover:bg-gray-50 text-gray-500 hover:text-orange-600 transition"
      >
        {isCollapsed ? (
          <ChevronRightIcon className="w-3 h-3" />
        ) : (
          <ChevronLeftIcon className="w-3 h-3" />
        )}
      </button>

      {/* --- HEADER --- */}
      <div className={`h-20 flex items-center border-b border-gray-100 ${isCollapsed ? "justify-center" : "px-6"}`}>
        <div className="flex items-center gap-3 overflow-hidden whitespace-nowrap">
          {/* Logo İkonu */}
          <div className="min-w-[32px] h-8 flex items-center justify-center bg-orange-100 rounded-lg text-orange-600">
             <Squares2X2Icon className="w-5 h-5" />
          </div>
          
          {/* Yazı (Collapse olunca gizlenir) */}
          <div className={`flex flex-col transition-opacity duration-200 ${isCollapsed ? "opacity-0 w-0 hidden" : "opacity-100"}`}>
            <span className="text-lg font-bold text-gray-900 tracking-tight">Admin</span>
            <span className="text-[10px] text-gray-400 font-medium uppercase">Panel v1.0</span>
          </div>
        </div>
      </div>

      {/* --- NAVIGATION --- */}
      <nav className="flex-1 py-6 px-3 space-y-2 overflow-y-auto overflow-x-hidden">
        {MENU_ITEMS.map((item) => (
          <NavLink
            key={item.path}
            to={item.path}
            end={item.end}
            title={isCollapsed ? item.name : ""} // Kapalıyken üzerine gelince ne olduğu yazsın
            className={({ isActive }) =>
              `flex items-center gap-3 py-3 rounded-lg transition-all duration-200 group
              ${isCollapsed ? "justify-center px-0" : "px-4"} 
              ${
                isActive
                  ? "bg-orange-50 text-orange-600 shadow-sm ring-1 ring-orange-200" 
                  : "text-gray-600 hover:bg-gray-50 hover:text-gray-900"
              }`
            }
          >
            {/* İkon */}
            <item.icon className={`w-6 h-6 flex-shrink-0 ${isCollapsed ? "" : ""}`} />

            {/* İsim (Collapse olunca gizlenir) */}
            <span className={`text-sm font-medium whitespace-nowrap overflow-hidden transition-all duration-300 ${isCollapsed ? "w-0 opacity-0 hidden" : "w-auto opacity-100"}`}>
              {item.name}
            </span>

            {/* Hover Tooltip (Sadece kapalıyken şıklık katar - Opsiyonel) */}
            {isCollapsed && (
               <div className="absolute left-16 bg-gray-900 text-white text-xs px-2 py-1 rounded opacity-0 group-hover:opacity-100 pointer-events-none transition-opacity z-50 whitespace-nowrap">
                  {item.name}
               </div>
            )}
          </NavLink>
        ))}
      </nav>

      {/* --- FOOTER --- */}
      <div className="p-4 border-t border-gray-100 bg-gray-50/50">
        <Link 
            to="/" 
            target="_blank" 
            title="Siteyi Görüntüle"
            className={`flex items-center gap-3 text-sm text-gray-600 hover:text-orange-600 transition rounded-lg hover:bg-white p-2
              ${isCollapsed ? "justify-center" : "px-4"}
            `}
        >
            <ArrowTopRightOnSquareIcon className="w-5 h-5 flex-shrink-0" />
            <span className={`font-medium whitespace-nowrap overflow-hidden transition-all duration-300 ${isCollapsed ? "w-0 opacity-0 hidden" : "w-auto opacity-100"}`}>
                Siteye Git
            </span>
        </Link>
      </div>

    </aside>
  );
}