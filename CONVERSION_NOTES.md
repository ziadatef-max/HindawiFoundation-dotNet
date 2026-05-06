# HindawiFoundation.Web — Conversion Notes

ASP.NET Core MVC scaffold (.NET 8) generated from your real static site.

## What is in this delivery

The Razor views were filled in from the actual uploaded HTML pages, not stubs.

- `HindawiFoundation.Web.csproj` — .NET 8 web SDK project
- `Program.cs` — minimal-hosting MVC startup with default route `{controller=Home}/{action=Index}/{id?}`
- `appsettings.json` / `appsettings.Development.json` / `Properties/launchSettings.json`
- `Controllers/HomeController.cs` — `Index`, `About`, `Partners`, `News`, `NewsDetails(int? id)`, `Contact`, `Donate`, `Error`
- `Views/_ViewImports.cshtml`, `Views/_ViewStart.cshtml`
- `Views/Shared/_Layout.cshtml` — head with favicon, Google Fonts, FontAwesome CDN, the 5 common stylesheets, `Styles` section, body class from `ViewData["BodyClass"]`, `RenderBody()`, `main.js` module script, `Scripts` section
- `Views/Shared/_Header.cshtml` — exact original header markup converted to tag helpers, with `is-active` driven by `ViewData["ActivePage"]`
- `Views/Shared/_Footer.cshtml` — exact original footer markup converted to tag helpers
- `Views/Home/{Index,About,Partners,News,NewsDetails,Contact,Donate}.cshtml` — each one has the real `<main>...</main>` content from the matching HTML page

CSS that was uploaded is included in `wwwroot/assets/css/`:

- `tokens.css`, `base.css`, `layout.css`, `utilities.css`, `components.css`
- `pages/home.css`, `pages/about.css`, `pages/partners.css`, `pages/news.css`, `pages/news-details.css`, `pages/contact.css`, `pages/donate.css`

## What you need to copy manually

I could not unzip the original archive in this session, so the binary asset folders weren't available. You need to drop these into `wwwroot/` keeping the same casing (Linux hosting is case-sensitive):

| Source | Destination |
|---|---|
| `Images/` | `wwwroot/Images/` |
| `Logos/` | `wwwroot/Logos/` |
| `Icons/` | `wwwroot/Icons/` |
| `Fonts/` | `wwwroot/Fonts/` |
| `News/` | `wwwroot/News/` |
| `assets/js/` (main.js, nav.js, slider.js, donate.js, language.js, animations.js) | `wwwroot/assets/js/` |
| `fontawesome/` | optional — see below |
| `Space_Grotesk/` | optional — see below |

The CSS is already wired to expect these top-level folder names inside `wwwroot`. CSS internal paths like `url("../../Images/hindawi_logo_side.svg?v=3")` (in `components.css`) and `url("../../../Images/hero_image.svg?v=3")` (in `pages/home.css`) and `url("../../Fonts/NotoSansCham-VariableFont_wght.ttf")` (in `tokens.css`) all resolve correctly when those folders sit at the `wwwroot/` root.

### About FontAwesome and Space_Grotesk

The original HTML pages do NOT use the local `fontawesome/` folder — they comment it out and load FontAwesome from the CDN:

```html
<link rel="stylesheet"
      href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.1/css/all.min.css"
      crossorigin="anonymous"
      referrerpolicy="no-referrer">
```

I kept that CDN link in `_Layout.cshtml` to match the original behaviour exactly, so you do NOT need to copy the local `fontawesome/` folder for the icons to work. Copy it only if you want a future offline fallback.

The original HTML loads Google Fonts (Space Grotesk, Source Sans 3, Playfair Display) from `fonts.googleapis.com`, again matching the CDN approach. The local `Space_Grotesk/` folder is not referenced anywhere in the HTML or CSS that you uploaded, so it is also optional.

The only locally-loaded font is `Noto Sans Cham`, which `tokens.css` pulls from `wwwroot/Fonts/NotoSansCham-VariableFont_wght.ttf`. That file MUST be present at that exact path or the site will fall back to the system stack.

## How shared state is propagated to the layout

Each page view sets three pieces of `ViewData`:

```csharp
ViewData["Title"]      = "About";          // browser tab title (joined with brand)
ViewData["BodyClass"]  = "page page--about"; // sets <body class="...">
ViewData["ActivePage"] = "about";           // marks the matching nav link is-active
```

`_Layout.cshtml` reads `BodyClass` and `Title`. `_Header.cshtml` reads `ActivePage` and writes `is-active` onto the matching primary-nav and mobile-nav links (matching the original per-page markup exactly).

## CSS load order in `_Layout.cshtml`

1. Favicon, Google Fonts preconnects + stylesheet
2. FontAwesome 6.5.1 from cdnjs
3. `~/assets/css/tokens.css`
4. `~/assets/css/base.css`
5. `~/assets/css/layout.css`
6. `~/assets/css/utilities.css`
7. `~/assets/css/components.css`
8. `@RenderSectionAsync("Styles")` — the page-specific CSS (e.g. `~/assets/css/pages/home.css`)

This matches the original HTML order exactly.

## JS

The original HTML loads only one script tag at the bottom of `<body>`:

```html
<script type="module" src="assets/js/main.js"></script>
```

So `main.js` is the entry point and presumably imports `nav.js`, `slider.js`, `language.js`, `animations.js` as ES modules. `_Layout.cshtml` includes that one tag (with `~/assets/js/main.js` and `asp-append-version="true"`). When you copy your `assets/js/` folder into `wwwroot/assets/js/`, the module imports will work as-is, no extra changes needed.

`donate.js` is exposed via `@section Scripts` only on the Donate page — but ONLY if `main.js` doesn't already import it. Inspect your `main.js`: if it already imports `donate.js`, delete the `@section Scripts { … }` block at the bottom of `Views/Home/Donate.cshtml` to avoid loading it twice.

## Internal link conversion

Every `<a href="*.html">` from the original pages was converted:

| Original | Razor |
|---|---|
| `home.html` | `<a asp-controller="Home" asp-action="Index">` |
| `about.html` | `<a asp-controller="Home" asp-action="About">` |
| `partners.html` | `<a asp-controller="Home" asp-action="Partners">` |
| `news.html` | `<a asp-controller="Home" asp-action="News">` |
| `news-details.html` | `<a asp-controller="Home" asp-action="NewsDetails">` |
| `contact.html` | `<a asp-controller="Home" asp-action="Contact">` |
| `donate.html` | `<a asp-controller="Donate" asp-action="Index">` |

External `https://`, `mailto:`, `tel:`, and `#` placeholder links are left untouched. The `privacy.html` and `terms.html` footer links currently point to `#` because no Razor views exist for them yet — wire them up if you decide to add those pages.

## Things to manually verify

- Drop your asset folders (`Images/`, `Logos/`, `Icons/`, `Fonts/`, `News/`, `assets/js/`) into `wwwroot/` with original casing.
- Check `Views/Home/Donate.cshtml` — if your `main.js` already imports `donate.js`, remove the `@section Scripts` block to avoid double-loading.
- The footer "Privacy Policy" and "Terms and Conditions" links currently go to `#`. Add controller actions/views if needed.
- Run `dotnet build` locally before deploying. I could not run `dotnet build` myself in this session.

## Run locally

```
cd HindawiFoundation.Web
dotnet restore
dotnet build
dotnet run
```

`dotnet run` will print a URL like `http://localhost:5000` (or `5001` over HTTPS); open that in a browser.

> I did NOT run `dotnet build` in this session. Verify the build on your machine. The project is conventional .NET 8 MVC and follows standard scaffolding conventions, but a real compile is the only way to confirm.
