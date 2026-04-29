/**
 * language.js
 * Language switcher with localStorage persistence.
 * Supported languages: en (default), ar
 */

const SUPPORTED_LANGUAGES = ["en", "ar"];
const DEFAULT_LANGUAGE = "en";
const STORAGE_KEY = "hindawi-lang";

const PRODUCT_URLS = {
  booktime: {
    en: "https://www.booktime.org/en",
    ar: "https://www.booktime.org/ar",
  },
  safahat: {
    en: "https://www.safahat.org/en/",
    ar: "https://www.safahat.org/ar/",
  },
  folios: {
    en: "https://folios.org/en",
    ar: "https://folios.org/ar",
  },
};

function getStoredLanguage() {
  try {
    const stored = localStorage.getItem(STORAGE_KEY);
    if (stored && SUPPORTED_LANGUAGES.includes(stored)) {
      return stored;
    }
  } catch (_) {
    // localStorage blocked (e.g. private mode restrictions)
  }
  return DEFAULT_LANGUAGE;
}

function saveLanguage(lang) {
  try {
    localStorage.setItem(STORAGE_KEY, lang);
  } catch (_) {}
}

function applyLanguage(lang) {
  if (!SUPPORTED_LANGUAGES.includes(lang)) return;

  document.documentElement.lang = lang;
  document.documentElement.dir = lang === "ar" ? "rtl" : "ltr";

  // Update all product links that have data-product attribute
  document.querySelectorAll("[data-product]").forEach((link) => {
    const product = link.dataset.product;
    const url = PRODUCT_URLS[product]?.[lang];
    if (url) link.href = url;
  });

  // Update active state in language menu
  document.querySelectorAll(".language-menu a[data-lang]").forEach((a) => {
    const isActive = a.dataset.lang === lang;
    a.classList.toggle("is-active", isActive);
    a.setAttribute("aria-current", isActive ? "true" : "false");
  });

  // Swap logo between EN and AR variants
  document.querySelectorAll(".site-logo__img").forEach((img) => {
    if (lang === "ar" && img.src.includes("hindawi_en.svg")) {
      img.src = img.src.replace("hindawi_en.svg", "hindawi_ar.svg");
      img.alt = "مؤسسة هنداوي";
    } else if (lang === "en" && img.src.includes("hindawi_ar.svg")) {
      img.src = img.src.replace("hindawi_ar.svg", "hindawi_en.svg");
      img.alt = "Hindawi Foundation";
    }
  });
}

export function initLanguage() {
  const currentLang = getStoredLanguage();
  applyLanguage(currentLang);

  document.querySelectorAll(".language-menu a[data-lang]").forEach((link) => {
    link.addEventListener("click", (event) => {
      event.preventDefault();
      const lang = link.dataset.lang;
      if (!SUPPORTED_LANGUAGES.includes(lang)) return;

      saveLanguage(lang);
      applyLanguage(lang);

      // Close language dropdown after selection
      const langSwitch = link.closest(".language-switch");
      if (langSwitch) {
        langSwitch.classList.remove("is-open");
        const toggle = langSwitch.querySelector("[data-language-toggle]");
        if (toggle) toggle.setAttribute("aria-expanded", "false");
      }
    });
  });
}
