import {
  PlusIcon,
  MinusIcon,
  TrashIcon,
} from "@heroicons/react/24/outline";
import { useCartStore } from "../../store/cartStore";

const formatPrice = (price) =>
  new Intl.NumberFormat("tr-TR", {
    style: "currency",
    currency: "TRY",
  }).format(price);

export default function CartItemRow({ item }) {
  const updateQuantity = useCartStore((s) => s.updateQuantity);
  const removeFromCart = useCartStore((s) => s.removeFromCart);

  return (
    <div className="flex gap-3 border rounded-lg p-3">
      {/* IMAGE */}
      <img
        src={item.productImage}
        alt={item.productName}
        className="w-16 h-16 object-contain"
      />

      {/* INFO */}
      <div className="flex-1 flex flex-col gap-1">
        <div className="text-sm font-medium line-clamp-2">
          {item.productName}
        </div>

        <div className="text-xs text-gray-400">
          Satıcı: {item.storeName}
        </div>

        <div className="flex items-center justify-between mt-2">
          {/* QUANTITY */}
          <div className="flex items-center border rounded-md">
            <button
              onClick={() => updateQuantity(item.listingId, item.quantity - 1)}
              className="p-1 hover:bg-gray-100"
            >
              <MinusIcon className="w-4 h-4" />
            </button>

            <span className="px-2 text-sm">{item.quantity}</span>

            <button
              onClick={() => updateQuantity(item.listingId, item.quantity + 1)}
              className="p-1 hover:bg-gray-100"
            >
              <PlusIcon className="w-4 h-4" />
            </button>
          </div>

          {/* PRICE */}
          <div className="text-sm font-bold">
            {formatPrice(item.totalPrice)}
          </div>
        </div>
      </div>

      {/* DELETE */}
      <button
        onClick={() => removeFromCart(item.listingId)}
        className="text-gray-400 hover:text-red-500"
      >
        <TrashIcon className="w-5 h-5" />
      </button>
    </div>
  );
}