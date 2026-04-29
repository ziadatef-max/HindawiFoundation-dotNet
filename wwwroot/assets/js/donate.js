/**
 * donate.js
 * Handles donation form UI: payment method tabs, frequency, placeholder submit.
 * Runs as a no-op on pages that don't have the donate form.
 */

export function initDonate() {
  // ── Payment method tabs ────────────────────────────────────────
  const methodBtns = document.querySelectorAll("[data-payment]");
  const panels = document.querySelectorAll("[data-payment-panel]");

  if (methodBtns.length && panels.length) {
    methodBtns.forEach((btn) => {
      btn.addEventListener("click", () => {
        const target = btn.dataset.payment;

        // Update tab active states
        methodBtns.forEach((b) => {
          b.classList.remove("is-active");
          b.setAttribute("aria-pressed", "false");
        });
        btn.classList.add("is-active");
        btn.setAttribute("aria-pressed", "true");

        // Show matching panel, hide others
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

  // ── Form placeholder submission ────────────────────────────────
  const donateForm = document.getElementById("donateForm");
  const donateMessage = document.getElementById("donateMessage");

  if (donateForm) {
    donateForm.addEventListener("submit", (event) => {
      event.preventDefault();
      if (donateMessage) {
        donateMessage.textContent =
          "Thank you for your generosity! Payment integration is coming soon.";
        donateMessage.hidden = false;
        donateMessage.setAttribute("tabindex", "-1");
        donateMessage.focus();
      }
    });
  }
}
