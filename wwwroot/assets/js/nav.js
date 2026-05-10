export function initNav() {
  const languageSwitch = document.querySelector(".language-switch");
  const languageToggle = document.querySelector("[data-language-toggle]");

  setActiveNavLink();

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
    ".site-nav__menu a, .site-nav__donate a, .mobile-bottom-nav__item"
  );

  let matched = false;
  navLinks.forEach((link) => {
    const linkPath = normalize(link.href);
    const shouldBeActive = linkPath === currentPath;
    link.classList.toggle("is-active", shouldBeActive);
    if (shouldBeActive) matched = true;
  });

  if (!matched) {
    navLinks.forEach((link) => {
      const linkPath = normalize(link.href);
      if (linkPath.length > 4 && currentPath.startsWith(linkPath + '/')) {
        link.classList.add("is-active");
        matched = true;
      }
    });
  }

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
