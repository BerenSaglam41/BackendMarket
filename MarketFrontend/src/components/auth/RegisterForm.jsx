import { useState } from "react";
import AuthInput from "./AuthInput";
import {
  UserIcon,
  EnvelopeIcon,
  LockClosedIcon,
  EyeIcon,
  EyeSlashIcon,
} from "@heroicons/react/24/outline";

export default function RegisterForm() {
  const [showPassword, setShowPassword] = useState(false);

  return (
    <form className="space-y-5">

      <div className="grid grid-cols-2 gap-3">
        <AuthInput label="Ad" placeholder="Adınız" icon={UserIcon} />
        <AuthInput label="Soyad" placeholder="Soyadınız" icon={UserIcon} />
      </div>

      <AuthInput
        label="E-posta"
        type="email"
        placeholder="ornek@mail.com"
        icon={EnvelopeIcon}
      />

      <AuthInput
        label="Şifre"
        placeholder="••••••••"
        type={showPassword ? "text" : "password"}
        icon={LockClosedIcon}
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
      />

      <button
        type="submit"
        className="w-full bg-orange-600 hover:bg-orange-700 text-white py-3 rounded-lg font-bold transition active:scale-95"
      >
        Üye Ol
      </button>

      <p className="text-xs text-gray-400 text-center leading-relaxed">
        Üye olarak{" "}
        <span className="underline cursor-pointer">
          Kullanım Koşulları
        </span>{" "}
        ve{" "}
        <span className="underline cursor-pointer">
          Gizlilik Politikası
        </span>
        ’nı kabul etmiş olursunuz
      </p>
    </form>
  );
}