import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import LanguageDetector from "i18next-browser-languagedetector";

import en from "./locales/en.json";
import hi from "./locales/hi.json";
import gu from "./locales/gu.json";

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources: {
      en: { translation: en },
      hi: { translation: hi },
      gu: { translation: gu },
    },
    fallbackLng: "en",
    supportedLngs: ["en", "hi", "gu"],
    interpolation: {
      escapeValue: false,
    },
    detection: {
      order: ["localStorage", "navigator"],
      caches: ["localStorage"],
    },
  });

export default i18n;

export const LANGUAGES = [
  { code: "en", name: "English",  flag: "🇬🇧" },
  { code: "hi", name: "हिन्दी",    flag: "🇮🇳" },
  { code: "gu", name: "ગુજરાતી",  flag: "🇮🇳" },
];