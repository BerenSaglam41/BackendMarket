import api from "../lib/axios";

/* ===== LOGIN ===== */
export const loginRequest = async (email, password) => {
  const res = await api.post("/auth/login", {
    email,
    password,
  });
  return res.data.data;
};

/* ===== REGISTER (ileride) ===== */
export const registerRequest = async (payload) => {
  const res = await api.post("/auth/register", payload);
  return res.data.data;
};

export const fetchMeRequest = async ()=>{
  const res = await api.get("/auth/me",{ silent: true });
  return res.data;
};