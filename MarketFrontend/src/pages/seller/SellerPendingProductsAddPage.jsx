import { useEffect, useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import {
  ArrowLeftIcon,
  PhotoIcon,
  CurrencyDollarIcon,
  ArchiveBoxIcon,
  TagIcon,
  ChatBubbleBottomCenterTextIcon,
  ExclamationCircleIcon,
  QrCodeIcon,
  CpuChipIcon
} from "@heroicons/react/24/outline";

// Stores
import { useSellerPendingProductStore } from "../../store/seller/SellerPendingProductStore";
import { useBrandStore } from "../../store/brandStore";
import { useCategoryStore } from "../../store/CategoryStore";

export default function SellerPendingProductAddPage() {
  const navigate = useNavigate();

  // Stores
  const { createPending, loading, error } = useSellerPendingProductStore();
  const { brands, fetchBrands } = useBrandStore();
  const { categories, fetchCategories } = useCategoryStore();

  // Local State
  const [isSlugManuallyChanged, setIsSlugManuallyChanged] = useState(false);
  const [validationErrors, setValidationErrors] = useState({});

  // DTO'daki TÜM alanlar
  const [form, setForm] = useState({
    name: "",
    slug: "",
    description: "",
    
    brandId: "",
    categoryId: "",
    sellerCategorySuggestion: "", 
    
    imageUrl: "",
    
    sellerSku: "",
    barcode: "",
    attributesJson: "", 
    sellerNote: "",     
    
    proposedPrice: "",
    discountPercentage: "", // YENİ EKLENDİ
    proposedStock: "",
    shippingTimeInDays: "",
  });

  // Verileri Yükle
  useEffect(() => {
    fetchBrands();
    fetchCategories();
  }, []);

  // Otomatik Slug Oluşturucu
  useEffect(() => {
    if (!isSlugManuallyChanged && form.name) {
      const slug = form.name
        .toLowerCase()
        .replace(/ /g, "-")
        .replace(/[^\w-]+/g, "");
      setForm((prev) => ({ ...prev, slug }));
    }
  }, [form.name, isSlugManuallyChanged]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));

    if (validationErrors[name]) {
        setValidationErrors(prev => ({...prev, [name]: null}));
    }

    if (name === "slug") setIsSlugManuallyChanged(true);
  };

  // --- VALIDASYON ---
  const validate = () => {
      const errors = {};
      
      // Zorunlu Alanlar
      if(!form.name.trim()) errors.name = "Ürün adı zorunludur.";
      if(!form.slug.trim()) errors.slug = "Slug alanı zorunludur.";
      
      if(!form.brandId) errors.brandId = "Marka seçimi zorunludur.";
      if(!form.categoryId) errors.categoryId = "Kategori seçimi zorunludur.";

      if(!form.proposedPrice || Number(form.proposedPrice) <= 0) 
          errors.proposedPrice = "Geçerli bir fiyat giriniz.";
      
      // İndirim Oranı Kontrolü
      if(form.discountPercentage && (Number(form.discountPercentage) < 0 || Number(form.discountPercentage) > 99))
          errors.discountPercentage = "0-99 arası olmalı.";

      if(form.proposedStock === "" || Number(form.proposedStock) < 0) 
          errors.proposedStock = "Stok bilgisi zorunludur.";
      
      if(!form.shippingTimeInDays || Number(form.shippingTimeInDays) <= 0) 
          errors.shippingTimeInDays = "Kargo süresi zorunludur.";

      setValidationErrors(errors);
      return Object.keys(errors).length === 0;
  }

  const onSubmit = async (e) => {
    e.preventDefault();
    if(!validate()) return;

    // DTO Formatına Dönüştürme
    const payload = {
      name: form.name.trim(),
      slug: form.slug.trim().toLowerCase(),
      description: form.description?.trim() || null,

      brandId: Number(form.brandId),
      categoryId: Number(form.categoryId),
      sellerCategorySuggestion: form.sellerCategorySuggestion?.trim() || null,

      imageUrl: form.imageUrl?.trim() || null,
      imageGallery: null,

      sellerSku: form.sellerSku?.trim() || null,
      barcode: form.barcode?.trim() || null,
      attributesJson: form.attributesJson?.trim() || null,
      sellerNote: form.sellerNote?.trim() || null,

      proposedPrice: Number(form.proposedPrice),
      discountPercentage: form.discountPercentage ? Number(form.discountPercentage) : 0, // YENİ
      proposedStock: Number(form.proposedStock),
      shippingTimeInDays: Number(form.shippingTimeInDays),
    };

    try {
        await createPending(payload);
        navigate("/seller/pending-products");
    } catch (err) {
        console.error(err);
    }
  };

  return (
    <div className="min-h-screen bg-slate-50/50 p-6 lg:p-10 font-sans text-slate-800">
      
      {/* HEADER */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 mb-8 max-w-6xl mx-auto">
        <div className="flex items-center gap-4">
             <button onClick={() => navigate(-1)} className="p-2 hover:bg-white hover:shadow-sm rounded-full transition text-slate-500">
                 <ArrowLeftIcon className="w-6 h-6"/>
             </button>
             <div>
                <h1 className="text-2xl font-bold text-slate-900 tracking-tight">Yeni Ürün Başvurusu</h1>
                <p className="text-slate-500 mt-1 text-sm">
                    Ürün detaylarını eksiksiz doldurun. Onaylandığında otomatik listelenecektir.
                </p>
             </div>
        </div>

        <div className="flex gap-3 ml-12 md:ml-0">
          <Link
            to="/seller/pending-products"
            className="px-5 py-2.5 rounded-xl border border-slate-200 bg-white text-slate-600 font-medium hover:bg-slate-50 transition"
          >
            Vazgeç
          </Link>
          <button
            onClick={onSubmit}
            disabled={loading}
            className="px-6 py-2.5 rounded-xl bg-orange-600 text-white font-bold hover:bg-orange-700 shadow-lg shadow-orange-600/20 transition active:scale-95 disabled:opacity-70"
          >
            {loading ? "Gönderiliyor..." : "Başvuruyu Gönder"}
          </button>
        </div>
      </div>

      {/* ERROR MESSAGE */}
      {error && (
        <div className="max-w-6xl mx-auto mb-6 bg-rose-50 border border-rose-200 text-rose-700 rounded-2xl p-4 flex items-center gap-3">
          <ExclamationCircleIcon className="w-6 h-6 shrink-0"/>
          <span>{error}</span>
        </div>
      )}

      {/* MAIN FORM LAYOUT */}
      <div className="max-w-6xl mx-auto grid grid-cols-1 lg:grid-cols-3 gap-8">
        
        {/* --- LEFT COLUMN --- */}
        <div className="lg:col-span-2 space-y-6">
            
            {/* BASIC INFO */}
            <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
                <h3 className="text-lg font-bold text-slate-900 mb-5 flex items-center gap-2">
                    <ArchiveBoxIcon className="w-5 h-5 text-slate-400"/>
                    Temel Bilgiler
                </h3>
                
                <div className="space-y-4">
                    <InputField 
                        label="Ürün Adı *" 
                        name="name" 
                        value={form.name} 
                        onChange={handleChange} 
                        error={validationErrors.name}
                        placeholder="Örn: iPhone 15 Pro Max"
                    />

                    <div className="relative">
                        <InputField 
                            label="Slug (URL) *" 
                            name="slug" 
                            value={form.slug} 
                            onChange={handleChange} 
                            error={validationErrors.slug}
                            placeholder="urun-adi-otomatik-olur"
                        />
                        <span className="absolute top-9 right-3 text-xs text-slate-400">market.com/urun/</span>
                    </div>

                    <div>
                        <label className="block text-sm font-semibold text-slate-700 mb-1.5">Açıklama</label>
                        <textarea
                            name="description"
                            value={form.description}
                            onChange={handleChange}
                            rows={5}
                            className="w-full px-4 py-3 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-orange-500 focus:bg-white outline-none transition text-sm resize-none"
                            placeholder="Ürün özelliklerini detaylıca anlatın..."
                        />
                    </div>
                </div>
            </div>

            {/* MEDIA */}
            <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
                <h3 className="text-lg font-bold text-slate-900 mb-5 flex items-center gap-2">
                    <PhotoIcon className="w-5 h-5 text-slate-400"/>
                    Görseller
                </h3>

                <InputField 
                    label="Kapak Görsel URL" 
                    name="imageUrl" 
                    value={form.imageUrl} 
                    onChange={handleChange} 
                    placeholder="https://..."
                />

                <div className="mt-4 p-4 border-2 border-dashed border-slate-200 rounded-xl bg-slate-50 flex items-center justify-center min-h-[150px]">
                    {form.imageUrl ? (
                        <img 
                            src={form.imageUrl} 
                            alt="Preview" 
                            className="max-h-40 rounded-lg shadow-sm object-contain"
                            onError={(e) => {e.target.style.display='none'}}
                        />
                    ) : (
                        <div className="flex flex-col items-center text-slate-400">
                            <PhotoIcon className="w-10 h-10 mb-2 opacity-50"/>
                            <span className="text-xs">Görsel URL'si girerseniz önizleme burada çıkar.</span>
                        </div>
                    )}
                </div>
            </div>

             {/* SALES INFO */}
             <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
                <h3 className="text-lg font-bold text-slate-900 mb-5 flex items-center gap-2">
                    <CurrencyDollarIcon className="w-5 h-5 text-slate-400"/>
                    Satış Bilgileri
                </h3>
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                    <InputField 
                        label="Önerilen Fiyat (₺) *" 
                        name="proposedPrice" 
                        value={form.proposedPrice} 
                        onChange={handleChange} 
                        error={validationErrors.proposedPrice}
                        type="number"
                        placeholder="0.00"
                    />
                     {/* İNDİRİM ALANI EKLENDİ */}
                    <InputField 
                        label="İndirim Oranı (%)" 
                        name="discountPercentage" 
                        value={form.discountPercentage} 
                        onChange={handleChange} 
                        error={validationErrors.discountPercentage}
                        type="number"
                        placeholder="0"
                    />
                    <InputField 
                        label="Önerilen Stok *" 
                        name="proposedStock" 
                        value={form.proposedStock} 
                        onChange={handleChange} 
                        error={validationErrors.proposedStock}
                        type="number"
                        placeholder="0"
                    />
                    <InputField 
                        label="Kargo Süresi (Gün) *" 
                        name="shippingTimeInDays" 
                        value={form.shippingTimeInDays} 
                        onChange={handleChange} 
                        error={validationErrors.shippingTimeInDays}
                        type="number"
                        placeholder="3"
                    />
                </div>
             </div>
        </div>

        {/* --- RIGHT COLUMN (SIDEBAR) --- */}
        <div className="space-y-6">
            
            {/* ORGANIZATION */}
            <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
                <h3 className="text-lg font-bold text-slate-900 mb-5 flex items-center gap-2">
                    <TagIcon className="w-5 h-5 text-slate-400"/>
                    Organizasyon
                </h3>

                <div className="space-y-4">
                    {/* Brand Select */}
                    <div>
                        <label className="block text-sm font-semibold text-slate-700 mb-1.5 flex justify-between">
                            Marka *
                            {validationErrors.brandId && <span className="text-xs text-rose-500 font-normal">{validationErrors.brandId}</span>}
                        </label>
                        <select
                            name="brandId"
                            value={form.brandId}
                            onChange={handleChange}
                            className={`w-full px-3 py-2.5 bg-slate-50 border rounded-xl focus:ring-2 focus:ring-orange-500 outline-none text-sm ${
                                validationErrors.brandId ? "border-rose-300" : "border-slate-200"
                            }`}
                        >
                            <option value="">Seçiniz...</option>
                            {brands.map((b) => (
                                <option key={b.brandId} value={b.brandId}>{b.name}</option>
                            ))}
                        </select>
                    </div>

                    {/* Category Select */}
                    <div>
                        <label className="block text-sm font-semibold text-slate-700 mb-1.5 flex justify-between">
                            Kategori *
                            {validationErrors.categoryId && <span className="text-xs text-rose-500 font-normal">{validationErrors.categoryId}</span>}
                        </label>
                        <select
                            name="categoryId"
                            value={form.categoryId}
                            onChange={handleChange}
                            className={`w-full px-3 py-2.5 bg-slate-50 border rounded-xl focus:ring-2 focus:ring-orange-500 outline-none text-sm ${
                                validationErrors.categoryId ? "border-rose-300" : "border-slate-200"
                            }`}
                        >
                            <option value="">Seçiniz...</option>
                            {categories.map((c) => (
                                <option key={c.categoryId} value={c.categoryId}>{c.name}</option>
                            ))}
                        </select>
                    </div>

                    {/* Category Suggestion */}
                    <div>
                         <label className="block text-xs font-medium text-slate-400 mb-1">
                             Kategori listede yoksa öneriniz:
                         </label>
                         <input 
                            name="sellerCategorySuggestion"
                            value={form.sellerCategorySuggestion}
                            onChange={handleChange}
                            className="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-lg text-sm focus:ring-2 focus:ring-orange-500 outline-none"
                            placeholder="Örn: Akıllı Ev Aletleri"
                         />
                    </div>
                </div>
            </div>

            {/* PRODUCT CODES & DETAILS */}
            <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
                <h3 className="text-lg font-bold text-slate-900 mb-5 flex items-center gap-2">
                    <QrCodeIcon className="w-5 h-5 text-slate-400"/>
                    Ürün Kodları
                </h3>

                <div className="space-y-4">
                    <InputField 
                        label="Seller SKU (Stok Kodu)" 
                        name="sellerSku" 
                        value={form.sellerSku} 
                        onChange={handleChange} 
                        placeholder="Sizin tarafınızdaki kod"
                    />
                    <InputField 
                        label="Barkod (EAN/UPC)" 
                        name="barcode" 
                        value={form.barcode} 
                        onChange={handleChange} 
                        placeholder="869..."
                    />
                </div>
            </div>

            {/* ATTRIBUTES JSON */}
            <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
                <h3 className="text-sm font-bold text-slate-900 mb-3 flex items-center gap-2">
                     <CpuChipIcon className="w-5 h-5 text-slate-400"/>
                     Teknik Özellikler (JSON)
                </h3>
                <textarea
                  name="attributesJson"
                  value={form.attributesJson}
                  onChange={handleChange}
                  className="w-full px-4 py-3 rounded-xl border border-slate-200 bg-slate-50 focus:ring-2 focus:ring-orange-500 outline-none min-h-[100px] font-mono text-xs"
                  placeholder='{"renk": "Siyah", "hafiza": "256GB"}'
                />
            </div>

            {/* SELLER NOTE */}
            <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
                 <h3 className="text-sm font-bold text-slate-900 mb-3 flex items-center gap-2">
                     <ChatBubbleBottomCenterTextIcon className="w-4 h-4 text-slate-400"/>
                     Satıcı Notu (Admine)
                 </h3>
                 <textarea
                    name="sellerNote"
                    value={form.sellerNote}
                    onChange={handleChange}
                    className="w-full px-4 py-3 rounded-xl border border-slate-200 bg-slate-50 focus:ring-2 focus:ring-orange-500 outline-none min-h-[100px] text-sm"
                    placeholder="Varsa admine iletmek istediğiniz not..."
                  />
            </div>

        </div>
      </div>
    </div>
  );
}

// --- REUSABLE INPUT COMPONENT ---
function InputField({ label, name, value, onChange, error, type = "text", placeholder }) {
  return (
    <div>
      <label className="block text-sm font-semibold text-slate-700 mb-1.5 flex justify-between">
          {label}
          {error && <span className="text-xs text-rose-500 font-normal">{error}</span>}
      </label>
      <input
        type={type}
        name={name}
        value={value}
        onChange={onChange}
        placeholder={placeholder}
        className={`w-full px-4 py-2.5 bg-slate-50 border rounded-xl focus:ring-2 focus:bg-white outline-none transition text-sm ${
            error 
            ? "border-rose-300 focus:ring-rose-200 bg-rose-50 text-rose-900" 
            : "border-slate-200 focus:ring-orange-500"
        }`}
      />
    </div>
  );
}