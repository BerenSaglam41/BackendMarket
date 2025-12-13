import { create } from "zustand";
import api from "../lib/axios";
import { fetchMeRequest, loginRequest } from "../services/AuthService";
import { useCartStore } from "./cartStore";

const TOKEN_KEY = "market_token";
const loadToken = () => localStorage.getItem(TOKEN_KEY);

export const useAuthStore = create((set) => ({
  token: loadToken(),
  user: null,
  isAuthenticated: !!loadToken(),

  /* ===== LOGIN ===== */
  login: async (email, password) => {
    const data = await loginRequest(email, password);
    const { token } = data;

    localStorage.setItem(TOKEN_KEY, token);
    api.defaults.headers.common["Authorization"] = `Bearer ${token}`;

    set({
      token,
      isAuthenticated: true,
    });

    await useAuthStore.getState().fetchMe();
    await useCartStore.getState().syncLocalCartToBackend();
  },

  /* ===== FETCH ME ===== */
  fetchMe: async () => {
    const res = await fetchMeRequest();
    set({ user: res.data });
  },

  /* ===== LOGOUT ===== */
  logout: () => {
    localStorage.removeItem(TOKEN_KEY);
    delete api.defaults.headers.common["Authorization"];

    set({
      token: null,
      user: null,
      isAuthenticated: false,
    });

    useCartStore.getState().clearCart();
  },

  /* ===== INIT ===== */
  initAuth: async () => {
    const token = loadToken();
    if (!token) return;

    api.defaults.headers.common["Authorization"] = `Bearer ${token}`;

    set({
      token,
      isAuthenticated: true,
    });

    await useAuthStore.getState().fetchMe();
  },
}));