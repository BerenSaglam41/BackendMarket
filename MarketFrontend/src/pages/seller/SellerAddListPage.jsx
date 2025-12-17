import { useEffect, useMemo, useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import {
  ArrowLeftIcon,
  PhotoIcon,
  CurrencyDollarIcon,
  TruckIcon,
  TagIcon,
  ArchiveBoxIcon,
  ExclamationCircleIcon,
  MagnifyingGlassIcon,
  CheckBadgeIcon
} from "@heroicons/react/24/outline";

import { useSellerListingStore } from "../../store/seller/SellerListingStore";
import { useSellerAvailableForListingStore } from "../../store/seller/SellerAvailableForListingStore";

export default function SellerAddListingPage() {
  const navigate = useNavigate();

  /* -------- STORES -------- */
  const { createListing, loading: creating, error: createError } = useSellerListingStore();
  const {
    items: availableProducts,
    loading: productsLoading,
    error: productsError,
    fetchAvailableForListing,
  } = useSellerAvailableForListingStore();

  /* -------- LOCAL STATES -------- */
  const [searchTerm, setSearchTerm] = useState("");
  const [form, setForm] = useState({
    productId: "",
    originalPrice: "",
    discountPercentage: "",
    stock: "",
    shippingTimeInDays: "",
    shippingCost: "",
    sellerNote: "",
  });

  /* -------- FETCH -------- */
  useEffect(() => {
    fetchAvailableForListing();
  }, []);

  /* -------- FILTERING -------- */
  const filteredProducts = useMemo(() => {
      if(!searchTerm) return availableProducts;
      return availableProducts.filter(p => 
          p.name.toLowerCase().includes(searchTerm.toLowerCase()) || 
          p.slug.toLowerCase().includes(searchTerm.toLowerCase())
      );
  }, [availableProducts, searchTerm]);

  /* -------- SELECTED PRODUCT -------- */
  const selectedProduct = useMemo(
    () => availableProducts.find((p) => String(p.productId) === String(form.productId)),
    [form.productId, availableProducts]
  );

  /* -------- PRICE CALC -------- */
  const unitPrice = useMemo(() => {
    const price = Number(form.originalPrice);
    const discount = Number(form.discountPercentage);
    if (!price) return 0;
    if (!discount || discount <= 0) return price;
    return price - price * (discount / 100);
  }, [form.originalPrice, form.discountPercentage]);

  const onChange = (key, value) =>
    setForm((prev) => ({ ...prev, [key]: value }));

  const onSubmit = async (e) => {
    e.preventDefault();
    if (!form.productId) return;

    const payload = {
      productId: Number(form.productId),
      originalPrice: Number(form.originalPrice),
      discountPercentage: Number(form.discountPercentage) || 0,
      stock: Number(form.stock),
      shippingTimeInDays: Number(form.shippingTimeInDays),
      shippingCost: Number(form.shippingCost) || 0,
      sellerNote: form.sellerNote?.trim() || null,
    };

    await createListing(payload);
    navigate("/seller/listings"); // İşlem bitince listeye dön
  };

  if (productsLoading && availableProducts.length === 0) {
      return (
        <div className="min-h-screen bg-slate-50/50 flex items-center justify-center">
             <div className="w-10 h-10 border-4 border-slate-200 border-t-orange-500 rounded-full animate-spin"></div>
        </div>
      );
  }

  return (
    <div className="min-h-screen bg-slate-50/50 p-6 lg:p-10 font-sans text-slate-800">
      
      {/* HEADER */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 mb-8 max-w-6xl mx-auto">
        <div className="flex items-center gap-4">
             <button onClick={() => navigate(-1)} className="p-2 hover:bg-white hover:shadow-sm rounded-full transition text-slate-500">
                 <ArrowLeftIcon className="w-6 h-6"/>
             </button>
             <div>
                <h1 className="text-2xl font-bold text-slate-900 tracking-tight">Yeni Satış (Listing) Ekle</h1>
                <p className="text-slate-500 mt-1 text-sm">
                    Platformda onaylı olan bir ürünü kendi mağazanızda satışa açın.
                </p>
             </div>
        </div>

        <Link
          to="/seller/lists"
          className="px-5 py-2.5 rounded-xl border border-slate-200 bg-white text-slate-600 font-medium hover:bg-slate-50 transition"
        >
          Vazgeç
        </Link>
      </div>

      {/* ERROR MESSAGE */}
      {(createError || productsError) && (
        <div className="max-w-6xl mx-auto mb-6 bg-rose-50 border border-rose-200 text-rose-700 rounded-2xl p-4 flex items-center gap-3">
          <ExclamationCircleIcon className="w-6 h-6 shrink-0"/>
          <span>{createError || productsError}</span>
        </div>
      )}

      {/* MAIN GRID */}
      <div className="max-w-6xl mx-auto grid grid-cols-1 lg:grid-cols-3 gap-8">
        
        {/* --- LEFT COLUMN: PRODUCT SELECTION --- */}
        <div className="space-y-6">
            
            {/* SEARCH & LIST */}
            <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm h-[600px] flex flex-col">
                <h3 className="text-lg font-bold text-slate-900 mb-4 flex items-center gap-2">
                    <ArchiveBoxIcon className="w-5 h-5 text-orange-500"/>
                    Ürün Seçimi
                </h3>
                
                {/* Search Bar */}
                <div className="relative mb-4">
                    <MagnifyingGlassIcon className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400"/>
                    <input 
                        type="text" 
                        placeholder="Ürün adı veya kod ara..." 
                        className="w-full pl-10 pr-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-orange-500 outline-none text-sm"
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                    />
                </div>

                {/* Product List */}
                <div className="flex-1 overflow-y-auto space-y-2 pr-2 custom-scrollbar">
                    {filteredProducts.map((p) => (
                        <div 
                            key={p.productId}
                            onClick={() => onChange("productId", String(p.productId))}
                            className={`p-3 rounded-xl border cursor-pointer transition-all flex items-center gap-3 ${
                                String(form.productId) === String(p.productId)
                                ? "bg-orange-50 border-orange-200 ring-1 ring-orange-200 shadow-sm"
                                : "bg-white border-slate-100 hover:bg-slate-50"
                            }`}
                        >
                            <div className="w-12 h-12 bg-slate-100 rounded-lg border border-slate-200 flex-shrink-0 overflow-hidden flex items-center justify-center">
                                {p.imageUrl ? (
                                    <img src={p.imageUrl} className="w-full h-full object-cover" alt=""/>
                                ) : (
                                    <PhotoIcon className="w-6 h-6 text-slate-300"/>
                                )}
                            </div>
                            <div className="flex-1 min-w-0">
                                <h4 className={`text-sm font-bold truncate ${String(form.productId) === String(p.productId) ? "text-orange-700" : "text-slate-700"}`}>
                                    {p.name}
                                </h4>
                                <p className="text-xs text-slate-400 truncate">{p.categoryName || "Kategorisiz"}</p>
                            </div>
                            {String(form.productId) === String(p.productId) && (
                                <CheckBadgeIcon className="w-6 h-6 text-orange-500"/>
                            )}
                        </div>
                    ))}
                    
                    {filteredProducts.length === 0 && (
                        <div className="text-center py-10 text-slate-400 text-sm">
                            Ürün bulunamadı. <br/> 
                            <Link to="/seller/pending-products/add" className="text-orange-600 hover:underline">Yeni ürün başvurusu yapın.</Link>
                        </div>
                    )}
                </div>
            </div>
        </div>

        {/* --- RIGHT COLUMN: FORM --- */}
        <div className="lg:col-span-2 space-y-6">
            
            {/* SELECTED PRODUCT SUMMARY CARD */}
            {selectedProduct ? (
                <div className="bg-white p-5 rounded-2xl border border-orange-100 shadow-sm flex items-center gap-5">
                    <div className="w-20 h-20 bg-slate-100 rounded-xl border border-slate-200 overflow-hidden flex-shrink-0">
                        {selectedProduct.imageUrl ? (
                            <img src={selectedProduct.imageUrl} className="w-full h-full object-cover" alt=""/>
                        ) : (
                            <PhotoIcon className="w-8 h-8 text-slate-300 m-auto mt-6"/>
                        )}
                    </div>
                    <div>
                        <h2 className="text-xl font-bold text-slate-900">{selectedProduct.name}</h2>
                        <div className="flex items-center gap-3 mt-1 text-sm text-slate-500">
                            <span className="bg-slate-100 px-2 py-0.5 rounded text-slate-700 font-medium">{selectedProduct.brandName || "Markasız"}</span>
                            <span>•</span>
                            <span>{selectedProduct.categoryName}</span>
                        </div>
                    </div>
                </div>
            ) : (
                <div className="bg-slate-100 border-2 border-dashed border-slate-200 rounded-2xl p-8 text-center text-slate-400">
                    <ArchiveBoxIcon className="w-10 h-10 mx-auto mb-2 opacity-50"/>
                    <p>Lütfen soldaki listeden satışa açmak istediğiniz ürünü seçin.</p>
                </div>
            )}

            {/* FORM CARD */}
            <form onSubmit={onSubmit} className={`bg-white p-6 rounded-2xl border border-slate-200 shadow-sm transition-opacity duration-300 ${!selectedProduct ? 'opacity-50 pointer-events-none' : ''}`}>
                
                {/* PRICING */}
                <div className="mb-8">
                    <h3 className="text-lg font-bold text-slate-900 mb-5 flex items-center gap-2">
                        <CurrencyDollarIcon className="w-5 h-5 text-orange-500"/>
                        Fiyatlandırma
                    </h3>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        <InputField 
                            label="Satış Fiyatı (₺)" 
                            value={form.originalPrice} 
                            onChange={(val) => onChange("originalPrice", val)}
                            type="number"
                            placeholder="0.00"
                            required
                        />
                        <InputField 
                            label="İndirim Oranı (%)" 
                            value={form.discountPercentage} 
                            onChange={(val) => onChange("discountPercentage", val)}
                            type="number"
                            placeholder="Opsiyonel"
                        />
                    </div>
                    {/* Live Preview */}
                    <div className="mt-4 bg-emerald-50 border border-emerald-100 rounded-xl p-4 flex justify-between items-center">
                        <span className="text-sm font-medium text-emerald-700">Müşterinin Göreceği Fiyat:</span>
                        <span className="text-xl font-bold text-emerald-700">
                            {unitPrice.toLocaleString("tr-TR", { minimumFractionDigits: 2 })} ₺
                        </span>
                    </div>
                </div>

                {/* STOCK & LOGISTICS */}
                <div className="mb-8">
                    <h3 className="text-lg font-bold text-slate-900 mb-5 flex items-center gap-2">
                        <TruckIcon className="w-5 h-5 text-orange-500"/>
                        Stok ve Lojistik
                    </h3>
                    <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                        <InputField 
                            label="Stok Adedi" 
                            value={form.stock} 
                            onChange={(val) => onChange("stock", val)}
                            type="number"
                            required
                        />
                        <InputField 
                            label="Kargo Süresi (Gün)" 
                            value={form.shippingTimeInDays} 
                            onChange={(val) => onChange("shippingTimeInDays", val)}
                            type="number"
                            required
                        />
                        <InputField 
                            label="Kargo Ücreti (₺)" 
                            value={form.shippingCost} 
                            onChange={(val) => onChange("shippingCost", val)}
                            type="number"
                            placeholder="0 = Ücretsiz"
                        />
                    </div>
                </div>

                {/* NOTE */}
                <div className="mb-8">
                    <h3 className="text-lg font-bold text-slate-900 mb-4 flex items-center gap-2">
                        <TagIcon className="w-5 h-5 text-orange-500"/>
                        Kendime Not
                    </h3>
                    <textarea
                        value={form.sellerNote}
                        onChange={(e) => onChange("sellerNote", e.target.value)}
                        className="w-full px-4 py-3 rounded-xl border border-slate-200 bg-slate-50 focus:ring-2 focus:ring-orange-500 outline-none min-h-[80px] text-sm resize-none"
                        placeholder="Bu satışla ilgili özel notunuz..."
                    />
                </div>

                {/* SUBMIT */}
                <div className="flex justify-end pt-4 border-t border-slate-100">
                    <button
                        type="submit"
                        disabled={creating || !selectedProduct}
                        className="px-8 py-3 rounded-xl bg-orange-600 text-white font-bold hover:bg-orange-700 shadow-lg shadow-orange-600/20 transition active:scale-95 disabled:opacity-60 disabled:cursor-not-allowed"
                    >
                        {creating ? "Oluşturuluyor..." : "Satışı Başlat"}
                    </button>
                </div>

            </form>
        </div>

      </div>
    </div>
  );
}

// --- UI COMPONENTS ---
function InputField({ label, value, onChange, type = "text", placeholder, required }) {
  return (
    <div>
      <label className="block text-sm font-semibold text-slate-700 mb-1.5 flex justify-between">
          {label}
          {required && <span className="text-orange-500">*</span>}
      </label>
      <input
        type={type}
        value={value}
        onChange={(e) => onChange(e.target.value)}
        placeholder={placeholder}
        required={required}
        className="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-orange-500 focus:bg-white outline-none transition text-sm"
      />
    </div>
  );
}