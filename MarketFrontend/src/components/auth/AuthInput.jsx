export default function AuthInput({ label, icon: Icon, rightIcon, ...props }) {
  return (
    <div className="space-y-1">
      <label className="text-xs font-medium text-gray-600">
        {label}
      </label>

      <div className="relative">
        {Icon && (
          <Icon className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
        )}

        <input
          {...props}
          className={`w-full py-2.5 rounded-lg border border-gray-200 
            focus:outline-none focus:ring-2 focus:ring-orange-500 transition
            ${Icon ? "pl-10" : "pl-4"} 
            ${rightIcon ? "pr-10" : "pr-4"}
          `}
        />

        {rightIcon && (
          <div className="absolute right-3 top-1/2 -translate-y-1/2">
            {rightIcon}
          </div>
        )}
      </div>
    </div>
  );
}