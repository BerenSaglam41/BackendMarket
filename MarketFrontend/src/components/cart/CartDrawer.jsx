import { XMarkIcon } from "@heroicons/react/24/outline";
import { useUIStore } from "../../store/uiStore";
import { useCartStore } from "../../store/cartStore";
import CartItemRow from "./CartItemRow";
import { Link } from "react-router-dom";

const formatPrice = (price) =>
  new Intl.NumberFormat("tr-TR", {
    style: "currency",
    currency: "TRY",
  }).format(price);

export default function CartDrawer() {
  const isOpen = useUIStore((s) => s.isCartOpen);
  const closeCart = useUIStore((s) => s.closeCart);

  const items = useCartStore((s) => s.items);
  const totalPrice = useCartStore((s) => s.totalPrice());

  return (
    <>
      {/* BACKDROP */}
      {isOpen && (
        <div
          onClick={closeCart}
          className="fixed inset-0 bg-black/40 z-40"
        />
      )}

      {/* DRAWER */}
      <div
        className={`fixed top-0 right-0 h-full w-[380px] bg-white z-50 shadow-2xl
        transform transition-transform duration-300
        ${isOpen ? "translate-x-0" : "translate-x-full"}`}
      >
        {/* HEADER */}
        <div className="flex items-center justify-between px-4 py-4 border-b">
          <h2 className="text-lg font-bold">Sepetim</h2>
          <button onClick={closeCart}>
            <XMarkIcon className="w-6 h-6 text-gray-500 hover:text-gray-800" />
          </button>
        </div>

        {/* CONTENT */}
        <div className="flex-1 overflow-y-auto px-4 py-4 space-y-4">
          {items.length === 0 ? (
            <div className="text-center text-gray-500 mt-10">
              Sepetiniz boş
            </div>
          ) : (
            items.map((item) => (
              <CartItemRow key={item.listingId} item={item} />
            ))
          )}
        </div>

        {/* FOOTER */}
        {items.length > 0 && (
          <div className="border-t px-4 py-4 space-y-3">
            <div className="flex justify-between text-sm">
              <span className="text-gray-500">Toplam</span>
              <span className="font-bold text-gray-900">
                {formatPrice(totalPrice)}
              </span>
            </div>

            <Link
              to="/checkout"
              onClick={closeCart}
              className="block w-full text-center bg-orange-500 text-white py-3 rounded-lg font-semibold hover:bg-orange-600 transition"
            >
              Siparişi Tamamla
            </Link>
          </div>
        )}
      </div>
    </>
  );
}