import { useState } from "react";
import { Link } from "react-router-dom";
import {
  MagnifyingGlassIcon,
  ShoppingCartIcon,
  UserIcon,
} from "@heroicons/react/24/outline";
import { useUIStore } from "../../store/uiStore";
import { useCartStore } from "../../store/cartStore";
import { useAuthStore } from "../../store/authStore";

export default function Navbar() {
  const [search, setSearch] = useState("");

  const totalQuantity = useCartStore((s) => s.totalQuantity());
  const toggleCart = useUIStore((s) => s.toggleCart);

  const { isAuthenticated, user, logout } = useAuthStore();
  return (
    <header className="w-full shadow-md bg-white sticky top-0 z-50">
      <div className="max-w-7xl mx-auto px-4 py-3 flex items-center gap-6">

        {/* LOGO */}
        <Link to="/" className="font-bold text-xl text-orange-500">
          MarketApp
        </Link>

        {/* SEARCH */}
        <div className="flex-1">
          <div className="flex items-center bg-gray-100 rounded-lg overflow-hidden">
            <input
              type="text"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Ürün, kategori veya marka ara..."
              className="w-full px-4 py-2 bg-transparent outline-none"
            />
            <button className="p-2 bg-orange-500 text-white hover:bg-orange-600 transition">
              <MagnifyingGlassIcon className="h-5 w-5" />
            </button>
          </div>
        </div>

        {/* CART */}
        <button
          onClick={toggleCart}
          className="relative p-2 rounded-full hover:bg-gray-100 transition"
        >
          <ShoppingCartIcon className="w-6 h-6 text-gray-700" />
          {totalQuantity > 0 && (
            <span className="absolute -top-1 -right-1 bg-orange-500 text-white text-xs font-bold w-5 h-5 flex items-center justify-center rounded-full">
              {totalQuantity}
            </span>
          )}
        </button>

        {/* ACCOUNT */}
        {!isAuthenticated ? (
          <div className="flex items-center gap-3">
            <Link
              to="/auth?mode=login"
              className="font-medium hover:text-orange-500"
            >
              Giriş Yap
            </Link>
            <Link
              to="/auth?mode=register"
              className="font-medium bg-orange-500 text-white px-3 py-1 rounded-md hover:bg-orange-600 transition"
            >
              Üye Ol
            </Link>
          </div>
        ) : (
          <div className="relative group">
            <div className="flex items-center gap-2 cursor-pointer">
              <UserIcon className="h-6 w-6 text-gray-600" />
              <span className="font-medium">
                {user?.firstName}
              </span>
            </div>

            {/* DROPDOWN */}
            <div className="absolute right-0 mt-3 w-44 bg-white shadow-lg rounded-md opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-all">
              <Link
                to="/profile"
                className="block px-4 py-2 hover:bg-gray-100"
              >
                Profilim
              </Link>
              <Link
                to="/orders"
                className="block px-4 py-2 hover:bg-gray-100"
              >
                Siparişlerim
              </Link>
              <button
                onClick={logout}
                className="w-full text-left px-4 py-2 hover:bg-gray-100 text-red-600"
              >
                Çıkış Yap
              </button>
            </div>
          </div>
        )}

      </div>
    </header>
  );
}