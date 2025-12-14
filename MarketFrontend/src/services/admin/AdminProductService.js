import api from "../../lib/axios";

export const fetchAdminProducts = (params) => 
    api.get("/product",{params, silent:true});

export const fetchAdminProductBySlug= (slug) =>
    api.get(`/product/${slug}`, { silent:true });

export const updateProduct = (id, payload) =>
    api.put(`/product/${id}`, payload);

export const deleteProduct = (id) =>
    api.delete(`/product/${id}`);

export const createProduct = (payload) => 
    api.post("/product", payload);