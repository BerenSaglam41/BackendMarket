import CategoryBar from "./CategoryBar";
import Navbar from "./NavBar";

export default function MainLayout({ children }) {
  return (
    <>
      <Navbar />
      <CategoryBar/>
      <main className="pt-4">{children}</main>
    </>
  );
}