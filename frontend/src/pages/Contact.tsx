import { useState } from "react";
import {
  Mail,
  Phone,
  MapPin,
  Clock,
  Send,
  MessageSquare,
  CheckCircle,
} from "lucide-react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import Input from "@/components/ui/Input";
import Button from "@/components/ui/Button";
import { APP_NAME } from "@/utils/constants";
import toast from "react-hot-toast";

const schema = z.object({
  name:    z.string().min(2, "Name is required"),
  email:   z.string().email("Invalid email"),
  subject: z.string().min(3, "Subject is required"),
  message: z.string().min(10, "Message must be at least 10 characters"),
});

type ContactForm = z.infer<typeof schema>;

const Contact = () => {
  const [isSubmitted, setIsSubmitted] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    reset,
  } = useForm<ContactForm>({
    resolver: zodResolver(schema),
  });

  const onSubmit = async (data: ContactForm) => {
    // Simulate sending
    await new Promise((r) => setTimeout(r, 1000));
    console.log("Contact form:", data);
    toast.success("Message sent successfully!");
    setIsSubmitted(true);
    reset();
  };

  const contactInfo = [
    {
      icon: Mail,
      label: "Email",
      value: "support@shopsphere.com",
      link: "mailto:support@shopsphere.com",
      color: "bg-blue-50 text-blue-600",
    },
    {
      icon: Phone,
      label: "Phone",
      value: "+91 98765 43210",
      link: "tel:+919876543210",
      color: "bg-green-50 text-green-600",
    },
    {
      icon: MapPin,
      label: "Address",
      value: "Surat, Gujarat, India",
      link: null,
      color: "bg-purple-50 text-purple-600",
    },
    {
      icon: Clock,
      label: "Hours",
      value: "Mon-Sat, 9AM - 6PM IST",
      link: null,
      color: "bg-orange-50 text-orange-600",
    },
  ];

  return (
    <div className="max-w-6xl mx-auto px-4 py-12">

      {/* Header */}
      <div className="text-center mb-12">
        <div className="inline-flex items-center justify-center w-14 h-14 bg-primary-100 text-primary-600 rounded-2xl mb-4">
          <MessageSquare className="w-7 h-7" />
        </div>
        <h1 className="text-4xl font-bold text-gray-900 mb-3">Contact Us</h1>
        <p className="text-gray-500 max-w-xl mx-auto">
          Have a question or need help? We'd love to hear from you.
        </p>
      </div>

      {/* Contact Info Cards */}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-12">
        {contactInfo.map(({ icon: Icon, label, value, link, color }) => (
          <div
            key={label}
            className="bg-white rounded-xl border border-gray-100 shadow-sm p-5 text-center hover:shadow-md transition"
          >
            <div className={`w-12 h-12 rounded-xl ${color} flex items-center justify-center mx-auto mb-3`}>
              <Icon className="w-5 h-5" />
            </div>
            <h3 className="text-sm font-semibold text-gray-800 mb-1">{label}</h3>
            {link ? (
              <a href={link} className="text-sm text-primary-600 hover:underline">
                {value}
              </a>
            ) : (
              <p className="text-sm text-gray-500">{value}</p>
            )}
          </div>
        ))}
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">

        {/* Contact Form */}
        <div className="bg-white rounded-2xl border border-gray-100 shadow-sm p-8">
          {!isSubmitted ? (
            <>
              <h2 className="text-2xl font-bold text-gray-900 mb-2">
                Send us a Message
              </h2>
              <p className="text-gray-500 text-sm mb-6">
                Fill out the form and we'll get back to you within 24 hours
              </p>

              <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                  <Input
                    label="Your Name"
                    placeholder="John Doe"
                    error={errors.name?.message}
                    {...register("name")}
                  />
                  <Input
                    label="Email"
                    type="email"
                    placeholder="you@example.com"
                    error={errors.email?.message}
                    {...register("email")}
                  />
                </div>

                <Input
                  label="Subject"
                  placeholder="How can we help?"
                  error={errors.subject?.message}
                  {...register("subject")}
                />

                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1.5">
                    Message
                  </label>
                  <textarea
                    placeholder="Tell us more about your inquiry..."
                    rows={5}
                    className="w-full px-3 py-2.5 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500/20 focus:border-primary-500 resize-none transition"
                    {...register("message")}
                  />
                  {errors.message && (
                    <p className="mt-1 text-xs text-red-500">
                      {errors.message.message}
                    </p>
                  )}
                </div>

                <Button type="submit" size="lg" fullWidth isLoading={isSubmitting}>
                  <Send className="w-4 h-4" /> Send Message
                </Button>
              </form>
            </>
          ) : (
            <div className="text-center py-12">
              <div className="w-16 h-16 bg-green-50 rounded-full flex items-center justify-center mx-auto mb-4">
                <CheckCircle className="w-8 h-8 text-green-500" />
              </div>
              <h3 className="text-xl font-bold text-gray-900 mb-2">
                Message Sent!
              </h3>
              <p className="text-gray-500 text-sm mb-6">
                We'll get back to you within 24 hours.
              </p>
              <button
                onClick={() => setIsSubmitted(false)}
                className="text-primary-600 hover:underline text-sm font-medium"
              >
                Send another message
              </button>
            </div>
          )}
        </div>

        {/* Map / Additional Info */}
        <div className="space-y-6">
          {/* Map Placeholder */}
          <div className="bg-gray-100 rounded-2xl overflow-hidden h-64 flex items-center justify-center border border-gray-200">
            <div className="text-center text-gray-400">
              <MapPin className="w-10 h-10 mx-auto mb-2" />
              <p className="text-sm font-medium">Surat, Gujarat, India</p>
              <p className="text-xs mt-1">{APP_NAME} Headquarters</p>
            </div>
          </div>

          {/* Common Topics */}
          <div className="bg-white rounded-2xl border border-gray-100 shadow-sm p-6">
            <h3 className="text-lg font-semibold text-gray-800 mb-4">
              Common Topics
            </h3>
            <div className="space-y-3">
              {[
                { label: "Order Status & Tracking",   link: "/faq" },
                { label: "Returns & Refunds",          link: "/returns" },
                { label: "Payment Issues",             link: "/faq" },
                { label: "Account & Profile Help",     link: "/faq" },
                { label: "Product Information",        link: "/products" },
              ].map(({ label, link }) => (
                <a
                  key={label}
                  href={link}
                  className="flex items-center justify-between p-3 bg-gray-50 rounded-lg hover:bg-gray-100 transition text-sm text-gray-700 font-medium"
                >
                  {label}
                  <span className="text-primary-600">→</span>
                </a>
              ))}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Contact;