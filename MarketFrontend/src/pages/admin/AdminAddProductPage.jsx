import { useState, useEffect } from "react";
import { useAdminProductStore } from "../../store/admin/adminProductStore";
import { useNavigate } from "react-router-dom";
import {
  PhotoIcon,
  ArrowLeftIcon,
  CheckCircleIcon,
  ExclamationCircleIcon,
} from "@heroicons/react/24/outline";
import { useBrandStore } from "../../store/brandStore";
import { useCategoryStore } from "../../store/CategoryStore";
export default function AdminAddProductPage() {
  const { createProduct, loading } = useAdminProductStore();
  const { brands, fetchBrands } = useBrandStore();
  const { categories, fetchCategories } = useCategoryStore();
  const navigate = useNavigate();

  // Initial State
  const initialForm = {
    name: "",
    slug: "",
    description: "",
    brandId: "",
    categoryId: "",
    imageUrl: "",
    metaTitle: "",
    metaDescription: "",
  };

  const [form, setForm] = useState(initialForm);
  const [errors, setErrors] = useState({});
  const [isSlugManuallyChanged, setIsSlugManuallyChanged] = useState(false);

  // --- SLUG GENERATOR ---
  // Kullanıcı "Ürün Adı"nı yazarken slug otomatik oluşsun
  useEffect(() => {
    if (!isSlugManuallyChanged) {
      const slug = form.name
        .toLowerCase()
        .replace(/ /g, "-")
        .replace(/[^\w-]+/g, "");
      setForm((prev) => ({ ...prev, slug }));
    }
  }, [form.name, isSlugManuallyChanged]);

  useEffect(() => {
    fetchBrands();
    fetchCategories();
  }, []);

  // --- HANDLERS ---
  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm({ ...form, [name]: value });

    // Validasyon hatasını temizle (kullanıcı yazmaya başlayınca)
    if (errors[name]) {
      setErrors({ ...errors, [name]: null });
    }

    // Slug elle değiştirildiyse otomasyonu durdur
    if (name === "slug") setIsSlugManuallyChanged(true);
  };

  const validate = () => {
    const newErrors = {};
    if (!form.name.trim()) newErrors.name = "Ürün adı zorunludur.";
    if (!form.slug.trim()) newErrors.slug = "Slug alanı zorunludur.";
    if (!form.categoryId) newErrors.categoryId = "Kategori seçimi zorunludur.";
    // Diğer kurallar eklenebilir...

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validate()) return;

    try {
      await createProduct(form);
      navigate("/admin/products");
    } catch (error) {
      console.error("Hata:", error);
      alert("Ürün eklenirken bir hata oluştu.");
    }
  };

  return (
    <div className="min-h-screen bg-slate-50/50 p-6 lg:p-10 font-sans text-slate-800">
      {/* TOP BAR */}
      <div className="flex items-center justify-between mb-8 max-w-5xl mx-auto">
        <div className="flex items-center gap-4">
          <button
            onClick={() => navigate(-1)}
            className="p-2 rounded-full hover:bg-white hover:shadow-sm transition text-slate-500"
          >
            <ArrowLeftIcon className="w-5 h-5" />
          </button>
          <h1 className="text-2xl font-bold text-slate-900">Yeni Ürün Ekle</h1>
        </div>
        <div className="flex gap-3">
          <button
            onClick={() => navigate(-1)}
            className="px-5 py-2 text-sm font-medium text-slate-600 hover:bg-white hover:shadow-sm rounded-lg transition"
          >
            Vazgeç
          </button>
          <button
            onClick={handleSubmit}
            disabled={loading}
            className="flex items-center gap-2 px-6 py-2 text-sm font-bold text-white bg-orange-600 hover:bg-orange-700 rounded-lg shadow-lg shadow-orange-600/20 transition active:scale-95 disabled:opacity-70"
          >
            {loading ? "Kaydediliyor..." : "Ürünü Kaydet"}
          </button>
        </div>
      </div>

      <div className="max-w-5xl mx-auto grid grid-cols-1 lg:grid-cols-3 gap-8">
        {/* --- LEFT COLUMN (MAIN INFO) --- */}
        <div className="lg:col-span-2 space-y-6">
          {/* Basic Info Card */}
          <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
            <h3 className="text-lg font-bold text-slate-800 mb-4">
              Temel Bilgiler
            </h3>
            <div className="space-y-4">
              <InputField
                label="Ürün Adı"
                name="name"
                value={form.name}
                onChange={handleChange}
                error={errors.name}
                placeholder="Örn: iPhone 15 Pro Max"
              />

              <div className="relative">
                <InputField
                  label="Slug (URL)"
                  name="slug"
                  value={form.slug}
                  onChange={handleChange}
                  error={errors.slug}
                  placeholder="urun-adi-otomatik-olur"
                />
                <span className="absolute top-9 right-3 text-xs text-slate-400">
                  market.com/urun/
                </span>
              </div>

              <div>
                <label className="block text-sm font-semibold text-slate-700 mb-1.5">
                  Açıklama
                </label>
                <textarea
                  name="description"
                  value={form.description}
                  onChange={handleChange}
                  rows={6}
                  className="w-full px-4 py-3 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-orange-500 focus:bg-white outline-none transition text-sm resize-none"
                  placeholder="Ürün özelliklerini detaylıca anlatın..."
                />
              </div>
            </div>
          </div>

          {/* Media Card */}
          <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
            <h3 className="text-lg font-bold text-slate-800 mb-4">Görseller</h3>
            <InputField
              label="Ana Görsel URL"
              name="imageUrl"
              value={form.imageUrl}
              onChange={handleChange}
              placeholder="https://..."
            />

            {/* Image Preview Area */}
            <div className="mt-4 p-4 border-2 border-dashed border-slate-200 rounded-xl bg-slate-50 flex items-center justify-center min-h-[200px]">
              {form.imageUrl ? (
                <img
                  src={form.imageUrl}
                  alt="Preview"
                  className="max-h-64 rounded-lg shadow-sm object-contain"
                  onError={(e) => (e.target.style.display = "none")}
                />
              ) : (
                <div className="flex flex-col items-center text-slate-400">
                  <PhotoIcon className="w-12 h-12 mb-2 opacity-50" />
                  <span className="text-sm">
                    Görsel önizlemesi burada görünecek
                  </span>
                </div>
              )}
            </div>
          </div>

          {/* SEO Card */}
          <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
            <h3 className="text-lg font-bold text-slate-800 mb-4">
              SEO Ayarları
            </h3>
            <div className="space-y-4">
              <InputField
                label="Meta Başlık (Title)"
                name="metaTitle"
                value={form.metaTitle}
                onChange={handleChange}
              />
              <InputField
                label="Meta Açıklama (Desc)"
                name="metaDescription"
                value={form.metaDescription}
                onChange={handleChange}
              />
            </div>
          </div>
        </div>

        {/* --- RIGHT COLUMN (SIDEBAR) --- */}
        <div className="space-y-6">
          {/* Organization Card */}
          <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
            <h3 className="text-lg font-bold text-slate-800 mb-4">
              Organizasyon
            </h3>

            <div className="space-y-4">
              <div>
                <label className="block text-sm font-semibold text-slate-700 mb-1.5">
                  Marka
                </label>
                <select
                  name="brandId"
                  value={form.brandId}
                  onChange={handleChange}
                  className="w-full px-3 py-2.5 bg-slate-50 border border-slate-200 rounded-xl"
                >
                  <option value="">Seçiniz...</option>
                  {brands.map((b) => (
                    <option key={b.brandId} value={b.brandId}>
                      {b.name}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label className="block text-sm font-semibold text-slate-700 mb-1.5">
                  Kategori <span className="text-rose-500">*</span>
                </label>
                <select
                  name="categoryId"
                  value={form.categoryId}
                  onChange={handleChange}
                  className={`w-full px-3 py-2.5 bg-slate-50 border rounded-xl ${
                    errors.categoryId ? "border-rose-300" : "border-slate-200"
                  }`}
                >
                  <option value="">Seçiniz...</option>
                  {categories.map((c) => (
                    <option key={c.categoryId} value={c.categoryId}>
                      {c.name}
                    </option>
                  ))}
                </select>
                {errors.categoryId && (
                  <p className="text-xs text-rose-500 mt-1">
                    {errors.categoryId}
                  </p>
                )}
              </div>
            </div>
          </div>

          {/* Status Card (Optional) */}
          <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
            <h3 className="text-sm font-bold text-slate-800 mb-3">Durum</h3>
            <div className="flex items-center gap-2 text-sm text-emerald-600 bg-emerald-50 p-3 rounded-lg border border-emerald-100">
              <CheckCircleIcon className="w-5 h-5" />
              <span>Ürün oluşturulduktan sonra aktif olacaktır.</span>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

// --- REUSABLE INPUT COMPONENT ---
function InputField({
  label,
  name,
  value,
  onChange,
  error,
  placeholder,
  type = "text",
}) {
  return (
    <div>
      <label className="block text-sm font-semibold text-slate-700 mb-1.5 flex justify-between">
        {label}
        {error && (
          <span className="text-rose-500 text-xs font-normal flex items-center gap-1">
            <ExclamationCircleIcon className="w-3 h-3" /> {error}
          </span>
        )}
      </label>
      <input
        type={type}
        name={name}
        value={value}
        onChange={onChange}
        placeholder={placeholder}
        className={`w-full px-4 py-2.5 bg-slate-50 border rounded-xl focus:ring-2 focus:bg-white outline-none transition text-sm ${
          error
            ? "border-rose-300 focus:ring-rose-200 bg-rose-50 text-rose-900 placeholder:text-rose-300"
            : "border-slate-200 focus:ring-orange-500 text-slate-900"
        }`}
      />
    </div>
  );
}
