import { useState } from "react";
import { Link } from "react-router-dom";
import {
  MagnifyingGlassIcon,
  ShoppingCartIcon,
  UserIcon,
} from "@heroicons/react/24/outline";

export default function Navbar() {
  const [search, setSearch] = useState("");

  // TODO: Auth store'dan çekilecek (şimdilik sahte)
  const isLoggedIn = false;
  const user = { firstName: "Beren" }; // Örnek

  return (
    <header className="w-full shadow-md bg-white sticky top-0 z-50">
      <div className="max-w-7xl mx-auto px-4 py-3 flex items-center gap-6">

        {/* LOGO */}
        <Link to="/" className="text-2xl font-extrabold text-orange-500">
          MarketApp
        </Link>

        {/* SEARCH BAR */}
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

        {/* ACCOUNT */}
        {!isLoggedIn ? (
          <div className="flex items-center gap-3">
            <Link to="/login" className="font-medium hover:text-orange-500">
              Giriş Yap
            </Link>
            <Link
              to="/register"
              className="font-medium bg-orange-500 text-white px-3 py-1 rounded-md hover:bg-orange-600 transition"
            >
              Üye Ol
            </Link>
          </div>
        ) : (
          <div className="relative group cursor-pointer">
            <div className="flex items-center gap-2">
              <UserIcon className="h-6 w-6 text-gray-600" />
              <span className="font-medium">{user.firstName}</span>
            </div>

            {/* DROPDOWN */}
            <div className="absolute right-0 mt-3 w-40 bg-white shadow-md rounded-md hidden group-hover:block">
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
              <button className="w-full text-left px-4 py-2 hover:bg-gray-100">
                Çıkış Yap
              </button>
            </div>
          </div>
        )}

        {/* CART */}
        <Link to="/cart" className="relative">
          <ShoppingCartIcon className="h-7 w-7 text-gray-700" />
          <span className="absolute -top-2 -right-2 bg-orange-500 text-white rounded-full text-xs px-1">
            2
          </span>
        </Link>
      </div>
    </header>
  );
}