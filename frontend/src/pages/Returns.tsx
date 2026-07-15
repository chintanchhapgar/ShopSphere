import {
  RefreshCw,
  CheckCircle,
  Clock,
  Package,
  Truck,
  Shield,
  AlertTriangle,
  XCircle,
} from "lucide-react";
import { Link } from "react-router-dom";
import { APP_NAME } from "@/utils/constants";
import { cn } from "@/utils/cn";

const STEPS = [
  {
    icon: Package,
    title: "Initiate Return",
    desc: "Go to My Orders → Select order → Click Return",
    color: "bg-blue-50 text-blue-600",
  },
  {
    icon: Clock,
    title: "Approval",
    desc: "Our team reviews your request within 24 hours",
    color: "bg-yellow-50 text-yellow-600",
  },
  {
    icon: Truck,
    title: "Pickup",
    desc: "We arrange free pickup from your address",
    color: "bg-purple-50 text-purple-600",
  },
  {
    icon: RefreshCw,
    title: "Refund",
    desc: "Refund processed within 5-7 business days",
    color: "bg-green-50 text-green-600",
  },
];

const RETURNABLE = [
  "Electronics (unopened, within 7 days)",
  "Clothing & Fashion (unused, with tags)",
  "Books (undamaged, within 15 days)",
  "Home Appliances (within 10 days)",
  "Furniture (within 7 days, unassembled)",
  "Sports Equipment (unused, within 15 days)",
  "Toys (unopened, within 30 days)",
];

const NON_RETURNABLE = [
  "Personal care & beauty products",
  "Undergarments & innerwear",
  "Grocery & food items",
  "Customized or personalized products",
  "Digital downloads",
  "Items marked as non-returnable",
];

const POLICIES = [
  {
    icon: Clock,
    title: "30-Day Window",
    desc: "Return most items within 30 days of delivery. Some categories have shorter windows.",
  },
  {
    icon: Shield,
    title: "Original Condition",
    desc: "Items must be unused, in original packaging with all tags and accessories intact.",
  },
  {
    icon: Truck,
    title: "Free Returns",
    desc: "We cover the return shipping cost. Our logistics partner will pick up from your address.",
  },
  {
    icon: RefreshCw,
    title: "Quick Refunds",
    desc: "Refunds are processed within 5-7 business days after the item is received and inspected.",
  },
];

const Returns = () => {
  return (
    <div className="max-w-4xl mx-auto px-4 py-12">

      {/* Header */}
      <div className="text-center mb-12">
        <div className="inline-flex items-center justify-center w-14 h-14 bg-primary-100 text-primary-600 rounded-2xl mb-4">
          <RefreshCw className="w-7 h-7" />
        </div>
        <h1 className="text-4xl font-bold text-gray-900 mb-3">
          Returns & Refund Policy
        </h1>
        <p className="text-gray-500 max-w-xl mx-auto">
          We want you to be completely satisfied with your purchase at {APP_NAME}
        </p>
      </div>

      {/* Policy Highlights */}
      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 mb-12">
        {POLICIES.map(({ icon: Icon, title, desc }) => (
          <div
            key={title}
            className="bg-white rounded-xl border border-gray-100 shadow-sm p-5 hover:shadow-md transition"
          >
            <div className="flex items-start gap-3">
              <div className="w-10 h-10 bg-primary-50 text-primary-600 rounded-xl flex items-center justify-center shrink-0">
                <Icon className="w-5 h-5" />
              </div>
              <div>
                <h3 className="text-sm font-bold text-gray-800 mb-1">{title}</h3>
                <p className="text-sm text-gray-500 leading-relaxed">{desc}</p>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* How It Works */}
      <div className="mb-12">
        <h2 className="text-2xl font-bold text-gray-900 mb-6 text-center">
          How Returns Work
        </h2>
        <div className="grid grid-cols-1 sm:grid-cols-4 gap-4">
          {STEPS.map(({ icon: Icon, title, desc, color }, idx) => (
            <div key={title} className="relative text-center">
              <div className={cn("w-14 h-14 rounded-2xl flex items-center justify-center mx-auto mb-3", color)}>
                <Icon className="w-6 h-6" />
              </div>
              <div className="absolute top-7 left-[60%] right-[-40%] h-0.5 bg-gray-200 hidden sm:block last:hidden" />
              <h3 className="text-sm font-bold text-gray-800 mb-1">
                Step {idx + 1}: {title}
              </h3>
              <p className="text-xs text-gray-500">{desc}</p>
            </div>
          ))}
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-12">

        {/* Returnable */}
        <div className="bg-white rounded-xl border border-green-200 p-6">
          <h3 className="text-lg font-bold text-gray-900 mb-4 flex items-center gap-2">
            <CheckCircle className="w-5 h-5 text-green-500" />
            Returnable Items
          </h3>
          <ul className="space-y-2.5">
            {RETURNABLE.map((item) => (
              <li key={item} className="flex items-start gap-2 text-sm text-gray-600">
                <CheckCircle className="w-4 h-4 text-green-500 shrink-0 mt-0.5" />
                {item}
              </li>
            ))}
          </ul>
        </div>

        {/* Non-Returnable */}
        <div className="bg-white rounded-xl border border-red-200 p-6">
          <h3 className="text-lg font-bold text-gray-900 mb-4 flex items-center gap-2">
            <XCircle className="w-5 h-5 text-red-500" />
            Non-Returnable Items
          </h3>
          <ul className="space-y-2.5">
            {NON_RETURNABLE.map((item) => (
              <li key={item} className="flex items-start gap-2 text-sm text-gray-600">
                <XCircle className="w-4 h-4 text-red-400 shrink-0 mt-0.5" />
                {item}
              </li>
            ))}
          </ul>
        </div>
      </div>

      {/* Refund Info */}
      <div className="bg-gray-50 rounded-2xl border border-gray-100 p-8 mb-12">
        <h2 className="text-2xl font-bold text-gray-900 mb-4">
          Refund Information
        </h2>
        <div className="space-y-4">
          {[
            { method: "Credit / Debit Card", time: "5-7 business days", note: "Refunded to original card" },
            { method: "PayPal",              time: "3-5 business days", note: "Refunded to PayPal account" },
            { method: "Cash on Delivery",    time: "7-10 business days", note: "Bank transfer to your account" },
          ].map(({ method, time, note }) => (
            <div key={method} className="flex items-center justify-between p-4 bg-white rounded-xl border border-gray-100">
              <div>
                <p className="text-sm font-semibold text-gray-800">{method}</p>
                <p className="text-xs text-gray-500">{note}</p>
              </div>
              <span className="text-sm font-medium text-primary-600 bg-primary-50 px-3 py-1 rounded-full">
                {time}
              </span>
            </div>
          ))}
        </div>
      </div>

      {/* Important Notes */}
      <div className="bg-yellow-50 rounded-2xl border border-yellow-200 p-6 mb-12">
        <h3 className="text-lg font-bold text-gray-900 mb-3 flex items-center gap-2">
          <AlertTriangle className="w-5 h-5 text-yellow-500" />
          Important Notes
        </h3>
        <ul className="space-y-2">
          {[
            "Items must be returned in their original packaging",
            "Please include all accessories, manuals, and free gifts",
            "Damaged or used items will not be accepted for return",
            "Return requests after the specified window will be declined",
            "Partial returns are allowed for multi-item orders",
          ].map((note) => (
            <li key={note} className="flex items-start gap-2 text-sm text-gray-600">
              <span className="text-yellow-500 mt-1">•</span>
              {note}
            </li>
          ))}
        </ul>
      </div>

      {/* CTA */}
      <div className="text-center p-8 bg-gradient-to-r from-primary-50 to-blue-50 rounded-2xl border border-primary-100">
        <h3 className="text-xl font-bold text-gray-900 mb-2">Need to Return Something?</h3>
        <p className="text-gray-600 mb-6">
          Go to your orders page to initiate a return
        </p>
        <div className="flex flex-col sm:flex-row gap-3 justify-center">
          <Link
            to="/orders"
            className="inline-flex items-center gap-2 px-6 py-3 bg-primary-600 text-white font-medium rounded-xl hover:bg-primary-700 transition"
          >
            <Package className="w-4 h-4" /> My Orders
          </Link>
          <Link
            to="/contact"
            className="inline-flex items-center gap-2 px-6 py-3 border border-gray-200 text-gray-700 font-medium rounded-xl hover:bg-gray-50 transition"
          >
            Contact Support
          </Link>
        </div>
      </div>
    </div>
  );
};

export default Returns;