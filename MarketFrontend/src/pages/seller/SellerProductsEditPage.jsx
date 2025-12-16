import { useEffect, useState, useMemo } from "react";
import { useNavigate, useParams, Link } from "react-router-dom";
import {
  ArrowLeftIcon,
  CurrencyDollarIcon,
  ArchiveBoxIcon,
  TruckIcon,
  CheckCircleIcon,
  ExclamationTriangleIcon,
  PhotoIcon,
  CalculatorIcon,
  TrashIcon,
  ChatBubbleBottomCenterTextIcon
} from "@heroicons/react/24/outline";
import { useSellerListingStore } from "../../store/seller/SellerListingStore";

export default function SellerListingEditPage() {
  const { listingId } = useParams();
  const navigate = useNavigate();

  const {
    items,
    fetchMyListings,
    updateListing,
    deleteListing,
    loading,
    error,
  } = useSellerListingStore();

  // Listing'i bul
  const listing = useMemo(() => 
    items.find((x) => String(x.listingId) === String(listingId)), 
  [items, listingId]);

  const [form, setForm] = useState({
    originalPrice: "",
    discountPercentage: "",
    stock: "",
    shippingTimeInDays: "",
    shippingCost: "",
    sellerNote: "",
    isActive: "true", // Select için string tutuyoruz, gönderirken boolean yapacağız
  });

  // Sayfa yüklendiğinde veri yoksa çek
  useEffect(() => {
    if (items.length === 0) {
        fetchMyListings();
    }
  }, []);

  // Listing verisi gelince formu doldur
  useEffect(() => {
    if (listing) {
      setForm({
        originalPrice: listing.originalPrice ?? "",
        discountPercentage: listing.discountPercentage ?? 0,
        stock: listing.stock ?? "",
        shippingTimeInDays: listing.shippingTimeInDays ?? "",
        shippingCost: listing.shippingCost ?? "",
        sellerNote: listing.sellerNote ?? "",
        isActive: listing.isActive ? "true" : "false",
      });
    }
  }, [listing]);

  // Input Change
  const onChange = (key, val) => setForm((s) => ({ ...s, [key]: val }));

  // Form Submit
  const onSubmit = async (e) => {
    e.preventDefault();

    const payload = {
      originalPrice: Number(form.originalPrice),
      discountPercentage: Number(form.discountPercentage) || 0,
      stock: Number(form.stock),
      shippingTimeInDays: Number(form.shippingTimeInDays),
      shippingCost: Number(form.shippingCost),
      sellerNote: form.sellerNote?.trim() || null,
      isActive: form.isActive === "true",
    };

    await updateListing(Number(listingId), payload);
    navigate("/seller/products");
  };

  // Delete Handler
  const onDelete = async () => {
    if (!window.confirm("DİKKAT: Bu ürünü satıştan tamamen kaldırmak üzeresiniz. Bu işlem geri alınamaz.\n\nDevam etmek istiyor musunuz?")) return;
    await deleteListing(Number(listingId));
    navigate("/seller/products");
  };

  // Hesaplanan Satış Fiyatı (Önizleme)
  const calculatedPrice = useMemo(() => {
      const price = Number(form.originalPrice) || 0;
      const discount = Number(form.discountPercentage) || 0;
      if(discount <= 0) return price;
      return price - (price * discount / 100);
  }, [form.originalPrice, form.discountPercentage]);

  if (loading && !listing) {
      return (
        <div className="min-h-screen bg-slate-50/50 flex items-center justify-center">
             <div className="w-10 h-10 border-4 border-slate-200 border-t-orange-500 rounded-full animate-spin"></div>
        </div>
      );
  }

  if (!listing) return <div className="p-10 text-center text-slate-500">İlan bulunamadı.</div>;

  return (
    <div className="min-h-screen bg-slate-50/50 p-6 lg:p-10 font-sans text-slate-800">
      
      {/* HEADER */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 mb-8 max-w-6xl mx-auto">
        <div className="flex items-center gap-4">
             <button onClick={() => navigate(-1)} className="p-2 hover:bg-white hover:shadow-sm rounded-full transition text-slate-500">
                 <ArrowLeftIcon className="w-6 h-6"/>
             </button>
             <div>
                <h1 className="text-2xl font-bold text-slate-900 tracking-tight">Satışı Düzenle</h1>
                <p className="text-slate-500 mt-1 text-sm">
                    Aktif satışta olan ürününüzün fiyat, stok ve kargo bilgilerini güncelleyin.
                </p>
             </div>
        </div>

        <div className="flex gap-3 ml-12 md:ml-0">
          <Link
            to="/seller/products"
            className="px-5 py-2.5 rounded-xl border border-slate-200 bg-white text-slate-600 font-medium hover:bg-slate-50 transition"
          >
            Vazgeç
          </Link>
          <button
            onClick={onSubmit}
            disabled={loading}
            className="px-6 py-2.5 rounded-xl bg-orange-600 text-white font-bold hover:bg-orange-700 shadow-lg shadow-orange-600/20 transition active:scale-95 disabled:opacity-70"
          >
            {loading ? "Kaydediliyor..." : "Değişiklikleri Kaydet"}
          </button>
        </div>
      </div>

      {/* ERROR MESSAGE */}
      {error && (
        <div className="max-w-6xl mx-auto mb-6 bg-rose-50 border border-rose-200 text-rose-700 rounded-2xl p-4 flex items-center gap-3">
          <ExclamationTriangleIcon className="w-6 h-6 shrink-0"/>
          <span>{error}</span>
        </div>
      )}

      {/* MAIN GRID */}
      <div className="max-w-6xl mx-auto grid grid-cols-1 lg:grid-cols-3 gap-8">
        
        {/* --- LEFT COLUMN (EDITABLE FIELDS) --- */}
        <div className="lg:col-span-2 space-y-6">
            
            {/* PRICING CARD */}
            <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
                <h3 className="text-lg font-bold text-slate-900 mb-5 flex items-center gap-2">
                    <CurrencyDollarIcon className="w-5 h-5 text-slate-400"/>
                    Fiyatlandırma
                </h3>
                
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                    <InputField 
                        label="Liste Fiyatı (₺)" 
                        value={form.originalPrice} 
                        onChange={(val) => onChange("originalPrice", val)}
                        type="number"
                        placeholder="0.00"
                    />
                    
                    <InputField 
                        label="İndirim Oranı (%)" 
                        value={form.discountPercentage} 
                        onChange={(val) => onChange("discountPercentage", val)}
                        type="number"
                        placeholder="0"
                    />
                </div>

                {/* Price Preview */}
                <div className="mt-6 bg-slate-50 border border-slate-200 rounded-xl p-4 flex items-center justify-between">
                    <div className="flex items-center gap-2 text-slate-500 text-sm">
                        <CalculatorIcon className="w-5 h-5"/>
                        <span>Müşterinin Göreceği Fiyat:</span>
                    </div>
                    <div className="text-xl font-bold text-emerald-600">
                        {calculatedPrice.toLocaleString('tr-TR', { minimumFractionDigits: 2 })} ₺
                    </div>
                </div>
            </div>

            {/* STOCK & SHIPPING CARD */}
            <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
                <h3 className="text-lg font-bold text-slate-900 mb-5 flex items-center gap-2">
                    <ArchiveBoxIcon className="w-5 h-5 text-slate-400"/>
                    Stok ve Lojistik
                </h3>
                
                <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                    <InputField 
                        label="Stok Adedi" 
                        value={form.stock} 
                        onChange={(val) => onChange("stock", val)}
                        type="number"
                    />
                     <InputField 
                        label="Kargo Süresi (Gün)" 
                        value={form.shippingTimeInDays} 
                        onChange={(val) => onChange("shippingTimeInDays", val)}
                        type="number"
                    />
                     <InputField 
                        label="Kargo Ücreti (₺)" 
                        value={form.shippingCost} 
                        onChange={(val) => onChange("shippingCost", val)}
                        type="number"
                    />
                </div>
                <div className="mt-3 flex items-center gap-2 text-xs text-slate-400">
                    <TruckIcon className="w-4 h-4"/>
                    <span>Kargo ücreti 0 girilirse "Ücretsiz Kargo" olarak görünür.</span>
                </div>
            </div>

            {/* SELLER NOTE */}
            <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
                 <h3 className="text-sm font-bold text-slate-900 mb-3 flex items-center gap-2">
                     <ChatBubbleBottomCenterTextIcon className="w-4 h-4 text-slate-400"/>
                     Satıcı Notu (Kendiniz için)
                 </h3>
                 <textarea
                    value={form.sellerNote}
                    onChange={(e) => onChange("sellerNote", e.target.value)}
                    className="w-full px-4 py-3 rounded-xl border border-slate-200 bg-slate-50 focus:ring-2 focus:ring-orange-500 outline-none min-h-[80px] text-sm resize-none"
                    placeholder="Bu satışla ilgili kendinize özel not ekleyebilirsiniz..."
                  />
            </div>
        </div>

        {/* --- RIGHT COLUMN (READONLY INFO & ACTIONS) --- */}
        <div className="space-y-6">
            
            {/* PRODUCT SUMMARY (Read Only) */}
            <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
                 <h3 className="text-xs font-bold text-slate-400 uppercase tracking-wider mb-4">Ürün Bilgisi</h3>
                 
                 <div className="flex flex-col items-center text-center mb-4">
                     <div className="w-24 h-24 rounded-xl bg-slate-100 border border-slate-200 flex items-center justify-center overflow-hidden mb-3">
                        
                         {listing.productImageUrl ? (
                             <img src={listing.productImageUrl} alt="" className="w-full h-full object-cover"/>
                         ) : (
                             <PhotoIcon className="w-10 h-10 text-slate-300"/>
                         )}
                     </div>
                     <h4 className="font-bold text-slate-900 line-clamp-2">{listing.productName}</h4>
                     <p className="text-xs text-slate-500 mt-1 font-mono">{listing.productSlug}</p>
                 </div>

                 <div className="pt-4 border-t border-slate-100 text-xs text-slate-500 space-y-2">
                     <p>Bu alanlar sabittir. Ürün isminde veya görselinde hata varsa Admin ile iletişime geçiniz.</p>
                 </div>
            </div>

            {/* STATUS SELECT */}
            <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
                <h3 className="text-sm font-bold text-slate-900 mb-3">Satış Durumu</h3>
                <div className="relative">
                    <select
                        value={form.isActive}
                        onChange={(e) => onChange("isActive", e.target.value)}
                        className={`w-full px-4 py-3 border rounded-xl outline-none appearance-none font-medium transition ${
                            form.isActive === "true" 
                            ? "bg-emerald-50 border-emerald-200 text-emerald-700" 
                            : "bg-rose-50 border-rose-200 text-rose-700"
                        }`}
                    >
                        <option value="true">Satışta (Aktif)</option>
                        <option value="false">Satışa Kapalı (Pasif)</option>
                    </select>
                    <div className="absolute right-4 top-1/2 -translate-y-1/2 pointer-events-none">
                        {form.isActive === "true" 
                            ? <CheckCircleIcon className="w-5 h-5 text-emerald-600"/> 
                            : <ExclamationTriangleIcon className="w-5 h-5 text-rose-600"/>
                        }
                    </div>
                </div>
            </div>

            {/* DANGER ZONE */}
            <div className="bg-rose-50 p-6 rounded-2xl border border-rose-100">
                <h3 className="text-sm font-bold text-rose-800 mb-2">Tehlikeli Bölge</h3>
                <p className="text-xs text-rose-600/80 mb-4">
                    Bu satışı listeden tamamen kaldırırsınız. Ürün stoğu sıfırlanır ve müşteriler ürünü göremez.
                </p>
                <button
                    type="button"
                    onClick={onDelete}
                    className="w-full py-2.5 bg-white border border-rose-200 text-rose-600 rounded-xl text-sm font-bold hover:bg-rose-600 hover:text-white transition shadow-sm flex items-center justify-center gap-2"
                >
                    <TrashIcon className="w-4 h-4"/>
                    Satışı Kaldır
                </button>
            </div>

        </div>
      </div>
    </div>
  );
}

// --- REUSABLE COMPONENT ---
function InputField({ label, value, onChange, type = "text", placeholder }) {
  return (
    <div>
      <label className="block text-sm font-semibold text-slate-700 mb-1.5">{label}</label>
      <input
        type={type}
        value={value}
        onChange={(e) => onChange(e.target.value)}
        placeholder={placeholder}
        className="w-full px-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-orange-500 focus:bg-white outline-none transition text-sm"
      />
    </div>
  );
}