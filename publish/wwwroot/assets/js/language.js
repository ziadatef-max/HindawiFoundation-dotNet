/**
 * language.js
 * The language switcher is now driven entirely by server-rendered <a href> URLs
 * pointing to the equivalent page under the other culture segment. JS does NOT:
 *   - read or write localStorage
 *   - mutate body classes or text content
 *   - prevent default on language links
 *
 * The only client behaviour for the language switcher is opening/closing the
 * dropdown menu, which is handled by initNav() in nav.js (data-language-toggle).
 *
 * This module is intentionally a no-op kept as an export so main.js doesn't
 * need to change.
 */

export function initLanguage() {
  // no-op: server controls language via URL culture segment
}
