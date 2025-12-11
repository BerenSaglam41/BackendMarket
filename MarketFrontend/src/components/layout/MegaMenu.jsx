import { useState } from "react";
import { Link } from "react-router-dom";

export default function MegaMenu({ categories }) {
  const [activeMain, setActiveMain] = useState(null);
  const [activeSub, setActiveSub] = useState(null);

  return (
    <div className="bg-white shadow-xl border flex w-full h-[380px] overflow-hidden">

      {/* LEFT COLUMN – MAIN CATEGORIES */}
      <div className="w-1/4 border-r overflow-y-auto">
        {categories.map((cat) => (
          <Link
            key={cat.categoryId}
            to={`/category/${cat.slug}`}
          >
            <div
              onMouseEnter={() => {
                setActiveMain(cat);
                setActiveSub(null);
              }}
              className={`px-4 py-3 cursor-pointer hover:bg-gray-100 text-[15px] transition ${
                activeMain?.categoryId === cat.categoryId ? "bg-gray-100 font-semibold" : ""
              }`}
            >
              {cat.name}
            </div>
          </Link>
        ))}
      </div>

      {/* MIDDLE COLUMN – SUB CATEGORIES */}
      <div className="w-1/4 border-r overflow-y-auto">
        {activeMain && activeMain.children.length > 0 ? (
          activeMain.children.map((sub) => (
            <Link key={sub.categoryId} to={`/category/${sub.slug}`}>
              <div
                onMouseEnter={() => setActiveSub(sub)}
                className={`px-4 py-3 cursor-pointer hover:bg-gray-100 text-[15px] transition ${
                  activeSub?.categoryId === sub.categoryId ? "bg-gray-100 font-semibold" : ""
                }`}
              >
                {sub.name}
              </div>
            </Link>
          ))
        ) : (
          <div className="p-4 text-gray-400">Alt kategori yok</div>
        )}
      </div>

      {/* RIGHT COLUMN – SUB SUB CATEGORIES */}
      <div className="w-2/4 p-4 overflow-y-auto">
        {activeSub ? (
          activeSub.children.length > 0 ? (
            <div className="grid grid-cols-2 gap-3">
              {activeSub.children.map((c3) => (
                <Link
                  key={c3.categoryId}
                  to={`/category/${c3.slug}`}
                  className="block px-3 py-2 rounded hover:bg-gray-100 text-[14px]"
                >
                  {c3.name}
                </Link>
              ))}
            </div>
          ) : (
            <Link
              to={`/category/${activeSub.slug}`}
              className="block px-4 py-3 text-orange-600 font-semibold hover:bg-gray-100 rounded text-[15px]"
            >
              {activeSub.name} kategorisine git →
            </Link>
          )
        ) : (
          <div className="text-gray-400">Bir alt kategori seçin</div>
        )}
      </div>

    </div>
  );
}