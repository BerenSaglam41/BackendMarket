import api from "../../lib/axios";

export const fetchAdminPendingProducts = ({
    page = 1,
    pageSize = 20,
    status,
    sellerId,
}) => {
    return api.get("/admin/pending-products",
        {
            params: {
                page,
                pageSize,
                status,
                sellerId,
            },
            silent : true,
        }
    )
}

export const fetchAdminPendingProductsByIdApi = (id) => {
    return api.get(`/admin/pending-products/${id}`, { silent: true });
}

/* ===== ACTIONS ===== */
export const approvePendingProductApi = (id, payload) => {
  return api.post(`/admin/pending-products/${id}/approve`, payload);
};

export const rejectPendingProductApi = (id, payload) => {
  return api.post(`/admin/pending-products/${id}/reject`, payload);
};

export const requestUpdatePendingProductApi = (id, payload) => {
  return api.post(`/admin/pending-products/${id}/request-update`, payload);
};