import { cn } from "@/utils/cn";

interface CardProps {
  children: React.ReactNode;
  className?: string;
  hover?: boolean;
}

const Card = ({ children, className, hover = false }: CardProps) => {
  return (
    <div
      className={cn(
        "bg-white rounded-xl border border-gray-100 shadow-sm",
        hover && "hover:shadow-md hover:-translate-y-0.5 transition-all duration-200 cursor-pointer",
        className
      )}
    >
      {children}
    </div>
  );
};

export default Card;