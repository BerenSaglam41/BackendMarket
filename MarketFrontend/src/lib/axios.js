import axios from "axios";
import { toast } from "react-toastify";

const api = axios.create({
  baseURL: "http://localhost:5062/api",
});

/* ===== RESPONSE INTERCEPTOR ===== */
api.interceptors.response.use(
  (response) => {
    const { success, message } = response.data || {};
    const silent = response.config.silent;

    if (!silent && success && message) {
      toast.success(message);
    }

    return response;
  },
  (error) => {
    const silent = error.config?.silent;
    if (!silent) {
      toast.error(
        error.response?.data?.message || "Bir hata oluştu"
      );
    }
    return Promise.reject(error);
  }
);

export default api;