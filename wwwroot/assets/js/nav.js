export function initNav() {
  const body = document.body;
  const mobileToggle = document.querySelector("[data-mobile-toggle]");
  const mobileMenu = document.getElementById("mobileMenu");
  const languageSwitch = document.querySelector(".language-switch");
  const languageToggle = document.querySelector("[data-language-toggle]");

  setActiveNavLink();

  if (mobileToggle && mobileMenu) {
    const closeMobileMenu = () => {
      mobileMenu.classList.remove("is-open");
      mobileMenu.setAttribute("hidden", "");
      mobileToggle.setAttribute("aria-expanded", "false");
      const icon = mobileToggle.querySelector("i");
      if (icon) icon.className = "fa-solid fa-bars";
      body.style.overflow = "";
    };

    const openMobileMenu = () => {
      mobileMenu.classList.add("is-open");
      mobileMenu.removeAttribute("hidden");
      mobileToggle.setAttribute("aria-expanded", "true");
      const icon = mobileToggle.querySelector("i");
      if (icon) icon.className = "fa-solid fa-xmark";
      body.style.overflow = "hidden";
    };

    mobileToggle.addEventListener("click", () => {
      const isOpen = mobileMenu.classList.contains("is-open");
      if (isOpen) {
        closeMobileMenu();
      } else {
        openMobileMenu();
      }
    });

    mobileMenu.querySelectorAll("a").forEach((link) => {
      link.addEventListener("click", closeMobileMenu);
    });

    window.addEventListener("keydown", (event) => {
      if (event.key === "Escape" && mobileMenu.classList.contains("is-open")) {
        closeMobileMenu();
      }
    });
  }

  if (languageSwitch && languageToggle) {
    languageToggle.addEventListener("click", (event) => {
      event.preventDefault();
      const isOpen = languageSwitch.classList.toggle("is-open");
      languageToggle.setAttribute("aria-expanded", String(isOpen));
    });

    window.addEventListener("click", (event) => {
      if (!languageSwitch.contains(event.target)) {
        languageSwitch.classList.remove("is-open");
        languageToggle.setAttribute("aria-expanded", "false");
      }
    });

    window.addEventListener("keydown", (event) => {
      if (event.key === "Escape") {
        languageSwitch.classList.remove("is-open");
        languageToggle.setAttribute("aria-expanded", "false");
      }
    });
  }
}

function setActiveNavLink() {
  const normalize = (href) => {
    try {
      const url = new URL(href, window.location.href);
      return url.pathname.replace(/\/$/, "").replace(/\.html$/, "") || "/";
    } catch (_) {
      return "";
    }
  };

  const currentPath = normalize(window.location.href);

  const navLinks = document.querySelectorAll(
    ".site-nav__menu a, .site-nav__donate a, .mobile-nav a"
  );

  let matched = false;
  navLinks.forEach((link) => {
    const linkPath = normalize(link.href);
    const shouldBeActive = linkPath === currentPath;
    link.classList.toggle("is-active", shouldBeActive);
    if (shouldBeActive) matched = true;
  });

  // Fallback 1: sub-path match — /ar/News/Details activates the /ar/News link
  if (!matched) {
    navLinks.forEach((link) => {
      const linkPath = normalize(link.href);
      if (linkPath.length > 4 && currentPath.startsWith(linkPath + '/')) {
        link.classList.add("is-active");
        matched = true;
      }
    });
  }

  // Fallback 2: hyphen-suffix strip (e.g. /news-details → /news)
  if (!matched) {
    const parentPath = currentPath.replace(/-[^/]+$/, "");
    navLinks.forEach((link) => {
      const linkPath = normalize(link.href);
      if (linkPath === parentPath) {
        link.classList.add("is-active");
      }
    });
  }
}

