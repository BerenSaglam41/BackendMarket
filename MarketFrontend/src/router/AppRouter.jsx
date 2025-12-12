import { BrowserRouter, Routes, Route } from "react-router-dom";
import MainLayout from "../components/layout/MainLayout";

// Pages
import Home from "../pages/Home";
import CategoryPage from "../pages/CategoryPage";
import ListingDetailPage from "../pages/ListingDetailPage";
import CartPage from "../pages/CartPage";
import AuthPage from "../pages/auth/AuthPage";

export default function AppRouter() {
  return (
    <BrowserRouter>
      <Routes>

        {/* ===== MAIN LAYOUT ===== */}
        <Route element={<MainLayout />}>
          <Route path="/" element={<Home />} />
          <Route path="/category/:slug" element={<CategoryPage/>} />
          <Route path="/listing/:slug" element={<ListingDetailPage/>} />
          <Route path="/cart" element={<CartPage/>} />
          <Route path="/auth" element={<AuthPage />} />

        </Route>

        {/* ===== AUTH PAGES (Layout DIŞI) ===== */}

        {/* ===== 404 (ileride) ===== */}
        {/* <Route path="*" element={<NotFound />} /> */}

      </Routes>
    </BrowserRouter>
  );
}