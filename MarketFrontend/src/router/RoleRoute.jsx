import { Navigate, Outlet } from "react-router-dom";
import { useAuthStore } from "../store/authStore";

export default function RoleRoute({ allowedRoles = [] }) {
  const user = useAuthStore((s) => s.user);

  if (!user) {
    return <Navigate to="/auth?mode=login" replace />;
  }

  const userRoles = user.roles || [];
  const hasAccess = allowedRoles.some(role => userRoles.includes(role));

  if (!hasAccess) {
    return <Navigate to="/" replace />;
  }

  return <Outlet />;
}