/**
 * donate.js
 * Lightweight UX helpers for the donation page that are shared with non-form
 * pages. Intentionally NO submit handler here — the real submit flow runs from
 * the inline Braintree script that ships with the donate view.
 *
 * Runs as a no-op on pages that don't have payment-method tabs.
 */

export function initDonate() {
  const methodBtns = document.querySelectorAll("[data-payment]");
  const panels = document.querySelectorAll("[data-payment-panel]");

  if (!methodBtns.length || !panels.length) return;

  methodBtns.forEach((btn) => {
    btn.addEventListener("click", () => {
      const target = btn.dataset.payment;

      methodBtns.forEach((b) => {
        b.classList.remove("is-active");
        b.setAttribute("aria-pressed", "false");
      });
      btn.classList.add("is-active");
      btn.setAttribute("aria-pressed", "true");

      panels.forEach((panel) => {
        if (panel.dataset.paymentPanel === target) {
          panel.classList.add("is-active");
          panel.hidden = false;
        } else {
          panel.classList.remove("is-active");
          panel.hidden = true;
        }
      });
    });
  });
}
