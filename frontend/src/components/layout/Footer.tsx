import { Link } from "react-router-dom";
import { Package } from "lucide-react";
import { APP_NAME } from "@/utils/constants";

const Footer = () => {
  return (
    <footer className="bg-gray-900 text-gray-400 mt-auto">
      <div className="max-w-7xl mx-auto px-4 py-12">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-8">
          <div>
            <div className="flex items-center gap-2 text-white font-bold text-lg mb-3">
              <Package className="w-5 h-5 text-primary-400" />
              {APP_NAME}
            </div>
            <p className="text-sm leading-relaxed">
              Your one-stop destination for quality products at great prices.
            </p>
          </div>

          {[
            {
              title: "Shop",
              links: [
                { label: "All Products", to: "/products" },
                { label: "New Arrivals", to: "/products?sortBy=0" },
                { label: "Top Rated",    to: "/products?sortBy=5" },
              ],
            },
            {
              title: "Account",
              links: [
                { label: "Login",      to: "/login" },
                { label: "Register",   to: "/register" },
                { label: "My Orders",  to: "/orders" },
                { label: "Wishlist",   to: "/wishlist" },
                { label: "My Profile", to: "/profile" },
              ],
            },
            {
              title: "Support",
              links: [
                { label: "FAQ",            to: "/faq" },
                { label: "Contact Us",     to: "/contact" },
                { label: "Returns Policy", to: "/returns" },
              ],
            },
          ].map((section) => (
            <div key={section.title}>
              <h3 className="text-white font-semibold mb-3">{section.title}</h3>
              <ul className="space-y-2">
                {section.links.map((link) => (
                  <li key={link.label}>
                    <Link to={link.to} className="text-sm hover:text-white transition">
                      {link.label}
                    </Link>
                  </li>
                ))}
              </ul>
            </div>
          ))}
        </div>
        <div className="mt-8 pt-8 border-t border-gray-800 text-center text-sm">
          © {new Date().getFullYear()} {APP_NAME}. All rights reserved.
        </div>
      </div>
    </footer>
  );
};

export default Footer;