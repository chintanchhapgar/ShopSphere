import { cn } from "@/utils/cn";

interface SpinnerProps {
  size?: "sm" | "md" | "lg";
  className?: string;
}

const sizes = {
  sm: "w-4 h-4",
  md: "w-6 h-6",
  lg: "w-10 h-10",
};

const Spinner = ({ size = "md", className }: SpinnerProps) => {
  return (
    <div
      className={cn(
        "animate-spin rounded-full border-2 border-current border-t-transparent",
        sizes[size],
        className
      )}
    />
  );
};

export default Spinner;