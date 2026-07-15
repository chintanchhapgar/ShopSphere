import { useState } from "react";
import { ChevronDown, HelpCircle, Search } from "lucide-react";
import { cn } from "@/utils/cn";
import { APP_NAME } from "@/utils/constants";

interface FAQItem {
  question: string;
  answer: string;
  category: string;
}

const FAQS: FAQItem[] = [
  // Orders
  {
    category: "Orders",
    question: "How do I place an order?",
    answer: "Browse our products, add items to your cart, proceed to checkout, select a delivery address, choose a payment method, and place your order. You'll receive an order confirmation via email.",
  },
  {
    category: "Orders",
    question: "Can I cancel my order?",
    answer: "Yes, you can cancel your order if it's still in 'Pending' or 'Confirmed' status. Go to My Orders, find the order, and click 'Cancel Order'. Once the order is being processed or shipped, it cannot be cancelled.",
  },
  {
    category: "Orders",
    question: "How can I track my order?",
    answer: "Go to My Orders page and click on any order to see its current status and progress. You'll see a timeline showing each stage from Order Placed to Delivered.",
  },
  {
    category: "Orders",
    question: "What payment methods do you accept?",
    answer: "We accept Credit Cards (Visa, Mastercard, RuPay), Debit Cards, PayPal, and Cash on Delivery (COD). All online payments are processed securely with 256-bit SSL encryption.",
  },

  // Shipping
  {
    category: "Shipping",
    question: "How much does shipping cost?",
    answer: "Shipping is FREE on all orders above ₹500. For orders below ₹500, a flat shipping fee of ₹49 is charged.",
  },
  {
    category: "Shipping",
    question: "How long does delivery take?",
    answer: "Standard delivery takes 3-7 business days depending on your location. Metro cities usually receive orders within 3-4 days. You can track your order status in real-time from the Orders page.",
  },
  {
    category: "Shipping",
    question: "Do you deliver to all locations?",
    answer: "We currently deliver across India. We support all major cities and most pin codes. Enter your delivery address during checkout to confirm availability.",
  },

  // Returns
  {
    category: "Returns",
    question: "What is your return policy?",
    answer: "We offer a 30-day return policy on most products. Items must be unused, in original packaging, and in the same condition as received. Some categories like personal care and food items are non-returnable.",
  },
  {
    category: "Returns",
    question: "How do I return a product?",
    answer: "Go to My Orders, find the order with the item you want to return, and click 'Return'. Fill in the reason for return and our team will arrange a pickup. Refunds are processed within 5-7 business days after we receive the item.",
  },
  {
    category: "Returns",
    question: "When will I receive my refund?",
    answer: "Refunds are processed within 5-7 business days after the returned item is received and inspected. The refund will be credited to your original payment method. COD refunds are processed via bank transfer.",
  },

  // Account
  {
    category: "Account",
    question: "How do I create an account?",
    answer: "Click 'Sign Up' on the top right, enter your name, email, and password. You'll receive a verification email - click the link to activate your account.",
  },
  {
    category: "Account",
    question: "I forgot my password. How do I reset it?",
    answer: "Click 'Forgot Password?' on the login page, enter your registered email, and we'll send you a password reset link. Click the link in your email to set a new password.",
  },
  {
    category: "Account",
    question: "How do I update my profile or address?",
    answer: "Go to My Profile page where you can view your information. To manage addresses, go to the Addresses tab where you can add, edit, delete, and set a default delivery address.",
  },

  // Products
  {
    category: "Products",
    question: "Are all products genuine?",
    answer: "Yes, we only sell 100% authentic and genuine products sourced directly from authorized brands and distributors. Every product comes with a manufacturer warranty where applicable.",
  },
  {
    category: "Products",
    question: "Can I write a review for a product?",
    answer: "Yes! After purchasing a product, you can write a review on the product detail page. Your review will be published after admin approval to maintain quality.",
  },
];

const CATEGORIES = ["All", ...Array.from(new Set(FAQS.map((f) => f.category)))];

const FAQ = () => {
  const [openIndex, setOpenIndex]       = useState<number | null>(null);
  const [searchQuery, setSearchQuery]   = useState("");
  const [activeCategory, setActiveCategory] = useState("All");

  const filtered = FAQS.filter((faq) => {
    const matchSearch =
      !searchQuery ||
      faq.question.toLowerCase().includes(searchQuery.toLowerCase()) ||
      faq.answer.toLowerCase().includes(searchQuery.toLowerCase());

    const matchCategory =
      activeCategory === "All" || faq.category === activeCategory;

    return matchSearch && matchCategory;
  });

  return (
    <div className="max-w-4xl mx-auto px-4 py-12">

      {/* Header */}
      <div className="text-center mb-12">
        <div className="inline-flex items-center justify-center w-14 h-14 bg-primary-100 text-primary-600 rounded-2xl mb-4">
          <HelpCircle className="w-7 h-7" />
        </div>
        <h1 className="text-4xl font-bold text-gray-900 mb-3">
          Frequently Asked Questions
        </h1>
        <p className="text-gray-500 max-w-xl mx-auto">
          Find quick answers to common questions about {APP_NAME}
        </p>
      </div>

      {/* Search */}
      <div className="relative mb-8">
        <Search className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
        <input
          type="text"
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          placeholder="Search questions..."
          className="w-full pl-12 pr-4 py-3.5 border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-primary-500/20 focus:border-primary-400 transition"
        />
      </div>

      {/* Category Tabs */}
      <div className="flex flex-wrap gap-2 mb-8">
        {CATEGORIES.map((cat) => (
          <button
            key={cat}
            onClick={() => setActiveCategory(cat)}
            className={cn(
              "px-4 py-2 rounded-full text-sm font-medium transition",
              activeCategory === cat
                ? "bg-primary-600 text-white"
                : "bg-gray-100 text-gray-600 hover:bg-gray-200"
            )}
          >
            {cat}
          </button>
        ))}
      </div>

      {/* FAQ List */}
      {filtered.length === 0 ? (
        <div className="text-center py-16 bg-gray-50 rounded-xl">
          <HelpCircle className="w-12 h-12 text-gray-200 mx-auto mb-3" />
          <p className="text-gray-600 font-medium">No questions found</p>
          <p className="text-sm text-gray-400 mt-1">Try different search terms</p>
        </div>
      ) : (
        <div className="space-y-3">
          {filtered.map((faq, idx) => {
            const isOpen = openIndex === idx;
            return (
              <div
                key={idx}
                className={cn(
                  "bg-white rounded-xl border transition overflow-hidden",
                  isOpen ? "border-primary-200 shadow-sm" : "border-gray-100"
                )}
              >
                <button
                  onClick={() => setOpenIndex(isOpen ? null : idx)}
                  className="w-full flex items-start justify-between gap-4 p-5 text-left"
                >
                  <div className="flex items-start gap-3">
                    <span className="px-2 py-0.5 bg-gray-100 text-gray-500 text-xs font-medium rounded-full shrink-0 mt-0.5">
                      {faq.category}
                    </span>
                    <h3 className={cn(
                      "text-sm font-semibold transition",
                      isOpen ? "text-primary-600" : "text-gray-800"
                    )}>
                      {faq.question}
                    </h3>
                  </div>
                  <ChevronDown
                    className={cn(
                      "w-5 h-5 text-gray-400 shrink-0 transition-transform",
                      isOpen && "rotate-180 text-primary-600"
                    )}
                  />
                </button>
                {isOpen && (
                  <div className="px-5 pb-5 pl-[72px]">
                    <p className="text-sm text-gray-600 leading-relaxed">
                      {faq.answer}
                    </p>
                  </div>
                )}
              </div>
            );
          })}
        </div>
      )}

      {/* Still need help */}
      <div className="mt-12 p-8 bg-gradient-to-r from-primary-50 to-blue-50 rounded-2xl border border-primary-100 text-center">
        <h3 className="text-xl font-bold text-gray-900 mb-2">
          Still have questions?
        </h3>
        <p className="text-gray-600 mb-4">
          Can't find what you're looking for? Contact our support team.
        </p>
        <a
          href="/contact"
          className="inline-flex items-center gap-2 px-6 py-3 bg-primary-600 text-white font-medium rounded-xl hover:bg-primary-700 transition shadow-sm"
        >
          Contact Support
        </a>
      </div>
    </div>
  );
};

export default FAQ;