import { useEffect, useState } from "react";
import AuthCard from "../../components/auth/AuthCard";
import { useSearchParams } from "react-router-dom";
export default function AuthPage() {
  const [searchParams,setSearchParams] = useSearchParams();
  const [mode, setMode] = useState(searchParams.get("mode") || "login"); // login | register
  return (
    <div className="min-h-[calc(100vh-64px)] bg-gray-50 flex items-center justify-center px-4">
      <AuthCard mode={mode} setMode={setMode} />
    </div>
  );
}