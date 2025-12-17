import { useEffect, useState, Fragment } from "react";
import { Dialog, Transition, Menu } from "@headlessui/react";
import {
  TruckIcon,
  CheckCircleIcon,
  ClockIcon,
  ChevronDownIcon,
  ChevronUpIcon,
  MapPinIcon,
  UserIcon,
  MagnifyingGlassIcon,
  FunnelIcon,
  XMarkIcon,
  ShoppingBagIcon,
  NoSymbolIcon,
  EllipsisVerticalIcon
} from "@heroicons/react/24/outline";
import { useSellerOrderStore } from "../../store/seller/SellerOrderStore";

// --- STATUS BADGE COMPONENT ---
const StatusBadge = ({ status }) => {
  const styles = {
    AwaitingPayment: { bg: "bg-slate-100", text: "text-slate-600", border: "border-slate-200", icon: ClockIcon, label: "Ödeme Bekliyor" },
    Processing: { bg: "bg-blue-50", text: "text-blue-700", border: "border-blue-100", icon: ShoppingBagIcon, label: "Hazırlanıyor" },
    Shipped: { bg: "bg-orange-50", text: "text-orange-700", border: "border-orange-100", icon: TruckIcon, label: "Kargolandı" },
    Delivered: { bg: "bg-emerald-50", text: "text-emerald-700", border: "border-emerald-100", icon: CheckCircleIcon, label: "Teslim Edildi" },
    Cancelled: { bg: "bg-rose-50", text: "text-rose-700", border: "border-rose-100", icon: NoSymbolIcon, label: "İptal Edildi" },
    Returned: { bg: "bg-purple-50", text: "text-purple-700", border: "border-purple-100", icon: ArrowPathIcon, label: "İade" }
  };

  const style = styles[status] || styles.AwaitingPayment;
  const Icon = style.icon;

  return (
    <span className={`inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full text-xs font-bold border ${style.bg} ${style.text} ${style.border}`}>
      <Icon className="w-3.5 h-3.5" />
      {style.label}
    </span>
  );
};

// --- HELPER ICONS ---
import { ArrowPathIcon } from "@heroicons/react/24/outline";

export default function SellerOrdersPage() {
  const {
    items,
    loading,
    error,
    fetchOrders,
    updateOrderStatus,
    page,
    totalPages,
    setPage,
  } = useSellerOrderStore();

  // Local States
  const [expandedOrderId, setExpandedOrderId] = useState(null); // Detay açma/kapama
  const [selectedStatusFilter, setSelectedStatusFilter] = useState("All");
  const [searchTerm, setSearchTerm] = useState("");
  
  // Modal States (Kargo Girişi)
  const [isShipModalOpen, setIsShipModalOpen] = useState(false);
  const [selectedOrderForShip, setSelectedOrderForShip] = useState(null);
  const [trackingInfo, setTrackingInfo] = useState({ provider: "", number: "" });

  useEffect(() => {
    fetchOrders();
  }, [page]);

  // Expand Toggle
  const toggleDetails = (id) => {
    setExpandedOrderId(expandedOrderId === id ? null : id);
  };

  // Filter Logic (Client Side - Backend destekliyorsa oraya taşınabilir)
  const filteredOrders = items.filter(order => {
      const matchesStatus = selectedStatusFilter === "All" || order.orderStatus === selectedStatusFilter;
      const matchesSearch = order.orderNumber.toLowerCase().includes(searchTerm.toLowerCase()) || 
                            order.items.some(i => i.productName.toLowerCase().includes(searchTerm.toLowerCase()));
      return matchesStatus && matchesSearch;
  });

  // Handle Status Update
  const handleStatusChange = async (orderId, newStatus, extraData = {}) => {
      const success = await updateOrderStatus(orderId, { newStatus, ...extraData });
      if(success) {
          setIsShipModalOpen(false); // Modal açıksa kapat
          setTrackingInfo({ provider: "", number: "" }); // Formu temizle
      }
  };

  // Open Shipping Modal
  const openShippingModal = (order) => {
      setSelectedOrderForShip(order);
      setTrackingInfo({ provider: "Aras Kargo", number: "" }); // Default provider
      setIsShipModalOpen(true);
  };

  return (
    <div className="min-h-screen bg-slate-50/50 p-6 lg:p-10 font-sans text-slate-800">
      
      {/* HEADER */}
      <div className="flex flex-col md:flex-row md:items-center justify-between gap-4 mb-8">
        <div>
          <h1 className="text-3xl font-bold text-slate-900 tracking-tight">Sipariş Yönetimi</h1>
          <p className="text-slate-500 mt-1">Gelen siparişleri görüntüleyin, kargolayın ve durumlarını güncelleyin.</p>
        </div>
      </div>

      {/* ERROR MESSAGE */}
      {error && (
        <div className="mb-6 bg-rose-50 border border-rose-200 text-rose-700 rounded-2xl p-4 flex items-center gap-3">
           <NoSymbolIcon className="w-5 h-5"/>
           <span>{error}</span>
        </div>
      )}

      {/* FILTERS & SEARCH */}
      <div className="bg-white p-4 rounded-2xl border border-slate-200 shadow-sm mb-6 flex flex-col md:flex-row gap-4 justify-between items-center">
          
          {/* Status Tabs */}
          <div className="flex overflow-x-auto gap-2 w-full md:w-auto pb-2 md:pb-0">
              {["All", "Processing", "Shipped", "Delivered", "Cancelled"].map((status) => (
                  <button
                    key={status}
                    onClick={() => setSelectedStatusFilter(status)}
                    className={`px-4 py-2 rounded-xl text-sm font-medium transition whitespace-nowrap ${
                        selectedStatusFilter === status 
                        ? "bg-slate-900 text-white shadow-md" 
                        : "bg-slate-50 text-slate-600 hover:bg-slate-100"
                    }`}
                  >
                      {status === "All" ? "Tümü" : 
                       status === "Processing" ? "Hazırlanıyor" :
                       status === "Shipped" ? "Kargoda" :
                       status === "Delivered" ? "Tamamlanan" : "İptal"}
                  </button>
              ))}
          </div>

          {/* Search Bar */}
          <div className="relative w-full md:w-80">
              <MagnifyingGlassIcon className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-slate-400"/>
              <input 
                  type="text" 
                  placeholder="Sipariş No veya Ürün Ara..." 
                  className="w-full pl-10 pr-4 py-2.5 bg-slate-50 border border-slate-200 rounded-xl focus:ring-2 focus:ring-orange-500 outline-none text-sm transition"
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
              />
          </div>
      </div>

      {/* ORDERS LIST */}
      <div className="space-y-4">
          {loading ? (
              <div className="p-12 flex justify-center">
                  <div className="w-10 h-10 border-4 border-slate-200 border-t-orange-500 rounded-full animate-spin"></div>
              </div>
          ) : filteredOrders.length === 0 ? (
              <div className="bg-white rounded-2xl border border-slate-200 p-16 text-center text-slate-400 flex flex-col items-center">
                  <ShoppingBagIcon className="w-16 h-16 mb-4 opacity-20"/>
                  <p className="text-lg font-medium text-slate-600">Sipariş Bulunamadı</p>
                  <p className="text-sm">Seçili kriterlere uygun sipariş yok.</p>
              </div>
          ) : (
              filteredOrders.map((order) => (
                  <div key={order.orderId} className={`bg-white rounded-2xl border transition-all duration-300 ${expandedOrderId === order.orderId ? 'border-orange-200 shadow-md ring-1 ring-orange-100' : 'border-slate-200 shadow-sm hover:shadow-md'}`}>
                      
                      {/* ORDER HEADER (SUMMARY) */}
                      <div className="p-5 flex flex-col md:flex-row md:items-center justify-between gap-4 cursor-pointer" onClick={() => toggleDetails(order.orderId)}>
                          
                          {/* Left: Info */}
                          <div className="flex items-center gap-4">
                              <div className="p-3 bg-slate-50 rounded-xl border border-slate-100 hidden sm:block">
                                  <ShoppingBagIcon className="w-6 h-6 text-slate-400"/>
                              </div>
                              <div>
                                  <div className="flex items-center gap-3">
                                      <h3 className="font-bold text-slate-900 text-lg">{order.orderNumber}</h3>
                                      <StatusBadge status={order.orderStatus} />
                                  </div>
                                  <div className="flex items-center gap-3 text-sm text-slate-500 mt-1">
                                      <span className="flex items-center gap-1">
                                          <ClockIcon className="w-4 h-4"/> 
                                          {new Date(order.createdAt).toLocaleDateString("tr-TR", {day: 'numeric', month: 'long', hour: '2-digit', minute:'2-digit'})}
                                      </span>
                                      <span>•</span>
                                      <span className="font-medium text-slate-700">{order.items.length} Ürün</span>
                                      <span>•</span>
                                      <span className="font-bold text-slate-900">{order.subtotal.toLocaleString('tr-TR')} ₺</span>
                                  </div>
                              </div>
                          </div>

                          {/* Right: Actions & Toggle */}
                          <div className="flex items-center gap-3">
                              
                              {/* ACTION BUTTONS (Quick Actions) */}
                              {order.orderStatus === "Processing" && (
                                  <button 
                                    onClick={(e) => { e.stopPropagation(); openShippingModal(order); }}
                                    className="hidden sm:flex items-center gap-2 px-4 py-2 bg-orange-600 text-white rounded-xl hover:bg-orange-700 transition font-medium shadow-sm shadow-orange-200"
                                  >
                                      <TruckIcon className="w-4 h-4"/>
                                      Kargola
                                  </button>
                              )}
                              
                              {order.orderStatus === "Shipped" && (
                                  <button 
                                    onClick={(e) => { e.stopPropagation(); handleStatusChange(order.orderId, "Delivered"); }}
                                    className="hidden sm:flex items-center gap-2 px-4 py-2 bg-emerald-600 text-white rounded-xl hover:bg-emerald-700 transition font-medium shadow-sm shadow-emerald-200"
                                  >
                                      <CheckCircleIcon className="w-4 h-4"/>
                                      Teslim Edildi Yap
                                  </button>
                              )}

                              <button className={`p-2 rounded-lg transition ${expandedOrderId === order.orderId ? 'bg-orange-50 text-orange-600' : 'hover:bg-slate-50 text-slate-400'}`}>
                                  {expandedOrderId === order.orderId ? <ChevronUpIcon className="w-5 h-5"/> : <ChevronDownIcon className="w-5 h-5"/>}
                              </button>
                          </div>
                      </div>

                      {/* ORDER DETAILS (EXPANDABLE) */}
                      {expandedOrderId === order.orderId && (
                          <div className="border-t border-slate-100 bg-slate-50/50 p-6 rounded-b-2xl animate-fade-in-down">
                              
                              <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
                                  
                                  {/* Column 1: Items */}
                                  <div className="lg:col-span-2 space-y-4">
                                      <h4 className="font-bold text-slate-800 flex items-center gap-2 mb-3">
                                          <ShoppingBagIcon className="w-4 h-4 text-orange-500"/>
                                          Sipariş İçeriği
                                      </h4>
                                      <div className="space-y-3">
                                          {order.items.map((item) => (
                                              <div key={item.orderItemId} className="flex items-center gap-4 bg-white p-3 rounded-xl border border-slate-200">
                                                  <div className="w-14 h-14 bg-slate-100 rounded-lg border border-slate-200 overflow-hidden flex-shrink-0">
                                                      {item.productImage ? (
                                                          <img src={item.productImage} className="w-full h-full object-cover" alt=""/>
                                                      ) : (
                                                          <ShoppingBagIcon className="w-6 h-6 text-slate-300 m-auto mt-4"/>
                                                      )}
                                                  </div>
                                                  <div className="flex-1">
                                                      <div className="font-semibold text-slate-900 line-clamp-1">{item.productName}</div>
                                                      <div className="text-xs text-slate-500 mt-0.5">Adet: <span className="font-bold text-slate-800">{item.quantity}</span> • Birim Fiyat: {item.unitPrice} ₺</div>
                                                  </div>
                                                  <div className="text-right font-bold text-slate-800">
                                                      {item.totalPrice.toLocaleString('tr-TR')} ₺
                                                  </div>
                                              </div>
                                          ))}
                                      </div>
                                  </div>

                                  {/* Column 2: Address & Customer Info */}
                                  <div className="space-y-6">
                                      
                                      {/* Delivery Address */}
                                      <div className="bg-white p-4 rounded-xl border border-slate-200">
                                          <h4 className="font-bold text-slate-800 flex items-center gap-2 mb-3 text-sm">
                                              <MapPinIcon className="w-4 h-4 text-orange-500"/>
                                              Teslimat Adresi
                                          </h4>
                                          <div className="text-sm text-slate-600 space-y-1">
                                              <p className="font-semibold text-slate-900">{order.shippingAddress.contactName}</p>
                                              <p>{order.shippingAddress.fullAddress}</p>
                                              <p>{order.shippingAddress.district} / {order.shippingAddress.city}</p>
                                              <p className="text-slate-400 text-xs mt-2">{order.shippingAddress.contactPhone}</p>
                                          </div>
                                      </div>

                                      {/* Customer Note */}
                                      {order.customerNote && (
                                          <div className="bg-amber-50 p-4 rounded-xl border border-amber-100">
                                              <h4 className="font-bold text-amber-800 flex items-center gap-2 mb-2 text-sm">
                                                  <UserIcon className="w-4 h-4"/>
                                                  Müşteri Notu
                                              </h4>
                                              <p className="text-sm text-amber-700 italic">"{order.customerNote}"</p>
                                          </div>
                                      )}

                                      {/* Tracking Info (If Shipped) */}
                                      {order.orderStatus === "Shipped" && order.trackingNumber && (
                                          <div className="bg-blue-50 p-4 rounded-xl border border-blue-100">
                                              <h4 className="font-bold text-blue-800 flex items-center gap-2 mb-2 text-sm">
                                                  <TruckIcon className="w-4 h-4"/>
                                                  Kargo Bilgisi
                                              </h4>
                                              <p className="text-xs text-blue-600 font-medium uppercase">{order.shippingProvider || "Kargo Firması"}</p>
                                              <p className="text-sm font-mono font-bold text-blue-900 mt-1">{order.trackingNumber}</p>
                                          </div>
                                      )}

                                      {/* Mobile Actions (Only visible on small screens inside details) */}
                                      <div className="sm:hidden flex flex-col gap-2 pt-2">
                                          {order.orderStatus === "Processing" && (
                                              <button 
                                                onClick={() => openShippingModal(order)}
                                                className="w-full flex items-center justify-center gap-2 px-4 py-3 bg-orange-600 text-white rounded-xl font-medium"
                                              >
                                                  <TruckIcon className="w-5 h-5"/> Kargola
                                              </button>
                                          )}
                                          {order.orderStatus === "Shipped" && (
                                              <button 
                                                onClick={() => handleStatusChange(order.orderId, "Delivered")}
                                                className="w-full flex items-center justify-center gap-2 px-4 py-3 bg-emerald-600 text-white rounded-xl font-medium"
                                              >
                                                  <CheckCircleIcon className="w-5 h-5"/> Teslim Edildi
                                              </button>
                                          )}
                                      </div>

                                  </div>
                              </div>
                          </div>
                      )}
                  </div>
              ))
          )}
      </div>

      {/* PAGINATION */}
      {!loading && totalPages > 1 && (
          <div className="flex items-center justify-between px-6 py-6 border-t border-slate-200 mt-4">
            <span className="text-sm text-slate-500">
              Sayfa {page} / {totalPages}
            </span>
            <div className="flex gap-2">
              <button
                onClick={() => setPage(Math.max(1, page - 1))}
                disabled={page <= 1}
                className="px-4 py-2 rounded-xl border border-slate-200 text-sm font-medium hover:bg-slate-50 disabled:opacity-50 disabled:cursor-not-allowed transition"
              >
                Önceki
              </button>
              <button
                onClick={() => setPage(Math.min(totalPages, page + 1))}
                disabled={page >= totalPages}
                className="px-4 py-2 rounded-xl border border-slate-200 text-sm font-medium hover:bg-slate-50 disabled:opacity-50 disabled:cursor-not-allowed transition"
              >
                Sonraki
              </button>
            </div>
          </div>
        )}

      {/* SHIPPING MODAL (Dialog) */}
      <Transition appear show={isShipModalOpen} as={Fragment}>
        <Dialog as="div" className="relative z-50" onClose={() => setIsShipModalOpen(false)}>
          <Transition.Child
            as={Fragment}
            enter="ease-out duration-300"
            enterFrom="opacity-0"
            enterTo="opacity-100"
            leave="ease-in duration-200"
            leaveFrom="opacity-100"
            leaveTo="opacity-0"
          >
            <div className="fixed inset-0 bg-black/30 backdrop-blur-sm" />
          </Transition.Child>

          <div className="fixed inset-0 overflow-y-auto">
            <div className="flex min-h-full items-center justify-center p-4 text-center">
              <Transition.Child
                as={Fragment}
                enter="ease-out duration-300"
                enterFrom="opacity-0 scale-95"
                enterTo="opacity-100 scale-100"
                leave="ease-in duration-200"
                leaveFrom="opacity-100 scale-100"
                leaveTo="opacity-0 scale-95"
              >
                <Dialog.Panel className="w-full max-w-md transform overflow-hidden rounded-2xl bg-white p-6 text-left align-middle shadow-xl transition-all">
                  <div className="flex justify-between items-center mb-4">
                      <Dialog.Title as="h3" className="text-lg font-bold leading-6 text-gray-900 flex items-center gap-2">
                        <TruckIcon className="w-5 h-5 text-orange-600"/>
                        Siparişi Kargola
                      </Dialog.Title>
                      <button onClick={() => setIsShipModalOpen(false)} className="text-slate-400 hover:text-slate-600"><XMarkIcon className="w-6 h-6"/></button>
                  </div>
                  
                  <div className="mt-2 space-y-4">
                    <p className="text-sm text-slate-500">
                      <span className="font-bold text-slate-800">{selectedOrderForShip?.orderNumber}</span> nolu sipariş için kargo bilgilerini giriniz.
                    </p>

                    <div>
                        <label className="block text-sm font-medium text-slate-700 mb-1">Kargo Firması</label>
                        <select 
                            className="w-full px-3 py-2 border border-slate-200 rounded-xl focus:ring-2 focus:ring-orange-500 outline-none text-sm bg-slate-50"
                            value={trackingInfo.provider}
                            onChange={(e) => setTrackingInfo({...trackingInfo, provider: e.target.value})}
                        >
                            <option value="Aras Kargo">Aras Kargo</option>
                            <option value="Yurtiçi Kargo">Yurtiçi Kargo</option>
                            <option value="MNG Kargo">MNG Kargo</option>
                            <option value="PTT Kargo">PTT Kargo</option>
                            <option value="Sürat Kargo">Sürat Kargo</option>
                            <option value="Diğer">Diğer</option>
                        </select>
                    </div>

                    <div>
                        <label className="block text-sm font-medium text-slate-700 mb-1">Takip Numarası</label>
                        <input 
                            type="text" 
                            className="w-full px-3 py-2 border border-slate-200 rounded-xl focus:ring-2 focus:ring-orange-500 outline-none text-sm"
                            placeholder="Örn: 1234567890"
                            value={trackingInfo.number}
                            onChange={(e) => setTrackingInfo({...trackingInfo, number: e.target.value})}
                        />
                    </div>
                  </div>

                  <div className="mt-6 flex justify-end gap-3">
                    <button
                      type="button"
                      className="px-4 py-2 rounded-xl border border-slate-200 text-slate-600 text-sm font-medium hover:bg-slate-50"
                      onClick={() => setIsShipModalOpen(false)}
                    >
                      İptal
                    </button>
                    <button
                      type="button"
                      disabled={!trackingInfo.number}
                      className="px-4 py-2 rounded-xl bg-slate-900 text-white text-sm font-bold hover:bg-slate-800 disabled:opacity-50 disabled:cursor-not-allowed shadow-md"
                      onClick={() => handleStatusChange(selectedOrderForShip.orderId, "Shipped", { trackingNumber: trackingInfo.number, shippingProvider: trackingInfo.provider })}
                    >
                      Kargoya Ver
                    </button>
                  </div>
                </Dialog.Panel>
              </Transition.Child>
            </div>
          </div>
        </Dialog>
      </Transition>

    </div>
  );
}