import { useState } from "react";
import { Link, useNavigate } from "react-router-dom"; // useNavigate eklendi
import {
  MagnifyingGlassIcon,
  ShoppingCartIcon,
  UserIcon,
  ArrowRightOnRectangleIcon, // Çıkış ikonu
  HeartIcon // Favoriler için (Genelde istenir)
} from "@heroicons/react/24/outline";

import { useUIStore } from "../../store/uiStore";
import { useCartStore } from "../../store/cartStore";
import { useAuthStore } from "../../store/authStore";

export default function Navbar() {
  const navigate = useNavigate(); // Sayfa yönlendirmesi için
  const { isAuthenticated, user, logout } = useAuthStore();
  const totalQuantity = useCartStore((s) => s.totalQuantity());
  const toggleCart = useUIStore((s) => s.toggleCart);

  const [search, setSearch] = useState("");

  // Kullanıcı rolleri güvenli kontrol
  const roles = user?.roles || [];
  const isAdmin = roles.includes("Admin");
  const isSeller = roles.includes("Seller");

  // Arama Fonksiyonu
  const handleSearch = (e) => {
    e.preventDefault(); // Sayfa yenilenmesini engelle
    if (search.trim()) {
      // Arama sayfasına yönlendir (Query param olarak)
      navigate(`/search?q=${encodeURIComponent(search)}`);
    }
  };

  return (
    <header className="bg-white shadow-md sticky top-0 z-50 h-20 flex items-center">
      <div className="container mx-auto px-4 flex items-center justify-between gap-8">
        
        {/* --- 1. LOGO --- */}
        <Link to="/" className="text-2xl font-bold text-orange-600 tracking-tight flex-shrink-0">
          MarketApp
        </Link>

        {/* --- 2. SEARCH BAR (FORM) --- */}
        {/* flex-1: Boşluğu doldur, max-w-xl: Çok genişleme */}
        <form onSubmit={handleSearch} className="flex-1 max-w-2xl hidden md:flex relative group">
          <input
            type="text"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder="Ürün, kategori veya marka ara..."
            className="w-full pl-5 pr-12 py-2.5 bg-gray-100 border border-transparent rounded-full outline-none focus:bg-white focus:border-orange-500 transition-all text-sm"
          />
          <button 
            type="submit"
            className="absolute right-2 top-1/2 -translate-y-1/2 p-1.5 bg-orange-500 text-white rounded-full hover:bg-orange-600 transition"
          >
            <MagnifyingGlassIcon className="h-5 w-5" />
          </button>
        </form>

        {/* --- 3. SAĞ MENÜ (IKONLAR & HESAP) --- */}
        <div className="flex items-center gap-4 flex-shrink-0">
          
          {/* Giriş Yap / Üye Ol (Giriş Yapılmamışsa) */}
          {!isAuthenticated ? (
            <div className="hidden md:flex items-center gap-3">
              <Link 
                to="/auth?mode=login" 
                className="text-sm font-semibold text-gray-600 hover:text-orange-600 transition"
              >
                Giriş Yap
              </Link>
              <Link 
                to="/auth?mode=register" 
                className="text-sm font-semibold bg-orange-500 text-white px-4 py-2 rounded-full hover:bg-orange-600 transition shadow-sm"
              >
                Üye Ol
              </Link>
            </div>
          ) : (
            <>
              {/* Favoriler (Opsiyonel) */}
              <Link to="/favorites" className="hidden md:block text-gray-600 hover:text-orange-500 transition">
                 <HeartIcon className="w-6 h-6" />
              </Link>

              {/* SEPET */}
              <button
                onClick={toggleCart}
                className="relative text-gray-600 hover:text-orange-500 transition group"
              >
                <ShoppingCartIcon className="w-6 h-6" />
                {totalQuantity > 0 && (
                  <span className="absolute -top-1.5 -right-1.5 bg-orange-500 text-white text-[10px] font-bold w-5 h-5 rounded-full flex items-center justify-center border-2 border-white">
                    {totalQuantity}
                  </span>
                )}
              </button>

              {/* HESAP DROPDOWN */}
              <div className="relative group py-2"> {/* py-2: Mouse kaçırmayı önlemek için görünmez köprü */}
                <div className="flex items-center gap-2 cursor-pointer text-gray-700 hover:text-orange-600">
                  <UserIcon className="w-6 h-6" />
                  <span className="hidden md:block text-sm font-medium max-w-[100px] truncate">
                    {user?.firstName}
                  </span>
                </div>

                {/* Dropdown Menü */}
                <div className="absolute right-0 top-full w-56 bg-white shadow-xl rounded-lg border border-gray-100 py-2 opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all duration-200 transform origin-top-right z-50">
                  
                  {/* Karşılama */}
                  <div className="px-4 py-2 border-b border-gray-100 text-xs text-gray-500">
                    Merhaba, <span className="font-bold text-gray-800">{user?.firstName}</span>
                  </div>

                  <Link to="/profile" className="block px-4 py-2 text-sm text-gray-700 hover:bg-orange-50 hover:text-orange-600">
                    Profilim
                  </Link>
                  <Link to="/orders" className="block px-4 py-2 text-sm text-gray-700 hover:bg-orange-50 hover:text-orange-600">
                    Siparişlerim
                  </Link>

                  {isSeller && (
                    <Link to="/seller" className="block px-4 py-2 text-sm text-blue-600 hover:bg-blue-50 font-medium">
                      Satıcı Paneli
                    </Link>
                  )}

                  {isAdmin && (
                    <Link to="/admin" className="block px-4 py-2 text-sm text-purple-600 hover:bg-purple-50 font-medium">
                      Yönetim Paneli (Admin)
                    </Link>
                  )}

                  <div className="border-t border-gray-100 mt-1 pt-1">
                    <button
                      onClick={logout}
                      className="w-full text-left px-4 py-2 text-sm text-red-500 hover:bg-red-50 flex items-center gap-2"
                    >
                      <ArrowRightOnRectangleIcon className="w-4 h-4" />
                      Çıkış Yap
                    </button>
                  </div>
                </div>
              </div>
            </>
          )}
        </div>
      </div>
    </header>
  );
}