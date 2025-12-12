export default function AuthTabs({ mode, setMode }) {
  return (
    <div className="grid grid-cols-2 border-b">
      <button
        onClick={() => setMode("login")}
        className={`py-3 text-sm font-semibold transition
          ${
            mode === "login"
              ? "border-b-2 border-orange-500 text-orange-600"
              : "text-gray-400 hover:text-gray-600"
          }
        `}
      >
        Giriş Yap
      </button>

      <button
        onClick={() => setMode("register")}
        className={`py-3 text-sm font-semibold transition
          ${
            mode === "register"
              ? "border-b-2 border-orange-500 text-orange-600"
              : "text-gray-400 hover:text-gray-600"
          }
        `}
      >
        Üye Ol
      </button>
    </div>
  );
}