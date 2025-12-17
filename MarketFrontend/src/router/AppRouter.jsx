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
import SellerDashboard from "../pages/seller/SellerDashboard";
import AdminOrdersPage from "../pages/admin/AdminOrdersPage";
import AdminPendingProductsPage from "../pages/admin/AdminPendingProductsPage";
import AdminProductsPage from "../pages/admin/AdminProductsPage";
import AdminAddProductPage from "../pages/admin/AdminAddProductPage";
import AdminEditProductPage from "../pages/admin/AdminEditProductPage";
import SellerLayout from "../pages/seller/SellerLayout";
import SellerProductsPage from "../pages/seller/SellerProductsPage";
import SellerPendingProductsPage from "../pages/seller/SellerPendingProductsPage";
import SellerPendingProductsAddPage from "../pages/seller/SellerPendingProductsAddPage";
import SellerPendingProductEditPage from "../pages/seller/SellerPendingProductEditPage";
import SellerProductsEditPage from "../pages/seller/SellerProductsEditPage";
import SellerOrdersPage from "../pages/seller/SellerOrdersPage";
import SellerAddListPage from "../pages/seller/SellerAddListPage";

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
        <Route 
          element={<RoleRoute allowedRoles={["Seller"]} />}
        >
          <Route path="/seller" element={<SellerLayout/>}>
            <Route index element={<SellerDashboard />} />
  
              <Route path="pending-products"
                element={<SellerPendingProductsPage/>}
              />
              <Route path="pending-products/add"
                element={<SellerPendingProductsAddPage />}
              />
              <Route path="pending-products/edit/:id"
                element={<SellerPendingProductEditPage />}
              />
              <Route path="lists"
                element={<SellerProductsPage/>}
              />
              <Route path="lists/add"
                element={<SellerAddListPage />}
              />
              <Route path="lists/edit/:listingId"
                element={<SellerProductsEditPage />}
              />
              <Route path="orders"
                element={<SellerOrdersPage />}
              />
          </Route>
        </Route>



        {/* ===== 404 (SONRA) ===== */}
        {/* <Route path="*" element={<NotFound />} /> */}

      </Routes>
    </BrowserRouter>
  );
}