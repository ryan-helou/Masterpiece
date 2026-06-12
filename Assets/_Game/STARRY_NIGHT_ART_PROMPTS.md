# Starry Night — Art Asset Briefs (for Gemini image gen)

The game loads PNGs from `Assets/_Game/Resources/Level1/` and builds sprites at runtime.
Generate **clean painterly art (NOT pixel art)** — the engine's pixel-downscale filter adds the retro look,
so you get gorgeous source art *and* cohesion. Each asset below is a full brief: copy the whole block in.

## Global rules (apply to every asset)
- **Output:** PNG, **transparent background** (true alpha). If your tool can't do alpha, render on flat
  **magenta #ff00ff** and tell me — I key it out. Exception: `ground` is a solid opaque tile.
- **One object, centered, no scenery, no cast shadow, no ground line, no text/UI, no frame/border.**
- **Aspect ratio + orientation matter; exact pixel size does not** — I scale each in-engine via pixels-per-unit.
  (Suggested px is just for quality.)
- Anything with a facing faces **RIGHT**. Light source is **cool moonlight from the top-left**, plus each
  object's own glow.
- **Brushwork rule:** short, curved, visible **impasto oil strokes that swirl** like *The Starry Night* — no
  smooth airbrush gradients, no flat vector fills, no hard black cartoon outline (separate shapes with a
  **dark cobalt edge** instead).
- **Consistency:** make `player.png` first. For every other asset, attach it and add *"match the exact
  brushwork, palette, edge treatment and lighting of this reference image."*

## Palette (paste hexes as needed)
| swatch | hex | use |
|---|---|---|
| Night Cobalt | `#0a1530` | deepest shadow / core sky |
| Deep Blue | `#14264f` | main mid-tone blue |
| Royal Blue | `#1b3a6b` | lit blue |
| Cerulean | `#6fb7ff` | cool rim-light / highlights |
| Sky Cream | `#ffe9b0` | pale skin / cloth |
| Pale Glow | `#fff4d0` | white-hot glow cores |
| Chrome Yellow | `#ffd23a` | stars, coins, lantern |
| Soft Gold | `#f4d35e` | warm secondary glow |
| Cypress | `#0c1a10` | near-black green silhouettes |
| Grass Green | `#3a5a2e` | swirling grass |

---

# A. Gameplay objects (priority)

## 1. `player.png` — "Lumen," the Celestial Wanderer  *(aspect 3:4 portrait · pivot: between the feet · ~768×1024)*
- **Concept:** a small, plucky night-spirit explorer who carries a captured shard of starlight — a hero you instantly root for.
- **Form / silhouette:** compact chibi proportions, ~1.4 heads tall, large readable hooded head, narrow little feet, one clear focal glow (the star-lantern) so the eye locks on. Silhouette must stay legible shrunk to ~64 px.
- **Brushwork:** the cloak is painted in short curved impasto strokes that flow down the body and **curl at the hem** like the swirls in the sky; tiny stroke-flecks of light scattered on the shoulders.
- **Palette:** cloak `#14264f` deepening to `#0a1530` in the folds, edge-lit `#6fb7ff`; face pale `#ffe9b0`; star-lantern `#ffd23a` with a `#fff4d0` white-hot core; soft cyan glowing eyes.
- **Light:** cool moonlight rims the head & left shoulder in pale cerulean; the lantern throws a **warm gold under-glow** onto the lower cloak and chin — two-tone (cool top, warm bottom).
- **Framing:** three-quarter side view facing right, standing tall and ready, weight centered, **feet flat at bottom-center** with a few px of margin below.
- **Avoid:** no platform/ground, no cast shadow, no extra props, no text, no hard black outline.

## 2. `ground.png` — Night-Earth Tile  *(aspect 1:1 SQUARE · horizontally SEAMLESS · SOLID/opaque · ~512×512)*
- **Concept:** the painted earth of the night world — a single repeating terrain block.
- **Form:** fills the whole square. Top ~30% is a band of luminous **swirling grass**; the lower ~70% is packed soil. A crisp bright line of light runs along the **very top edge**.
- **Brushwork:** grass = upward-curling viridian strokes flicked with yellow tips; soil = short horizontal-ish curved strokes, denser/darker toward the bottom, with a handful of tiny buried glowing specks like stars under the dirt.
- **Palette:** grass `#3a5a2e` with `#ffd23a` tip-highlights; soil `#14264f`→`#0a1530`; top-edge rim `#fff4d0`.
- **Light:** top-lit crest, shadowed base.
- **Framing / CRITICAL:** the **left and right edges must match exactly** so copies tile horizontally with zero seam; keep the design horizontally continuous (no centered hero detail).
- **Avoid:** transparency (this one is opaque), no isolated rocks that would repeat obviously, no text.

## 3. `platform.png` — Floating Painted Ledge  *(aspect 3:1 wide · pivot: center · transparent · ~768×256)*
- **Concept:** a chunk of night-earth torn loose and hovering — same material as the ground but a floating island.
- **Form:** wide, low, rounded-rectangular top; the underside **tapers and frays into drifting brushstroke wisps** at the bottom, as if the paint is dripping off into the dark.
- **Brushwork:** swirling-grass top, curved impasto soil body; the bottom wisps trail downward in loose comet-like strokes.
- **Palette:** grass top `#3a5a2e` + `#ffd23a` tips; body `#14264f`→`#0a1530`; **chrome-yellow `#ffd23a` rim-light along the entire top edge.**
- **Light:** glowing top edge, dark belly.
- **Framing:** horizontal, much wider than tall, centered, transparent above and below.
- **Avoid:** no flat slab look, no straight machine edges, no shadow, no text.

## 4. `spike.png` — Star-Shard Spike  *(aspect ~1:1.3 tall · pivot: bottom-center · transparent · ~400×512)*
- **Concept:** a frozen splinter of night sky — beautiful but lethal. Must read as DANGER in half a second.
- **Form:** a single sharp crystal pointing **straight up**, wide base narrowing to a needle tip; clean faceted geometry (a couple of flat planes), not blobby.
- **Brushwork:** mostly smooth crystal facets but with faint swirling strokes trapped inside the glass; a bright hot glow concentrated at the very tip.
- **Palette:** body `#1b3a6b` lit facet / `#0a1530` shadow facet, thin dark `#0c1a10` rim, a luminous **white-hot tip** `#fff4d0` over `#ffd23a`, hot inner glow bleeding down the upper third.
- **Light:** lit left facet (top-left moon), shadowed right facet, self-emissive tip.
- **Framing:** base flat at **bottom-center**, point at top, vertical.
- **Avoid:** no red blood, no cartoon, no shadow, no text — communicate menace through sharpness + glow.

## 5. `coin.png` — Star Coin  *(aspect 1:1 · pivot: center · transparent · ~384×384)*
- **Concept:** the collectible — a radiant five-point star medallion you want to grab.
- **Form:** a plump 5-point star with a subtly rounded, coin-like thickness; a smaller bright star or dot in the center.
- **Brushwork:** swirling impasto around the rim (a little vortex of gold), smooth glowing center.
- **Palette:** `#ffd23a` body, `#fff4d0` center highlight, `#f4d35e` mid, soft `#ffd23a` halo bloom around it; a single crisp white **glint at top-left**.
- **Light:** front-lit, glowing, slight top-left specular.
- **Framing:** front-facing, centered, generous transparent margin (it spins in-game).
- **Avoid:** no currency symbols, no text, no shadow.

## 6. `spring.png` — Bounce Bloom  *(aspect 1:1 · pivot: bottom-center · transparent · ~400×400)*
- **Concept:** a "jump here!" bounce pad themed as a coiled star-trampoline / glowing night flower.
- **Form:** a small dark flat base, a visible **coiled spring** (2–3 curls) in the middle, topped by a wide luminous **domed pad** that clearly looks springy and inviting.
- **Brushwork:** the coil in curved metallic strokes; the dome a soft glowing surface with a swirl on top.
- **Palette:** base & coil `#14264f` with `#6fb7ff` edge-light; dome **chrome-yellow `#ffd23a`** with `#fff4d0` highlight.
- **Light:** glowing dome, cool-lit coil.
- **Framing:** base flat at **bottom-center**, pad facing straight up, vertical/centered.
- **Avoid:** ambiguity — it must obviously read as a bouncy pad; no shadow, no text.

## 7. `ring.png` — Swirl Jump-Ring  *(aspect 1:1 · pivot: center · HOLLOW transparent center · ~448×448)*
- **Concept:** a glowing ring you fly through (Geometry-Dash style), made of a Van Gogh sky-vortex.
- **Form:** a fairly thin circular **ring/torus** with a wide, **fully transparent open center** (you must see straight through the hole).
- **Brushwork:** the ring band is swirling cobalt-and-gold impasto strokes spiraling around the circumference, with a luminous glowing outer edge.
- **Palette:** band `#1b3a6b`↔`#6fb7ff` with `#ffd23a` flecks, glowing `#ffd23a`/`#fff4d0` edge bloom.
- **Light:** self-emissive ring, soft pulse-glow.
- **Framing:** perfectly front-facing circle, centered; **center transparent, not filled.**
- **Avoid:** filling the hole, any solid disc, shadow, text.

## 8. `feather.png` — Starlight Feather  *(aspect 2:3 · pivot: center · transparent · ~384×576)*
- **Concept:** a magic glide feather — pick it up to float on low gravity.
- **Form:** one long, elegant, slightly curved feather, angled gently up to the right; soft barbs, a fine quill, a sparkle trail off the tip.
- **Brushwork:** barbs in light feathery strokes; the trail in little flicked star-dust dots.
- **Palette:** barbs `#fff4d0`/`#ffe9b0` glowing to `#ffd23a` near the spine, quill `#14264f`; faint cerulean `#6fb7ff` aura.
- **Light:** luminous, gently self-glowing.
- **Framing:** single feather, centered, diagonal, plenty of transparent margin.
- **Avoid:** a whole wing, multiple feathers, shadow, text.

## 9. `goal.png` — Star-Gate Finish  *(aspect 1:2 tall · pivot: bottom-center of the pole · transparent · ~512×1024)*
- **Concept:** the triumphant END of the level — unmistakably "you made it."
- **Form:** a slender dark **pole** rising from the bottom, crowned by a brilliant **swirling starburst** wrapped in a vortex halo, with a glowing pennant/banner streaming to the right and soft light beams.
- **Brushwork:** the starburst & halo in bold swirling impasto; the pennant in flowing strokes.
- **Palette:** pole `#0c1a10`/`#14264f`; starburst **chrome-yellow `#ffd23a`** with `#fff4d0` core; pennant cobalt-and-gold; cerulean `#6fb7ff` beams.
- **Light:** the star is the brightest thing on screen; warm bloom radiating out.
- **Framing:** vertical, **pole base at bottom-center**, star up top.
- **Avoid:** checkered racing flags, text/"FINISH", shadow.

## 10. `enemy.png` — Night Imp (walker)  *(aspect 1:1 · pivot: bottom-center · facing right · transparent · ~448×448)*
- **Concept:** the goomba-equivalent patroller — cute but clearly hostile; you stomp it from above.
- **Form:** a round blob of living shadow-paint with a slightly grumpy face, two stubby little feet, no arms; a strong round silhouette readable when small.
- **Brushwork:** the body swirls like a tiny dark vortex; a soft fuzzy painted edge (not hard outline).
- **Palette:** body `#14264f`→`#0a1530`, swirling `#1b3a6b` accents; two glowing **sour-yellow `#ffd23a` eyes**, a small dark frown; faint `#6fb7ff` rim-light on top.
- **Light:** cool top-left rim, glowing eyes as the focal point.
- **Framing:** three-quarter side view facing **right**, **feet at bottom-center**.
- **Avoid:** weapons, gore, text, shadow; keep it charming-menacing, not scary.

---

# B. Background pieces (optional — kept separate so I can parallax & animate them)

## 11. `bg-moon.png` — Crescent Moon  *(1:1 · transparent · ~512×512)*
- A glowing crescent moon, thick `#ffd23a`/`#fff4d0` impasto, ringed by **concentric swirling halo strokes** radiating outward into a soft `#f4d35e` bloom that fades to transparent. Centered. Brushwork swirls; no hard edge; no text.

## 12. `bg-star.png` — Radiant Star  *(1:1 · transparent · ~384×384)*
- One radiant night-sky star: a bright `#fff4d0` core, a `#ffd23a` glowing halo ring, and short swirling brushstroke rays curling around it (like the iconic Starry-Night stars). Soft bloom fading to transparent. Centered; no text.

## 13. `bg-swirl.png` — Sky Vortex  *(1:1 · semi-transparent · OPEN center · ~640×640)*
- A large swirling sky-current: long curling strokes of `#1b3a6b`, `#6fb7ff` and `#fff4d0` spiraling around an **open faint center**, semi-transparent and luminous, designed to **slowly rotate** behind the level. Circular, centered, edges fading to transparent; no hard rim, no text.

## 14. `bg-cypress.png` — Cypress Tree  *(aspect 1:3 tall · base at bottom · transparent · ~384×1152)*
- A tall dark cypress like in *The Starry Night*: a **flame-shaped** tree of upward-curling impasto strokes twisting toward the sky, very tall and narrow, near-black green `#0c1a10` with faint `#1b3a6b` cobalt edge-light and a few `#3a5a2e` flecks. Base at bottom-center, tapering point at top; transparent around it; no text.

## 15. `bg-village.png` — Sleepy Village  *(aspect 3:1 wide · buildings on bottom edge · transparent above · ~1152×384)*
- A horizon strip of a sleepy village: clustered simple **dark cobalt cottages** `#0a1530`/`#14264f` with one tall **church steeple**, a few tiny warm **glowing windows** `#ffd23a`, all in swirling brushwork. Buildings sit on the bottom edge; fully transparent above the rooftops; no text.

## 16. `bg-hills.png` — Rolling Hills  *(aspect 4:1 wide · on bottom edge · transparent above · ~1536×384)*
- A wide band of gentle rolling night hills in swirling `#14264f`/`#1b3a6b` and `#3a5a2e` strokes, crests slightly lighter (`#6fb7ff` rim), curving horizontally across the frame. Silhouette strip on the bottom edge, transparent above; should read continuously if I repeat it; no text.

---

## Optional: animation
Want movement? Generate `player_run1..4.png` and `player_jump.png` in the **identical pose, scale and framing**
as `player.png` (only legs/lantern shift) and I'll add a frame animator. One static `player.png` is fine to start.

## Priority order
player → spike → platform → ground → coin → enemy → goal → spring → ring → feather → background set.
