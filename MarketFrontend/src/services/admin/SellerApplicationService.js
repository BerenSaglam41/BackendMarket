import api from "../../lib/axios";
/* ===== ADMIN – SELLER APPLICATIONS ===== */

export const fetchSellerApplicationsApi = () => {
  return api.get("/seller-applications/admin",{silent:true});
};

export const approveSellerApplicationApi = (applicationId) => {
  return api.post(`/seller-applications/admin/${applicationId}/approve`);
};

export const rejectSellerApplicationApi = (applicationId, adminNote) => {
  return api.post(`/seller-applications/admin/${applicationId}/reject`, {
    adminNote,
  });
};

export const requestUpdateSellerApplicationApi = (
  applicationId,
  adminNote
) => {
  return api.post(
    `/seller-applications/admin/${applicationId}/needs-update`,
    { adminNote }
  );
};