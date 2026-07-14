import { useEffect, useState } from "react";
import { useParams, Link, useNavigate } from "react-router-dom";
import {
  ShoppingCart,
  Star,
  Package,
  Truck,
  Shield,
  RefreshCw,
  Heart,
  Share2,
  Minus,
  Plus,
  Tag,
  CheckCircle,
  XCircle,
  Send,
  ChevronLeft,
  ChevronRight,
  ArrowLeft,
  MessageSquare,
} from "lucide-react";
import { productApi } from "@/api/product.api";
import { reviewApi } from "@/api/review.api";
import { wishlistApi } from "@/api/wishlist.api";
import { useCart } from "@/hooks/useCart";
import { useAuth } from "@/hooks/useAuth";
import type {
  ProductDetail as ProductDetailType,
  Review,
  ReviewStatistics,
  AddReviewRequest,
} from "@/types";
import Button from "@/components/ui/Button";
import Badge from "@/components/ui/Badge";
import Spinner from "@/components/ui/Spinner";
import { formatPrice } from "@/utils/formatPrice";
import { cn } from "@/utils/cn";
import toast from "react-hot-toast";

const RATING_LABELS = ["", "Poor", "Fair", "Good", "Very Good", "Excellent"];

const ProductDetail = () => {
  const { id }     = useParams<{ id: string }>();
  const navigate   = useNavigate();
  const { addToCart, isLoading: cartLoading } = useCart();
  const { isAuthenticated, user }             = useAuth();

  // ── Product State ──────────────────────────────────────────────────────────
  const [product, setProduct]                   = useState<ProductDetailType | null>(null);
  const [selectedImageIdx, setSelectedImageIdx] = useState(0);
  const [isLoading, setIsLoading]               = useState(true);
  const [quantity, setQuantity]                 = useState(1);
  const [activeTab, setActiveTab]               = useState<"description" | "reviews">("description");

  // ── Wishlist State ─────────────────────────────────────────────────────────
  const [isWishlisted, setIsWishlisted] = useState(false);
  const [wishLoading, setWishLoading]   = useState(false);

  // ── Review State ───────────────────────────────────────────────────────────
  const [reviews, setReviews]       = useState<Review[]>([]);
  const [stats, setStats]           = useState<ReviewStatistics>({
    averageRating: 0,
    totalReviews:  0,
    fiveStar:      0,
    fourStar:      0,
    threeStar:     0,
    twoStar:       0,
    oneStar:       0,
  });
  const [reviewsLoading, setReviewsLoading]     = useState(false);
  const [showReviewForm, setShowReviewForm]     = useState(false);
  const [reviewRating, setReviewRating]         = useState(5);
  const [reviewHover, setReviewHover]           = useState(0);
  const [reviewComment, setReviewComment]       = useState("");
  const [submittingReview, setSubmittingReview] = useState(false);

  // ── Load Product ───────────────────────────────────────────────────────────
  useEffect(() => {
    if (!id) return;
    const load = async () => {
      setIsLoading(true);
      try {
        const data = await productApi.getById(id);
        setProduct(data);
      } catch {
        toast.error("Failed to load product");
      } finally {
        setIsLoading(false);
      }
    };
    load();
  }, [id]);

  // ── Load Reviews ───────────────────────────────────────────────────────────
  const loadReviews = async () => {
    if (!id) return;
    setReviewsLoading(true);
    try {
      const data = await reviewApi.getProductReviews(id);
      setStats(data.statistics);
      setReviews(data.reviews);
    } catch {
      setReviews([]);
    } finally {
      setReviewsLoading(false);
    }
  };

  useEffect(() => {
    loadReviews();
  }, [id]);

  // ── Check Wishlist ─────────────────────────────────────────────────────────
  useEffect(() => {
    if (!id || !isAuthenticated) return;
    const checkWishlist = async () => {
      try {
        const items = await wishlistApi.getWishlist();
        const found = items.some((item) => item.productId === id);
        setIsWishlisted(found);
      } catch {
        // ignore
      }
    };
    checkWishlist();
  }, [id, isAuthenticated]);

  // ── Handlers ───────────────────────────────────────────────────────────────
  const handleAddToCart = async () => {
    if (!isAuthenticated) {
      toast.error("Please login first");
      navigate("/login", { state: { from: `/products/${id}` } });
      return;
    }
    if (!product) return;
    await addToCart(product.id, quantity);
    toast.success(`${product.name} added to cart!`);
  };

  const handleWishlist = async () => {
    if (!isAuthenticated) {
      toast.error("Please login to use wishlist");
      navigate("/login", { state: { from: `/products/${id}` } });
      return;
    }
    if (!product) return;

    setWishLoading(true);
    try {
      if (isWishlisted) {
        await wishlistApi.removeFromWishlist(product.id);
        setIsWishlisted(false);
        toast.success("Removed from wishlist");
      } else {
        await wishlistApi.addToWishlist({ productId: product.id });
        setIsWishlisted(true);
        toast.success("Added to wishlist ❤️");
      }
    } catch (err) {
      toast.error((err as Error).message || "Failed");
    } finally {
      setWishLoading(false);
    }
  };

  const handleShare = () => {
    navigator.clipboard.writeText(window.location.href);
    toast.success("Link copied!");
  };

  const handleSubmitReview = async () => {
    if (!id) return;
    if (!isAuthenticated) {
      toast.error("Please login to submit a review");
      navigate("/login", { state: { from: `/products/${id}` } });
      return;
    }
    if (reviewRating < 1 || reviewRating > 5) {
      toast.error("Please select a rating between 1-5");
      return;
    }

    setSubmittingReview(true);
    try {
      const payload: AddReviewRequest = {
        rating:  reviewRating,
        comment: reviewComment.trim() || null,
      };
      await reviewApi.addReview(id, payload);
      toast.success("Review submitted successfully!");

      setShowReviewForm(false);
      setReviewComment("");
      setReviewRating(5);
      setReviewHover(0);

      await loadReviews();
      setActiveTab("reviews");
    } catch (err) {
      toast.error((err as Error).message || "Failed to submit review");
    } finally {
      setSubmittingReview(false);
    }
  };

  // ── Computed ───────────────────────────────────────────────────────────────
  const images       = product?.images ?? [];
  const currentImage = images.length > 0 ? images[selectedImageIdx]?.imageUrl : null;
  const placeholder  = product
    ? `https://placehold.co/600x600/e2e8f0/64748b?text=${encodeURIComponent(product.name.charAt(0))}`
    : "https://placehold.co/600x600/e2e8f0/64748b?text=?";

  const hasDiscount =
    product?.costPrice != null && product.costPrice > product.basePrice;

  const discountPct = hasDiscount
    ? Math.round(((product!.costPrice! - product!.basePrice) / product!.costPrice!) * 100)
    : 0;

  const hasUserReviewed = reviews.some(
    (r) => r.userId === user?.id || r.userId === user?.email
  );

  const ratingBreakdown = [
    { stars: 5, count: stats.fiveStar  },
    { stars: 4, count: stats.fourStar  },
    { stars: 3, count: stats.threeStar },
    { stars: 2, count: stats.twoStar   },
    { stars: 1, count: stats.oneStar   },
  ];

  // ── Loading ────────────────────────────────────────────────────────────────
  if (isLoading) {
    return (
      <div className="flex justify-center items-center py-32">
        <Spinner size="lg" className="text-primary-600" />
      </div>
    );
  }

  if (!product) {
    return (
      <div className="max-w-7xl mx-auto px-4 py-20 text-center">
        <Package className="w-16 h-16 text-gray-200 mx-auto mb-4" />
        <h2 className="text-2xl font-semibold text-gray-700 mb-3">Product not found</h2>
        <Link to="/products"><Button>Back to Products</Button></Link>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 py-8">

      {/* ── Breadcrumb ────────────────────────────────────────────────────────── */}
      <nav className="flex items-center gap-2 text-sm text-gray-500 mb-6 flex-wrap">
        <Link to="/" className="hover:text-primary-600 transition">Home</Link>
        <span className="text-gray-300">/</span>
        <Link to="/products" className="hover:text-primary-600 transition">Products</Link>
        <span className="text-gray-300">/</span>
        <span className="text-gray-400">{product.category.name}</span>
        <span className="text-gray-300">/</span>
        <span className="text-gray-700 font-medium truncate max-w-xs">{product.name}</span>
      </nav>

      <button
        onClick={() => navigate(-1)}
        className="flex items-center gap-2 text-sm text-gray-500 hover:text-primary-600 mb-6 transition"
      >
        <ArrowLeft className="w-4 h-4" /> Back
      </button>

      {/* ── Main Grid ─────────────────────────────────────────────────────────── */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-10 mb-16">

        {/* ── Left: Images ────────────────────────────────────────────────────── */}
        <div className="space-y-4">
          <div className="relative bg-gray-50 rounded-2xl overflow-hidden aspect-square group border border-gray-100">
            <img
              src={currentImage || placeholder}
              alt={product.name}
              onError={(e) => { (e.target as HTMLImageElement).src = placeholder; }}
              className="w-full h-full object-cover transition-transform duration-300 group-hover:scale-105"
            />

            {!product.isActive && (
              <div className="absolute inset-0 bg-black/30 flex items-center justify-center">
                <Badge variant="danger">Unavailable</Badge>
              </div>
            )}

            {hasDiscount && (
              <div className="absolute top-4 left-4">
                <span className="bg-green-500 text-white text-xs font-bold px-2.5 py-1 rounded-full shadow-sm">
                  {discountPct}% OFF
                </span>
              </div>
            )}

            {/* ✅ Wishlist on Image */}
            <button
              onClick={handleWishlist}
              disabled={wishLoading}
              className={cn(
                "absolute top-4 right-4 p-2 rounded-full shadow-md transition",
                wishLoading && "opacity-50",
                isWishlisted
                  ? "bg-red-50 opacity-100"
                  : "bg-white/90 opacity-0 group-hover:opacity-100"
              )}
            >
              <Heart className={cn("w-5 h-5", isWishlisted ? "fill-red-500 text-red-500" : "text-gray-400")} />
            </button>

            {images.length > 1 && (
              <>
                <button onClick={() => setSelectedImageIdx((selectedImageIdx - 1 + images.length) % images.length)} className="absolute left-3 top-1/2 -translate-y-1/2 p-2 bg-white/90 backdrop-blur rounded-full shadow-md hover:bg-white transition opacity-0 group-hover:opacity-100">
                  <ChevronLeft className="w-4 h-4 text-gray-700" />
                </button>
                <button onClick={() => setSelectedImageIdx((selectedImageIdx + 1) % images.length)} className="absolute right-3 top-1/2 -translate-y-1/2 p-2 bg-white/90 backdrop-blur rounded-full shadow-md hover:bg-white transition opacity-0 group-hover:opacity-100">
                  <ChevronRight className="w-4 h-4 text-gray-700" />
                </button>
                <div className="absolute bottom-3 right-3 bg-black/50 text-white text-xs px-2 py-1 rounded-full">
                  {selectedImageIdx + 1}/{images.length}
                </div>
              </>
            )}
          </div>

          {images.length > 1 && (
            <div className="flex gap-2 overflow-x-auto pb-1">
              {images.map((img, idx) => (
                <button key={img.id} onClick={() => setSelectedImageIdx(idx)} className={cn("w-16 h-16 rounded-lg overflow-hidden border-2 shrink-0 transition", idx === selectedImageIdx ? "border-primary-600 shadow-sm" : "border-gray-200 hover:border-gray-300")}>
                  <img src={img.imageUrl} alt="" className="w-full h-full object-cover" />
                </button>
              ))}
            </div>
          )}
        </div>

        {/* ── Right: Details ───────────────────────────────────────────────────── */}
        <div className="flex flex-col">

          <div className="flex items-center gap-3 mb-3">
            <span className="text-sm font-bold text-primary-600 bg-primary-50 px-3 py-1 rounded-full">{product.brand.name}</span>
            <span className="inline-flex items-center gap-1 text-sm text-gray-500">
              <Tag className="w-3.5 h-3.5" />{product.category.name}
            </span>
          </div>

          <h1 className="text-3xl font-bold text-gray-900 mb-3 leading-tight">{product.name}</h1>

          {/* Rating */}
          <div className="flex items-center gap-3 mb-4">
            <div className="flex items-center gap-0.5">
              {Array.from({ length: 5 }, (_, i) => (
                <Star key={i} className={cn("w-5 h-5", i < Math.round(stats.averageRating) ? "fill-yellow-400 text-yellow-400" : "text-gray-200")} />
              ))}
            </div>
            {stats.totalReviews > 0 ? (
              <>
                <span className="text-sm font-bold text-gray-800">{stats.averageRating.toFixed(1)}</span>
                <button onClick={() => setActiveTab("reviews")} className="text-sm text-primary-600 hover:underline">
                  {stats.totalReviews} review{stats.totalReviews !== 1 ? "s" : ""}
                </button>
              </>
            ) : (
              <button onClick={() => { setActiveTab("reviews"); if (isAuthenticated) setShowReviewForm(true); }} className="text-sm text-gray-400 hover:text-primary-600 transition">
                No reviews – Be the first!
              </button>
            )}
          </div>

          <p className="text-xs text-gray-400 mb-5 font-mono">SKU: {product.sku}</p>

          {/* Price */}
          <div className="bg-gray-50 rounded-xl p-4 mb-6">
            <div className="flex items-baseline gap-3 flex-wrap">
              <span className="text-4xl font-bold text-gray-900">{formatPrice(product.basePrice)}</span>
              {hasDiscount && (
                <>
                  <span className="text-lg text-gray-400 line-through">{formatPrice(product.costPrice!)}</span>
                  <span className="text-sm font-semibold text-green-600 bg-green-50 px-2.5 py-1 rounded-full">{discountPct}% off</span>
                </>
              )}
            </div>
            {hasDiscount && (
              <p className="text-xs text-gray-500 mt-1">You save <span className="font-semibold text-green-600">{formatPrice(product.costPrice! - product.basePrice)}</span></p>
            )}
            <p className="text-xs text-gray-400 mt-2">Inclusive of all taxes</p>
          </div>

          <p className="text-gray-600 leading-relaxed mb-5">{product.description}</p>

          {/* Stock */}
          <div className="flex items-center gap-2 mb-5">
            {product.isActive ? (
              <div className="flex items-center gap-1.5 text-green-600"><CheckCircle className="w-4 h-4" /><span className="text-sm font-medium">In Stock</span></div>
            ) : (
              <div className="flex items-center gap-1.5 text-red-500"><XCircle className="w-4 h-4" /><span className="text-sm font-medium">Out of Stock</span></div>
            )}
          </div>

          <hr className="border-gray-100 mb-5" />

          {/* Quantity */}
          <div className="flex items-center gap-4 mb-6">
            <label className="text-sm font-medium text-gray-700 shrink-0">Quantity:</label>
            <div className="flex items-center border border-gray-200 rounded-xl overflow-hidden">
              <button onClick={() => setQuantity(Math.max(1, quantity - 1))} disabled={quantity <= 1} className="px-3 py-2.5 text-gray-600 hover:bg-gray-50 disabled:opacity-40 transition"><Minus className="w-4 h-4" /></button>
              <span className="px-5 py-2.5 font-bold text-gray-900 min-w-[3.5rem] text-center border-x border-gray-200">{quantity}</span>
              <button onClick={() => setQuantity(quantity + 1)} className="px-3 py-2.5 text-gray-600 hover:bg-gray-50 transition"><Plus className="w-4 h-4" /></button>
            </div>
            {quantity > 1 && <span className="text-sm text-gray-500">= {formatPrice(product.basePrice * quantity)}</span>}
          </div>

          {/* ✅ Action Buttons with Wishlist API */}
          <div className="flex gap-3 mb-6">
            <Button size="lg" onClick={handleAddToCart} isLoading={cartLoading} disabled={!product.isActive} className="flex-1">
              <ShoppingCart className="w-5 h-5" />{product.isActive ? "Add to Cart" : "Unavailable"}
            </Button>

            <button
              onClick={handleWishlist}
              disabled={wishLoading}
              title={isWishlisted ? "Remove from wishlist" : "Add to wishlist"}
              className={cn(
                "p-3 border rounded-xl transition",
                wishLoading && "opacity-50",
                isWishlisted
                  ? "border-red-200 bg-red-50 text-red-500"
                  : "border-gray-200 text-gray-500 hover:border-red-200 hover:bg-red-50 hover:text-red-500"
              )}
            >
              <Heart className={cn("w-5 h-5", isWishlisted && "fill-red-500")} />
            </button>

            <button onClick={handleShare} className="p-3 border border-gray-200 rounded-xl text-gray-500 hover:border-primary-200 hover:text-primary-600 transition">
              <Share2 className="w-5 h-5" />
            </button>
          </div>

          {/* Trust */}
          <div className="grid grid-cols-3 gap-3">
            {[
              { icon: Truck, label: "Free Delivery", sub: "Orders over ₹500" },
              { icon: Shield, label: "Secure Pay", sub: "100% Safe" },
              { icon: RefreshCw, label: "Easy Returns", sub: "30-Day Policy" },
            ].map(({ icon: Icon, label, sub }) => (
              <div key={label} className="flex flex-col items-center text-center p-3 bg-gray-50 rounded-xl border border-gray-100">
                <div className="w-8 h-8 bg-primary-100 rounded-full flex items-center justify-center mb-2"><Icon className="w-4 h-4 text-primary-600" /></div>
                <p className="text-xs font-semibold text-gray-700">{label}</p>
                <p className="text-xs text-gray-400 mt-0.5">{sub}</p>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* ── Tabs ──────────────────────────────────────────────────────────────── */}
      <div className="border-t border-gray-100 pt-10">
        <div className="flex gap-1 border-b border-gray-200 mb-8">
          {(["description", "reviews"] as const).map((tab) => (
            <button
              key={tab}
              onClick={() => setActiveTab(tab)}
              className={cn(
                "px-6 py-3 text-sm font-medium border-b-2 transition -mb-px flex items-center gap-2",
                activeTab === tab ? "border-primary-600 text-primary-600" : "border-transparent text-gray-500 hover:text-gray-700"
              )}
            >
              {tab === "description" ? "Description" : (
                <>
                  <MessageSquare className="w-4 h-4" />
                  Reviews
                  {stats.totalReviews > 0 && (
                    <span className={cn("px-2 py-0.5 text-xs rounded-full font-medium", activeTab === "reviews" ? "bg-primary-100 text-primary-700" : "bg-gray-100 text-gray-500")}>
                      {stats.totalReviews}
                    </span>
                  )}
                </>
              )}
            </button>
          ))}
        </div>

        {/* ── Description Tab ──────────────────────────────────────────────────── */}
        {activeTab === "description" && (
          <div className="max-w-3xl space-y-6">
            <div>
              <h3 className="text-xl font-semibold text-gray-800 mb-3">About this product</h3>
              <p className="text-gray-600 leading-relaxed">{product.description}</p>
            </div>
            <div className="bg-gray-50 rounded-xl p-6 border border-gray-100">
              <h4 className="text-sm font-bold text-gray-700 mb-4 uppercase tracking-wider">Specifications</h4>
              <div className="divide-y divide-gray-100">
                {[
                  { label: "Product Name", value: product.name },
                  { label: "SKU",          value: product.sku },
                  { label: "Brand",        value: product.brand.name },
                  { label: "Category",     value: product.category.name },
                  { label: "Price",        value: formatPrice(product.basePrice) },
                  { label: "Status",       value: product.isActive ? "In Stock" : "Out of Stock" },
                  { label: "Rating",       value: stats.totalReviews > 0 ? `${stats.averageRating.toFixed(1)} (${stats.totalReviews} reviews)` : "No reviews" },
                ].map(({ label, value }) => (
                  <div key={label} className="flex justify-between py-3 first:pt-0 last:pb-0">
                    <span className="text-sm text-gray-500">{label}</span>
                    <span className="text-sm font-medium text-gray-800">{value}</span>
                  </div>
                ))}
              </div>
            </div>
          </div>
        )}

        {/* ── Reviews Tab ──────────────────────────────────────────────────────── */}
        {activeTab === "reviews" && (
          <div className="max-w-3xl">

            {/* Rating Summary */}
            <div className="flex flex-col sm:flex-row items-center gap-8 mb-8 p-6 bg-gradient-to-r from-gray-50 to-white rounded-xl border border-gray-100">
              <div className="text-center shrink-0">
                <p className="text-6xl font-bold text-gray-900">
                  {stats.totalReviews > 0 ? stats.averageRating.toFixed(1) : "–"}
                </p>
                <div className="flex items-center gap-0.5 mt-2 justify-center">
                  {Array.from({ length: 5 }, (_, i) => (
                    <Star key={i} className={cn("w-5 h-5", i < Math.round(stats.averageRating) ? "fill-yellow-400 text-yellow-400" : "text-gray-200")} />
                  ))}
                </div>
                <p className="text-sm text-gray-500 mt-1">
                  {stats.totalReviews > 0 ? `${stats.totalReviews} review${stats.totalReviews !== 1 ? "s" : ""}` : "No reviews yet"}
                </p>
              </div>
              <div className="flex-1 w-full space-y-2">
                {ratingBreakdown.map(({ stars, count }) => {
                  const pct = stats.totalReviews > 0 ? (count / stats.totalReviews) * 100 : 0;
                  return (
                    <div key={stars} className="flex items-center gap-3">
                      <div className="flex items-center gap-1 shrink-0 w-14">
                        <span className="text-sm font-medium text-gray-600">{stars}</span>
                        <Star className="w-3.5 h-3.5 fill-yellow-400 text-yellow-400" />
                      </div>
                      <div className="flex-1 h-2.5 bg-gray-200 rounded-full overflow-hidden">
                        <div className="h-full bg-yellow-400 rounded-full transition-all duration-700" style={{ width: `${pct}%` }} />
                      </div>
                      <span className="text-sm text-gray-400 w-8 text-right">{count}</span>
                    </div>
                  );
                })}
              </div>
            </div>

            {/* Write Review CTA */}
            {isAuthenticated && !showReviewForm && !hasUserReviewed && (
              <button onClick={() => setShowReviewForm(true)} className="mb-8 flex items-center gap-2 px-6 py-3 bg-primary-600 text-white text-sm font-medium rounded-xl hover:bg-primary-700 transition shadow-sm">
                <Star className="w-4 h-4" /> Write a Review
              </button>
            )}

            {isAuthenticated && hasUserReviewed && !showReviewForm && (
              <div className="mb-8 p-4 bg-green-50 border border-green-200 rounded-xl">
                <div className="flex items-center gap-2 text-green-700">
                  <CheckCircle className="w-4 h-4" />
                  <p className="text-sm font-medium">You've already reviewed this product</p>
                </div>
              </div>
            )}

            {!isAuthenticated && (
              <div className="mb-8 p-4 bg-blue-50 border border-blue-100 rounded-xl">
                <p className="text-sm text-blue-700">
                  <Link to="/login" state={{ from: `/products/${id}` }} className="font-semibold underline">Login</Link>{" "}to write a review
                </p>
              </div>
            )}

            {/* Review Form */}
            {showReviewForm && (
              <div className="mb-8 p-6 bg-white border border-gray-200 rounded-2xl shadow-sm">
                <h4 className="text-lg font-bold text-gray-800 mb-6">Your Review</h4>
                <div className="mb-6">
                  <label className="text-sm font-medium text-gray-700 mb-3 block">How would you rate this product?</label>
                  <div className="flex items-center gap-2">
                    <div className="flex items-center gap-1">
                      {[1, 2, 3, 4, 5].map((star) => (
                        <button key={star} onClick={() => setReviewRating(star)} onMouseEnter={() => setReviewHover(star)} onMouseLeave={() => setReviewHover(0)} className="p-1 hover:scale-125 transition-transform">
                          <Star className={cn("w-8 h-8 transition-colors duration-150", star <= (reviewHover || reviewRating) ? "fill-yellow-400 text-yellow-400" : "text-gray-200 hover:text-yellow-300")} />
                        </button>
                      ))}
                    </div>
                    <span className="ml-3 text-sm font-medium text-gray-600 bg-gray-100 px-3 py-1 rounded-full">
                      {RATING_LABELS[reviewHover || reviewRating]}
                    </span>
                  </div>
                </div>
                <div className="mb-6">
                  <label className="text-sm font-medium text-gray-700 mb-2 block">Your Review (optional)</label>
                  <textarea value={reviewComment} onChange={(e) => setReviewComment(e.target.value)} placeholder="What did you like or dislike about this product?" rows={4} maxLength={500} className="w-full px-4 py-3 border border-gray-200 rounded-xl text-sm focus:outline-none focus:ring-2 focus:ring-primary-500/20 focus:border-primary-400 resize-none transition" />
                  <div className="flex justify-end mt-1"><span className="text-xs text-gray-400">{reviewComment.length}/500</span></div>
                </div>
                <div className="flex items-center gap-3">
                  <button onClick={handleSubmitReview} disabled={submittingReview || reviewRating < 1} className="flex items-center gap-2 px-6 py-2.5 bg-primary-600 text-white text-sm font-medium rounded-xl hover:bg-primary-700 disabled:opacity-50 transition shadow-sm">
                    {submittingReview ? <Spinner size="sm" /> : <Send className="w-4 h-4" />}
                    Submit Review
                  </button>
                  <button onClick={() => { setShowReviewForm(false); setReviewComment(""); setReviewRating(5); setReviewHover(0); }} className="px-6 py-2.5 border border-gray-200 text-gray-700 text-sm font-medium rounded-xl hover:bg-gray-50 transition">
                    Cancel
                  </button>
                </div>
              </div>
            )}

            {/* Reviews List */}
            {reviewsLoading ? (
              <div className="flex justify-center py-12"><Spinner className="text-primary-600" /></div>
            ) : reviews.length === 0 && stats.totalReviews === 0 ? (
              <div className="text-center py-16 bg-gray-50 rounded-xl border border-gray-100">
                <Star className="w-14 h-14 text-gray-200 mx-auto mb-3" />
                <p className="text-gray-600 font-semibold text-lg">No reviews yet</p>
                <p className="text-sm text-gray-400 mt-1 mb-4">Be the first to share your experience</p>
                {isAuthenticated && !hasUserReviewed && (
                  <button onClick={() => setShowReviewForm(true)} className="px-5 py-2 bg-primary-600 text-white text-sm font-medium rounded-xl hover:bg-primary-700 transition">Write a Review</button>
                )}
              </div>
            ) : reviews.length === 0 && stats.totalReviews > 0 ? (
              <div className="text-center py-12 bg-yellow-50 border border-yellow-200 rounded-xl">
                <MessageSquare className="w-12 h-12 text-yellow-400 mx-auto mb-3" />
                <p className="text-yellow-700 font-semibold">{stats.totalReviews} review{stats.totalReviews !== 1 ? "s" : ""} pending approval</p>
                <p className="text-sm text-yellow-600 mt-1">Reviews will appear after admin approval</p>
              </div>
            ) : (
              <div className="space-y-4">
                <h3 className="text-sm font-semibold text-gray-700 mb-2">
                  All Reviews ({reviews.length})
                  {stats.totalReviews > reviews.length && (
                    <span className="text-xs text-yellow-600 ml-2">({stats.totalReviews - reviews.length} pending)</span>
                  )}
                </h3>
                {reviews.map((review) => (
                  <div key={review.id} className="p-5 bg-white border border-gray-100 rounded-xl hover:shadow-sm transition">
                    <div className="flex items-start justify-between mb-3">
                      <div className="flex items-center gap-3">
                        <div className="w-10 h-10 bg-gradient-to-br from-primary-400 to-primary-600 text-white rounded-full flex items-center justify-center text-sm font-bold shrink-0">
                          {(review.userName || review.userId || "U").charAt(0).toUpperCase()}
                        </div>
                        <div>
                          <p className="text-sm font-semibold text-gray-800">{review.userName || "Anonymous User"}</p>
                          <div className="flex items-center gap-1 mt-0.5">
                            {Array.from({ length: 5 }, (_, i) => (
                              <Star key={i} className={cn("w-3.5 h-3.5", i < review.rating ? "fill-yellow-400 text-yellow-400" : "text-gray-200")} />
                            ))}
                            <span className="text-xs text-gray-500 ml-1">{RATING_LABELS[review.rating]}</span>
                          </div>
                        </div>
                      </div>
                      <span className="text-xs text-gray-400 shrink-0">
                        {new Date(review.createdAt).toLocaleDateString("en-IN", { year: "numeric", month: "short", day: "numeric" })}
                      </span>
                    </div>
                    {review.comment && (
                      <p className="text-sm text-gray-600 leading-relaxed mt-2 pl-[52px]">{review.comment}</p>
                    )}
                    {review.isApproved && (
                      <div className="flex items-center gap-1 mt-3 pl-[52px]">
                        <CheckCircle className="w-3.5 h-3.5 text-green-500" />
                        <span className="text-xs text-green-600 font-medium">Verified Purchase</span>
                      </div>
                    )}
                  </div>
                ))}
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
};

export default ProductDetail;