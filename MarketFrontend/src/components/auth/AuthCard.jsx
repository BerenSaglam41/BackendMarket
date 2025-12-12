import AuthTabs from "./AuthTabs";
import LoginForm from "./LoginForm";
import RegisterForm from "./RegisterForm";

export default function AuthCard({ mode, setMode }) {
  return (
    <div className="w-full max-w-md bg-white rounded-2xl shadow-xl p-6 sm:p-8">

      {/* LOGO / BRAND */}
      <div className="text-center mb-6">
        <h1 className="text-2xl font-extrabold text-orange-600">
          MarketApp
        </h1>
        <p className="text-sm text-gray-500 mt-1">
          Alışverişe devam etmek için giriş yap
        </p>
      </div>

      <AuthTabs mode={mode} setMode={setMode} />

      <div className="mt-6">
        {mode === "login" ? <LoginForm /> : <RegisterForm />}
      </div>
    </div>
  );
}