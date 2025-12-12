import api from "../lib/axios";

/* =========================
   CART API SERVICE
   ========================= */

export const addToCartApi = (listingId, quantity) => {
  return api.post(
    "/cart",
    { listingId, quantity },
    { silent: true } 
  );
};

export const fetchCartApi = () => {
  return api.get("/cart");
};

export const updateCartItemQtyApi = (cartItemId, quantity) => {
  return api.put(`/cart/${cartItemId}`, { quantity });
};

export const removeCartItemApi = (cartItemId) => {
  return api.delete(`/cart/${cartItemId}`);
};

export const clearCartApi = () => {
  return api.delete("/cart/clear");
};