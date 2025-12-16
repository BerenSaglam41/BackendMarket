import { useEffect, useState } from "react";
import { useNavigate, useParams, Link } from "react-router-dom";
import { 
  ArrowLeftIcon, 
  PhotoIcon, 
  ExclamationCircleIcon,
  CheckCircleIcon,
  ClockIcon,
  ArchiveBoxIcon,
  CurrencyDollarIcon,
  TagIcon,
  ChatBubbleBottomCenterTextIcon,
  QrCodeIcon,
  CpuChipIcon
} from "@heroicons/react/24/outline";

// Services & Stores
import { SellerPendingProductService } from "../../services/seller/SellerPendingProductService";
import { useSellerPendingProductStore } from "../../store/seller/SellerPendingProductStore";
import { useBrandStore } from "../../store/brandStore";
import { useCategoryStore } from "../../store/CategoryStore";

export default function SellerPendingProductEditPage() {
  const { id } = useParams();
  const navigate = useNavigate();

  // Stores
  const { updatePending, loading: updating, error: updateError } = useSellerPendingProductStore();
  const { brands, fetchBrands } = useBrandStore();
  const { categories, fetchCategories } = useCategoryStore();

  // Local States
  const [initialLoading, setInitialLoading] = useState(true);
  const [localError, setLocalError] = useState(null);
  const [item, setItem] = useState(null);

  // Form State - DTO'daki tüm alanlar dahil
  const [form, setForm] = useState({
    name: "",
    slug: "",
    description: "",
    imageUrl: "",
    
    brandId: "",
    categoryId: "",
    sellerCategorySuggestion: "",

    sellerSku: "",
    barcode: "",
    attributesJson: "",
    sellerNote: "",

    proposedPrice: "",
    discountPercentage: "", // YENİ
    proposedStock: "",
    shippingTimeInDays: "",
  });

  // Verileri Çekme (Ürün + Markalar + Kategoriler)
  useEffect(() => {
    const fetchData = async () => {
      setInitialLoading(true);
      setLocalError(null);
      try {
        const [productRes] = await Promise.all([
           SellerPendingProductService.getPendingById(id),
           fetchBrands(),
           fetchCategories()
        ]);

        const data = productRes.data?.data;
        setItem(data);

        // Formu doldur (Null check ile)
        setForm({
          name: data?.name || "",
          slug: data?.slug || "",
          description: data?.description || "",
          imageUrl: data?.imageUrl || "",
          
          brandId: data?.brandId || "", 
          categoryId: data?.categoryId || "",
          sellerCategorySuggestion: data?.sellerCategorySuggestion || "",

          sellerSku: data?.sellerSku || "",
          barcode: data?.barcode || "",
          attributesJson: data?.attributesJson || "",
          sellerNote: data?.sellerNote || "",

          proposedPrice: data?.proposedPrice ?? "",
          discountPercentage: data?.discountPercentage ?? "", // YENİ
          proposedStock: data?.proposedStock ?? "",
          shippingTimeInDays: data?.shippingTimeInDays ?? "",
        });

      } catch (e) {
        setLocalError(e?.response?.data?.message || "Veriler yüklenirken bir hata oluştu.");
      } finally {
        setInitialLoading(false);
      }
    };

    fetchData();
  }, [id]);

  // Input Change Handler
  const handleChange = (e) => {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: value }));
  };

  // Düzenleme İzni Kontrolü
  const canEdit = item?.status === "Waiting" || item?.status === "NeedsUpdate";

  // Form Submit
  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!canEdit) return;

    // Backend DTO formatına uygun payload
    const payload = {
      name: form.name?.trim() || null,
      slug: form.slug?.trim().toLowerCase() || null,
      description: form.description?.trim() || null,
      imageUrl: form.imageUrl?.trim() || null,
      
      brandId: form.brandId ? Number(form.brandId) : null,
      categoryId: form.categoryId ? Number(form.categoryId) : null,
      sellerCategorySuggestion: form.sellerCategorySuggestion?.trim() || null,

      sellerSku: form.sellerSku?.trim() || null,
      barcode: form.barcode?.trim() || null,
      attributesJson: form.attributesJson?.trim() || null,
      sellerNote: form.sellerNote?.trim() || null,

      proposedPrice: form.proposedPrice !== "" ? Number(form.proposedPrice) : null,
      discountPercentage: form.discountPercentage !== "" ? Number(form.discountPercentage) : 0, // YENİ
      proposedStock: form.proposedStock !== "" ? Number(form.proposedStock) : null,
      shippingTimeInDays: form.shippingTimeInDays !== "" ? Number(form.shippingTimeInDays) : null,
    };

    try {
      await updatePending(Number(id), payload);
      navigate("/seller/pending-products");
    } catch (err) {
      // Hata store tarafında handle ediliyor
    }
  };

  if (initialLoading) {
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
        <div>
          <div className="flex items-center gap-3">
             <button onClick={() => navigate(-1)} className="p-1 hover:bg-slate-200 rounded-full transition">
                 <ArrowLeftIcon className="w-6 h-6 text-slate-500"/>
             </button>
             <h1 className="text-2xl font-bold text-slate-900 tracking-tight">Başvuruyu Düzenle</h1>
          </div>
          <p className="text-slate-500 mt-1 ml-9">
            Admin güncelleme istemişse gerekli alanları düzeltin ve tekrar gönderin.
          </p>
        </div>

        <div className="flex gap-3 ml-9 md:ml-0">
          <Link
            to="/seller/pending-products"
            className="px-5 py-2.5 rounded-xl border border-slate-200 bg-white text-slate-600 font-medium hover:bg-slate-50 transition"
          >
            Vazgeç
          </Link>
          <button
            onClick={handleSubmit}
            disabled={!canEdit || updating}
            className="px-6 py-2.5 rounded-xl bg-orange-600 text-white font-bold hover:bg-orange-700 shadow-lg shadow-orange-600/20 transition active:scale-95 disabled:opacity-60 disabled:cursor-not-allowed"
          >
            {updating ? "Kaydediliyor..." : "Güncellemeyi Gönder"}
          </button>
        </div>
      </div>

      {/* ERROR MESSAGES */}
      {(updateError || localError) && (
        <div className="max-w-6xl mx-auto mb-6 bg-rose-50 border border-rose-200 text-rose-700 rounded-2xl p-4 flex items-center gap-3">
           <ExclamationCircleIcon className="w-6 h-6 shrink-0"/>
           <span>{localError || updateError}</span>
        </div>
      )}

      {/* ADMIN NOTE ALERT */}
      {item?.adminNote && (
         <div className="max-w-6xl mx-auto mb-6 bg-amber-50 border border-amber-200 rounded-2xl p-5 flex gap-4">
             <ExclamationCircleIcon className="w-6 h-6 text-amber-600 shrink-0 mt-0.5" />
             <div>
                 <h4 className="font-bold text-amber-800">Admin Notu (Düzeltme İsteği):</h4>
                 <p className="text-amber-700 text-sm mt-1">{item.adminNote}</p>
             </div>
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
                label="Ürün Adı" 
                name="name" 
                value={form.name} 
                onChange={handleChange} 
                disabled={!canEdit}
                placeholder="Örn: Kablosuz Kulaklık"
              />
              
              <InputField 
                label="Slug (URL)" 
                name="slug" 
                value={form.slug} 
                onChange={handleChange} 
                disabled={!canEdit}
                placeholder="kategori-urun-adi"
              />

              <div>
                <label className="block text-sm font-semibold text-slate-700 mb-1.5">Açıklama</label>
                <textarea
                  name="description"
                  value={form.description}
                  onChange={handleChange}
                  disabled={!canEdit}
                  rows={5}
                  className="w-full px-4 py-3 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-orange-500 focus:bg-white outline-none transition text-sm resize-none disabled:opacity-60"
                  placeholder="Ürün özelliklerini buraya girin..."
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
                disabled={!canEdit}
                placeholder="https://example.com/image.jpg"
            />

            {/* Image Preview */}
            <div className="mt-4 p-4 border-2 border-dashed border-slate-200 rounded-xl bg-slate-50 flex items-center justify-center min-h-[150px]">
              {form.imageUrl ? (
                <img 
                    src={form.imageUrl} 
                    alt="Preview" 
                    className="max-h-40 rounded-lg shadow-sm object-contain"
                    onError={(e) => {e.target.style.display='none';}}
                />
              ) : (
                <div className="flex flex-col items-center text-slate-400">
                    <PhotoIcon className="w-10 h-10 mb-2 opacity-50"/>
                    <span className="text-sm">Görsel önizlemesi</span>
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
                    label="Önerilen Fiyat (₺)" 
                    name="proposedPrice" 
                    value={form.proposedPrice} 
                    onChange={handleChange} 
                    disabled={!canEdit}
                    type="number"
                />
                {/* İNDİRİM ALANI EKLENDİ */}
                <InputField 
                    label="İndirim Oranı (%)" 
                    name="discountPercentage" 
                    value={form.discountPercentage} 
                    onChange={handleChange} 
                    disabled={!canEdit}
                    type="number"
                />
                <InputField 
                    label="Önerilen Stok" 
                    name="proposedStock" 
                    value={form.proposedStock} 
                    onChange={handleChange} 
                    disabled={!canEdit}
                    type="number"
                />
                 <InputField 
                    label="Kargo Süresi (Gün)" 
                    name="shippingTimeInDays" 
                    value={form.shippingTimeInDays} 
                    onChange={handleChange} 
                    disabled={!canEdit}
                    type="number"
                />
             </div>
           </div>

        </div>

        {/* --- RIGHT COLUMN (SIDEBAR) --- */}
        <div className="space-y-6">
            
            {/* STATUS CARD */}
            <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
                <h3 className="text-sm font-bold text-slate-900 mb-4 uppercase tracking-wider text-xs text-slate-500">Durum Bilgisi</h3>
                
                <div className={`p-3 rounded-lg border text-sm font-medium flex items-center gap-2 ${
                    item?.status === 'Waiting' ? 'bg-orange-50 text-orange-700 border-orange-200' :
                    item?.status === 'NeedsUpdate' ? 'bg-amber-50 text-amber-700 border-amber-200' :
                    item?.status === 'Approved' ? 'bg-emerald-50 text-emerald-700 border-emerald-200' :
                    'bg-slate-50 text-slate-700 border-slate-200'
                }`}>
                    {item?.status === 'Waiting' && <ClockIcon className="w-5 h-5"/>}
                    {item?.status === 'NeedsUpdate' && <ExclamationCircleIcon className="w-5 h-5"/>}
                    {item?.status === 'Approved' && <CheckCircleIcon className="w-5 h-5"/>}
                    
                    <span>
                        {item?.status === 'Waiting' && 'Beklemede'}
                        {item?.status === 'NeedsUpdate' && 'Güncelleme İstendi'}
                        {item?.status === 'Approved' && 'Onaylandı'}
                        {item?.status === 'Rejected' && 'Reddedildi'}
                    </span>
                </div>
                
                {!canEdit && (
                    <p className="text-xs text-slate-400 mt-3">
                        Bu başvuru şu anki statüsünde düzenlenemez.
                    </p>
                )}
            </div>

            {/* ORGANIZATION */}
            <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
                <h3 className="text-lg font-bold text-slate-900 mb-5 flex items-center gap-2">
                    <TagIcon className="w-5 h-5 text-slate-400"/>
                    Organizasyon
                </h3>
                
                <div className="space-y-4">
                    {/* Marka Seçimi */}
                    <div>
                        <label className="block text-sm font-semibold text-slate-700 mb-1.5">Marka</label>
                        <select
                            name="brandId"
                            value={form.brandId}
                            onChange={handleChange}
                            disabled={!canEdit}
                            className="w-full px-3 py-2.5 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-orange-500 outline-none text-sm disabled:opacity-60"
                        >
                            <option value="">Seçiniz...</option>
                            {brands.map((b) => (
                                <option key={b.brandId} value={b.brandId}>{b.name}</option>
                            ))}
                        </select>
                    </div>

                    {/* Kategori Seçimi */}
                    <div>
                        <label className="block text-sm font-semibold text-slate-700 mb-1.5">Kategori</label>
                        <select
                            name="categoryId"
                            value={form.categoryId}
                            onChange={handleChange}
                            disabled={!canEdit}
                            className="w-full px-3 py-2.5 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-orange-500 outline-none text-sm disabled:opacity-60"
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
                            disabled={!canEdit}
                            className="w-full px-3 py-2 bg-slate-50 border border-slate-200 rounded-lg text-sm focus:ring-2 focus:ring-orange-500 outline-none disabled:opacity-60"
                            placeholder="Örn: Akıllı Ev Aletleri"
                         />
                    </div>
                </div>
            </div>

             {/* PRODUCT CODES */}
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
                        disabled={!canEdit}
                        placeholder="Sizin tarafınızdaki kod"
                    />
                    <InputField 
                        label="Barkod (EAN/UPC)" 
                        name="barcode" 
                        value={form.barcode} 
                        onChange={handleChange} 
                        disabled={!canEdit}
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
                  disabled={!canEdit}
                  className="w-full px-4 py-3 rounded-xl border border-slate-200 bg-slate-50 focus:ring-2 focus:ring-orange-500 outline-none min-h-[100px] font-mono text-xs disabled:opacity-60"
                  placeholder='{"renk": "Kırmızı", "beden": "L"}'
                />
                 <p className="text-xs text-slate-400 mt-2">Teknik özellikler JSON formatında girilmelidir.</p>
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
                    disabled={!canEdit}
                    className="w-full px-4 py-3 rounded-xl border border-slate-200 bg-slate-50 focus:ring-2 focus:ring-orange-500 outline-none min-h-[100px] text-sm disabled:opacity-60"
                    placeholder="Varsa admine iletmek istediğiniz not..."
                  />
            </div>

        </div>
      </div>
    </div>
  );
}

// --- REUSABLE COMPONENT ---
function InputField({ label, name, value, onChange, disabled, type = "text", placeholder }) {
  return (
    <div>
      <label className="block text-sm font-semibold text-slate-700 mb-1.5">{label}</label>
      <input
        type={type}
        name={name}
        value={value}
        onChange={onChange}
        disabled={disabled}
        placeholder={placeholder}
        className="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-orange-500 focus:bg-white outline-none transition text-sm disabled:opacity-60 disabled:bg-slate-100"
      />
    </div>
  );
}