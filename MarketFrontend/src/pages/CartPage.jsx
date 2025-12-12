import { Link } from "react-router-dom";
import { useCartStore } from "../store/cartStore";
import CartItemRow from "../components/cart/CartItemRow";

const formatPrice = (price) =>
  new Intl.NumberFormat("tr-TR", {
    style: "currency",
    currency: "TRY",
    minimumFractionDigits: 2,
  }).format(price);

export default function CartPage() {
  const items = useCartStore((s) => s.items);
    console.log(items)
  const selectedItems = items.filter(i => i.isSelectedForCheckout);
  const totalQuantity = selectedItems.reduce((sum, i) => sum + i.quantity, 0);
  const totalPrice = selectedItems.reduce((sum, i) => sum + i.totalPrice, 0);

  const isEmpty = items.length === 0;
  const isCheckoutDisabled = selectedItems.length === 0;

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto px-4 py-6">
        
        {/* BREADCRUMB */}
        <nav className="text-sm text-gray-500 mb-6">
          <Link to="/" className="hover:text-orange-500">
            Anasayfa
          </Link>
          <span className="mx-2">/</span>
          <span className="text-gray-900 font-medium">Sepetim</span>
        </nav>

        {/* TITLE */}
        <h1 className="text-2xl font-bold text-gray-900 mb-8">
          Sepetim ({totalQuantity} ürün)
        </h1>

        <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
          
          {/* LEFT */}
          <section className="lg:col-span-8 space-y-4">
            {isEmpty ? (
              <div className="bg-white border rounded-lg p-10 text-center text-gray-500">
                Sepetiniz boş
              </div>
            ) : (
              items.map(item => (
                <CartItemRow
                  key={item.listingId}
                  item={item}
                />
              ))
            )}
          </section>

          {/* RIGHT */}
          <aside className="lg:col-span-4">
            <div className="bg-white border rounded-xl p-6 space-y-4 sticky top-24">
              
              <h2 className="text-lg font-bold text-gray-900">
                Sipariş Özeti
              </h2>

              <div className="flex justify-between text-sm text-gray-600">
                <span>Ara Toplam</span>
                <span>{formatPrice(totalPrice)}</span>
              </div>

              <div className="flex justify-between text-sm text-gray-600">
                <span>Kargo</span>
                <span className="text-green-600 font-medium">
                  Ücretsiz
                </span>
              </div>

              <div className="border-t pt-3 flex justify-between font-bold text-gray-900 text-lg">
                <span>Toplam</span>
                <span>{formatPrice(totalPrice)}</span>
              </div>

              <button
                disabled={isCheckoutDisabled}
                className={`w-full py-3 rounded-lg font-semibold transition
                  ${
                    isCheckoutDisabled
                      ? "bg-gray-200 text-gray-400 cursor-not-allowed"
                      : "bg-orange-600 text-white hover:bg-orange-700 active:scale-95"
                  }
                `}
              >
                Satın Almaya Devam Et
              </button>

              <div className="text-xs text-gray-400 text-center">
                Satın almak için giriş yapmanız gerekecek
              </div>
            </div>
          </aside>

        </div>
      </div>
    </div>
  );
}