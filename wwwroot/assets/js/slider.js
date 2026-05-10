export function initSlider() {
  const sliderRoot = document.querySelector("[data-slider]");
  if (!sliderRoot) {
    return;
  }

  const slides = Array.from(sliderRoot.querySelectorAll("[data-slide]"));
  const dots = Array.from(sliderRoot.querySelectorAll("[data-slide-to]"));
  const mobileQuery = window.matchMedia("(max-width: 768px)");
  let activeIndex = 0;

  if (!slides.length || !dots.length) {
    return;
  }

  const setActiveDot = (index) => {
    dots.forEach((dot, dotIndex) => {
      const isActive = dotIndex === index;
      dot.classList.toggle("is-active", isActive);
      dot.setAttribute("aria-current", isActive ? "true" : "false");
    });
  };

  const render = () => {
    if (mobileQuery.matches) {
      slides.forEach((slide, index) => {
        slide.hidden = index !== activeIndex;
      });
      setActiveDot(activeIndex);
    } else {
      slides.forEach((slide) => {
        slide.hidden = false;
      });
      setActiveDot(0);
    }
  };

  dots.forEach((dot, index) => {
    dot.addEventListener("click", () => {
      activeIndex = index;
      render();
    });
  });

  const onQueryChange = () => {
    if (!mobileQuery.matches) {
      activeIndex = 0;
    }
    render();
  };

  if (typeof mobileQuery.addEventListener === "function") {
    mobileQuery.addEventListener("change", onQueryChange);
  } else {
    mobileQuery.addListener(onQueryChange);
  }

  sliderRoot.addEventListener("keydown", (event) => {
    if (!mobileQuery.matches) {
      return;
    }

    if (event.key === "ArrowRight") {
      activeIndex = (activeIndex + 1) % slides.length;
      render();
    }

    if (event.key === "ArrowLeft") {
      activeIndex = (activeIndex - 1 + slides.length) % slides.length;
      render();
    }
  });

  render();
}
