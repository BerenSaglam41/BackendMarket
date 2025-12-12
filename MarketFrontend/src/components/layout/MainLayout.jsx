import { Outlet } from "react-router-dom";
import CategoryBar from "./CategoryBar";
import Navbar from "./NavBar";
import CartDrawer from "../cart/CartDrawer";
export default function MainLayout() {
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