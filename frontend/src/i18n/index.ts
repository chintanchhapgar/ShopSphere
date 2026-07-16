import i18n from "i18next";
import { initReactI18next } from "react-i18next";
import LanguageDetector from "i18next-browser-languagedetector";

// Import all language files
import en from "./locales/en.json";
import hi from "./locales/hi.json";
import gu from "./locales/gu.json";
import es from "./locales/es.json";
import fr from "./locales/fr.json";
import de from "./locales/de.json";
import ar from "./locales/ar.json";
import zh from "./locales/zh.json";
import ja from "./locales/ja.json";
import pt from "./locales/pt.json";

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources: {
      en: { translation: en },
      hi: { translation: hi },
      gu: { translation: gu },
      es: { translation: es },
      fr: { translation: fr },
      de: { translation: de },
      ar: { translation: ar },
      zh: { translation: zh },
      ja: { translation: ja },
      pt: { translation: pt },
    },
    fallbackLng: "en",
    supportedLngs: ["en", "hi", "gu", "es", "fr", "de", "ar", "zh", "ja", "pt"],
    interpolation: {
      escapeValue: false,
    },
    detection: {
      order: ["localStorage", "navigator"],
      caches: ["localStorage"],
    },
  });

export default i18n;

// ─── Supported Languages ─────────────────────────────────────────────────────
export interface Language {
  code:      string;
  name:      string;
  nativeName: string;
  flag:      string;
  rtl?:      boolean;
}

export const LANGUAGES: Language[] = [
  { code: "en", name: "English",    nativeName: "English",    flag: "🇬🇧" },
  { code: "hi", name: "Hindi",      nativeName: "हिन्दी",      flag: "🇮🇳" },
  { code: "gu", name: "Gujarati",   nativeName: "ગુજરાતી",    flag: "🇮🇳" },
  { code: "es", name: "Spanish",    nativeName: "Español",    flag: "🇪🇸" },
  { code: "fr", name: "French",     nativeName: "Français",   flag: "🇫🇷" },
  { code: "de", name: "German",     nativeName: "Deutsch",    flag: "🇩🇪" },
  { code: "ar", name: "Arabic",     nativeName: "العربية",    flag: "🇸🇦", rtl: true },
  { code: "zh", name: "Chinese",    nativeName: "中文",        flag: "🇨🇳" },
  { code: "ja", name: "Japanese",   nativeName: "日本語",      flag: "🇯🇵" },
  { code: "pt", name: "Portuguese", nativeName: "Português",  flag: "🇵🇹" },
];