import { useState, useRef, useEffect } from "react";
import { Link } from "react-router-dom";
import {
  MessageSquare,
  X,
  Send,
  Sparkles,
  Loader,
  Bot,
  User as UserIcon,
  ShoppingCart,
  RefreshCw,
} from "lucide-react";
import { aiApi } from "@/api/ai.api";
import { useAuth } from "@/hooks/useAuth";
import { useCart } from "@/hooks/useCart";
import type { ChatMessage, ProductSuggestion } from "@/types";
import { formatPrice } from "@/utils/formatPrice";
import { cn } from "@/utils/cn";
import toast from "react-hot-toast";

const WELCOME_MESSAGE: ChatMessage = {
  role: "assistant",
  content: "👋 Hi! I'm your ShopSphere AI assistant. I can help you:\n\n• Find products\n• Recommend items\n• Answer questions about orders & shipping\n\nWhat are you looking for today?",
};

const SUGGESTED_PROMPTS = [
  "Find me a smartphone under ₹80,000",
  "Recommend a good laptop for work",
  "What running shoes do you have?",
  "Show me kitchen appliances",
];

const AIChatbot = () => {
  const { isAuthenticated } = useAuth();
  const { addToCart } = useCart();

  const [isOpen, setIsOpen]         = useState(false);
  const [input, setInput]           = useState("");
  const [messages, setMessages]     = useState<ChatMessage[]>([WELCOME_MESSAGE]);
  const [isTyping, setIsTyping]     = useState(false);
  const [unreadCount, setUnreadCount] = useState(0);

  const messagesEndRef = useRef<HTMLDivElement>(null);
  const inputRef       = useRef<HTMLInputElement>(null);

  // ── Auto-scroll to bottom ──────────────────────────────────────────────────
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages, isTyping]);

  // ── Focus input on open ────────────────────────────────────────────────────
  useEffect(() => {
    if (isOpen) {
      inputRef.current?.focus();
      setUnreadCount(0);
    }
  }, [isOpen]);

  // ── Send Message ───────────────────────────────────────────────────────────
  const sendMessage = async (messageText: string) => {
    if (!messageText.trim() || isTyping) return;

    if (!isAuthenticated) {
      toast.error("Please login to use AI assistant");
      return;
    }

    const userMessage: ChatMessage = {
      role: "user",
      content: messageText.trim(),
      timestamp: new Date().toISOString(),
    };

    setMessages((prev) => [...prev, userMessage]);
    setInput("");
    setIsTyping(true);

    try {
      // Send last 10 messages as history (excluding welcome)
      const history = messages
        .filter((m) => m !== WELCOME_MESSAGE)
        .slice(-10)
        .map((m) => ({ role: m.role, content: m.content }));

      const response = await aiApi.sendMessage({
        message: userMessage.content,
        history,
      });

      const assistantMessage: ChatMessage = {
        role:      "assistant",
        content:   response.message,
        products:  response.products || undefined,
        timestamp: new Date().toISOString(),
      };

      setMessages((prev) => [...prev, assistantMessage]);

      if (!isOpen) setUnreadCount((c) => c + 1);
    } catch (err) {
      toast.error((err as Error).message || "Failed to get response");

      setMessages((prev) => [...prev, {
        role: "assistant",
        content: "Sorry, I'm having trouble right now. Please try again.",
      }]);
    } finally {
      setIsTyping(false);
    }
  };

  // ── Add to Cart Handler ────────────────────────────────────────────────────
  const handleAddToCart = async (product: ProductSuggestion) => {
    try {
      await addToCart(product.id, 1);
      toast.success(`${product.name} added to cart!`);
    } catch (err) {
      toast.error((err as Error).message || "Failed");
    }
  };

  // ── Reset Chat ─────────────────────────────────────────────────────────────
  const resetChat = () => {
    if (window.confirm("Start a new conversation?")) {
      setMessages([WELCOME_MESSAGE]);
    }
  };

  return (
    <>
      {/* ── Floating Button ──────────────────────────────────────────────────── */}
      <button
        onClick={() => setIsOpen(!isOpen)}
        className={cn(
          "fixed bottom-6 right-6 z-50 w-14 h-14 rounded-full shadow-lg",
          "bg-gradient-to-br from-primary-500 to-primary-700 text-white",
          "flex items-center justify-center hover:scale-110 transition-all",
          "hover:shadow-xl active:scale-95"
        )}
        title="AI Shopping Assistant"
      >
        {isOpen ? (
          <X className="w-6 h-6" />
        ) : (
          <>
            <MessageSquare className="w-6 h-6" />
            {unreadCount > 0 && (
              <span className="absolute -top-1 -right-1 bg-red-500 text-white text-xs w-5 h-5 rounded-full flex items-center justify-center font-bold animate-pulse">
                {unreadCount}
              </span>
            )}
            <span className="absolute -top-1 -right-1 bg-yellow-400 text-yellow-900 text-xs w-5 h-5 rounded-full flex items-center justify-center animate-pulse">
              <Sparkles className="w-3 h-3" />
            </span>
          </>
        )}
      </button>

      {/* ── Chat Window ──────────────────────────────────────────────────────── */}
      {isOpen && (
        <div className="fixed bottom-24 right-6 z-40 w-full max-w-sm bg-white rounded-2xl shadow-2xl border border-gray-200 flex flex-col overflow-hidden animate-fadeIn"
             style={{ height: "600px", maxHeight: "calc(100vh - 120px)" }}>

          {/* ── Header ────────────────────────────────────────────────────────── */}
          <div className="bg-gradient-to-r from-primary-600 to-primary-800 text-white p-4 flex items-center justify-between">
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 bg-white/20 backdrop-blur rounded-full flex items-center justify-center">
                <Bot className="w-5 h-5" />
              </div>
              <div>
                <h3 className="font-bold text-sm flex items-center gap-1">
                  AI Assistant
                  <Sparkles className="w-3 h-3 text-yellow-300" />
                </h3>
                <p className="text-xs text-primary-100 flex items-center gap-1">
                  <span className="w-2 h-2 bg-green-400 rounded-full animate-pulse"></span>
                  Online now
                </p>
              </div>
            </div>
            <button
              onClick={resetChat}
              className="p-1.5 hover:bg-white/20 rounded-lg transition"
              title="New Conversation"
            >
              <RefreshCw className="w-4 h-4" />
            </button>
          </div>

          {/* ── Messages ──────────────────────────────────────────────────────── */}
          <div className="flex-1 overflow-y-auto p-4 space-y-3 bg-gray-50">
            {messages.map((msg, idx) => (
              <div
                key={idx}
                className={cn(
                  "flex gap-2",
                  msg.role === "user" ? "justify-end" : "justify-start"
                )}
              >
                {msg.role === "assistant" && (
                  <div className="w-7 h-7 bg-gradient-to-br from-primary-500 to-primary-700 rounded-full flex items-center justify-center shrink-0">
                    <Bot className="w-4 h-4 text-white" />
                  </div>
                )}

                <div className={cn("max-w-[80%]")}>
                  {/* Message Bubble */}
                  <div
                    className={cn(
                      "px-4 py-2.5 rounded-2xl text-sm whitespace-pre-wrap",
                      msg.role === "user"
                        ? "bg-primary-600 text-white rounded-br-sm"
                        : "bg-white border border-gray-100 text-gray-800 rounded-bl-sm shadow-sm"
                    )}
                  >
                    {msg.content}
                  </div>

                  {/* Product Suggestions */}
                  {msg.products && msg.products.length > 0 && (
                    <div className="mt-2 space-y-2">
                      {msg.products.map((product) => {
                        const imgSrc = product.imageUrl ||
                          `https://placehold.co/60x60/e2e8f0/64748b?text=${encodeURIComponent(product.name.charAt(0))}`;

                        return (
                          <div
                            key={product.id}
                            className="bg-white border border-gray-200 rounded-xl p-3 hover:shadow-md transition"
                          >
                            <div className="flex gap-3">
                              <Link to={`/products/${product.id}`}>
                                <img
                                  src={imgSrc}
                                  alt={product.name}
                                  onError={(e) => {
                                    (e.target as HTMLImageElement).src =
                                      `https://placehold.co/60x60/e2e8f0/64748b?text=${product.name.charAt(0)}`;
                                  }}
                                  className="w-14 h-14 rounded-lg object-cover bg-gray-50"
                                />
                              </Link>
                              <div className="flex-1 min-w-0">
                                <Link
                                  to={`/products/${product.id}`}
                                  className="text-xs font-semibold text-gray-800 hover:text-primary-600 line-clamp-2"
                                >
                                  {product.name}
                                </Link>
                                <p className="text-xs text-gray-400 mt-0.5">
                                  {product.category}
                                </p>
                                <div className="flex items-center justify-between mt-1.5">
                                  <span className="text-sm font-bold text-primary-600">
                                    {formatPrice(product.price)}
                                  </span>
                                  <button
                                    onClick={() => handleAddToCart(product)}
                                    className="flex items-center gap-1 px-2 py-1 bg-primary-600 text-white text-xs font-medium rounded-lg hover:bg-primary-700 transition"
                                  >
                                    <ShoppingCart className="w-3 h-3" />
                                    Add
                                  </button>
                                </div>
                              </div>
                            </div>
                          </div>
                        );
                      })}
                    </div>
                  )}
                </div>

                {msg.role === "user" && (
                  <div className="w-7 h-7 bg-gray-300 rounded-full flex items-center justify-center shrink-0">
                    <UserIcon className="w-4 h-4 text-gray-600" />
                  </div>
                )}
              </div>
            ))}

            {/* Typing Indicator */}
            {isTyping && (
              <div className="flex gap-2 justify-start">
                <div className="w-7 h-7 bg-gradient-to-br from-primary-500 to-primary-700 rounded-full flex items-center justify-center shrink-0">
                  <Bot className="w-4 h-4 text-white" />
                </div>
                <div className="bg-white border border-gray-100 rounded-2xl rounded-bl-sm px-4 py-3 shadow-sm">
                  <div className="flex gap-1">
                    <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style={{ animationDelay: "0ms" }} />
                    <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style={{ animationDelay: "150ms" }} />
                    <div className="w-2 h-2 bg-gray-400 rounded-full animate-bounce" style={{ animationDelay: "300ms" }} />
                  </div>
                </div>
              </div>
            )}

            <div ref={messagesEndRef} />
          </div>

          {/* ── Suggested Prompts (show only initially) ──────────────────────── */}
          {messages.length === 1 && !isTyping && (
            <div className="px-4 py-2 border-t border-gray-100 bg-white">
              <p className="text-xs text-gray-400 mb-2">Try asking:</p>
              <div className="flex flex-wrap gap-1.5">
                {SUGGESTED_PROMPTS.map((prompt) => (
                  <button
                    key={prompt}
                    onClick={() => sendMessage(prompt)}
                    className="text-xs px-2.5 py-1 bg-primary-50 text-primary-600 rounded-full hover:bg-primary-100 transition"
                  >
                    {prompt}
                  </button>
                ))}
              </div>
            </div>
          )}

          {/* ── Input ─────────────────────────────────────────────────────────── */}
          <form
            onSubmit={(e) => {
              e.preventDefault();
              sendMessage(input);
            }}
            className="p-3 border-t border-gray-100 bg-white flex gap-2"
          >
            <input
              ref={inputRef}
              type="text"
              value={input}
              onChange={(e) => setInput(e.target.value)}
              placeholder={isAuthenticated ? "Ask me anything..." : "Login to chat..."}
              disabled={!isAuthenticated || isTyping}
              className="flex-1 px-4 py-2 border border-gray-200 rounded-full text-sm focus:outline-none focus:ring-2 focus:ring-primary-500/20 focus:border-primary-400 disabled:bg-gray-50 disabled:cursor-not-allowed"
            />
            <button
              type="submit"
              disabled={!input.trim() || isTyping || !isAuthenticated}
              className={cn(
                "w-10 h-10 rounded-full flex items-center justify-center transition",
                input.trim() && !isTyping && isAuthenticated
                  ? "bg-primary-600 text-white hover:bg-primary-700"
                  : "bg-gray-100 text-gray-400 cursor-not-allowed"
              )}
            >
              {isTyping ? (
                <Loader className="w-4 h-4 animate-spin" />
              ) : (
                <Send className="w-4 h-4" />
              )}
            </button>
          </form>
        </div>
      )}
    </>
  );
};

export default AIChatbot;