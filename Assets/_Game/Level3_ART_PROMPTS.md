# The Swing — Complete Art Briefs for Level 3 (for Gemini image gen)

Level 3 is Jean-Honoré Fragonard's *The Swing* (*Les Hasards heureux de l'escarpolette*, 1767)
brought to life as a playable rococo garden. The level is **fully built and physics-QA'd in
code** — every object renders as a procedural placeholder until this art folder is filled. This
document is the complete set: **28 assets** covering terrain, the rose-briar hazard, the three
signature pendulum toys (swing-seat, fountain-jet, warp arbor), note-blocks, all three creatures,
the goal, a nine-layer rococo-garden backdrop, and the avatar.

**Everything is already wired in code.** Save each image with the **exact base filename** into
`Assets/_Game/Resources/Level3/` (create the folder; `.jpeg` or `.png`), stop Play, let Unity
recompile, press Play. Any file you skip keeps its procedural fallback, so generate in any order.

---

## THE LEVEL — what the art has to serve

Level 3's verb is **the pendulum**, and its soul is **momentum kept alive**. The redesign is the
classic teach → develop → twist → climax arc:

1. **Act 1 — teach.** Warm-up precision hops over briar, a free note-block bounce, then the same
   blocks bridging a thorn pit, and finally the **swing-seat** (`swing-seat`): a ridden pendulum
   you board at its near apex, ride across the chasm, and leap from the far apex. Joy before
   danger.
2. **Act 2 — the garden climbs.** A floor-blade alley, a hedge-patrolled landing, a spring fling,
   a **spring tower** and a **fountain-jet lift** (`jet`) that elevate you up the garden terraces,
   sky-hops with a high coin, a dove crossing the arc, a long swing-ride over a deep chasm, and a
   committed stair-down past a non-stompable hedgehog.
3. **Act 3 — the twist: route choice.** A **warp arbor** (`portal`): jump into the trellis arch
   to skip a blade lane entirely (the fast line) — or run the lane to claim the coin hidden in
   the pocket between two sweeping blades (the 100% line). The fast line and the completionist
   line are *different lines*. Then a note-block-and-blade bounce chain and a tightened blade alley.
4. **Climax — THE SWING.** The painting's own swing, made playable: a grand seat pendulums over a
   briar chasm, its near apex grazing the bower deck so you board from home ground and commit at
   the far apex. The leap threads a swinging blade, grazes the coin on the descent, and lands on a
   note-block relay that bounces you home.
5. **Victory lap** at full tilt to the flowered finish.

**The one visual law of the level: warm stone, terrace marble and swing-wood = SAFE footing; the
thorned rose-briar = DEATH.** Every solid surface you can stand on is sun-warmed sandstone, mossy
balustrade marble, or weathered swing-timber. Everything that is **dark rose-cane bristling with
vermilion thorn-tips** is trying to kill you. The player must make that read at a full sprint,
pixelated, in a sixteenth of a second — so keep the safe world warm and golden-brown, and reserve
the **briar's hot vermilion `#ff3b56`** strictly for the lethal thorns.

---

## THE VIBE — paste this whole paragraph at the top of every Gemini prompt

> This artwork is for a 2D speedrun platformer level set *inside* Jean-Honoré Fragonard's *The
> Swing* (1767) — the French Rococo masterpiece brought to life as a playable garden world. The
> visual language is **late Rococo oil painting**: lush, feathery, sun-dappled foliage rendered in
> soft loaded-brush strokes, warm golden light breaking through a deep shadowy bower, frothy
> confectionary highlights, and the famous **powder-pink, cream, and gold** palette set against
> **deep verdant greens** — NOT flat vector art, NOT anime, NOT photorealism, NOT woodblock. Edges
> are soft and painterly with glazed shadow and creamy impasto highlights; foliage is built from
> countless small feathered leaf-strokes the way Fragonard painted his trees; stone and marble are
> smooth, cool and luminous. The palette is the painting's own: deep garden greens `#16240f`
> `#2a4019` `#426033`, warm sandstone and timber `#3a2b1c` `#5e4830` `#8a6a44`, luminous Rococo
> pink `#f4a6c0`, ivory cream `#fff0dd`, gilt gold `#ffe08a` `#ffd86a`, and a stolen shaft of warm
> sunlight `#ffe69a`. **In this game warm stone, marble and wood mean SAFE footing, and the
> thorned rose-briar means death** — so the lethal hazard is always dark rose-cane bristling with
> hot vermilion `#ff3b56` thorn-tips, and that hot red appears NOWHERE except on things that kill
> you. Light is warm, golden and directional, pouring from the upper RIGHT (the direction of
> travel), with deep cool-green shadow pooling in the lower left of every form — but keep
> silhouettes clean and readable, because the game pixel-downscales at runtime, so render crisp
> bold shapes with confident value contrast; soft Rococo froth survives pixelation only if the big
> shapes read first. The mood is **a secret sunlit garden, overgrown and a little wild — frivolous,
> luminous, and quietly dangerous**: a place of swings and fountains where the roses bite. **ABSOLUTE
> RULE: depict NO human figures, NO people, NO bodies, NO faces, NO hands, NO cherubs, putti, or
> statue-figures of any kind — the famous girl, her lover and the cherubs of the original painting
> are ALL gone; the swing hangs EMPTY, the statuary is non-figurative urns and fountains only.**
> Animals (a songbird, a hedgehog, a garden critter) are allowed and welcome. The game engine
> applies a pixel-downscale filter at runtime, so render crisp graphic source art with strong
> value structure; bold readable shapes survive pixelation better than anything. I will attach the
> original painting as the style reference — **match its exact palette, its golden light, its
> feathery foliage and its frothy pink-and-cream rococo touch** so every asset looks cut from the
> same canvas.

**Attach `Assets/_Game/Resources/Paintings/the-swing.jpg`** to every generation as the style
reference (it's the actual painting). As your generated set grows, also attach 1–2 finished assets
(e.g. `ground.jpeg` and `player.jpeg`) so the set stays self-consistent in brushwork and palette.

---

## Global delivery rules (every asset)

- **Background: flat magenta `#ff00ff` fill — NOT a checkerboard, NOT white** (the loader keys it
  out at runtime). Exception: `ground` and `wall` are full-bleed opaque tiles with no background.
- **One subject, centered. No scenery, no ground line, no cast shadow on the ground, no text, no
  frame/border, no signature.** (The painting is busy; each asset is ONE clean object on magenta.)
- Aspect ratio and orientation matter; exact pixel counts are quality suggestions only.
- Anything with a face or a facing direction faces **RIGHT** (the direction of travel).
- Silhouettes must stay readable shrunk to ~48–64 px — lead with the big shape, then add froth.
- **No humans, no body parts, no faces, no cherubs/putti, no figurative statues — ever. The swing
  is always EMPTY.** (Worth repeating inside every single prompt.)

## Palette

| swatch | hex | use |
|---|---|---|
| Bower Deep | `#16240f` | deepest foliage shadow, darkest keylines |
| Garden Green | `#2a4019` | foliage body, hedge mass |
| Leaf Lit | `#426033` | sunlit leaf tops, mid-green |
| Earth Brown | `#3a2b1c` | soil, deep timber — SAFE |
| Sandstone | `#5e4830` | path stone, terrace body — SAFE |
| Warm Stone Lit | `#8a6a44` | sunlit stone & wood top edges — SAFE |
| Rococo Pink | `#f4a6c0` | the avatar, blossoms, UI accent (NEVER footing, NEVER danger) |
| Ivory Cream | `#fff0dd` | froth highlights, marble, foam of fountains |
| Gilt Gold | `#ffe08a` | coins, gilt fittings, ribbon |
| Sun Shaft | `#ffe69a` | the stolen sunlight, glows |
| Briar Cane | `#3a1320` | the hazard's dark thorned cane body |
| Thorn Vermilion | `#ff3b56` | lethal thorn-tips & danger ONLY |

**Color budget across the whole level: ~55% verdant green (foliage, shadow), ~30% warm
stone/timber/earth (the safe footing), ~15% pink-cream-gold (accents, light, blossoms).** The hot
vermilion `#ff3b56` is a thin spice reserved entirely for the briar's killing tips — when in doubt,
push the safe world warmer and browner and let only the thorns burn red.

---

# THE SWING — the level's signature toy (context for several assets)

The level's identity is the **ridden pendulum**. Three assets build it, and they recur:

- **`swing-seat`** — an empty wooden swing-seat on two ropes. In game it is a *platform that
  swings*: the player jumps onto the seat at one end of its arc, rides it across a thorn chasm, and
  leaps off at the far end. It is the heart of Act 1, Act 2 and the climax. **It is always empty —
  no rider, ever.** The seat is the only thing the player stands on over the briar, so its board
  must read instantly as warm, solid, trustworthy wood.
- **`jet`** — a column of fountain water with a frothy crown. In game it is a *vertical* swinging
  platform — a garden elevator: you step onto the foaming crown at the basin lip, ride it up as the
  jet surges, and step off at the crest onto the terrace above. The crown is the footing; the
  column of water hangs below it.
- **`portal`** — a rose-twined trellis arbor. In game it is a *warp gate*: jump up into the arch and
  you're whisked past the blade lane. Two arches per pair (entry low, exit high); they read as a
  matched set of garden arbors humming with rococo-gold light.

None of these ever show a person. The swing's empty seat — the held breath of the painting, the
instant *after* she has gone — is the emotional core of the whole level.

---

# A. Terrain & hazards

## 1. `ground.jpeg` — Sunlit Garden Path  *(1:1 SQUARE · horizontally SEAMLESS · SOLID/opaque · ~512×512)*

- **Concept:** the walkable world — a warm sandstone garden path edged with sun-dappled turf, the
  safe earth of the whole level. Everything you can stand on is this warm stone-and-soil.
- **Form:** fills the whole square, no background. The **top ~22%** is a band of close-cropped
  sunlit lawn — small feathery grass strokes, `#426033` lit to a warm `#6f8a48` along a crisp pale
  top edge (this is the walkable surface). Below it, the path body: smooth warm **sandstone**
  `#5e4830` blocks with soft mortar shadows and a few scattered fallen pink petals and tiny gold
  flecks of broken sunlight; the lowest third sinks to cool shadowed soil `#3a2b1c` → `#16240f`.
- **Style:** soft Rococo oil — glazed shadows, creamy lit edges, feathered grass; bold enough value
  steps that the band structure survives pixelation.
- **Palette:** lawn `#426033`→`#6f8a48` with `#fff0dd` lit line; stone `#8a6a44`→`#5e4830`; shadow
  soil `#3a2b1c`→`#16240f`.
- **CRITICAL:** the **left and right edges must match exactly** for seamless horizontal tiling; keep
  the grass line and stone courses horizontally continuous, stagger the block joints, no centered
  focal detail, no single obvious repeating flower.
- **Avoid:** any hot pink or vermilion on this tile (warm stone = safe is the law), photographic
  grass/stone, perspective, cast shadows, text.

## 2. `ground-edge.jpeg` — Broken Terrace Stone  *(tall ~2:3 · ~384×576 · overlays the LEFT end of every ledge; auto-mirrored for right ends)*

- **Concept:** where the garden path breaks off over the chasm — a crumbled sandstone terrace edge,
  roots and a trailing tendril of ivy spilling from the wound.
- **Form:** designed for the **LEFT edge** of a ground block: the **right half matches `ground.jpeg`
  exactly** — lawn band over sandstone (it overlaps the tile and must blend invisibly); the left
  half is the break — stone blocks snapped at staggered lengths, exposed pale broken faces, a few
  roots and one trailing ivy strand curling left off the lip, with a couple of pink petals caught
  in it.
- **Palette:** identical to `ground.jpeg`; fresh broken stone faces a touch paler `#a0825a`; a thin
  `#fff0dd` sunlit edge wraps the corner; ivy in `#2a4019`.
- **Framing:** vertical; content bleeds off the RIGHT edge; magenta to the LEFT beyond the break;
  nothing below the bottom edge.
- **Avoid:** a ruler-straight break, identical block lengths (this appears at dozens of ledge ends),
  any thorn/briar on the safe stone, brightness exceeding the lawn band, text.

## 3. `wall.jpeg` — Mossy Balustrade Marble  *(1:1 square · ~512×512 · MUST tile seamlessly BOTH directions · opaque)*

- **Concept:** the cool marble face of a terrace retaining wall — the texture for the spring-tower
  column and the fountain-lift's terrace rise. It must recede quietly behind gameplay.
- **Form:** all-over material filling the square: smooth pale balustrade marble in subtly varying
  blocks, slim cool shadow seams between them, sparse quiet detail — a faint moss bloom `#2a4019`
  in a corner, one hairline crack, a whisper of gold lichen; soft glaze over everything.
- **Palette:** narrow, recessive, cool-warm neutral: `#8a6a44` warm-stone down to `#3a2b1c` shadow,
  with rare `#fff0dd` marble highlight and `#2a4019` moss; seams `#16240f`-dark.
- **Light:** flat, even — NO single light direction (it tiles anywhere, including under overhangs).
- **CRITICAL:** all four edges wrap seamlessly into their opposites; avoid features that form visible
  rows/grids when repeated; keep contrast LOW so it never competes with the playfield.
- **Avoid:** hot pink, vermilion, thorns, statues/figures in relief, brightness, text.

## 4. `platform.jpeg` — Floating Garden Ledge  *(wide 3:1 · pivot center · transparent · ~768×256)*

- **Concept:** a fragment of garden terrace adrift on a cushion of foliage — the level's every
  airborne ledge. It must read warm-stone-on-top (safe) at a glance.
- **Form:** a wide, low horizontal shard: a sunlit lawn-and-sandstone top band (walkable, flat and
  clean), and below it the stone tapers into a hanging tangle of feathery green leaves, a trailing
  ivy strand and one or two pink blossoms peeking from the underside; **both ends round off softly**,
  the right end a touch finer.
- **Style:** soft Rococo brush; the foliage is clearly BELOW the walk surface — the top edge is 100%
  warm stone and grass.
- **Palette:** lawn `#426033`→`#6f8a48` lit line; stone `#8a6a44`→`#5e4830`; underside foliage
  `#2a4019`→`#16240f`; tiny `#f4a6c0` blossoms.
- **Framing:** horizontal, much wider than tall, centered, magenta above and below.
- **Avoid:** thorns or hot red anywhere (must read safe), foliage spilling onto the TOP surface, a
  symmetric slab look, text.

## 5. `spike.jpeg` — Rose Briar Thorn  *(~1:1.3 tall · pivot bottom-center · transparent · ~400×512)*

- **Concept:** the level's lethal hazard — a single barbed shoot of rose-briar risen from the
  ground, beautiful and vicious. The garden's "blue water," its killing element.
- **Form:** one upward-curving cane of dark rose-wood, bristling with sharp hooked thorns along its
  length and crowned by a single half-open blood-red rose; 2–3 smaller thorned offshoots curl from
  the base; a couple of fallen petals frozen beside it.
- **Palette:** dark cane `#3a1320` with deep-green leaf accents `#16240f`; the rose a deep crimson
  `#a01830`; and **a burning vermilion `#ff3b56` flush concentrated on every thorn-tip and the
  rose's heart** — the game's universal "lethal" signal. The hot red must be unmistakable and must
  appear nowhere else in the level except the briar.
- **Framing:** base flat at bottom-center, thorns/bloom up, vertical, centered. The silhouette must
  read as a spiked, dangerous shape at 48 px (NOT as a friendly flower — barbs first, bloom second).
- **Avoid:** soft pastel pink (that's the player's color — keep the danger DEEP red with hot tips),
  warm stone or wood anywhere on it, a tidy bouquet look, more than one rose, symmetry, text.

## 6. `pool.jpeg` — The Briar Thicket  *(wide ~2:1 · horizontally SEAMLESS · ~1024×512)*

- **Concept:** the death-thicket that fills every deep pit — a dense, impassable bramble of
  rose-briar, gorgeous and absolutely fatal. (It replaces spike rows in the deepest pits — the
  fountain basin and the chasms — so it shares their danger language.)
- **Form:** a seething mat of interlaced dark thorned canes filling the whole tile below a thin strip
  of flat magenta at the very top (the air above). The **top surface line** is a row of hooked
  thorns and half-open blood-red roses, all leaning consistently one way; below, layered tangles of
  cane sink from `#3a1320` to `#16240f` near-black at the bottom, with vermilion thorn-tips
  glinting throughout.
- **CRITICAL:** LEFT and RIGHT edges must match exactly (it tiles into one continuous thicket); keep
  the thorn/rose pattern irregular enough that the repeat isn't obvious.
- **Palette discipline:** the body is dark cane and crimson rose; the **vermilion `#ff3b56`** lives
  only as sharp glints on the thorn-tips along the top line — enough to scream "death," never a wash.
- **Avoid:** soft pastel pink, any stone/wood/path, photographic foliage, petals drifting above the
  surface strip, text.

---

# B. Interactive objects

## 7. `note-block.jpeg` — Topiary Bouncer  *(1:1 square · pivot center · transparent · ~448×448 · tiles edge-to-edge in a row)*

- **Concept:** a springy clipped-boxwood cube bound in gilt ribbon — land on it and it launches you
  skyward. The garden's trampoline. (In game these sit in 1–2 cell runs and flash gold on bounce.)
- **Form:** a rounded-cube of dense, neatly-clipped topiary boxwood, its top face slightly domed and
  springy-looking, wrapped by a crossed **gilt-gold ribbon** with a small bow on the front; a few
  tiny pink buds dot the green; the whole thing reads bouncy and inviting (the opposite of the
  briar).
- **Style:** soft feathery boxwood texture, crisp gold ribbon with a bright `#fff0dd` highlight; the
  top face catches the warm light.
- **Palette:** boxwood `#2a4019`→`#426033` lit, ribbon `#ffe08a`/`#ffd86a` with `#fff0dd` glint, buds
  `#f4a6c0`.
- **Framing:** centered, fills most of the square so it abuts cleanly when several tile side-by-side;
  magenta in the corners only. Keep the design roughly **left-right symmetric** so a row of them
  looks continuous.
- **Avoid:** thorns, hot red, a hard mechanical block look, text. It must read SAFE and springy.

## 8. `swing-seat.jpeg` — The Empty Swing  *(tall ~2:3 · pivot BOTTOM-center · transparent · ~512×768)* — THE signature asset

- **Concept:** Fragonard's swing, abandoned mid-arc — a polished wooden seat-board on two ropes,
  swinging empty through the bower. This is the single most important and most-seen object in the
  level: the player stands ON the seat-board and rides it across the chasm.
- **Form:** a horizontal **wooden plank seat** at the BOTTOM of the image (this is the footing — it
  must read as solid, warm, trustworthy wood, wide and flat on top), with two **ropes rising from
  its ends** up to a single anchor point near the top (a bough or a gilt ring), the ropes draped
  with a trailing garland of leaves and a few pink roses and one fluttering gold ribbon. The seat
  is **empty** — gently tilted as if just vacated. A faint warm sunlight catches the front rope.
- **Style:** the seat-board is crisp, lit, confident wood (`#8a6a44` top → `#5e4830`); the ropes are
  twisted cord in cream and shadow; the floral garland is soft and feathery; the whole thing glows
  a little against the dark bower.
- **Palette:** wood `#8a6a44`/`#5e4830`/`#3a2b1c`, ropes `#fff0dd`/`#cbb089`, garland `#2a4019` with
  `#f4a6c0` roses and a `#ffe08a` ribbon.
- **Framing:** vertical; the **seat-board's top sits at the bottom-center pivot** (the engine seats
  it on the platform top), ropes rising the full height above; magenta all around. The board's top
  surface must read clearly as standable.
- **Avoid (CRITICAL):** **any rider, person, figure, body, leg, shoe, or hand — the swing is EMPTY.**
  No thorns, no hot red, no clutter on the seat surface (it's a clean footing). Don't let the ropes
  read as a cage.

## 9. `jet.jpeg` — Fountain Water-Jet  *(tall ~2:3 · pivot TOP-center · transparent · ~448×768)*

- **Concept:** a surging jet of fountain water with a frothy crown — the garden's elevator. The
  player rides the foaming crown up and down; the crown is the footing, the water-column hangs below.
- **Form:** at the **TOP**, a wide rounded **crown of churning white-cream foam** (the standable
  surface — domed, frothy, catching the light), and from it a tapering **column of clear
  blue-green-tinged water** falling away downward, breaking into droplets and a fine spray mist at
  the bottom; a few sun-sparkles `#fff0dd` in the foam.
- **Style:** soft Rococo water — luminous, translucent, frothy at the crown, with confident value so
  the crown reads as a solid cap from a distance.
- **Palette:** foam crown `#fff0dd`/white with `#ffe69a` sun-glints; water column a cool
  `#9fc6c0`→`#5e7a6e` (a green-tinged garden water, NOT the sea's Prussian blue — keep it gardeny),
  spray fading to magenta at the very bottom.
- **Framing:** vertical; the **foam crown's top sits at the top-center pivot**; the water column
  hangs below it; magenta around. Feather the bottom of the spray so it fades rather than cutting.
- **Avoid:** hot red/pink, hard outlines, a solid pillar look (it must read as water), fish or
  figures, text.

## 10. `spring.jpeg` — Coiled Vine Bloom  *(1:1 · pivot BOTTOM-center · transparent · ~384×448)*

- **Concept:** a tightly-coiled springy vine topped by a fat blossom — step on it and it flings you
  straight up. The single-bounce cousin of the topiary block.
- **Form:** a low, fat **coiled green tendril** (like a clock-spring of vine) seated on the ground,
  crowned by a broad upturned **golden-cream flower cup** that reads as the bouncy pad on top; two
  small leaves at the base.
- **Palette:** coil `#2a4019`→`#426033`, flower cup `#ffe08a`/`#fff0dd` with a `#f4a6c0` blush at the
  rim.
- **Framing:** base flat at bottom-center, blossom up, centered, magenta around.
- **Avoid:** thorns, hot red, a metal-spring look, text. Reads safe and bouncy.

## 11. `portal.jpeg` — Rose Trellis Arbor  *(tall ~3:4 · pivot center · transparent · ~512×640)*

- **Concept:** a garden arbor — a free-standing trellis arch twined with roses and humming with a
  soft gold rococo light — the level's warp gate. (Two appear per pair; the same art serves both.)
- **Form:** a tall arched **wooden lattice trellis**, thickly twined with leafy vine and pink roses,
  framing an **open archway** whose interior glows with a warm, swirling gold light (`#ffe69a` →
  `#fff0dd` center) — clearly a magical threshold, inviting and a little uncanny. A few petals drift
  in the glow.
- **Style:** the trellis is soft sunlit wood; the rose vine feathery; the glow is a gentle radial
  bloom that fades to magenta at the image edges.
- **Palette:** trellis `#8a6a44`/`#5e4830`, vine `#2a4019` with `#f4a6c0` roses, glow `#ffe69a`/
  `#ffd86a`/`#fff0dd`.
- **Framing:** vertical, the arch centered; the glowing opening is the focal point; magenta around.
- **Avoid:** a hard door/portal-ring sci-fi look, thorns/hot-red (this is safe and magical), any
  figure inside the arch, text.

## 12. `saw.jpeg` — The Briar Wheel  *(1:1 square · pivot center · transparent · ~512×512 · spins fast in-game)*

- **Concept:** the level's moving blade — a spinning wheel of braided rose-cane and hooked thorns,
  a vicious garden buzz-saw. (It both skims the floor and sweeps vertically over chasms.)
- **Form:** a roughly circular, radially-symmetric wheel of interwoven dark thorned canes with
  sharp hooked barbs jutting all around the rim like saw-teeth, a tight crimson rose at the hub;
  built to read as a menacing spinning disc from any rotation.
- **Style:** dark and barbed, with the thorn-tips catching hot light; designed so that when spun at
  speed it blurs into a lethal ring.
- **Palette:** cane `#3a1320`→`#16240f`, hub rose `#a01830`, and **vermilion `#ff3b56` on every
  barb-tip** (matching `spike`/`pool` — all the level's death shares this hot red).
- **Framing:** centered, fills most of the square, radially balanced; magenta in the corners.
- **Avoid:** soft pastel pink, a metal-saw look, an obvious "up" orientation (it spins — keep it
  rotationally even), text.

## 13. `coin.jpeg` — Gilt Rococo Medallion  *(1:1 · pivot center · transparent · ~384×384)*

- **Concept:** the collectible — a small ornate gold medallion, a scrap of the garden's gilded
  finery. (Exactly 3 per level, each in a nasty spot.)
- **Form:** a round, richly-embossed **gold medallion** with a scalloped rococo rim and a tiny
  embossed rose or scroll at its center, a bright `#fff0dd` specular glint upper-left.
- **Palette:** `#ffe08a`→`#ffd86a` gold with `#8a6a44` shadow in the relief and a white-cream glint.
- **Framing:** centered, magenta around; reads as a crisp gold disc at 32 px.
- **Avoid:** hot red/pink, a flat coin look (give it rococo ornament), text/numerals.

## 14. `goal.jpeg` — The Flowered Finish Arch  *(tall ~1:2 · pivot bottom-center · transparent · ~512×1024)*

- **Concept:** the level's end marker — a tall garden archway heavy with roses and a draped gold
  ribbon, the triumphant exit of the bower.
- **Form:** a slender vertical **arched garland post** — a wooden pole wound top-to-bottom with leafy
  vine and abundant pink-and-cream roses, crowned by a billowing **gilt ribbon/banner** at the top
  that catches the sun; the base plants firmly on the ground.
- **Palette:** vine `#2a4019` with `#f4a6c0`/`#fff0dd` roses, ribbon `#ffe08a`/`#ffd86a`, pole
  `#5e4830`.
- **Framing:** tall and narrow, pole base at bottom-center, banner at top; magenta around.
- **Avoid:** thorns/hot-red, a checkered race-flag, any figure, text.

---

# C. Creatures (charming, hostile, and emphatically not human)

## 15. `enemy.jpeg` — Garden Beetle (walker)  *(1:1 · pivot bottom-center · faces RIGHT · transparent · ~448×448)*

- **Concept:** the ground patroller — a plump, grumpy rose-chafer beetle trundling the paths. Round,
  stompable, charming-menacing. The walker family's Level-3 cousin.
- **Form:** a round, low beetle with a domed iridescent shell, two stubby legs, small frowning face,
  little antennae; clearly squashable on top (a smooth rounded back). Faces right.
- **Palette:** shell deep green-gold `#2a4019`→`#426033` with a warm `#8a6a44` iridescent sheen and a
  `#fff0dd` rim; eyes hot `#ffe08a`. Keep it warm/green — NOT pink, NOT vermilion (it's not the
  hazard).
- **Framing:** bottom-center pivot, faces right, magenta around.
- **Avoid:** thorns/hot-red on the body (reserved for hazards), gore, a flat bug-clipart look, text.

## 16. `enemy-spiky.jpeg` — Garden Hedgehog  *(1:1 · pivot bottom-center · faces RIGHT · transparent · ~448×448)*

- **Concept:** the un-stompable hazard mob — a bristling hedgehog whose back is a bed of spines you
  must NOT land on. Same family as the beetle, but armored on top.
- **Form:** a round hedgehog, low to the ground, its whole back and crown densely covered in sharp
  spines; small face and feet at the front; clearly "no safe top." Faces right.
- **Palette:** body warm brown `#5e4830`, spines dark `#3a1320` tipped with **hot vermilion
  `#ff3b56`** (the "don't touch" signal, matching the briar — the only mob allowed the hazard red,
  because it kills like the briar); eyes `#ffe08a`.
- **Framing:** bottom-center pivot, faces right, magenta around.
- **Avoid:** a soft/rounded landable top (it must read dangerous), cuteness that hides the spines,
  text.

## 17. `enemy-flyer.jpeg` — Garden Dove  *(1:1 · faces RIGHT · transparent · ~512×512 · bobs and drifts in-game)*

- **Concept:** the flyer — a plump white dove that drifts across the jump arcs. The player can stomp
  it mid-air, so its back must read landable. (Fragonard's garden is full of fluttering birds.)
- **Form:** a soft, round dove in mid-glide, wings spread mid-beat, a small ribbon or sprig of rose
  in its beak (a rococo touch), facing right; a rounded stompable crown.
- **Palette:** cream-white `#fff0dd` body with soft `#f4a6c0` blush in the wing shadows and a
  `#ffe08a` beak; gentle, luminous, NOT a hazard color.
- **Framing:** centered, faces right, magenta around; reads as a bird at 48 px.
- **Avoid:** thorns/hot-red, a menacing raptor look, text. Charming, not scary.

---

# D. Backdrop & atmosphere (the garden's depth — no figures, ever)

These scroll in parallax behind the playfield. They are dimmed and pushed back by an atmosphere
scrim and vignette at runtime, so render them at full confidence — the engine will settle them.

## 18. `bg-canopy.jpeg` — The Bower Ceiling  *(wide ~3:1 · pivot TOP-center · transparent · ~1344×448 · hangs from the frame top, feathered ends, repeated)*

- **Concept:** the leaf ceiling of the overgrown bower — a continuous band of dense, sun-shot
  foliage hanging down from the top of the world over everything.
- **Form:** a horizontal band of layered feathery treetop foliage seen from below, hanging DOWN from
  the top edge; gaps of warm sky-glow `#ffe69a` break through between leaf clusters; a few trailing
  vines and pink blossoms dangle from the underside.
- **Palette:** `#16240f`→`#2a4019`→`#426033` greens with `#ffe69a` light-gaps and `#f4a6c0` blossom
  dots.
- **Framing:** the foliage fills from the TOP and hangs down; **feather the LEFT and RIGHT edges** so
  copies blend seamlessly side-by-side; magenta along the bottom.
- **Avoid:** a hard bottom edge, figures, text. Keep the underside soft and irregular.

## 19. `bg-swing.jpeg` — The Distant Swing  *(tall ~3:4 · pivot TOP-center · transparent · ~512×640 · sways slowly, sparse)*

- **Concept:** THE swing of the painting, seen deep in the garden — empty, still gently swaying,
  hung from a high bough. It is the emotional anchor of the backdrop: found, not plastered
  everywhere.
- **Form:** an **empty wooden swing-seat on two long ropes**, hung from a high branch, drifting at a
  slight tilt, wreathed in the feathery foliage and a few roses; rendered softer and more atmospheric
  than the gameplay `swing-seat` (this one is scenery, far away).
- **Palette:** wood `#5e4830`, ropes `#cbb089`, foliage `#2a4019`, roses `#f4a6c0`; a warm sun-glow
  behind it.
- **Framing:** vertical, ropes from the TOP pivot (it sways from the anchor); magenta around.
- **Avoid (CRITICAL):** **any rider/figure/person — empty swing only.** No hot red, no text.

## 20. `bg-hedge.jpeg` — Garden Hedge Wall  *(wide ~3:1 · pivot bottom-center · transparent · ~1344×448 · feathered ends, two parallax bands, repeated)*

- **Concept:** the clipped hedge walls of the garden maze — a tall green band rising from the floor
  line, the middle-distance scenery the player runs past.
- **Form:** a horizontal band of dense, neatly-clipped-but-overgrown hedge, its top edge an
  irregular sunlit ripple of leaf-clumps, with the odd rose or topiary ball bulging out; solid green
  mass below.
- **Palette:** `#2a4019` body, `#426033`→`#6f8a48` sunlit top ripple, occasional `#f4a6c0` rose,
  shadow `#16240f` at the base.
- **Framing:** rises from the BOTTOM; **feather LEFT and RIGHT edges** for seamless repeats; magenta
  above the hedge top.
- **Avoid:** a flat wall look (give the top a living ripple), figures, text.

## 21. `bg-fountain.jpeg` — Marble Garden Fountain  *(tall ~3:4 · pivot bottom-center · transparent · ~512×640 · landmark, spaced far apart)*

- **Concept:** a tiered marble fountain punctuating the hedge line — a non-figurative garden
  landmark, plashing softly in the distance.
- **Form:** a classic **tiered stone fountain**: a wide basin, a central stem, an upper dish, with
  arcs of pale water spilling between tiers and a soft mist at the base; flanked by a little
  foliage. **Strictly architectural — no cherubs, no statues, no figures of any kind.**
- **Palette:** marble `#fff0dd`/`#8a6a44` shadow, water `#9fc6c0` with `#fff0dd` froth, foliage
  `#2a4019`.
- **Framing:** vertical, base at bottom-center, magenta around.
- **Avoid (CRITICAL):** **any figurative statue, cherub, putto, or carved person — urn/dish/stem
  forms only.** No hot red, no text.

## 22. `bg-urn.jpeg` — Stone Garden Urn  *(tall ~3:5 · pivot bottom-center · transparent · ~384×640 · landmark, between fountains)*

- **Concept:** a weathered ornamental urn overflowing with roses, the garden's quiet statuary — the
  non-figurative stand-in for the painting's cherubs.
- **Form:** a tall fluted **stone urn/vase** on a plinth, brimming with a cascade of feathery roses
  and trailing vine spilling over its lip; soft moss in the carved grooves.
- **Palette:** stone `#8a6a44`/`#fff0dd` highlight/`#3a2b1c` shadow, roses `#f4a6c0`/`#fff0dd`,
  vine `#2a4019`.
- **Framing:** vertical, plinth base at bottom-center, magenta around.
- **Avoid (CRITICAL):** **no carved figures/cherubs on the urn — fluting and scrollwork only.** No
  hot red, no text.

## 23. `bg-bloom.jpeg` — Rose Cluster  *(1:1 · pivot center · transparent · ~384×384 · drifts at mid-height, sways)*

- **Concept:** a free-floating cluster of roses and leaves — the soft pink confetti of the garden
  that drifts in the mid-depth, the painting's famous froth of pink.
- **Form:** a loose, rounded **bouquet-cluster of full-blown pink-and-cream roses** with feathery
  leaves, soft and luminous, catching the warm light.
- **Palette:** `#f4a6c0`/`#fff0dd` roses, `#2a4019`/`#426033` leaves, `#ffe69a` light.
- **Framing:** centered, soft edges fading toward magenta, magenta corners.
- **Avoid:** thorns/hot-red (this is decorative pink, the safe/soft pink — NOT the briar), hard
  outlines, text.

## 24. `bg-rays.jpeg` — Stolen Sunlight  *(tall ~3:4 · transparent · ~640×768 · one warm shaft from upper-right, gently pulsing)*

- **Concept:** the painting's signature — the single warm beam of sunlight breaking through the dark
  bower onto the swing. Here it pours from the upper RIGHT, so the whole run is a sprint toward the
  light.
- **Form:** a broad, soft **shaft of golden light** angling down from the upper-right, with a few
  brighter god-ray streaks and floating motes within it; fades to nothing (magenta) at its edges.
- **Palette:** `#ffe69a`→`#fff0dd` core, fading warm; pure glow, no hard edges.
- **Framing:** diagonal shaft, brightest at upper-right; everything fades to magenta.
- **Avoid:** hard-edged rays, lens-flare circles, figures, text.

## 25. `bg-clouds.jpeg` — Rococo Sky Wisp  *(wide ~3:1 · transparent · ~1344×448 · high parallax band, repeated)*

- **Concept:** the soft confectionary clouds in the keyhole of warm sky the canopy leaves open
  overhead.
- **Form:** a horizontal band of soft, rounded **Rococo cumulus** — pink-and-cream lit tops, cool
  lilac-green undersides — drifting; airy and feathery.
- **Palette:** `#fff0dd`/`#f4a6c0` lit tops, `#9fae8a` cool undersides, warm `#ffe69a` glow behind.
- **Framing:** wide, centered, **feather the left/right ends** for seamless repeats; magenta above
  and below.
- **Avoid:** storm-grey, hard edges, figures, text.

## 26. `fg-mist.jpeg` — Drifting Petals  *(wide ~3.5:1 · transparent · ~1344×384 · drifts IN FRONT of the level at ~12% opacity)*

- **Concept:** the foreground perfume — a thin drift of pink rose-petals and golden pollen-motes
  that floats IN FRONT of the playfield, barely there.
- **Form:** a sparse, wide scatter of soft pink petals and tiny gold light-motes on transparency,
  denser at the edges, almost nothing in the center (so it never blocks gameplay).
- **Palette:** `#f4a6c0` petals, `#ffe69a`/`#fff0dd` motes; very soft.
- **Framing:** wide, content fading to magenta everywhere; **feather the left/right ends**.
- **Avoid:** anything opaque or high-contrast (it renders at ~12% over the action), figures, text.

---

# E. The avatar

## 27. `player.jpeg` — The Garden Sprite  *(1:1 · pivot center · faces RIGHT · transparent · ~448×448)*

- **Concept:** the hero — the game's small hooded sprite, here re-skinned as a **rococo garden
  spirit**: a little cloaked figure-of-foliage clutching a glowing rose-lantern. (It carries the
  same round silhouette and reads as a CHARACTER, never as a person — no human face/skin.)
- **Form:** a small, round, hooded sprite whose cloak is woven of pink-and-cream rose petals and
  green leaves, a soft glow under the hood (two warm light-eyes, no human features), holding a tiny
  lantern that glows gold; light, springy, ready to run. Faces right.
- **Palette:** cloak `#f4a6c0`/`#fff0dd` petals over `#2a4019` leaf-shadow; lantern + eyes
  `#ffe08a`/`#ffe69a`; a `#fff0dd` rim-light from the upper-right.
- **Framing:** centered, faces right, magenta around; must read clearly at 48 px against both dark
  foliage and bright stone.
- **Avoid:** a realistic human face/hands/skin, the original painting's girl, thorns/hot-red (the
  player is soft pink, the hazard is hot red — keep them distinct), text.

## 28. `movement.jpeg` — Walk Cycle Sheet  *(square sheet sliced 2 columns × 3 rows = 6 frames, left-to-right then top-to-bottom · faces RIGHT · transparent · ~1024×1536)*

- **Concept:** a 6-frame run/skip cycle of the garden sprite — light, bouncy, momentum-loving (this
  is a level about keeping the swing alive, so the run should feel like a joyful skip).
- **Form:** the **same sprite as `player.jpeg`** in 6 progressive frames of a run-skip cycle (lean,
  push, float, plant, recover, stride), the rose-petal cloak and lantern trailing the motion;
  consistent size and baseline so feet plant on the same line each frame. Arrange in a clean **2×3
  grid** (frame 1 top-left, frame 2 top-right, frame 3 mid-left, … frame 6 bottom-right), each
  frame the same dimensions, all facing right.
- **Palette:** identical to `player.jpeg`.
- **Framing:** one flat magenta background spanning the whole sheet; even gutters; each cell
  centered on its frame.
- **Avoid:** inconsistent scale/baseline between frames, a human figure, motion-blur smears, frame
  numbers/text, drifting horizontal position (keep the body centered in each cell).

---

## Priority order

If generating in passes, this order makes the level look intentional fastest:

1. **`player` + `movement`** — you see the avatar constantly.
2. **`ground`, `ground-edge`, `spike`** — the floor, its lips, and the killing briar (the core read).
3. **`swing-seat`** — the signature toy; the level is named for it.
4. **`note-block`, `jet`, `spring`, `portal`, `saw`** — the interactive set.
5. **`coin`, `goal`** — collectible + finish.
6. **`enemy`, `enemy-spiky`, `enemy-flyer`** — the creatures.
7. **`wall`, `platform`, `pool`** — secondary terrain.
8. **`bg-canopy`, `bg-hedge`, `bg-rays`, `bg-swing`** — the backdrop's load-bearing layers.
9. **`bg-fountain`, `bg-urn`, `bg-bloom`, `bg-clouds`, `fg-mist`** — backdrop polish.

## Checklist when the images are ready

- [ ] Saved into `Assets/_Game/Resources/Level3/` with the **exact** base filenames above.
- [ ] Backgrounds are flat magenta `#ff00ff` (except `ground` and `wall`, which are opaque).
- [ ] Everything that faces a direction faces **RIGHT**.
- [ ] **No humans, faces, hands, cherubs, putti, or figurative statues anywhere. The swing is EMPTY.**
- [ ] The killing hot vermilion `#ff3b56` appears ONLY on the briar, the briar-wheel saw, and the
      hedgehog's spines — never on safe footing or the soft-pink player/blooms.
- [ ] `ground` / `wall` tile seamlessly; `pool` / `bg-*` band ends feather or match.
- [ ] Stop Play, let Unity recompile the keyed sprites, press Play — and run the garden.
