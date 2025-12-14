import { BrowserRouter, Routes, Route } from "react-router-dom";
import MainLayout from "../components/layout/MainLayout";

// Pages
import Home from "../pages/Home";
import CategoryPage from "../pages/CategoryPage";
import ListingDetailPage from "../pages/ListingDetailPage";
import CartPage from "../pages/CartPage";
import AuthPage from "../pages/auth/AuthPage";

// Guards
import RoleRoute from "./RoleRoute";

// Admin
import AdminLayout from "../pages/admin/AdminLayout";
import AdminDashboard from "../pages/admin/AdminDashboard";
import SellerApplications from "../pages/admin/SellerApplication";
// Seller
import SellerDashboard from "../pages/SellerDashboard";
import AdminOrdersPage from "../pages/admin/AdminOrdersPage";
import AdminPendingProductsPage from "../pages/admin/AdminPendingProductsPage";
import AdminProductsPage from "../pages/admin/AdminProductsPage";
import AdminAddProductPage from "../pages/admin/AdminAddProductPage";
import AdminEditProductPage from "../pages/admin/AdminEditProductPage";

export default function AppRouter() {
  return (
    <BrowserRouter>
      <Routes>

        {/* ===== PUBLIC + AUTH LAYOUT ===== */}
        <Route element={<MainLayout />}>
          <Route path="/" element={<Home />} />
          <Route path="/category/:slug" element={<CategoryPage />} />
          <Route path="/listing/:slug" element={<ListingDetailPage />} />
          <Route path="/cart" element={<CartPage />} />
          <Route path="/auth" element={<AuthPage />} />

          {/* ===== SELLER ===== */}
          <Route element={<RoleRoute allowedRoles={["Seller"]} />}>
            <Route path="/seller" element={<SellerDashboard />} />
          </Route>
        </Route>

        {/* ===== ADMIN (AYRI LAYOUT) ===== */}
        <Route
          element={<RoleRoute allowedRoles={["Admin"]} />}
        >
          <Route path="/admin" element={<AdminLayout />}>
            <Route index element={<AdminDashboard />} />
            <Route
              path="seller-applications"
              element={<SellerApplications />}
            />
            <Route 
              path="orders"
              element={<AdminOrdersPage />}
            />
            <Route
              path="pending-products"
              element={<AdminPendingProductsPage />}
            />
            <Route
              path="products"
              element={<AdminProductsPage />}
            />
            <Route
              path="products/add"
              element={<AdminAddProductPage />}
            />
            <Route
              path="products/edit/:productSlug"
              element={<AdminEditProductPage />}
            />
          </Route>
        </Route>

        {/* ===== 404 (SONRA) ===== */}
        {/* <Route path="*" element={<NotFound />} /> */}

      </Routes>
    </BrowserRouter>
  );
}