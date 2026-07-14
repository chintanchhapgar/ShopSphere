import { ButtonHTMLAttributes, forwardRef } from "react";
import { cn } from "@/utils/cn";
import Spinner from "./Spinner";

interface ButtonProps extends ButtonHTMLAttributes<HTMLButtonElement> {
  variant?: "primary" | "secondary" | "outline" | "ghost" | "danger";
  size?: "sm" | "md" | "lg";
  isLoading?: boolean;
  fullWidth?: boolean;
}

const variants = {
  primary: "bg-primary-600 text-white hover:bg-primary-700 shadow-sm",
  secondary: "bg-gray-100 text-gray-900 hover:bg-gray-200",
  outline: "border border-primary-600 text-primary-600 hover:bg-primary-50",
  ghost: "text-gray-600 hover:bg-gray-100",
  danger: "bg-red-600 text-white hover:bg-red-700",
};

const sizes = {
  sm: "px-3 py-1.5 text-sm",
  md: "px-4 py-2 text-sm",
  lg: "px-6 py-3 text-base",
};

const Button = forwardRef<HTMLButtonElement, ButtonProps>(
  (
    {
      className,
      variant = "primary",
      size = "md",
      isLoading = false,
      fullWidth = false,
      disabled,
      children,
      ...props
    },
    ref
  ) => {
    return (
      <button
        ref={ref}
        disabled={disabled || isLoading}
        className={cn(
          "inline-flex items-center justify-center gap-2 font-medium rounded-lg",
          "transition-all duration-200 focus:outline-none focus:ring-2",
          "focus:ring-primary-500 focus:ring-offset-2 disabled:opacity-50",
          "disabled:cursor-not-allowed",
          variants[variant],
          sizes[size],
          fullWidth && "w-full",
          className
        )}
        {...props}
      >
        {isLoading && <Spinner size="sm" />}
        {children}
      </button>
    );
  }
);

Button.displayName = "Button";
export default Button;