import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { useAdminProductStore } from "../../store/admin/adminProductStore";
import { 
  ArrowLeftIcon, 
  ExclamationCircleIcon, 
  PhotoIcon,
  CheckCircleIcon,
  XCircleIcon,
  CalendarDaysIcon,
  ArrowPathIcon,
  WrenchScrewdriverIcon
} from "@heroicons/react/24/outline";
import { useCategoryStore } from "../../store/CategoryStore";
import { useBrandStore } from "../../store/brandStore";

export default function AdminEditProductPage() {
  const { productSlug } = useParams();
  const navigate = useNavigate();
  const { brands, fetchBrands } = useBrandStore();
  const { categories, fetchCategories } = useCategoryStore();
  const {
    selectedProduct,
    fetchProductBySlug,
    updateProduct,
    loading,
  } = useAdminProductStore();

  // Form State
  const [form, setForm] = useState({
    name: "",
    slug: "",
    description: "",
    brandId: "",
    categoryId: "",
    imageUrl: "",
    metaTitle: "",
    metaDescription: "",
    isActive: false
  });
  
  const [errors, setErrors] = useState({});
  const [isSaving, setIsSaving] = useState(false);
  const [isInitialized, setIsInitialized] = useState(false);

  // 1. Veriyi Çek
  useEffect(() => {
    if (productSlug) {
        fetchProductBySlug(productSlug).catch(() => {});
    }
  }, [productSlug]);
  useEffect(() => {
    fetchBrands();
    fetchCategories();
  }, []);
  // 2. Veri Gelince Formu Doldur
  useEffect(() => {
    if (selectedProduct && (selectedProduct.slug === productSlug || selectedProduct.slug)) {
      setForm({
        name: selectedProduct.name || "",
        slug: selectedProduct.slug || "",
        description: selectedProduct.description || "",
        brandId: selectedProduct.brandId || "", 
        categoryId: selectedProduct.categoryId || "", 
        imageUrl: selectedProduct.imageUrl || "",
        metaTitle: selectedProduct.metaTitle || "",
        metaDescription: selectedProduct.metaDescription || "",
        isActive: selectedProduct.isActive ?? false,
      });
      setIsInitialized(true);
    }
  }, [selectedProduct, productSlug]);

  // --- HANDLERS ---
  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setForm({ ...form, [name]: type === "checkbox" ? checked : value });
    
    // Kullanıcı yazarken o alanın hatasını temizle
    if (errors[name]) setErrors({ ...errors, [name]: null });
  };

  // Slug Otomatik Düzeltici
  const handleFixSlug = () => {
    const fixedSlug = form.slug.toLowerCase().trim().replace(/\s+/g, '-').replace(/[^a-z0-9-]/g, '').replace(/-+/g, '-');
    setForm({ ...form, slug: fixedSlug });
    setErrors({ ...errors, slug: null });
  };

  // --- DİNAMİK VALİDASYON ---
  // Sadece 'changes' (değişenler) objesindeki alanları kontrol eder
  const validate = (changes) => {
    const errs = {};
    
    // 1. İsim değiştiyse ve boşsa
    if (changes.hasOwnProperty('name') && !changes.name.trim()) {
        errs.name = "Ürün adı boş bırakılamaz.";
    }

    // 2. Slug değiştiyse
    if (changes.hasOwnProperty('slug')) {
        if (!changes.slug.trim()) {
            errs.slug = "Slug boş bırakılamaz.";
        } else {
            const slugRegex = /^[a-z0-9]+(?:-[a-z0-9]+)*$/;
            if (!slugRegex.test(changes.slug)) {
                errs.slug = "Geçersiz format (Küçük harf, rakam, tire).";
            }
        }
    }

    // 3. Marka/Kategori (Sadece sayısal kontrol)
    if (changes.hasOwnProperty('brandId') && !changes.brandId) errs.brandId = "Geçersiz marka.";
    if (changes.hasOwnProperty('categoryId') && !changes.categoryId) errs.categoryId = "Geçersiz kategori.";

    setErrors(errs);
    return Object.keys(errs).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setIsSaving(true);

    // 1. DIFFING (Sadece değişenleri bul)
    const changes = {};
    Object.keys(form).forEach((key) => {
        // Tip güvenliği için String'e çevirip karşılaştır
        const oldValue = selectedProduct[key] ?? "";
        const newValue = form[key];

        if (String(newValue) !== String(oldValue)) {
            // Sayısal alanları dönüştür
            if (["brandId", "categoryId"].includes(key)) {
                changes[key] = newValue ? Number(newValue) : null;
            } else {
                changes[key] = newValue;
            }
        }
    });

    // 2. DEĞİŞİKLİK YOKSA
    if (Object.keys(changes).length === 0) {
        alert("Herhangi bir değişiklik yapmadınız."); // Veya sessizce listeye dön
        setIsSaving(false);
        return;
    }

    // 3. VALİDASYON (Sadece değişenler için)
    if (!validate(changes)) {
        setIsSaving(false);
        window.scrollTo({ top: 0, behavior: 'smooth' });
        return;
    }

    // 4. GÖNDER
    try {
      await updateProduct(selectedProduct.productId, changes);
      navigate("/admin/products");
    } catch (error) {
      console.error("Update Hatası:", error);
      // Store zaten toast gösteriyor olabilir, göstermiyorsa burada alert açabilirsin
    } finally {
      setIsSaving(false);
    }
  };

  // LOADING STATE
  if (loading || (!selectedProduct && !isInitialized)) {
      return (
        <div className="min-h-screen bg-slate-50/50 p-10 flex flex-col items-center justify-center gap-4">
            <div className="w-12 h-12 border-4 border-slate-200 border-t-orange-500 rounded-full animate-spin"></div>
            <p className="text-slate-500 font-medium animate-pulse">Veriler hazırlanıyor...</p>
        </div>
      );
  }

  return (
    <div className="min-h-screen bg-slate-50/50 p-6 lg:p-10 font-sans text-slate-800">
      
      {/* HEADER */}
      <div className="flex items-center justify-between mb-8 max-w-6xl mx-auto">
        <div className="flex items-center gap-4">
            <button onClick={() => navigate(-1)} className="p-2 rounded-full hover:bg-white hover:shadow-sm transition text-slate-500">
                <ArrowLeftIcon className="w-5 h-5" />
            </button>
            <div>
                <h1 className="text-2xl font-bold text-slate-900">Ürünü Düzenle</h1>
                <p className="text-xs text-slate-500 font-mono mt-0.5">ID: {selectedProduct?.productId}</p>
            </div>
        </div>
        <div className="flex gap-3">
            <button onClick={() => navigate(-1)} className="px-5 py-2 text-sm font-medium text-slate-600 hover:bg-white hover:shadow-sm rounded-lg transition">
                Vazgeç
            </button>
            <button 
                onClick={handleSubmit} 
                disabled={isSaving}
                className="flex items-center gap-2 px-6 py-2 text-sm font-bold text-white bg-orange-600 hover:bg-orange-700 rounded-lg shadow-lg shadow-orange-600/20 transition active:scale-95 disabled:opacity-70"
            >
                {isSaving ? <ArrowPathIcon className="w-4 h-4 animate-spin"/> : "Değişiklikleri Kaydet"}
            </button>
        </div>
      </div>

      <div className="max-w-6xl mx-auto grid grid-cols-1 lg:grid-cols-3 gap-8">
        
        {/* --- SOL KOLON --- */}
        <div className="lg:col-span-2 space-y-6">
            
            {/* Temel Bilgiler */}
            <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
                <h3 className="text-lg font-bold text-slate-800 mb-4">Temel Bilgiler</h3>
                <div className="space-y-4">
                    <InputField 
                        label="Ürün Adı" 
                        name="name" 
                        value={form.name} 
                        onChange={handleChange} 
                        error={errors.name}
                    />
                    
                    <div>
                        <label className="block text-sm font-semibold text-slate-700 mb-1.5 flex justify-between">
                            Slug (URL)
                            {errors.slug && <span className="text-rose-500 text-xs font-normal flex items-center gap-1"><ExclamationCircleIcon className="w-3 h-3"/> {errors.slug}</span>}
                        </label>
                        <div className="relative">
                            <input
                                type="text"
                                name="slug"
                                value={form.slug}
                                onChange={handleChange}
                                className={`w-full px-4 py-2.5 bg-slate-50 border rounded-xl focus:ring-2 focus:bg-white outline-none transition text-sm ${errors.slug ? "border-rose-300 focus:ring-rose-200 bg-rose-50 text-rose-900" : "border-slate-200 focus:ring-orange-500"}`}
                            />
                            {errors.slug && (
                                <button type="button" onClick={handleFixSlug} className="absolute right-2 top-1.5 bg-rose-100 hover:bg-rose-200 text-rose-700 text-xs px-2 py-1 rounded-lg flex items-center gap-1 transition">
                                    <WrenchScrewdriverIcon className="w-3 h-3" /> Düzelt
                                </button>
                            )}
                        </div>
                    </div>

                    <div>
                        <label className="block text-sm font-semibold text-slate-700 mb-1.5">Açıklama</label>
                        <textarea
                            name="description"
                            value={form.description}
                            onChange={handleChange}
                            rows={6}
                            className="w-full px-4 py-3 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-orange-500 focus:bg-white outline-none transition text-sm resize-none"
                        />
                    </div>
                </div>
            </div>

            {/* Görseller */}
            <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
                <h3 className="text-lg font-bold text-slate-800 mb-4">Görseller</h3>
                <InputField 
                    label="Ana Görsel URL" 
                    name="imageUrl" 
                    value={form.imageUrl} 
                    onChange={handleChange} 
                />
                
                <div className="mt-4 p-4 border-2 border-dashed border-slate-200 rounded-xl bg-slate-50 flex items-center justify-center min-h-[200px]">
                    {form.imageUrl ? (
                        <img 
                            src={form.imageUrl} 
                            alt="Preview" 
                            className="max-h-64 rounded-lg shadow-sm object-contain" 
                            onError={(e) => e.target.style.display = 'none'} 
                        />
                    ) : (
                        <div className="flex flex-col items-center text-slate-400">
                            <PhotoIcon className="w-12 h-12 mb-2 opacity-50"/>
                            <span className="text-sm">Görsel yok</span>
                        </div>
                    )}
                </div>
            </div>

            {/* SEO */}
             <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
                <h3 className="text-lg font-bold text-slate-800 mb-4">SEO Ayarları</h3>
                <div className="space-y-4">
                    <InputField label="Meta Başlık" name="metaTitle" value={form.metaTitle} onChange={handleChange} />
                    <InputField label="Meta Açıklama" name="metaDescription" value={form.metaDescription} onChange={handleChange} />
                </div>
            </div>
        </div>

        {/* --- SAĞ KOLON --- */}
        <div className="space-y-6">
            
            {/* Durum */}
            <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
                 <h3 className="text-sm font-bold text-slate-800 mb-3">Yayın Durumu</h3>
                 <div className="flex items-center justify-between p-3 rounded-xl border border-slate-100 bg-slate-50">
                    <div className="flex items-center gap-2">
                        {form.isActive ? <CheckCircleIcon className="w-5 h-5 text-emerald-500"/> : <XCircleIcon className="w-5 h-5 text-slate-400"/>}
                        <span className={`text-sm font-medium ${form.isActive ? 'text-emerald-700' : 'text-slate-500'}`}>
                            {form.isActive ? "Yayında" : "Pasif"}
                        </span>
                    </div>
                    <label className="relative inline-flex items-center cursor-pointer">
                        <input type="checkbox" name="isActive" checked={form.isActive} onChange={handleChange} className="sr-only peer" />
                        <div className="w-11 h-6 bg-slate-200 peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-orange-600"></div>
                    </label>
                 </div>
                 {selectedProduct && selectedProduct.createdAt && (
                    <div className="mt-4 pt-4 border-t border-slate-100 text-xs text-slate-400 flex flex-col gap-1">
                        <span className="flex items-center gap-1"><CalendarDaysIcon className="w-3 h-3"/> Kayıt: {new Date(selectedProduct.createdAt).toLocaleDateString("tr-TR")}</span>
                    </div>
                 )}
            </div>

            {/* Organizasyon */}
            <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
                <h3 className="text-lg font-bold text-slate-800 mb-4">Organizasyon</h3>
                <div className="space-y-4">
                    <SelectField 
                        label="Marka"
                        name="brandId"
                        value={form.brandId}
                        onChange={handleChange}
                        options={brands.map(b => ({id: b.brandId, name: b.name}))}
                        error={errors.brandId}
                    />
                    <SelectField 
                        label="Kategori"
                        name="categoryId"
                        value={form.categoryId}
                        onChange={handleChange}
                        options={categories.map(c => ({id: c.categoryId, name: c.name}))}
                        error={errors.categoryId}
                    />
                </div>
            </div>
        </div>

      </div>
    </div>
  );
}

// --- SUB COMPONENTS ---
function InputField({ label, name, value, onChange, error, placeholder }) {
    return (
        <div>
            <label className="block text-sm font-semibold text-slate-700 mb-1.5 flex justify-between">
                {label}
                {error && <span className="text-rose-500 text-xs font-normal flex items-center gap-1"><ExclamationCircleIcon className="w-3 h-3"/> {error}</span>}
            </label>
            <input
                type="text"
                name={name}
                value={value || ""}
                onChange={onChange}
                placeholder={placeholder}
                className={`w-full px-4 py-2.5 bg-slate-50 border rounded-xl focus:ring-2 focus:bg-white outline-none transition text-sm ${
                    error ? "border-rose-300 focus:ring-rose-200 bg-rose-50 text-rose-900" : "border-slate-200 focus:ring-orange-500 text-slate-900"
                }`}
            />
        </div>
    );
}

function SelectField({ label, name, value, onChange, options, error }) {
    return (
        <div>
            <label className="block text-sm font-semibold text-slate-700 mb-1.5 flex justify-between">
                {label}
                {error && <span className="text-rose-500 text-xs font-normal">{error}</span>}
            </label>
            <select 
                name={name} 
                value={value || ""} 
                onChange={onChange}
                className={`w-full px-3 py-2.5 bg-slate-50 border rounded-xl focus:ring-2 focus:ring-orange-500 outline-none text-sm ${error ? 'border-rose-300' : 'border-slate-200'}`}
            >
                <option value="">Seçiniz...</option>
                {options.map(opt => (
                    <option key={opt.id} value={opt.id}>{opt.name}</option>
                ))}
            </select>
        </div>
    );
}