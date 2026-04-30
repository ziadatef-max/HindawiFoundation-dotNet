import { initNav } from "./nav.js";
import { initSlider } from "./slider.js";
import { initLanguage } from "./language.js";
import { initDonate } from "./donate.js";

function initApp() {
  initNav();
  initLanguage();
  initSlider();
  initDonate();
}

if (document.readyState === "loading") {
  document.addEventListener("DOMContentLoaded", initApp);
} else {
  initApp();
}
