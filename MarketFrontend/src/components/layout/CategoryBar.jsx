import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import { Bars3Icon, XMarkIcon } from "@heroicons/react/24/outline";
import MegaMenu from "./MegaMenu";
import { useCategoryStore } from "../../store/CategoryStore";

export default function CategoryBar() {
  const [openMobileMenu, setOpenMobileMenu] = useState(false);
  const [openMegaMenu, setOpenMegaMenu] = useState(false);
  const { tree, fetchTree } = useCategoryStore();

  const popularCategories = Array.isArray(tree) ? tree.slice(0, 5) : [];

  useEffect(() => {
    fetchTree();
  }, []);

  return (
    <>
      {/* DESKTOP CATEGORY BAR */}
      <div
        className="hidden md:flex items-center justify-between px-6 py-2 border-t bg-white shadow-sm relative"
        onMouseLeave={() => setOpenMegaMenu(false)}
      >
        {/* Sol: Tüm Kategoriler */}
        <div
          onMouseEnter={() => setOpenMegaMenu(true)}
          className="flex items-center gap-2 text-sm font-semibold text-gray-800 hover:text-orange-500 cursor-pointer"
        >
          <Bars3Icon className="h-5 w-5" />
          Tüm Kategoriler
        </div>

        {/* Sağ: Popüler kategoriler */}
        <div className="flex gap-6">
          {popularCategories.map((cat) => (
            <Link
              key={cat.categoryId}
              to={`/category/${cat.slug}`}
              className="text-sm font-medium text-gray-700 hover:text-orange-500 transition"
            >
              {cat.name}
            </Link>
          ))}
        </div>

        {/* MEGA MENU — Navbar’ın tamamını kapsayan geniş alan */}
        {openMegaMenu && (
          <div className="absolute left-0 top-full w-full z-50">
            <MegaMenu categories={tree} />
          </div>
        )}
      </div>

      {/* ----- MOBILE CATEGORY BAR ----- */}
      <div className="md:hidden flex items-center px-4 py-3 border-t bg-white shadow-sm justify-between">
        <button onClick={() => setOpenMobileMenu(true)}>
          <Bars3Icon className="h-7 w-7 text-gray-800" />
        </button>
        <span className="text-lg font-bold text-orange-500">MarketApp</span>
      </div>

      {/* MOBILE CATEGORY MENU */}
      {openMobileMenu && (
        <div className="fixed inset-0 bg-black bg-opacity-40 z-50 md:hidden">
          <div className="absolute left-0 top-0 h-full w-64 bg-white shadow-lg p-4">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-lg font-bold">Kategoriler</h2>
              <button onClick={() => setOpenMobileMenu(false)}>
                <XMarkIcon className="h-7 w-7 text-gray-800" />
              </button>
            </div>

            <div className="flex flex-col gap-4">
              {popularCategories.map((cat) => (
                <Link
                  key={cat.categoryId}
                  to={`/category/${cat.slug}`}
                  className="text-base text-gray-700 hover:text-orange-500"
                  onClick={() => setOpenMobileMenu(false)}
                >
                  {cat.name}
                </Link>
              ))}
            </div>
          </div>
        </div>
      )}
    </>
  );
}