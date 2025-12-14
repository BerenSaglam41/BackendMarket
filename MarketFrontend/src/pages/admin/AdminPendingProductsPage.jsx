import { useEffect, useState } from "react";
import { useAdminPendingProductStore } from "../../store/admin/adminPendingProductStore";
import { 
  CheckCircleIcon, 
  XCircleIcon, 
  ArrowPathIcon, 
  EyeIcon, 
  ArchiveBoxIcon,
  TagIcon,
  PhotoIcon,
  UserIcon,
  ChatBubbleLeftRightIcon,
  CalendarDaysIcon,
  CubeIcon,
  XMarkIcon,
  ClipboardDocumentIcon
} from "@heroicons/react/24/outline";

// --- CUSTOM ICONS ---
const CurrencyLiraIcon = ({ className }) => (
  <svg xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor" className={className}>
    <path strokeLinecap="round" strokeLinejoin="round" d="M8 7h9M8 11h9M9.5 20V4.5" />
    <path strokeLinecap="round" strokeLinejoin="round" d="M6 5l3.5-1" />
  </svg>
);

// --- HELPERS ---
const formatDate = (dateString) => {
    if(!dateString) return "-";
    return new Date(dateString).toLocaleDateString("tr-TR", { day: "numeric", month: "long", hour: "2-digit", minute: "2-digit" });
};
const formatCurrency = (val) => new Intl.NumberFormat("tr-TR", { style: "currency", currency: "TRY" }).format(val);

const copyToClipboard = (text) => {
    navigator.clipboard.writeText(text);
};

// --- ANA BİLEŞEN ---
export default function AdminPendingProductsPage() {
  const {
    items,
    loading,
    filters,
    setFilters,
    fetchPendingProduct,
  } = useAdminPendingProductStore();

  const [selectedProduct, setSelectedProduct] = useState(null);

  useEffect(() => {
    fetchPendingProduct();
  }, []);

  const closeModal = () => {
    setSelectedProduct(null);
  };

  // Liste Loading
  if (loading && items.length === 0) {
      return (
        <div className="flex items-center justify-center h-screen bg-slate-50">
            <div className="flex flex-col items-center gap-4">
                <div className="w-12 h-12 border-4 border-slate-200 border-t-orange-500 rounded-full animate-spin"></div>
                <span className="text-slate-400 font-medium animate-pulse">Başvurular Yükleniyor...</span>
            </div>
        </div>
      );
  }

  return (
    <div className="min-h-screen bg-slate-50/50 p-6 lg:p-10 font-sans text-slate-800">
      
      {/* HEADER */}
      <div className="flex flex-col md:flex-row md:items-end justify-between gap-6 mb-10">
        <div>
            <h1 className="text-3xl font-bold text-slate-900 tracking-tight">Ürün Başvuruları</h1>
            <p className="text-slate-500 mt-2 text-lg">
                Satıcıların kataloga eklemek istediği yeni ürünleri buradan yönetebilirsiniz.
            </p>
        </div>

        <div className="flex items-center gap-4">
             <button onClick={() => fetchPendingProduct()} className="p-2.5 bg-white border border-slate-200 text-slate-500 hover:text-orange-600 rounded-xl shadow-sm hover:shadow-md transition active:scale-95" title="Yenile">
                <ArrowPathIcon className={`w-5 h-5 ${loading ? "animate-spin" : ""}`} />
            </button>

            <div className="bg-white p-1.5 rounded-xl border border-slate-200 shadow-sm flex items-center px-4">
                 <span className="text-xs font-bold text-slate-400 mr-2 uppercase tracking-wider">Filtre:</span>
                 <select value={filters.status} onChange={(e) => setFilters({ status: e.target.value })} className="bg-transparent text-sm font-semibold text-slate-700 outline-none cursor-pointer py-1">
                    <option value="">Tümü</option>
                    <option value="Waiting">Bekleyenler</option>
                    <option value="NeedsUpdate">Güncelleme İstenenler</option>
                    <option value="Rejected">Reddedilenler</option>
                    <option value="Approved">Onaylananlar</option>
                </select>
            </div>
        </div>
      </div>

      {/* LİSTE İÇERİĞİ */}
      {items.length === 0 ? (
        <div className="flex flex-col items-center justify-center py-32 bg-white rounded-3xl border border-dashed border-slate-200 shadow-sm">
            <div className="w-20 h-20 bg-slate-50 rounded-full flex items-center justify-center mb-6 text-slate-300">
                <ArchiveBoxIcon className="w-10 h-10" />
            </div>
            <h3 className="text-slate-900 font-bold text-xl">Liste Boş</h3>
            <p className="text-slate-500 mt-2">Şu an gösterilecek bir başvuru bulunmuyor.</p>
        </div>
      ) : (
        <div className="bg-white border border-slate-200 rounded-3xl shadow-sm overflow-hidden">
            <table className="w-full text-left border-collapse">
                <thead className="bg-slate-50/50 border-b border-slate-200 text-xs uppercase text-slate-500 font-semibold tracking-wider">
                    <tr>
                        <th className="px-8 py-5">Ürün Detayı</th>
                        <th className="px-6 py-5">Satıcı</th>
                        <th className="px-6 py-5">Teklif</th>
                        <th className="px-6 py-5">Durum</th>
                        <th className="px-6 py-5 text-right">İşlem</th>
                    </tr>
                </thead>
                <tbody className="divide-y divide-slate-50">
                    {items.map((p) => (
                        <tr key={p.productPendingId} className="group hover:bg-slate-50/60 transition-colors">
                            <td className="px-8 py-5">
                                <div className="flex items-center gap-4">
                                    <div className="w-16 h-16 rounded-xl bg-slate-100 border border-slate-200 overflow-hidden shrink-0 flex items-center justify-center relative shadow-sm">
                                        {p.imageUrl ? (
                                            <img src={p.imageUrl} alt={p.name} className="w-full h-full object-cover" />
                                        ) : (
                                            <PhotoIcon className="w-8 h-8 text-slate-300" />
                                        )}
                                    </div>
                                    <div className="flex flex-col gap-1">
                                        <div className="font-bold text-slate-900 text-base line-clamp-1">{p.name}</div>
                                        <div className="flex items-center gap-1 group/slug cursor-pointer" onClick={() => copyToClipboard(p.slug)} title="Slug'ı Kopyala">
                                            <span className="text-xs text-slate-500 font-mono bg-slate-100 px-1.5 py-0.5 rounded border border-slate-200 hover:border-orange-200 hover:text-orange-600 transition">/{p.slug}</span>
                                            <ClipboardDocumentIcon className="w-3 h-3 text-slate-300 opacity-0 group-hover/slug:opacity-100 transition" />
                                        </div>
                                    </div>
                                </div>
                            </td>
                            <td className="px-6 py-5">
                                <div className="text-sm font-medium text-slate-700">{p.sellerName}</div>
                                <div className="text-xs text-slate-400 mt-0.5">{p.sellerEmail}</div>
                            </td>
                            <td className="px-6 py-5">
                                <div className="text-sm font-bold text-slate-900">{formatCurrency(p.proposedPrice)}</div>
                                <div className="text-xs text-slate-500 mt-0.5">{p.proposedStock} adet stok</div>
                            </td>
                            <td className="px-6 py-5">
                                <StatusBadge status={p.status} />
                            </td>
                            <td className="px-6 py-5 text-right">
                                <button onClick={() => setSelectedProduct(p)} className="inline-flex items-center gap-2 px-4 py-2 rounded-xl text-sm font-semibold text-slate-600 hover:text-orange-600 hover:bg-orange-50 transition-all group-hover:shadow-sm border border-transparent group-hover:border-orange-100">
                                    <EyeIcon className="w-4 h-4" />
                                    İncele
                                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
      )}

      {/* MODAL (AYRI COMPONENT OLARAK RENDER EDİLİYOR) */}
      {selectedProduct && (
        <PendingProductModal 
            product={selectedProduct} 
            onClose={closeModal} 
        />
      )}
    </div>
  );
}

// --- MODAL COMPONENT (KARMAŞIKLIĞI AZALTMAK İÇİN AYRILDI) ---
function PendingProductModal({ product, onClose }) {
    const { approveProduct, rejectProduct, requestUpdate, fetchPendingProduct } = useAdminPendingProductStore();
    
    // Local state for modal actions
    const [actionStep, setActionStep] = useState("view"); // view | reject | update
    const [adminNote, setAdminNote] = useState("");
    const [isProcessing, setIsProcessing] = useState(false);

    // 🔥 KRİTİK DÜZELTME BURADA:
    // forcedAction parametresi sayesinde state güncellenmesini beklemeden işlem yaparız.
    const handleAction = async (forcedAction = null) => {
        const actionToExecute = forcedAction || actionStep;
        
        setIsProcessing(true);
        try {
            if (actionToExecute === "reject") {
                await rejectProduct(product.productPendingId, { adminNote });
            } else if (actionToExecute === "update") {
                await requestUpdate(product.productPendingId, { adminNote });
            } else if (actionToExecute === "approve") {
                await approveProduct(product.productPendingId, {}); 
            }
            
            // İşlem başarılıysa kapat ve listeyi yenile
            await fetchPendingProduct();
            onClose();

        } catch (error) {
            console.error("İşlem hatası:", error);
        } finally {
            setIsProcessing(false);
        }
    };

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
          <div className="absolute inset-0 bg-slate-900/40 backdrop-blur-md transition-opacity" onClick={onClose} />
          
          <div className="relative bg-white w-full max-w-5xl rounded-3xl shadow-2xl overflow-hidden flex flex-col max-h-[90vh] animate-fadeInScale">
            
            {/* Modal Header */}
            <div className="px-8 py-5 border-b border-slate-100 flex justify-between items-center bg-white sticky top-0 z-10">
                <div className="flex items-center gap-3">
                    <div className="p-2 bg-orange-50 text-orange-600 rounded-lg">
                        <CubeIcon className="w-6 h-6" />
                    </div>
                    <div>
                        <h3 className="text-lg font-bold text-slate-900">Ürün İnceleme</h3>
                        <div className="flex items-center gap-2 text-xs text-slate-500">
                             <span className="font-mono">ID: {product.productPendingId}</span>
                             <span>•</span>
                             <span className="flex items-center gap-1"><CalendarDaysIcon className="w-3 h-3"/> {formatDate(product.createdAt)}</span>
                        </div>
                    </div>
                </div>
                <button onClick={onClose} className="p-2 rounded-full hover:bg-slate-100 text-slate-400 hover:text-slate-600 transition">
                    <XMarkIcon className="w-6 h-6" />
                </button>
            </div>

            {/* Modal Body */}
            <div className="p-8 overflow-y-auto grid grid-cols-1 lg:grid-cols-12 gap-8 bg-slate-50/50">
                
                {/* SOL KOLON */}
                <div className="lg:col-span-4 space-y-6">
                     <div className="aspect-square rounded-2xl bg-white border border-slate-200 overflow-hidden flex items-center justify-center relative shadow-sm group">
                        {product.imageUrl ? (
                            <img src={product.imageUrl} alt="Main" className="w-full h-full object-cover" />
                        ) : (
                            <PhotoIcon className="w-20 h-20 text-slate-300" />
                        )}
                        <span className="absolute bottom-3 left-3 bg-black/60 backdrop-blur text-white text-[10px] font-bold px-2 py-1 rounded-md uppercase tracking-wide">Ana Görsel</span>
                     </div>
                     
                     {product.imageGallery && product.imageGallery.length > 0 && (
                        <div>
                             <h4 className="text-xs font-bold text-slate-500 uppercase mb-2">Galeri</h4>
                             <div className="grid grid-cols-4 gap-2">
                                {product.imageGallery.map((img, idx) => (
                                    <div key={idx} className="aspect-square rounded-lg border border-slate-200 bg-white overflow-hidden cursor-pointer hover:border-orange-400 transition">
                                        <img src={img} alt={`Gallery ${idx}`} className="w-full h-full object-cover" />
                                    </div>
                                ))}
                            </div>
                        </div>
                     )}

                     <div className="bg-white p-5 rounded-2xl border border-slate-200 shadow-sm space-y-3">
                        <h4 className="text-xs font-bold text-slate-900 uppercase border-b border-slate-100 pb-2 mb-2">Teknik Detaylar</h4>
                        <div className="flex justify-between items-center"><span className="text-xs text-slate-500">SKU</span><span className="text-xs font-mono font-bold text-slate-700 bg-slate-100 px-2 py-1 rounded">{product.sellerSku || "-"}</span></div>
                        <div className="flex justify-between items-center"><span className="text-xs text-slate-500">Barkod</span><span className="text-xs font-mono font-bold text-slate-700 bg-slate-100 px-2 py-1 rounded">{product.barcode || "-"}</span></div>
                        <div className="flex justify-between items-center"><span className="text-xs text-slate-500">Kargo</span><span className="text-xs font-bold text-slate-700">{product.shippingTimeInDays} Gün</span></div>
                     </div>
                </div>

                {/* SAĞ KOLON */}
                <div className="lg:col-span-8 space-y-6">
                    <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm flex flex-col md:flex-row justify-between md:items-start gap-4">
                        <div className="flex-1">
                            <div className="flex items-center gap-2 mb-2">
                                <span className="inline-flex items-center gap-1 px-2.5 py-0.5 rounded-md bg-slate-100 text-slate-600 text-xs font-medium border border-slate-200"><TagIcon className="w-3 h-3"/> {product.brandName || "Marka Yok"}</span>
                                <span className="inline-flex items-center gap-1 px-2.5 py-0.5 rounded-md bg-blue-50 text-blue-700 text-xs font-medium border border-blue-100"><ArchiveBoxIcon className="w-3 h-3"/> {product.categoryName || product.sellerCategorySuggestion || "Kategori Yok"}</span>
                            </div>
                            <h2 className="text-2xl font-bold text-slate-900 leading-tight">{product.name}</h2>
                            <div className="flex items-center gap-2 mt-2 group cursor-pointer" onClick={() => copyToClipboard(product.slug)}>
                                <span className="text-xs font-mono text-slate-400">URL:</span>
                                <span className="text-xs font-mono font-medium text-slate-600 bg-slate-100 px-2 py-1 rounded hover:bg-orange-50 hover:text-orange-600 transition">/{product.slug}</span>
                            </div>
                        </div>
                        <div className="text-right pl-4 border-l border-slate-100">
                             <div className="text-3xl font-bold text-slate-900 tracking-tight">{formatCurrency(product.proposedPrice)}</div>
                             <div className="text-xs text-slate-500 font-medium bg-green-50 text-green-700 px-2 py-1 rounded mt-1 inline-block border border-green-100">Stok: {product.proposedStock}</div>
                        </div>
                    </div>

                    <div className="bg-white p-4 rounded-2xl border border-slate-200 shadow-sm flex items-center justify-between">
                         <div className="flex items-center gap-3">
                            <div className="w-10 h-10 rounded-full bg-orange-50 text-orange-600 flex items-center justify-center border border-orange-100 shadow-sm"><UserIcon className="w-5 h-5" /></div>
                            <div><div className="text-sm font-bold text-slate-900">{product.sellerName}</div><div className="text-xs text-slate-500">{product.sellerEmail}</div></div>
                         </div>
                         {product.sellerNote && (
                             <div className="max-w-xs text-right">
                                 <div className="text-[10px] font-bold text-slate-400 uppercase mb-1 flex items-center justify-end gap-1"><ChatBubbleLeftRightIcon className="w-3 h-3"/> Satıcı Notu</div>
                                 <p className="text-xs text-slate-600 italic bg-amber-50 px-3 py-2 rounded-lg border border-amber-100 text-amber-800">"{product.sellerNote}"</p>
                             </div>
                         )}
                    </div>
                    
                    <div className="grid grid-cols-1 gap-6">
                        <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
                            <h4 className="text-sm font-bold text-slate-900 uppercase tracking-wide mb-3 flex items-center gap-2"><TagIcon className="w-4 h-4 text-slate-400" /> Ürün Özellikleri</h4>
                            <AttributeDisplay jsonString={product.attributesJson} />
                        </div>
                        <div className="bg-white p-6 rounded-2xl border border-slate-200 shadow-sm">
                            <h4 className="text-sm font-bold text-slate-900 uppercase tracking-wide mb-3">Açıklama</h4>
                            <div className="prose prose-sm prose-slate max-w-none text-slate-600 leading-relaxed bg-slate-50 p-4 rounded-xl border border-slate-100">{product.description}</div>
                        </div>
                    </div>

                    {/* ACTION CENTER */}
                    {product.status === "Waiting" && (
                        <div className="bg-white border border-slate-200 rounded-2xl p-6 shadow-lg shadow-slate-200/50 sticky bottom-0 z-20">
                             {actionStep === "view" ? (
                                <div className="flex flex-col md:flex-row gap-3">
                                    <button onClick={() => setActionStep("reject")} className="flex-1 py-3.5 rounded-xl border border-rose-200 bg-rose-50 text-rose-700 font-bold hover:bg-rose-100 hover:border-rose-300 transition shadow-sm flex items-center justify-center gap-2">
                                        <XCircleIcon className="w-5 h-5"/> Reddet
                                    </button>
                                    <button onClick={() => setActionStep("update")} className="flex-1 py-3.5 rounded-xl border border-blue-200 bg-blue-50 text-blue-700 font-bold hover:bg-blue-100 hover:border-blue-300 transition shadow-sm flex items-center justify-center gap-2">
                                        <ArrowPathIcon className="w-5 h-5"/> Güncelleme İste
                                    </button>
                                    
                                    {/* 🔥 ONAYLA BUTONU DOĞRULANDI 🔥 */}
                                    <button 
                                        onClick={() => handleAction("approve")} 
                                        disabled={isProcessing}
                                        className="flex-1 py-3.5 rounded-xl bg-emerald-600 text-white font-bold shadow-lg shadow-emerald-600/20 hover:bg-emerald-700 transition active:scale-[0.98] flex items-center justify-center gap-2 disabled:opacity-50"
                                    >
                                        {isProcessing ? <ArrowPathIcon className="w-5 h-5 animate-spin"/> : <CheckCircleIcon className="w-5 h-5"/>}
                                        Onayla
                                    </button>
                                </div>
                             ) : (
                                <div className="animate-fadeIn">
                                    <div className="flex justify-between items-center mb-2">
                                        <label className="text-sm font-bold text-slate-800 flex items-center gap-2">
                                            {actionStep === "reject" ? <XCircleIcon className="w-5 h-5 text-rose-500"/> : <ArrowPathIcon className="w-5 h-5 text-blue-500"/>}
                                            {actionStep === "reject" ? "Reddetme Sebebi" : "İstenen Düzeltmeler"}
                                        </label>
                                        <button onClick={() => setActionStep("view")} className="text-xs text-slate-400 hover:text-slate-600 underline">Vazgeç</button>
                                    </div>
                                    <textarea
                                        value={adminNote}
                                        onChange={(e) => setAdminNote(e.target.value)}
                                        className="w-full border border-slate-300 bg-white rounded-xl p-3 text-sm focus:ring-2 focus:ring-orange-500 outline-none transition shadow-inner placeholder:text-slate-300"
                                        rows={3}
                                        placeholder={actionStep === "reject" ? "Bu ürün neden reddedildi?" : "Satıcı neleri düzeltmeli?"}
                                        autoFocus
                                    />
                                    {/* Bu buton parametresiz çağrılır, çünkü actionStep state'i zaten dolu */}
                                    <button 
                                        onClick={() => handleAction()} 
                                        disabled={!adminNote.trim() || isProcessing}
                                        className="w-full mt-3 py-3 bg-slate-900 text-white rounded-xl font-bold hover:bg-slate-800 disabled:opacity-50 transition shadow-lg shadow-slate-900/20 flex items-center justify-center gap-2"
                                    >
                                        {isProcessing && <ArrowPathIcon className="w-4 h-4 animate-spin"/>}
                                        {actionStep === "reject" ? "Kararı Onayla ve Reddet" : "Talebi Gönder"}
                                    </button>
                                </div>
                             )}
                        </div>
                    )}
                </div>
            </div>
          </div>
        </div>
    );
}

// --- SUB COMPONENTS (Helpers) ---
const StatusBadge = ({ status }) => {
    const styles = {
        Waiting: { class: "bg-amber-50 text-amber-700 ring-amber-600/20", label: "İnceleme Bekliyor" },
        NeedsUpdate: { class: "bg-blue-50 text-blue-700 ring-blue-600/20", label: "Güncelleme İstendi" },
        Rejected: { class: "bg-rose-50 text-rose-700 ring-rose-600/20", label: "Reddedildi" },
        Approved: { class: "bg-emerald-50 text-emerald-700 ring-emerald-600/20", label: "Onaylandı" },
    };
    const current = styles[status] || styles.Waiting;
    return (
        <span className={`inline-flex items-center gap-1.5 rounded-full px-2.5 py-1 text-xs font-bold ring-1 ring-inset ${current.class}`}>
            {current.label}
        </span>
    );
};

const AttributeDisplay = ({ jsonString }) => {
    try {
        if (!jsonString) return <span className="text-slate-400 text-xs italic">Özellik belirtilmemiş</span>;
        const attrs = JSON.parse(jsonString);
        if (Object.keys(attrs).length === 0) return <span className="text-slate-400 text-xs italic">Özellik belirtilmemiş</span>;
        return (
            <div className="flex flex-wrap gap-2 mt-1">
                {Object.entries(attrs).map(([key, value], idx) => (
                    <span key={idx} className="inline-flex items-center gap-1 px-2.5 py-1 bg-slate-100 text-slate-700 text-xs font-medium rounded-md border border-slate-200">
                        <span className="opacity-60 capitalize">{key}:</span>
                        <span className="font-bold">{value}</span>
                    </span>
                ))}
            </div>
        );
    } catch (e) {
        return <span className="text-rose-400 text-xs">Hatalı JSON Verisi</span>;
    }
};