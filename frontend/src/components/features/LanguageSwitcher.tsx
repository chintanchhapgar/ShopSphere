import { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import { Globe, Check, Search } from "lucide-react";
import { LANGUAGES } from "@/i18n";
import { cn } from "@/utils/cn";

const LanguageSwitcher = () => {
  const { i18n } = useTranslation();
  const [isOpen, setIsOpen]         = useState(false);
  const [searchQuery, setSearchQuery] = useState("");

  const currentLang = LANGUAGES.find((l) => l.code === i18n.language) || LANGUAGES[0];

  // ── Update document direction for RTL languages ────────────────────────────
  useEffect(() => {
    document.documentElement.dir = currentLang.rtl ? "rtl" : "ltr";
    document.documentElement.lang = currentLang.code;
  }, [currentLang]);

  const changeLanguage = (code: string) => {
    i18n.changeLanguage(code);
    setIsOpen(false);
    setSearchQuery("");
  };

  const filteredLanguages = LANGUAGES.filter(
    (lang) =>
      !searchQuery ||
      lang.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
      lang.nativeName.toLowerCase().includes(searchQuery.toLowerCase()) ||
      lang.code.toLowerCase().includes(searchQuery.toLowerCase())
  );

  return (
    <div className="relative">
      {/* Trigger Button */}
      <button
        onClick={() => setIsOpen(!isOpen)}
        className="flex items-center gap-1.5 px-3 py-2 text-sm text-gray-600 hover:text-primary-600 hover:bg-primary-50 dark:text-gray-300 dark:hover:bg-gray-800 rounded-lg transition"
        title={`Language: ${currentLang.nativeName}`}
      >
        <Globe className="w-4 h-4" />
        <span className="text-base">{currentLang.flag}</span>
        <span className="hidden sm:inline uppercase text-xs font-semibold">
          {currentLang.code}
        </span>
      </button>

      {/* Dropdown */}
      {isOpen && (
        <>
          <div className="fixed inset-0 z-10" onClick={() => setIsOpen(false)} />
          <div className="absolute right-0 top-full mt-2 w-64 bg-white dark:bg-gray-800 rounded-xl border border-gray-100 dark:border-gray-700 shadow-lg z-20 overflow-hidden">

            {/* Header */}
            <div className="p-3 border-b border-gray-100 dark:border-gray-700">
              <p className="text-xs font-semibold text-gray-500 dark:text-gray-400 uppercase tracking-wider mb-2">
                Select Language
              </p>
              {/* Search */}
              <div className="relative">
                <Search className="absolute left-2.5 top-1/2 -translate-y-1/2 w-3.5 h-3.5 text-gray-400" />
                <input
                  type="text"
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                  placeholder="Search languages..."
                  className="w-full pl-8 pr-3 py-1.5 text-sm border border-gray-200 dark:border-gray-700 dark:bg-gray-900 dark:text-gray-100 rounded-lg focus:outline-none focus:ring-2 focus:ring-primary-500/20"
                  autoFocus
                />
              </div>
            </div>

            {/* Language List */}
            <div className="max-h-80 overflow-y-auto py-1">
              {filteredLanguages.length === 0 ? (
                <p className="text-center text-sm text-gray-400 py-4">
                  No languages found
                </p>
              ) : (
                filteredLanguages.map((lang) => (
                  <button
                    key={lang.code}
                    onClick={() => changeLanguage(lang.code)}
                    className={cn(
                      "flex items-center justify-between gap-3 w-full px-4 py-2.5 text-sm transition",
                      "hover:bg-gray-50 dark:hover:bg-gray-700",
                      currentLang.code === lang.code
                        ? "bg-primary-50 dark:bg-primary-900/20 text-primary-600 dark:text-primary-400"
                        : "text-gray-700 dark:text-gray-300"
                    )}
                  >
                    <div className="flex items-center gap-3 min-w-0">
                      <span className="text-lg shrink-0">{lang.flag}</span>
                      <div className="text-left min-w-0">
                        <p className={cn(
                          "font-medium truncate",
                          currentLang.code === lang.code && "font-semibold"
                        )}>
                          {lang.nativeName}
                        </p>
                        <p className="text-xs text-gray-400 dark:text-gray-500 truncate">
                          {lang.name} • {lang.code.toUpperCase()}
                          {lang.rtl && " • RTL"}
                        </p>
                      </div>
                    </div>
                    {currentLang.code === lang.code && (
                      <Check className="w-4 h-4 text-primary-600 shrink-0" />
                    )}
                  </button>
                ))
              )}
            </div>

            {/* Footer */}
            <div className="px-4 py-2 border-t border-gray-100 dark:border-gray-700 bg-gray-50 dark:bg-gray-900">
              <p className="text-xs text-gray-400 text-center">
                {LANGUAGES.length} languages available
              </p>
            </div>
          </div>
        </>
      )}
    </div>
  );
};

export default LanguageSwitcher;