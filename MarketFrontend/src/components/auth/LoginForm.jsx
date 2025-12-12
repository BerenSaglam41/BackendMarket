import { useState } from "react";
import AuthInput from "./AuthInput";
import {
  EnvelopeIcon,
  LockClosedIcon,
  EyeIcon,
  EyeSlashIcon,
} from "@heroicons/react/24/outline";
import { useAuthStore } from "../../store/authStore";
import { useNavigate } from "react-router-dom";

export default function LoginForm() {
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);

  const login = useAuthStore((s) => s.login);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);

    try {
      await login(email, password);
      navigate("/");
    } finally {
      setLoading(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} className="space-y-5">

      <AuthInput
        label="E-posta"
        type="email"
        placeholder="ornek@mail.com"
        icon={EnvelopeIcon}
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        required
      />

      <AuthInput
        label="Şifre"
        type={showPassword ? "text" : "password"}
        placeholder="••••••••"
        icon={LockClosedIcon}
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        rightIcon={
          <button
            type="button"
            onClick={() => setShowPassword(!showPassword)}
            className="text-gray-400 hover:text-orange-500 transition"
          >
            {showPassword ? (
              <EyeSlashIcon className="w-5 h-5" />
            ) : (
              <EyeIcon className="w-5 h-5" />
            )}
          </button>
        }
        required
      />

      {/* ŞİFREMİ UNUTTUM */}
      <div className="flex justify-end">
        <button
          type="button"
          className="text-xs text-orange-600 hover:underline"
        >
          Şifremi unuttum
        </button>
      </div>

      {/* SUBMIT */}
      <button
        type="submit"
        disabled={loading}
        className={`w-full py-3 rounded-lg font-bold transition active:scale-95
          ${
            loading
              ? "bg-gray-300 text-gray-500 cursor-not-allowed"
              : "bg-orange-600 hover:bg-orange-700 text-white"
          }
        `}
      >
        {loading ? "Giriş Yapılıyor..." : "Giriş Yap"}
      </button>

      {/* ALT BİLGİ */}
      <p className="text-xs text-gray-400 text-center">
        Hesabınız yok mu? Üstten{" "}
        <span className="font-medium">Üye Ol</span> sekmesini seçin
      </p>
    </form>
  );
}