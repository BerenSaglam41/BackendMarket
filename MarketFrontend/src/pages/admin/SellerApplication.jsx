import { useEffect, useState } from "react";
import { useSellerApplicationStore } from "../../store/admin/sellerApplication";
import { 
  EyeIcon, 
  CheckCircleIcon, 
  XCircleIcon, 
  ArrowPathIcon,
  PhoneIcon,
  CalendarDaysIcon,
  ClockIcon,
  ExclamationTriangleIcon,
  ArchiveBoxIcon,
  BuildingStorefrontIcon,
  ChevronRightIcon
} from "@heroicons/react/24/outline";

// --- COMPONENTS ---

// 1. Modern Status Badge
const StatusBadge = ({ status }) => {
    const styles = {
        Pending: "bg-amber-50 text-amber-700 ring-amber-600/20",
        NeedsUpdate: "bg-orange-50 text-orange-700 ring-orange-600/20",
        Rejected: "bg-rose-50 text-rose-700 ring-rose-600/20",
        Approved: "bg-emerald-50 text-emerald-700 ring-emerald-600/20",
    };

    const icons = {
        Pending: <ClockIcon className="w-3.5 h-3.5" />,
        NeedsUpdate: <ExclamationTriangleIcon className="w-3.5 h-3.5" />,
        Rejected: <XCircleIcon className="w-3.5 h-3.5" />,
        Approved: <CheckCircleIcon className="w-3.5 h-3.5" />,
    };

    const labels = {
        Pending: "Onay Bekliyor",
        NeedsUpdate: "Güncelleme İstendi",
        Rejected: "Reddedildi",
        Approved: "Onaylandı",
    };

    return (
        <span className={`inline-flex items-center gap-1.5 rounded-full px-2.5 py-1 text-xs font-medium ring-1 ring-inset ${styles[status] || "bg-gray-50 text-gray-600 ring-gray-500/10"}`}>
            {icons[status]}
            {labels[status] || status}
        </span>
    );
};

// 2. Empty State
const EmptyState = ({ title, desc }) => (
    <div className="flex flex-col items-center justify-center py-20 bg-white rounded-3xl border border-dashed border-slate-200">
        <div className="w-16 h-16 bg-slate-50 rounded-full flex items-center justify-center mb-4 text-slate-300">
            <ArchiveBoxIcon className="w-8 h-8" />
        </div>
        <h3 className="text-slate-900 font-semibold text-lg">{title}</h3>
        <p className="text-slate-500 text-sm mt-1">{desc}</p>
    </div>
);

export default function SellerApplicationsPage() {
  const {
    applications,
    loading,
    fetchApplications,
    approve,
    reject,
    requestUpdate,
  } = useSellerApplicationStore();

  const [selectedApp, setSelectedApp] = useState(null);
  const [actionStep, setActionStep] = useState("view"); 
  const [note, setNote] = useState("");
  const [showHistory, setShowHistory] = useState(false);

  useEffect(() => {
    fetchApplications();
  }, []);

  // Filter Logic
  const pendingApps = applications.filter(app => ["Pending", "NeedsUpdate"].includes(app.status));
  const historyApps = applications.filter(app => ["Approved", "Rejected"].includes(app.status));
  const visibleApps = showHistory ? historyApps : pendingApps;

  // Actions
  const closeModel = () => { setSelectedApp(null); setActionStep("view"); setNote(""); };
  
  const handleConfirmAction = async () => {
    if (!selectedApp) return;
    if (actionStep === "reject_input") await reject(selectedApp.sellerApplicationId, note);
    else if (actionStep === "update_input") await requestUpdate(selectedApp.sellerApplicationId, note);
    closeModel();
  };

  const handleApprove = async () => {
    if(window.confirm("Bu mağazayı onaylamak istediğinize emin misiniz?")) {
        await approve(selectedApp.sellerApplicationId);
        closeModel();
    }
  }

  const isActionable = (status) => status === "Pending";

  if (loading && applications.length === 0) {
      return (
        <div className="flex items-center justify-center h-screen bg-slate-50">
            <div className="flex flex-col items-center gap-3">
                <div className="w-10 h-10 border-4 border-slate-200 border-t-orange-500 rounded-full animate-spin"></div>
                <span className="text-slate-500 text-sm font-medium">Başvurular yükleniyor...</span>
            </div>
        </div>
      );
  }

  return (
    <div className="min-h-screen bg-slate-50/50 p-6 lg:p-10 font-sans text-slate-800">
      
      {/* ===== HEADER ===== */}
      <div className="flex flex-col md:flex-row md:items-end justify-between gap-6 mb-10">
        <div>
            <h1 className="text-3xl font-bold text-slate-900 tracking-tight">Satıcı Başvuruları</h1>
            <p className="text-slate-500 mt-2 text-lg">
                Platforma katılmak isteyen mağazaları buradan yönetebilirsiniz.
            </p>
        </div>

        {/* Action Bar: Refresh & Tabs */}
        <div className="flex items-center gap-4">
            <button 
                onClick={() => fetchApplications()}
                disabled={loading}
                className="p-2.5 bg-white border border-slate-200 text-slate-500 hover:text-orange-600 rounded-xl shadow-sm hover:shadow-md transition active:scale-95"
                title="Yenile"
            >
                <ArrowPathIcon className={`w-5 h-5 ${loading ? "animate-spin" : ""}`} />
            </button>

            {/* Modern Segmented Control */}
            <div className="bg-white p-1.5 rounded-xl border border-slate-200 shadow-sm flex items-center">
                <button 
                    onClick={() => setShowHistory(false)}
                    className={`px-5 py-2 text-sm font-semibold rounded-lg transition-all ${!showHistory ? 'bg-slate-900 text-white shadow-md' : 'text-slate-500 hover:bg-slate-50'}`}
                >
                    Bekleyenler <span className="ml-1 opacity-60 text-xs">({pendingApps.length})</span>
                </button>
                <button 
                    onClick={() => setShowHistory(true)}
                    className={`px-5 py-2 text-sm font-semibold rounded-lg transition-all ${showHistory ? 'bg-slate-900 text-white shadow-md' : 'text-slate-500 hover:bg-slate-50'}`}
                >
                    Geçmiş <span className="ml-1 opacity-60 text-xs">({historyApps.length})</span>
                </button>
            </div>
        </div>
      </div>

      {/* ===== LIST CONTENT ===== */}
      <div className="space-y-4">
          {visibleApps.length === 0 ? (
             <EmptyState 
                title={showHistory ? "Geçmiş Bulunamadı" : "Bekleyen Başvuru Yok"} 
                desc={showHistory ? "Henüz sonuçlanmış bir başvuru kaydı yok." : "Harika! Tüm başvuruları incelediniz."}
             />
          ) : (
            <div className="bg-white border border-slate-200 rounded-3xl shadow-sm overflow-hidden">
                <table className="w-full text-left border-collapse">
                    <thead className="bg-slate-50 border-b border-slate-200 text-xs uppercase text-slate-500 font-semibold">
                        <tr>
                            <th className="px-8 py-5">Mağaza Bilgisi</th>
                            <th className="px-6 py-5">İletişim</th>
                            <th className="px-6 py-5">Durum</th>
                            <th className="px-6 py-5 text-right">İşlem</th>
                        </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-50">
                        {visibleApps.map((app) => (
                            <tr key={app.sellerApplicationId} className="group hover:bg-slate-50/80 transition-colors">
                                <td className="px-8 py-5">
                                    <div className="flex items-center gap-4">
                                        {/* Avatar with Gradient */}
                                        <div className="w-12 h-12 rounded-2xl bg-gradient-to-br from-orange-100 to-amber-100 text-orange-600 border border-orange-100 flex items-center justify-center font-bold text-lg shadow-sm">
                                            {app.storeName.charAt(0).toUpperCase()}
                                        </div>
                                        <div>
                                            <div className="font-bold text-slate-900 text-base">{app.storeName}</div>
                                            <div className="text-xs text-slate-400 font-medium flex items-center gap-1 mt-0.5">
                                                <CalendarDaysIcon className="w-3 h-3" />
                                                {new Date(app.createdAt).toLocaleDateString("tr-TR")}
                                            </div>
                                        </div>
                                    </div>
                                </td>
                                <td className="px-6 py-5">
                                    <div className="flex items-center gap-2 text-sm text-slate-600 font-medium">
                                        <PhoneIcon className="w-4 h-4 text-slate-400" />
                                        {app.storePhone}
                                    </div>
                                </td>
                                <td className="px-6 py-5">
                                    <StatusBadge status={app.status} />
                                </td>
                                <td className="px-6 py-5 text-right">
                                    <button
                                        onClick={() => setSelectedApp(app)}
                                        className="inline-flex items-center gap-2 px-4 py-2 rounded-xl text-sm font-semibold text-slate-600 hover:text-orange-600 hover:bg-orange-50 transition-all group-hover:shadow-sm border border-transparent group-hover:border-orange-100"
                                    >
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
      </div>

      {/* ===== PREMIUM MODAL (Backdrop Blur) ===== */}
      {selectedApp && (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
          {/* Backdrop */}
          <div className="absolute inset-0 bg-slate-900/40 backdrop-blur-sm transition-opacity" onClick={closeModel} />
          
          {/* Modal Content */}
          <div className="relative bg-white w-full max-w-3xl rounded-3xl shadow-2xl overflow-hidden flex flex-col max-h-[90vh] animate-fadeInScale">
            
            {/* Header */}
            <div className="px-8 py-6 border-b border-slate-100 flex justify-between items-center bg-white sticky top-0 z-10">
                <div>
                    <h3 className="text-xl font-bold text-slate-900">
                        {showHistory ? "Başvuru Kaydı" : "Başvuru İnceleme"}
                    </h3>
                    <p className="text-sm text-slate-500 mt-1">
                        Başvuru No: #{selectedApp.sellerApplicationId}
                    </p>
                </div>
                <button onClick={closeModel} className="p-2 rounded-full hover:bg-slate-100 text-slate-400 hover:text-slate-600 transition">
                    <XCircleIcon className="w-8 h-8" />
                </button>
            </div>

            {/* Body */}
            <div className="p-8 overflow-y-auto">
                
                {/* 1. Üst Özet Kartı */}
                <div className="flex flex-col md:flex-row gap-6 mb-8">
                    <div className="w-24 h-24 rounded-3xl bg-slate-100 flex items-center justify-center text-4xl font-bold text-slate-400 shadow-inner">
                        {selectedApp.storeName.charAt(0)}
                    </div>
                    <div className="flex-1 space-y-4">
                        <div>
                            <h2 className="text-3xl font-bold text-slate-900">{selectedApp.storeName}</h2>
                            <div className="flex items-center gap-4 mt-2">
                                <span className="flex items-center gap-1.5 text-sm font-medium text-slate-600 bg-slate-50 px-3 py-1 rounded-lg border border-slate-100">
                                    <PhoneIcon className="w-4 h-4 text-slate-400"/> {selectedApp.storePhone}
                                </span>
                                <span className="flex items-center gap-1.5 text-sm font-medium text-slate-600 bg-slate-50 px-3 py-1 rounded-lg border border-slate-100">
                                    <BuildingStorefrontIcon className="w-4 h-4 text-slate-400"/> {selectedApp.storeSlug}
                                </span>
                            </div>
                        </div>
                        <div className="flex items-center gap-3">
                             <StatusBadge status={selectedApp.status} />
                             {/* Admin Notu Varsa Göster */}
                             {selectedApp.adminNote && (
                                <span className="text-xs text-slate-500 italic">Not: "{selectedApp.adminNote}"</span>
                             )}
                        </div>
                    </div>
                </div>

                {/* 2. Açıklama Alanı */}
                <div className="mb-8">
                    <h4 className="text-sm font-bold text-slate-900 uppercase tracking-wide mb-3 flex items-center gap-2">
                        <ArchiveBoxIcon className="w-4 h-4 text-orange-500" />
                        Mağaza Açıklaması
                    </h4>
                    <div className="bg-slate-50 p-6 rounded-2xl border border-slate-100 text-slate-700 leading-relaxed text-sm">
                        {selectedApp.storeDescription || "Açıklama girilmemiş."}
                    </div>
                </div>

                {/* 3. Aksiyon Alanı */}
                <div className="bg-white border-t border-slate-100 pt-6">
                    {isActionable(selectedApp.status) ? (
                        <>
                            {actionStep === "view" ? (
                                <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                                    <button onClick={() => setActionStep("reject_input")} className="py-4 rounded-xl border border-rose-100 bg-rose-50 text-rose-700 font-bold hover:bg-rose-100 hover:border-rose-200 transition">
                                        Reddet
                                    </button>
                                    <button onClick={() => setActionStep("update_input")} className="py-4 rounded-xl border border-blue-100 bg-blue-50 text-blue-700 font-bold hover:bg-blue-100 hover:border-blue-200 transition">
                                        Güncelleme İste
                                    </button>
                                    <button onClick={handleApprove} className="py-4 rounded-xl bg-emerald-600 text-white font-bold shadow-lg shadow-emerald-200 hover:bg-emerald-700 hover:shadow-emerald-300 transition transform active:scale-[0.98]">
                                        Onayla
                                    </button>
                                </div>
                            ) : (
                                <div className="animate-fadeIn">
                                    <div className="flex justify-between items-center mb-4">
                                        <label className="text-sm font-bold text-slate-800 flex items-center gap-2">
                                            {actionStep === "reject_input" ? <XCircleIcon className="w-5 h-5 text-rose-500"/> : <ExclamationTriangleIcon className="w-5 h-5 text-blue-500"/>}
                                            {actionStep === "reject_input" ? "Reddetme Sebebi" : "İstenen Güncellemeler"}
                                        </label>
                                        <button onClick={() => setActionStep("view")} className="text-xs font-semibold text-slate-400 hover:text-slate-600">İptal Et</button>
                                    </div>
                                    <textarea
                                        value={note}
                                        onChange={(e) => setNote(e.target.value)}
                                        className="w-full border border-slate-200 bg-slate-50 rounded-2xl p-4 text-sm focus:ring-2 focus:ring-orange-500 focus:bg-white outline-none transition placeholder:text-slate-400"
                                        rows={4}
                                        placeholder="Satıcıya iletilecek mesajınızı buraya yazın..."
                                        autoFocus
                                    />
                                    <button 
                                        onClick={handleConfirmAction} 
                                        disabled={!note.trim()}
                                        className="w-full mt-4 py-3 bg-slate-900 text-white rounded-xl font-bold hover:bg-slate-800 disabled:opacity-50 disabled:cursor-not-allowed transition"
                                    >
                                        {actionStep === "reject_input" ? "Başvuruyu Reddet" : "Talebi Gönder"}
                                    </button>
                                </div>
                            )}
                        </>
                    ) : (
                        <div className="flex justify-end">
                             <button onClick={closeModel} className="px-8 py-3 bg-slate-100 hover:bg-slate-200 text-slate-700 rounded-xl font-bold transition">
                                Kapat
                            </button>
                        </div>
                    )}
                </div>

            </div>
          </div>
        </div>
      )}
    </div>
  );
}