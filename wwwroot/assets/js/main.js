import { initNav } from "./nav.js";
import { initSlider } from "./slider.js";
import { initDonate } from "./donate.js";

function initApp() {
  initNav();
  initSlider();
  initDonate();
}

if (document.readyState === "loading") {
  document.addEventListener("DOMContentLoaded", initApp);
} else {
  initApp();
}
