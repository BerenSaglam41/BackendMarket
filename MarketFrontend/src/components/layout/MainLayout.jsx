import { Outlet } from "react-router-dom";
import CategoryBar from "./CategoryBar";
import Navbar from "./NavBar";
import CartDrawer from "../cart/CartDrawer";
import { useEffect } from "react";
import { useAuthStore } from "../../store/authStore";
import { useCartStore } from "../../store/cartStore";
export default function MainLayout() {
  const initAuth = useAuthStore((s) => s.initAuth);
  const isAuthenticated = useAuthStore((s) => s.isAuthenticated);
  const fetchCartFromBackend = useCartStore((s) => s.fetchCartFromBackend);
  useEffect(() => {
    initAuth();
  }, []);
  useEffect(()=>{
    if(isAuthenticated){
      fetchCartFromBackend();
    }
  },[isAuthenticated])
  return (
    <>
      <Navbar />
      <CategoryBar />
      <CartDrawer/>
      <main className="pt-4">
        <Outlet />
      </main>
    </>
  );
}