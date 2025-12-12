import { create } from "zustand";
import api from "../lib/axios";
import { loginRequest } from "../services/AuthService";
import { useCartStore } from "./cartStore";

const TOKEN_KEY = "market_token";
const USER_KEY = "market_user";

const loadToken = () => localStorage.getItem(TOKEN_KEY);
const loadUser = () => {
  const data = localStorage.getItem(USER_KEY);
  return data ? JSON.parse(data) : null;
};

export const useAuthStore = create((set) => ({
  token: loadToken(),
  user: loadUser(),
  isAuthenticated: !!loadToken(),

  /* ===== LOGIN ===== */
  login: async (email, password) => {
    const data = await loginRequest(email, password);

    const { token, firstName, lastName, email: userEmail } = data;

    localStorage.setItem(TOKEN_KEY, token);
    localStorage.setItem(
      USER_KEY,
      JSON.stringify({ firstName, lastName, email: userEmail })
    );

    api.defaults.headers.common["Authorization"] = `Bearer ${token}`;

    set({
      token,
      user: { firstName, lastName, email: userEmail },
      isAuthenticated: true,
    });

    // 🔥 GUEST → AUTH CART SYNC
    await useCartStore.getState().syncLocalCartToBackend();
  },

  /* ===== LOGOUT ===== */
  logout: () => {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    delete api.defaults.headers.common["Authorization"];

    set({
      token: null,
      user: null,
      isAuthenticated: false,
    });

    useCartStore.getState().clearCart();
  },

  /* ===== INIT ===== */
  initAuth: () => {
    const token = loadToken();
    const user = loadUser();

    if (token) {
      api.defaults.headers.common["Authorization"] = `Bearer ${token}`;
      set({ token, user, isAuthenticated: true });
    }
  },
}));