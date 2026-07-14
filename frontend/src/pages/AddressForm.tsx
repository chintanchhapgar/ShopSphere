import { useEffect, useState } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
import { ArrowLeft, MapPin, Save } from "lucide-react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { addressApi } from "@/api/address.api";
import type { Address } from "@/types";
import Input from "@/components/ui/Input";
import Button from "@/components/ui/Button";
import Spinner from "@/components/ui/Spinner";
import toast from "react-hot-toast";

// ── Schema ───────────────────────────────────────────────────────────────────
const addressSchema = z.object({
  fullName:     z.string().min(2, "Full name is required"),
  phoneNumber:  z.string().min(10, "Valid phone number is required"),
  addressLine1: z.string().min(3, "Address is required"),
  addressLine2: z.string().optional(),
  city:         z.string().min(2, "City is required"),
  state:        z.string().min(2, "State is required"),
  postalCode:   z.string().min(4, "Postal code is required"),
  country:      z.string().min(2, "Country is required"),
  isDefault:    z.boolean(),
});

type AddressFormData = z.infer<typeof addressSchema>;

const AddressForm = () => {
  const { id }   = useParams<{ id: string }>();
  const navigate = useNavigate();
  const isEdit   = !!id;

  const [isLoading, setIsLoading]       = useState(false);
  const [isFetching, setIsFetching]     = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<AddressFormData>({
    resolver: zodResolver(addressSchema),
    defaultValues: {
      fullName:     "",
      phoneNumber:  "",
      addressLine1: "",
      addressLine2: "",
      city:         "",
      state:        "",
      postalCode:   "",
      country:      "India",
      isDefault:    false,
    },
  });

  // ── Load Existing Address (Edit Mode) ──────────────────────────────────────
  useEffect(() => {
    if (!id) return;
    const load = async () => {
      setIsFetching(true);
      try {
        const addresses = await addressApi.getAddresses();
        const addr = addresses.find((a) => a.id === id);
        if (addr) {
          reset({
            fullName:     addr.fullName,
            phoneNumber:  addr.phoneNumber,
            addressLine1: addr.addressLine1,
            addressLine2: addr.addressLine2 || "",
            city:         addr.city,
            state:        addr.state,
            postalCode:   addr.postalCode,
            country:      addr.country,
            isDefault:    addr.isDefault,
          });
        } else {
          toast.error("Address not found");
          navigate("/profile");
        }
      } catch {
        toast.error("Failed to load address");
        navigate("/profile");
      } finally {
        setIsFetching(false);
      }
    };
    load();
  }, [id]);

  // ── Submit ─────────────────────────────────────────────────────────────────
  const onSubmit = async (data: AddressFormData) => {
    setIsLoading(true);
    try {
      if (isEdit && id) {
        await addressApi.updateAddress(id, {
          addressId:    id,
          fullName:     data.fullName,
          phoneNumber:  data.phoneNumber,
          addressLine1: data.addressLine1,
          addressLine2: data.addressLine2 || null,
          city:         data.city,
          state:        data.state,
          postalCode:   data.postalCode,
          country:      data.country,
        });
        toast.success("Address updated!");
      } else {
        await addressApi.createAddress({
          fullName:     data.fullName,
          phoneNumber:  data.phoneNumber,
          addressLine1: data.addressLine1,
          addressLine2: data.addressLine2 || null,
          city:         data.city,
          state:        data.state,
          postalCode:   data.postalCode,
          country:      data.country,
          isDefault:    data.isDefault,
        });
        toast.success("Address added!");
      }
      navigate("/profile");
    } catch (err) {
      toast.error((err as Error).message || "Failed to save address");
    } finally {
      setIsLoading(false);
    }
  };

  if (isFetching) {
    return (
      <div className="flex justify-center py-32">
        <Spinner size="lg" className="text-primary-600" />
      </div>
    );
  }

  return (
    <div className="max-w-2xl mx-auto px-4 py-8">

      {/* Back */}
      <button
        onClick={() => navigate(-1)}
        className="flex items-center gap-2 text-sm text-gray-500 hover:text-primary-600 mb-6 transition"
      >
        <ArrowLeft className="w-4 h-4" /> Back
      </button>

      {/* Header */}
      <div className="flex items-center gap-3 mb-8">
        <div className="w-10 h-10 bg-primary-100 rounded-xl flex items-center justify-center">
          <MapPin className="w-5 h-5 text-primary-600" />
        </div>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">
            {isEdit ? "Edit Address" : "Add New Address"}
          </h1>
          <p className="text-sm text-gray-500">
            {isEdit ? "Update your shipping address" : "Add a new shipping address"}
          </p>
        </div>
      </div>

      {/* Form */}
      <div className="bg-white rounded-2xl border border-gray-100 shadow-sm p-6">
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">

          {/* Full Name & Phone */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <Input
              label="Full Name"
              placeholder="John Doe"
              error={errors.fullName?.message}
              {...register("fullName")}
            />
            <Input
              label="Phone Number"
              placeholder="+91 9876543210"
              error={errors.phoneNumber?.message}
              {...register("phoneNumber")}
            />
          </div>

          {/* Address Lines */}
          <Input
            label="Address Line 1"
            placeholder="House/Flat No., Street"
            error={errors.addressLine1?.message}
            {...register("addressLine1")}
          />
          <Input
            label="Address Line 2 (Optional)"
            placeholder="Landmark, Area"
            error={errors.addressLine2?.message}
            {...register("addressLine2")}
          />

          {/* City & State */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <Input
              label="City"
              placeholder="Mumbai"
              error={errors.city?.message}
              {...register("city")}
            />
            <Input
              label="State"
              placeholder="Maharashtra"
              error={errors.state?.message}
              {...register("state")}
            />
          </div>

          {/* Postal Code & Country */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <Input
              label="Postal Code"
              placeholder="400001"
              error={errors.postalCode?.message}
              {...register("postalCode")}
            />
            <Input
              label="Country"
              placeholder="India"
              error={errors.country?.message}
              {...register("country")}
            />
          </div>

          {/* Default Checkbox */}
          {!isEdit && (
            <label className="flex items-center gap-3 cursor-pointer p-3 bg-gray-50 rounded-xl border border-gray-100">
              <input
                type="checkbox"
                className="w-4 h-4 rounded border-gray-300 text-primary-600 focus:ring-primary-500"
                {...register("isDefault")}
              />
              <div>
                <p className="text-sm font-medium text-gray-700">
                  Set as default address
                </p>
                <p className="text-xs text-gray-400">
                  This address will be pre-selected during checkout
                </p>
              </div>
            </label>
          )}

          {/* Actions */}
          <div className="flex gap-3 pt-2">
            <Button
              type="submit"
              size="lg"
              isLoading={isLoading}
              className="flex-1"
            >
              <Save className="w-4 h-4" />
              {isEdit ? "Update Address" : "Save Address"}
            </Button>
            <Button
              type="button"
              variant="outline"
              size="lg"
              onClick={() => navigate(-1)}
            >
              Cancel
            </Button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default AddressForm;