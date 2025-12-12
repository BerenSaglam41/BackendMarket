import { useState } from "react";
import { Link } from "react-router-dom";

export default function MegaMenu({ categories, onClose }) {
  const [activeMain, setActiveMain] = useState(null);
  const [activeSub, setActiveSub] = useState(null);

  if (!categories || categories.length === 0) return null;

  return (
    <div className="w-full bg-white shadow-xl border-t border-gray-200 flex h-[420px]">
      
      {/* 1. SOL PANEL — ANA KATEGORİLER */}
      <div className="w-1/4 border-r overflow-y-auto bg-gray-50">
        {categories.map((cat) => (
          <Link
            key={cat.categoryId}
            to={`/category/${cat.slug}`} 
            onClick={onClose}            
            onMouseEnter={() => {        
              setActiveMain(cat);
              setActiveSub(null);
            }}
            className={`px-6 py-3 flex items-center gap-3 cursor-pointer transition text-sm block ${
              activeMain?.categoryId === cat.categoryId 
                ? "bg-white font-semibold text-orange-600 border-l-4 border-orange-500" 
                : "text-gray-700 hover:bg-gray-100"
            }`}
          >
             {cat.imageUrl && (
                <img 
                  src={cat.imageUrl} 
                  alt={cat.name} 
                  className="w-6 h-6 object-cover rounded-full"
                />
             )}
            <span>{cat.name}</span>
          </Link>
        ))}
      </div>

      {/* 2. ORTA PANEL — ALT KATEGORİLER */}
      <div className="w-1/4 border-r overflow-y-auto bg-white">
        {activeMain ? (
            activeMain.children && activeMain.children.length > 0 ? (
                activeMain.children.map((sub) => (
                    <Link
                      key={sub.categoryId}
                      to={`/category/${sub.slug}`}  
                      onClick={onClose}               
                      onMouseEnter={() => setActiveSub(sub)} 
                      className={`px-5 py-3 flex items-center gap-3 cursor-pointer text-sm transition block ${
                          activeSub?.categoryId === sub.categoryId 
                          ? "text-orange-600 font-semibold bg-gray-50" 
                          : "text-gray-600 hover:text-orange-500"
                      }`}
                    >
                      <span>{sub.name}</span>
                    </Link>
                ))
            ) : (
                <div className="p-5 text-gray-400 text-sm">Bu kategoride alt menü yok.</div>
            )
        ) : (
            <div className="p-5 text-gray-400 text-sm">Lütfen bir ana kategori seçin.</div>
        )}
      </div>

      {/* 3. SAĞ PANEL — EN ALT KATEGORİLER (Zaten Link idi, aynen koruyoruz) */}
      <div className="w-1/2 p-6 overflow-y-auto bg-white">
        {activeSub && activeSub.children && activeSub.children.length > 0 ? (
          <div>
            {/* Başlığa da Link verelim ki kullanıcı başlığa tıklayıp da gidebilsin */}
            <div className="mb-4 border-b pb-2">
                <Link 
                    to={`/category/${activeSub.slug}`}
                    onClick={onClose}
                    className="font-bold text-gray-800 hover:text-orange-600 transition"
                >
                    {activeSub.name}
                </Link>
            </div>
            
            <div className="grid grid-cols-2 gap-y-2 gap-x-4">
                {activeSub.children.map((c3) => (
                <Link
                    to={`/category/${c3.slug}`}
                    key={c3.categoryId}
                    onClick={onClose}
                    className="flex items-center gap-2 text-sm text-gray-600 hover:text-orange-600 hover:underline"
                >
                    {c3.name}
                </Link>
                ))}
            </div>
          </div>
        ) : activeSub ? (
          <div className="flex flex-col items-start">
            <h3 className="text-xl font-bold mb-2 text-gray-800">
              {activeSub.name}
            </h3>
            <p className="text-gray-500 text-sm mb-4">Bu kategorideki tüm ürünleri keşfet.</p>

            {(activeSub.imageUrl || activeMain.imageUrl) && (
                 <img
                 src={activeSub.imageUrl || activeMain.imageUrl}
                 alt={activeSub.name}
                 className="w-full h-48 object-cover rounded-lg mb-4 shadow-sm"
               />
            )}

            <Link
              to={`/category/${activeSub.slug}`}
              onClick={onClose}
              className="px-6 py-2 bg-orange-500 text-white text-sm font-semibold rounded shadow hover:bg-orange-600 transition"
            >
              {activeSub.name} Ürünlerine Git →
            </Link>
          </div>
        ) : (
          <div className="h-full flex flex-col items-center justify-center text-gray-300">
             <span className="text-lg">Kategori Detayları</span>
          </div>
        )}
      </div>
    </div>
  );
}