import { BrowserRouter, Routes, Route } from "react-router-dom";
import Home from "../pages/Home";
import Register from "../pages/auth/Register";
import Login from "../pages/auth/Login";
import MainLayout from "../components/layout/MainLayout";

// Pages

export default function AppRouter() {
  return (
    <BrowserRouter>
      <Routes>
        <Route 
            path="/"
            element={
                <MainLayout>
                    <Home />    
                </MainLayout>
            }
        />
        {/* Public Routes */}
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />

        {/* E-ticaret büyüdükçe buraya yüzlerce route eklenir */}
        
      </Routes>
    </BrowserRouter>
  );
}