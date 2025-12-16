import { Outlet } from "react-router-dom";
import SellerSidebar from "../../components/seller/SellerSidebar";
import SellerTopbar from "../../components/seller/SellerTopbar";

export default function SellerLayout() {
  return (
    <div className="min-h-screen flex bg-gray-100">

      {/* SIDEBAR */}
      <SellerSidebar />

      {/* MAIN */}
      <div className="flex-1 flex flex-col">
        <SellerTopbar />

        <main className="flex-1 p-6">
          <Outlet />
        </main>
      </div>

    </div>
  );
}