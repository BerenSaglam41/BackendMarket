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
      {/* ===== DESKTOP CATEGORY BAR ===== */}
      {/* onMouseLeave en dış katmanda olmalı ki mouse menüye geçtiğinde kapanmasın */}
      <div
        className="hidden md:block relative border-t bg-white shadow-sm z-40"
        onMouseLeave={() => setOpenMegaMenu(false)}
      >
        {/* TOP BAR */}
        <div className="container mx-auto flex items-center justify-between px-6 py-2">
          {/* SOL: Tüm Kategoriler Tetikleyicisi */}
          <div
            onMouseEnter={() => setOpenMegaMenu(true)}
            className="flex items-center gap-2 text-sm font-semibold text-gray-800 hover:text-orange-500 cursor-pointer py-2"
          >
            <Bars3Icon className="h-5 w-5" />
            Tüm Kategoriler
          </div>

          {/* SAĞ: Popüler Linkler */}
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
        </div>

        {/* ===== MEGA MENU ALANI ===== */}
        {/* Sadece openMegaMenu true ise render edilir */}
        <div
          className={`absolute left-0 top-full w-full transition-all duration-200 z-50 ${
            openMegaMenu
              ? "opacity-100 visible translate-y-0"
              : "opacity-0 invisible -translate-y-2"
          }`}
          onMouseEnter={() => setOpenMegaMenu(true)}
        >
            {/* onClose prop'unu MegaMenu içinde linke tıklayınca menüyü kapatmak için kullanacağız */}
            <MegaMenu categories={tree} onClose={() => setOpenMegaMenu(false)} />
        </div>
      </div>

      {/* ===== MOBILE CATEGORY BAR ===== */}
      <div className="md:hidden flex items-center px-4 py-3 border-t bg-white shadow-sm justify-between">
        <button onClick={() => setOpenMobileMenu(true)}>
          <Bars3Icon className="h-7 w-7 text-gray-800" />
        </button>
        <span className="text-lg font-bold text-orange-500">MarketApp</span>
      </div>

      {/* ===== MOBILE MENU ===== */}
      {openMobileMenu && (
        <div className="fixed inset-0 bg-black bg-opacity-40 z-50 md:hidden">
          <div className="absolute left-0 top-0 h-full w-64 bg-white shadow-lg p-4 animate-slideInLeft">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-lg font-bold">Kategoriler</h2>
              <button onClick={() => setOpenMobileMenu(false)}>
                <XMarkIcon className="h-7 w-7 text-gray-800" />
              </button>
            </div>

            <div className="flex flex-col gap-4 overflow-y-auto max-h-[80vh]">
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