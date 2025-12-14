import { Fragment } from "react";
import { Dialog, Transition } from "@headlessui/react"; // Headless UI kullanıyorsan harika olur, yoksa düz div ile de yapabiliriz. 
// Headless UI yoksa alttaki basit implementasyonu kullanabilirsin.
import { XMarkIcon, MapPinIcon, CreditCardIcon, TruckIcon, UserIcon } from "@heroicons/react/24/outline";
import { useOrderStore } from "../../../store/orderStore";

// Helper Formatter
const formatCurrency = (val) => new Intl.NumberFormat("tr-TR", { style: "currency", currency: "TRY" }).format(val);

export default function AdminOrderDetailsModal() {
  const { selectedOrder, setSelectedOrder } = useOrderStore();
  
  if (!selectedOrder) return null;

  const close = () => setSelectedOrder(null);

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
      {/* BACKDROP */}
      <div className="absolute inset-0 bg-gray-900/40 backdrop-blur-sm transition-opacity" onClick={close} />

      {/* CONTENT */}
      <div className="relative w-full max-w-4xl bg-white rounded-2xl shadow-2xl overflow-hidden flex flex-col max-h-[90vh] animate-fadeInScale">
        
        {/* HEADER */}
        <div className="flex items-center justify-between px-6 py-4 border-b border-gray-100 bg-gray-50/50">
          <div>
            <h3 className="text-lg font-bold text-gray-900">Sipariş Detayı</h3>
            <p className="text-xs text-gray-500 font-mono mt-0.5">{selectedOrder.orderNumber}</p>
          </div>
          <button onClick={close} className="p-2 rounded-full hover:bg-gray-200 text-gray-500 transition">
            <XMarkIcon className="w-6 h-6" />
          </button>
        </div>

        {/* SCROLLABLE BODY */}
        <div className="overflow-y-auto p-6 space-y-8">
          
          {/* 1. SECTON: ITEMS (ÜRÜNLER) */}
          <section>
            <h4 className="text-sm font-bold text-gray-900 mb-3 flex items-center gap-2">
              <TruckIcon className="w-4 h-4 text-orange-500"/> Sipariş İçeriği
            </h4>
            <div className="border rounded-xl overflow-hidden">
              <table className="w-full text-sm text-left">
                <thead className="bg-gray-50 text-gray-500 font-medium">
                  <tr>
                    <th className="p-3 pl-4">Ürün</th>
                    <th className="p-3">Satıcı</th>
                    <th className="p-3 text-center">Adet</th>
                    <th className="p-3 text-right pr-4">Tutar</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-100">
                  {selectedOrder.items?.map((item) => (
                    <tr key={item.orderItemId}>
                      <td className="p-3 pl-4">
                        <div className="flex items-center gap-3">
                          <div className="w-10 h-10 rounded bg-gray-100 border border-gray-200 flex items-center justify-center overflow-hidden shrink-0">
                            {item.productImage ? (
                                <img src={item.productImage} alt="" className="w-full h-full object-cover"/>
                            ) : (
                                <span className="text-xs font-bold text-gray-400">IMG</span>
                            )}
                          </div>
                          <div>
                            <div className="font-medium text-gray-900 line-clamp-1">{item.productName}</div>
                            <div className="text-xs text-gray-400">Kod: {item.productId}</div>
                          </div>
                        </div>
                      </td>
                      <td className="p-3 text-gray-600">{item.sellerStoreName}</td>
                      <td className="p-3 text-center font-medium">x{item.quantity}</td>
                      <td className="p-3 text-right pr-4 font-bold text-gray-900">
                        {formatCurrency(item.totalPrice)}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </section>

          {/* 2. SECTION: INFO GRID */}
          <section className="grid md:grid-cols-2 gap-6">
            
            {/* ADDRESS CARD */}
            <div className="bg-gray-50 rounded-xl p-5 border border-gray-100">
              <h4 className="text-sm font-bold text-gray-900 mb-3 flex items-center gap-2">
                <MapPinIcon className="w-4 h-4 text-blue-500"/> Teslimat Adresi
              </h4>
              <div className="text-sm text-gray-600 space-y-1 leading-relaxed">
                <p className="font-medium text-gray-900">{selectedOrder.shippingAddress?.title}</p>
                <p>{selectedOrder.shippingAddress?.fullAddress}</p>
                <p>{selectedOrder.shippingAddress?.district} / {selectedOrder.shippingAddress?.city}</p>
                <p className="text-gray-400 text-xs mt-2">{selectedOrder.shippingAddress?.contactPhone}</p>
              </div>
            </div>

            {/* PAYMENT & SUMMARY CARD */}
            <div className="bg-gray-50 rounded-xl p-5 border border-gray-100">
              <h4 className="text-sm font-bold text-gray-900 mb-3 flex items-center gap-2">
                <CreditCardIcon className="w-4 h-4 text-green-500"/> Ödeme Özeti
              </h4>
              
              <div className="space-y-2 text-sm">
                <div className="flex justify-between text-gray-500">
                  <span>Ara Toplam</span>
                  <span>{formatCurrency(selectedOrder.subtotal)}</span>
                </div>
                <div className="flex justify-between text-gray-500">
                  <span>Kargo</span>
                  <span>{formatCurrency(selectedOrder.shippingCost)}</span>
                </div>
                {selectedOrder.discountAmount > 0 && (
                    <div className="flex justify-between text-green-600">
                    <span>İndirim</span>
                    <span>-{formatCurrency(selectedOrder.discountAmount)}</span>
                    </div>
                )}
                <div className="border-t border-gray-200 pt-2 mt-2 flex justify-between font-bold text-lg text-gray-900">
                  <span>Toplam</span>
                  <span>{formatCurrency(selectedOrder.totalAmount)}</span>
                </div>
              </div>

              {/* Müşteri Notu Varsa */}
              {selectedOrder.customerNote && (
                 <div className="mt-4 pt-3 border-t border-gray-200">
                    <span className="text-xs font-bold text-orange-600 block mb-1">Müşteri Notu:</span>
                    <p className="text-xs text-gray-600 italic">"{selectedOrder.customerNote}"</p>
                 </div>
              )}
            </div>
          </section>

        </div>
        
        {/* FOOTER ACTIONS */}
        <div className="p-4 border-t border-gray-100 bg-gray-50 flex justify-end gap-3">
            <button onClick={close} className="px-5 py-2.5 bg-white border border-gray-300 rounded-lg text-gray-700 text-sm font-medium hover:bg-gray-50 transition">
                Kapat
            </button>
            {/* Buraya Fatura Yazdır vb. butonlar eklenebilir */}
            <button className="px-5 py-2.5 bg-slate-900 text-white rounded-lg text-sm font-medium hover:bg-slate-800 transition">
                Fatura Yazdır
            </button>
        </div>
      </div>
    </div>
  );
}