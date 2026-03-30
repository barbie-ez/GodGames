```markdown
# Design System Document: Celestial Brilliance

## 1. Overview & Creative North Star
**Creative North Star: "The Astral Archive"**

This design system is not a utility; it is an encounter. Moving away from the sterile "SaaS-blue" minimalism of the modern web, this system embraces **Organic Majesty**. It treats the screen as a window into a vast, obsidian void where information is illuminated by divine light rather than mere backlighting. 

To break the "template" look, we utilize **Intentional Asymmetry**. Layouts should feel like constellations—balanced but not perfectly centered. We favor large, editorial white space (or "void space") and overlapping elements where typography breaks the bounds of its containers, creating a sense of depth and scale that feels slightly intimidating and profoundly authoritative.

---

## 2. Colors & Tonal Depth

The palette is rooted in the contrast between the infinite void and the sudden, brilliant flare of a dying star.

### Core Palette
- **Background (Obsidian):** `#050508` / `surface` (`#131317`). The foundation is nearly absolute black, providing the necessary depth for light effects to resonate.
- **Primary (Solar Gold):** `primary_container` (`#FFD700`). Use this for moments of high importance. It represents the core of the sun.
- **Secondary (Star-White):** `primary` (`#fff6df`) and `secondary` (`#bfc8ce`). These represent the cooler, ethereal light of distant stars.

### The "No-Line" Rule
**Borders are strictly prohibited.** To define sections, use the `surface-container` hierarchy. A section change is signaled by moving from `surface` (#131317) to `surface_container_low` (#1b1b1f). This creates a "soft edge" that feels like a natural transition in space rather than a man-made box.

### Signature Textures & Gradients
- **Heavenly Backlighting:** All interactive cards must utilize a radial gradient background. Transition from `surface_container_high` at the center to `surface` at the edges.
- **Solar Flare:** For CTA buttons or Hero sections, use a linear gradient from `primary_fixed_dim` (#e9c400) to `primary` (#fff6df) at a 135-degree angle. This mimics a shimmering, metallic gold leaf.

---

## 3. Typography: The Ancient Voice

The typography system pairs the high-contrast authority of a serif with the functional clarity of a modern sans-serif.

- **Display & Headline (The Newsreader/Cinzel Script):** Used for all `display-lg` through `headline-sm`. These must be set with wider letter spacing (0.05em) to evoke the feeling of inscriptions on stone. This is the "Ancient" voice of the system.
- **Body & Labels (Work Sans):** Used for `body-md` and `label-sm`. This provides the "Functional" voice. It ensures that despite the majestic atmosphere, the interface remains highly legible and utilitarian.

**Editorial Tip:** Use `display-lg` (3.5rem) sparingly. When a headline is this large, ensure it has at least `spacing-16` (5.5rem) of margin beneath it to let the "authority" of the words breathe.

---

## 4. Elevation & Depth: Tonal Layering

In the vacuum of space, there are no drop shadows—only light and shadow.

- **The Layering Principle:** Depth is achieved by "stacking." 
    - Base Level: `surface`
    - Inset Content: `surface_container_lowest`
    - Raised Cards: `surface_container_high`
- **Ambient Glows:** Instead of shadows, use **Outer Glows**. For a floating element, apply a box-shadow with a 40px blur, 0px spread, using the `primary` color at 5% opacity. It should look like the object is emitting a soft, celestial light.
- **Glassmorphism:** For navigation bars or overlays, use `surface_container` with a `backdrop-filter: blur(20px)` and an opacity of 70%. This creates a "Frosted Obsidian" effect.

---

## 5. Components

### Buttons: The Solar Core
- **Primary:** No borders. Background is the "Solar Flare" gradient. Text is `on_primary` (#3a3000) in All Caps `label-md`.
- **Secondary:** Transparent background with a `Ghost Border` (outline-variant at 20% opacity). On hover, the border opacity increases to 100%.
- **Tertiary:** Pure text in `primary_fixed`, underlined with a 1px gold line that expands from the center on hover.

### Cards: The Monoliths
- **Constraint:** `border-radius: 0px` (Strictly).
- **Styling:** Use `surface_container_low`. To separate content within a card, do not use a line. Use `spacing-4` (1.4rem) of vertical space.
- **Effect:** Apply a very subtle radial lens flare (a 150px wide radial gradient, #FFD700 at 3% opacity) in the top-right corner of the card.

### Input Fields: The Altar
- **State:** Bottom-border only. Use `outline_variant` (#4d4732). 
- **Focus:** The bottom border transitions to `primary` (#fff6df), and a soft `primary_container` glow appears behind the text.

### Navigation: The Horizon
- Forbid standard top-bars with dividers. Use a floating "Glassmorphism" island at the bottom or top of the screen, utilizing `surface_container_highest` with 60% opacity.

---

## 6. Do’s and Don’ts

### Do:
- **Use Intentional Asymmetry:** Let a headline sit on the left while the body text sits on the right of a wide grid.
- **Embrace the Dark:** Allow large areas of the screen to remain pure `#050508`. Contrast is your strongest tool.
- **Scale Typography:** Make sure there is a dramatic difference between your `display-lg` and your `body-md`.

### Don’t:
- **No Rounded Corners:** Never use `border-radius`. Everything must feel like it was cut from obsidian glass.
- **No 1px Dividers:** If you feel the need to "separate" something, use a background color shift or more space.
- **No Pure Greys:** Every "grey" in this system is tinted with blue or gold. Avoid `#cccccc` or `#eeeeee`. Use the provided `secondary` and `outline` tokens.
- **No Rapid Animations:** Interactions should be slow and "heavy." Fades should be 400ms+, suggesting a majestic movement of celestial bodies.