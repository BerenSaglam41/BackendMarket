import api from "../lib/axios";

export const fetchCategoryTree = async () => {
  const res = await api.get("/category/tree",{ silent: true });
  return res.data.data;
};