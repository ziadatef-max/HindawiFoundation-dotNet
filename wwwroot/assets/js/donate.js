/**
 * donate.js
 * Lightweight UX helpers for the donation page that are shared with non-form
 * pages. Intentionally NO submit handler here — the real submit flow runs from
 * the inline Braintree script that ships with the donate view.
 *
 * Runs as a no-op on pages that don't have payment-method tabs.
 */

var PAYMENT_METHOD_KEY = 'hindawi_payment_method';

export function initDonate() {
  const methodBtns = document.querySelectorAll("[data-payment]");
  const panels = document.querySelectorAll("[data-payment-panel]");

  if (!methodBtns.length || !panels.length) return;

  function activateMethod(target) {
    methodBtns.forEach((b) => {
      b.classList.remove("is-active");
      b.setAttribute("aria-pressed", "false");
    });
    panels.forEach((panel) => {
      if (panel.dataset.paymentPanel === target) {
        panel.classList.add("is-active");
        panel.hidden = false;
      } else {
        panel.classList.remove("is-active");
        panel.hidden = true;
      }
    });
    const activeBtn = document.querySelector(`[data-payment="${target}"]`);
    if (activeBtn) {
      activeBtn.classList.add("is-active");
      activeBtn.setAttribute("aria-pressed", "true");
    }
  }

  const saved = sessionStorage.getItem(PAYMENT_METHOD_KEY);
  const validTargets = Array.from(methodBtns).map((b) => b.dataset.payment);
  if (saved && validTargets.includes(saved)) {
    activateMethod(saved);
  }

  methodBtns.forEach((btn) => {
    btn.addEventListener("click", () => {
      const target = btn.dataset.payment;
      sessionStorage.setItem(PAYMENT_METHOD_KEY, target);
      activateMethod(target);
    });
  });
}
