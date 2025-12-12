// src/services/cartService.js
import api from "../lib/axios";

/* ===== CART API ===== */

export const addToCartApi = (listingId, quantity) => {
  return api.post("/cart", { listingId, quantity },{silent:true});
};

export const fetchCartApi = () => {
  return api.get("/cart");
};

export const removeCartItemApi = (cartItemId) => {
  return api.delete(`/cart/${cartItemId}`);
};

export const updateCartItemQtyApi = (cartItemId, quantity) => {
  return api.put(`/cart/${cartItemId}`, { quantity });
};

export const clearCartApi = () => {
  return api.delete("/cart/clear");
};