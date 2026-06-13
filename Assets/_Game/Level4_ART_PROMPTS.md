# The Scream — Complete Art Briefs for Level 4 (for Gemini image gen)

Level 4 is Edvard Munch's *The Scream* (1893) made playable: a bridge that dies under your feet
over a freezing fjord, beneath a sky that will not stop screaming. The level is **fully built and
QA'd** — every chunk, gap and hazard is placed and physics-verified — and **everything is already
wired in code**. It renders as procedural placeholders until you fill the art folder. This document
is the complete set: **23 assets** covering the bridge terrain, the fjord hazards, the four signature
mechanics (dying planks, the drag-walkway, the reality-tear, the glide-feather), the whole
anguish-spawn creature family, the goal, and a five-layer Expressionist backdrop.

**To install:** save each image with the **exact base filename** into
`Assets/_Game/Resources/Level4/` (create the folder; `.jpeg` or `.png`), stop Play, let Unity
recompile, press Play, enter Level 4. Any file you skip keeps its procedural fallback, so generate
in any order.

---

## THE BIG RULE THAT SHAPES THIS ENTIRE LEVEL

**No humans. No human figures, faces, hands, bodies, or silhouettes of people — ever, anywhere.**
This is an absolute, permanent art-direction law. It is hardest here, because *The Scream*'s most
famous element — the skull-faced figure clutching its cheeks — is a human body. **We never draw it.**

Instead, the scream is reincarnated as a **force**: the anguish of the painting torn loose from any
body and given form as writhing knots of the screaming sky itself. The level's whole enemy family is
this — sound-made-visible, never a creature with a face. The two tiny distant figures on Munch's
bridge are also gone. The boats in the fjord are **empty**. The drama is carried by *line, colour,
water, timber, and sky* alone — exactly as in Levels 1–3.

---

## THE LEVEL — what the art has to serve

The level is built on the classic teach → develop → twist → climax arc, in The Scream's exclusive
vocabulary (no mechanic from another level appears here):

1. **Act 1 — the ground lies.** A precision warmup over the first fjord, then **crumble planks**
   (boards that crack and drop a half-second after you step on), then **donut planks** (they shake,
   then fall away), then your first meeting with **THE SCREAMER** — a plunging knot of anguish,
   taught over *safe* ground so a fumble only costs pride.
2. **Act 2 — the bridge dies over the fjord.** A patrolled island, then crumble planks *with* a
   diving screamer combined, a wall-climb up the fjord cliff, sky-hops with a coin, a drifting wail
   crossing your arc, then **THE DRAG-WALKWAY** — the world itself hauling you backward toward the
   spikes you just escaped, a blade sweeping its middle — and a spiked descent.
3. **Act 3 — reality tears.** **THE TEAR**: a rip in the world you must pass *through* (a wall
   blocks every other way). Roof-capped hops, a coin dangling low over the fjord, then the
   signature: **THE BRIDGE** — Munch's walkway as one long dying sprint, five crumbling spans under
   *two* plunging screamers, the only safe speed full speed.
4. **Climax — the leap into the sky.** A single **gull feather** grants a long floaty glide across
   a fjord no jump can cross; a screamer dives the glide-line — it is both the threat *and* the fast
   line (stomp its crown for a bounce). The coin hangs late in the drift: hold your nerve.
5. **Victory lap** — full-tilt hops to the threshold and out.

**The one visual law of the level, readable at a sprint in a sixteenth of a second:**
**warm bridge timber = safe footing; the deep blue-black fjord (and anything vermilion-tipped) =
death; the burning orange sky = atmosphere, never a hazard.** Never put fjord-blue on a safe
surface; never put timber-warmth on a deadly one.

---

## THE VIBE — paste this whole paragraph at the top of every Gemini prompt

> This artwork is for a 2D speedrun platformer level set *inside* Edvard Munch's *The Scream* (1893)
> — the Expressionist painting brought to life as a playable world. The visual language is
> **Munch's Expressionism**: thick, undulating, hand-dragged strokes of oil/tempera/pastel on
> raw cardboard, where **every line writhes and flows** as if a soundwave is passing through the
> whole world and bending it. NOT crisp vector art, NOT woodblock keylines, NOT impasto Van Gogh
> swirls, NOT anime, NOT photorealism — think wavering ribbons of colour smeared with the heel of
> the hand, the weave of the cardboard showing through, edges soft and trembling. Forms are
> simplified to flowing masses bounded by their own colour, not by outlines. The palette is the
> painting's own: a **screaming sky** of blood-vermilion `#c0301a` `#d6431f`, hot orange
> `#e8662a` `#f0853a`, and sickly cream-yellow `#ecc87a` `#e8d49a` in long horizontal undulating
> bands; a **freezing fjord** of blue-black `#16252e` `#0d1a24`, cold blue `#243f4e` `#2d4a55`,
> and muddy teal-green `#35514a`; the **bridge** in warm tan and timber `#b0814f` `#c49a64`
> `#6e4a2c` `#33220f` — in this game **warm timber means safe footing and the blue-black fjord
> means death**, so never mix the two on one object. Gold accents `#ffd23a`, and lethal danger
> always spoken in pure vermilion `#ff2d2d`. The mood is **dread, vertigo, and the held breath of
> a scream that never ends** — beautiful and sickening at once, the sky pressing down, the water
> pulling under. Light is flat and anxious — no realistic cast shadows, no volumetric glow; depth
> comes from the writhing strokes overlapping and the colour temperature (warm bridge in front,
> cold fjord behind, burning sky above). **ABSOLUTE RULE: depict NO human figures, NO bodies, NO
> faces, NO hands, NO skull-faces, NO silhouettes of people — none, anywhere.** The famous
> screaming figure is NEVER drawn; the boats are empty; the bridge is unpeopled. Animals (a gull,
> a feather) are allowed. The game engine applies a pixel-downscale filter at runtime, so render
> bold, legible source art — strong simple masses survive pixelation; fussy detail does not. I
> will attach the original painting as the style reference — **match its exact pigment palette,
> its wavering stroke direction, its sky-bands and fjord-swirl** so every asset looks torn from
> the same canvas.

**Attach `Assets/_Game/Resources/Paintings/the-scream.jpg`** to every generation as the style
reference (it is the actual painting). As your set grows, also attach 1–2 finished assets (e.g.
`ground.jpeg`) so the whole folder stays self-consistent.

## Global delivery rules (every asset)

- **Background: flat magenta `#ff00ff` fill — NOT a checkerboard, NOT white** (the loader keys it
  out). Exception: `ground`, `wall`, `pool` are full-bleed opaque tiles with no background.
- **One subject, centered. No scenery, no ground line, no cast shadow, no text, no frame/border,
  no signature.** (Munch's red signature strip on the canvas edge — never reproduce it.)
- Aspect ratio and orientation matter; exact pixel counts are quality suggestions only.
- Anything with a facing direction faces **RIGHT** (the direction of travel).
- Silhouettes must read shrunk to ~48–64 px.
- **No humans, no faces, no hands, no figures — ever.** (Worth repeating inside every prompt.)

## Palette

| swatch | hex | use |
|---|---|---|
| Scream Red | `#c0301a` | the burning sky's deepest band, danger glow |
| Vermilion | `#ff2d2d` | lethal danger ONLY (hazard tips) |
| Sky Orange | `#e8662a` | mid sky bands |
| Ember Orange | `#f0853a` | lit sky, warm rim light |
| Sky Cream | `#ecc87a` | pale sky bands, haze |
| Pale Cream | `#e8d49a` | lightest sky, distant haze |
| Fjord Black | `#0d1a24` | deepest water, the void of pits |
| Fjord Deep | `#16252e` | water mass, darkest shadow |
| Fjord Blue | `#243f4e` | lit water, cold mid-tone |
| Fjord Teal | `#35514a` | the muddy green-blue swirl |
| Timber Lit | `#c49a64` | sunlit bridge plank — SAFE footing |
| Timber Tan | `#b0814f` | bridge body |
| Timber Brown | `#6e4a2c` | plank shadow |
| Timber Deep | `#33220f` | deepest wood, plank seams |
| Coin Gold | `#ffd23a` | the collectible |
| Calm Blue | `#7fb0c4` | the goal's "way out" — the only friendly cool tone |

**Colour budget across the whole level:** the playfield is warm — **~55% timber/tan** (the bridge
you live on), **~25% burning sky** (the backdrop above), **~20% blue-black fjord** (the death
below). Blue belongs to the water and its weapons only; the sky-orange never touches a surface you
stand on; the timber never bleeds into the water.

---

# THE SCREAMER — the level's title character (read before drawing any creature)

The painting's screaming figure is a human body — **so we never draw it.** Instead the scream is
torn loose from any body and reborn as a **force of the sky**: the undulating bands of the burning
heaven, clenched into a writhing knot and given a hollow dark **void at its heart** — the place the
sound pours out of, drawn as pure negative space, **never a mouth, never a face.** It plunges from
above like a falling shriek, falls slower than you run, and its rounded upper coil is a trampoline
(you stomp it). The whole enemy family descends from this idea — lesser knots of the same anguish —
so the level reads as one painting screaming in four voices. **Not one of them has eyes, a face,
hands, or any anatomy.** The dread is in the line and the colour, never in a creature looking at you.

---

# A. Terrain & hazards

## 1. `ground.jpeg` — Bridge Plank Tile  *(1:1 SQUARE · horizontally SEAMLESS · SOLID/opaque · ~512×512)*

- **Concept:** the walkable world — the warm timber boardwalk of Munch's bridge, the one safe
  surface in a screaming world. Everything you can stand on is this wood.
- **Form:** fills the whole square, no background. Long timber **planks running horizontally**
  (the direction you run), rendered as Munch would — slightly wavering, hand-dragged strokes
  following the grain, not ruler-straight. Top ~22% is the sunlit walking surface, palest along
  the very top edge; below it the boards darken plank by plank toward the bottom. Faint cardboard
  tooth showing through. A few darker plank-seams and the odd knot, all softly drawn.
- **Style:** flowing painterly strokes, soft trembling edges, zero gloss; the wood looks *worried*,
  warped by the same force bending the sky — but unmistakably warm and solid.
- **Palette:** walking surface `#c49a64` lit to `#e8d49a` at the top line; planks below `#b0814f`
  → `#6e4a2c` → near `#33220f` at the bottom. **No blue, no orange anywhere on this tile.**
- **CRITICAL:** the **left and right edges must match exactly** for seamless horizontal tiling;
  keep plank lines horizontally continuous, stagger any vertical seams, no centered focal detail.
- **Avoid:** any fjord-blue or sky-orange (the law: wood = safe), photographic wood grain,
  ruler-straight CAD lines, perspective, text.

## 2. `ground-edge.jpeg` — Broken Bridge End  *(tall ~2:3 · ~384×576 · overlays the LEFT end of every ledge; auto-mirrored for right ends)*

- **Concept:** where the boardwalk breaks off over the void — splintered planks at the lip of the
  fjord, the wound where the bridge gives out.
- **Form:** designed for the **LEFT edge** of a ground block: the right half matches `ground.jpeg`
  exactly (it overlaps the tile and must blend invisibly); the left half is the break — planks
  snapped at staggered lengths with soft splintered ends pointing left, one or two splinters
  hanging loose, the strokes denser and more agitated at the fracture.
- **Palette:** identical to `ground.jpeg`; broken end-grain falls to `#33220f`; a thin `#e8d49a`
  lit edge wraps the top corner.
- **Framing:** vertical; content bleeds off the RIGHT edge; magenta to the LEFT beyond the
  splinters; nothing below the bottom edge.
- **Avoid:** a ruler-straight break, identical splinter lengths (it appears at dozens of ledge
  ends), any water on the wood, brightness exceeding the walking surface, text.

## 3. `wall.jpeg` — Fjord Cliff Face  *(1:1 square · ~512×512 · MUST tile seamlessly BOTH directions · opaque)*

- **Concept:** the sheer rock of the fjord shore — the vertical face the wall-climb chimney scales.
  It must recede quietly behind gameplay; it is not a place you stand, but it is not water either,
  so keep it earthy and neutral, never blue-black.
- **Form:** an all-over rock material filling the square: rough strata of stone drawn as horizontal
  wavering bands, sparse quiet detail — a crack, a clump of dark moss, a warmer ochre seam; cardboard
  tooth throughout. Flat even light (it tiles anywhere, including overhangs).
- **Palette:** a narrow earthy range — muted `#6e4a2c` to `#33220f` browns with cold `#35514a`
  only in the deepest cracks; nothing bright.
- **CRITICAL:** all four edges wrap seamlessly into their opposites; avoid features that form
  visible rows/grids when repeated; keep contrast LOW.
- **Avoid:** fjord-blue dominating (it must read as solid earth, not water), sky-orange, bright
  highlights, recognizable shapes, text.

## 4. `platform.jpeg` — Floating Bridge Shard  *(wide 3:1 · pivot center · transparent · ~768×256)*

- **Concept:** a torn-loose fragment of the boardwalk hanging in the air — the level's airborne
  ledges (the sky-hop platforms). Must read wood-on-top (safe) instantly, with the void clinging
  beneath it.
- **Form:** a wide, low horizontal shard: a sunlit plank deck on top (walkable, the palest part),
  the timber tapering and splintering below; hugging the underside, a thin smear of the cold fjord
  reaching up around the wood (cold blue, clearly UNDER the walk surface). Both ends taper — the
  right end finer and slightly upturned, the left blunter and broken.
- **Style:** soft Munch strokes, flowing edges; the top edge is **100% warm wood**, the cold smear
  only below.
- **Palette:** deck `#c49a64` → `#e8d49a` lit line; timber `#b0814f` → `#33220f`; the underside
  smear `#243f4e`/`#16252e`.
- **Framing:** horizontal, much wider than tall, centered, magenta above and below.
- **Avoid:** any blue on the TOP surface (must read safe), a symmetric slab look, hard outlines,
  text.

## 5. `pool.jpeg` — The Fjord  *(wide ~2:1 · horizontally SEAMLESS · opaque · ~1024×512)*

**The death-liquid that fills every pit — the freezing fjord of the painting, beautiful and fatal.**

- **Concept:** the cold blue-black water that swirls beneath Munch's bridge, here the surface of
  every chasm. Falling in is death.
- **Form:** the water fills the whole tile below a thin strip of flat magenta at the very top (the
  air above the surface). The **surface line** undulates in Munch's long wavering swells; below it,
  layered ribbons of cold colour swirl in his signature curving currents — `#243f4e` over `#16252e`
  over `#0d1a24` near-black at the bottom, with the odd muddy `#35514a` teal eddy. Drawn as flowing
  smeared strokes, never realistic water.
- **CRITICAL:** LEFT and RIGHT edges must match exactly (it tiles into one continuous fjord); keep
  the swirl irregular enough that the repeat isn't obvious.
- **Palette discipline:** a faint cold `#ff2d2d` shimmer may break the surface line *sparingly*
  (the danger whisper, echoing the sky reflected); the body stays cold blue-black.
- **Avoid:** boats, the screaming figure, sky reflection beyond a whisper, photographic water,
  warm timber tones, text.

## 6. `spike.jpeg` — Fjord Fang  *(~1:1.3 tall · pivot bottom-center · transparent · ~400×512)*

- **Concept:** the discrete lethal hazard where a continuous pool isn't used — a frozen fang of the
  fjord risen from the water, a cold tooth waiting at the bottom of a pit.
- **Form:** a single upward-curling tongue of blue-black water frozen mid-rise, its tip hooking
  slightly forward, drawn in wavering Munch strokes; a few cold spray-flecks frozen beside it.
- **Palette:** body `#243f4e` → `#16252e` → `#0d1a24`, with a burning **vermilion `#ff2d2d` flush
  at the very tip** (the universal "lethal" signal) bleeding a short way down. The whole upper
  silhouette must read "do not touch."
- **Framing:** base flat at bottom-center, fang at top, vertical, centered; reads as a fang at 48 px.
- **Avoid:** any warm wood tone (blue = death is the law), icicle/crystal regularity, more than one
  fang, symmetric drips, text.

## 7. `saw.jpeg` — The Shriek-Wheel  *(1:1 square · ~512×512 · spins at high speed in-game)*

- **Concept:** the sweeping blade hazard — a knot of the screaming sky torn into a spinning wheel of
  sound, a whirling shriek that patrols arcs through the level.
- **Form:** a near-perfect **circular silhouette** (it rotates — asymmetry will wobble): concentric
  wavering rings of the burning sky coiling inward, their outer rim breaking into a ring of 8–12
  hooked tongues of flame-coloured anguish (curled consistently one direction so they read as teeth
  when spinning); a small dark **void at the center** (negative space, NOT an eye, NOT a face).
- **Style:** Munch strokes coiled into a wheel; bold and legible; make it **hot**: vermilion and
  orange dominate so it reads as danger instantly.
- **Palette:** outer tongues and rings `#ff2d2d`/`#d6431f`/`#e8662a`; inner coils `#c0301a` darkening
  toward the void; the center void `#0d1a24`. No timber, no fjord-blue.
- **Framing:** perfectly centered and circular, generous magenta margin (it spins).
- **Avoid:** industrial sawblade look, motion blur (the engine spins it), an eye or face at the
  center, breaking the circular outline, text.

# B. The Scream's signature gameplay pieces

## 8. `crumble.jpeg` — Dying Plank  *(wide ~3:1 · pivot center · transparent · ~768×256)*  ★ signature

- **Concept:** the heart of the level — a bridge board that **lies**. The instant you step on it, it
  cracks, reddens with the sky's panic, and a half-second later drops into the fjord. Most of the
  level is crossed on these.
- **Form:** a wide horizontal board-pair (reads as two short planks lashed together), a warm timber
  deck like `ground` but **already failing**: a jagged crack splitting it, the wood stressed and
  splintering, the strokes agitated. Drawn so it reads "warm/safe to land on" at a glance (top
  surface is solid timber) but "doomed" on a second look (the crack, a faint vermilion fear-flush
  creeping in from the edges). The art is drawn a hair TALL with the broken underside hanging BELOW
  the walk line; the **top edge stays flush and honest** (it is real footing until it falls).
- **Style:** Munch timber strokes, the crack a dark wavering gash; soft edges.
- **Palette:** deck `#c49a64`/`#b0814f` warm and safe on top; the crack and underside `#33220f`;
  a low vermilion `#ff2d2d` flush bleeding in *only* from the broken edges (the dread, not enough to
  read as "death-blue").
- **Framing:** horizontal board-pair, centered, magenta above and below; top surface clearly the
  safe walking line.
- **Avoid:** fjord-blue on the deck (it must still read safe), a fully-shattered look (it has to
  invite the step), hard outlines, text.

## 9. `falling.jpeg` — Loosed Plank  *(wide ~3:1 · pivot center · transparent · ~768×256)*

- **Concept:** the donut-block sibling of the dying plank — a board only resting on its supports.
  Stand on it and it shakes, then drops away whole. Distinct from `crumble` at a glance: this one
  isn't cracked, it's *unmoored*.
- **Form:** a wide timber board-pair like `crumble` but **intact and loose** rather than splitting —
  the same warm deck, but the ends are clearly unattached (a gap of dark void showing at each end
  where it should be lashed down), maybe a frayed rope or a bent nail. Reads "this isn't fixed to
  anything." Drawn a hair tall, broken/loose underside below the flush top walk-line.
- **Palette:** deck `#c49a64`/`#b0814f`; the unmoored ends and underside `#33220f`; a faint cold
  `#243f4e` void peeking at the loose ends (the fjord waiting below) — but the TOP stays warm/safe.
- **Framing:** horizontal board-pair, centered, magenta above and below.
- **Avoid:** a crack (that's `crumble`'s tell — keep these visually different), blue on the top
  surface, hard outlines, text.

## 10. `conveyor.jpeg` — The Writhing Walkway  *(1:1 SQUARE · horizontally SEAMLESS · transparent or opaque · ~512×512 · art flows RIGHT, engine mirrors for the leftward belt)*  ★ signature

- **Concept:** the drag-belt — the bridge itself caught in the painting's undulation and *flowing*,
  hauling whatever stands on it backward. In this level the belt drags you back toward the spikes
  you escaped. The art must show **unmistakable directional flow** so the player reads which way the
  ground is pulling.
- **Form:** a timber walkway tile whose planks are caught mid-writhe — the boards streaming and
  curling in one direction (drawn flowing to the RIGHT; the engine mirrors it for the leftward
  belt), the grain smeared into long Munch motion-lines, a few chevron-like swells in the wood
  pointing the flow direction. It must still read as **warm safe timber** (you do stand on it) but
  alive and moving.
- **Style:** the most kinetic of the timber assets — strokes raked hard in the flow direction so
  even a still frame looks like it's sliding.
- **Palette:** timber `#c49a64`/`#b0814f`/`#6e4a2c` with `#33220f` seams; a faint warm `#f0853a`
  motion-streak riding the flow (energy, not danger — keep it warm, never vermilion).
- **CRITICAL:** left and right edges match for seamless tiling; the flow direction must be obvious
  at a glance and at 48 px.
- **Avoid:** arrows/chevrons as UI symbols (the grain-flow IS the arrow), fjord-blue, vermilion
  (this is moving ground, not a hazard), text.

## 11. `portal.jpeg` — The Tear  *(tall ~1:2 · pivot center · transparent · ~512×1024)*  ★ signature

- **Concept:** THE TEAR — where the scream's pressure rips the world open. A vertical gash torn
  through reality; walk into it and you come out the matching tear beyond an impassable wall. The
  single most surreal object in the level.
- **Form:** a tall vertical rip — as if the canvas itself were slashed and **peeled open**, the
  torn lips of the wound curling back to reveal, *through* the gap, a churning slot of the burning
  sky's deepest reds and a swirl of void. The edges of the tear are ragged and frayed (the cardboard
  weave showing, threads pulled), undulating along its length. Inside: a hot vermilion-and-black
  vortex, pulling inward.
- **Style:** Munch strokes torn vertical; the interior glows hotter than anything else on the
  playfield so it reads as a "doorway."
- **Palette:** torn lips in the surrounding world's muddy `#6e4a2c`/`#35514a`; the interior
  `#c0301a` → `#ff2d2d` glow over `#0d1a24` void; a thin hot rim where world meets tear.
- **Framing:** vertical, the slit running top-to-bottom, centered, generous magenta margin (it
  pulses and spins slightly in-game).
- **Avoid:** a clean sci-fi ring portal, a circular shape (it's a vertical SLASH), any figure or
  face inside the void, text.

## 12. `feather.jpeg` — Gull Feather  *(~1:1.6 tall · pivot center · transparent · ~384×600)*

- **Concept:** the climax power-up — a single gull's feather caught in the screaming wind, the one
  calm, weightless thing in the painting. Grab it and the wind carries you in a long floaty glide
  across a fjord no jump can clear.
- **Form:** a single elegant feather, drifting, faintly curved, tip up; rendered in Munch's soft
  strokes but **calm** where everything else writhes — it is the eye of the storm. A gentle warm
  glow rims it (the one gift the sky gives), with 3–5 tiny down-wisps shedding from it.
- **Style:** the one asset allowed a soft self-glow; keep it pale and serene against the level's
  fever.
- **Palette:** the feather pale cream `#e8d49a`/`#ecc87a` with a `#c49a64` spine; the rim-glow a
  soft warm `#f0853a` fading out; NO vermilion, NO fjord-blue (it is pure calm/safe).
- **Framing:** centered, the feather upright and slightly drifting, generous magenta margin (it
  sways in-game).
- **Avoid:** a whole bird, a wing with anatomy, sci-fi energy effects, electric colours, text.

## 13. `coin.jpeg` — The Gilded Tear  *(1:1 · pivot center · transparent · ~384×384)*

- **Concept:** the collectible — a single drop of gold suspended in the dread, like a tear of the
  sky caught and hardened. Exactly three exist per level, always in nasty spots: it must look
  *worth* the risk.
- **Form:** a round, slightly thick gold medallion, its rim minutely rippled with the painting's
  undulation (a wavering scalloped edge, not a clean circle), a smooth burnished empty center boss —
  no lettering, no hole, no portrait. A soft warm halo bloom around it; one crisp specular at
  upper-right.
- **Palette:** body `#ffd23a`, center glint `#e8d49a`, recesses `#c49a64` → `#6e4a2c`; warm halo.
- **Framing:** front-facing, centered, generous margin (it spins in-game).
- **Avoid:** currency symbols, kanji or ANY text, a square center hole, a portrait, a perfectly
  geometric circle (let the rim waver), text.

## 14. `goal.jpeg` — The Threshold  *(tall ~1:2 · pivot bottom-center · transparent · ~512×1024)*

- **Concept:** the way out — the far end of the bridge, where the screaming sky finally goes quiet
  and cool. A simple timber threshold-arch standing on the boardwalk, lit from beyond with the one
  patch of *calm* colour in the whole level: you are running toward peace.
- **Form:** a tall, plain timber doorway/arch (two warm posts and a lintel, Munch-soft, the safe
  wood of the bridge), framing an opening that glows a **calm cool blue-white** — the only place the
  fjord-blue is allowed to be friendly, because here it means "out." A faint warm rim on the posts.
- **Palette:** posts `#c49a64`/`#6e4a2c` warm timber; the opening `#7fb0c4` calm blue softening to
  `#e8d49a` cream at its heart; no vermilion anywhere (this is the safe place).
- **Framing:** vertical, posts' feet flat at bottom-center, the calm glow filling the arch,
  centered, magenta around.
- **Avoid:** a person in the doorway (NONE — empty threshold), a burning/red glow (the goal is
  *calm*), a literal door slab, text.

# C. Creatures — the anguish-spawn (emphatically NOT human, NO faces, NO hands)

## 15. `enemy-diver.jpeg` — THE SCREAMER  *(1:1 · pivot center · faces nothing / radial · transparent · ~512×512 · plunges from above in-game)*  ★★ title character

**Re-read "THE SCREAMER" above before drawing this.** It is the painting's anguish with the human
body *removed* — pure sound made visible.

- **Concept:** a clenched knot of the screaming sky, torn loose and plunging like a falling shriek.
  Falls slower than you run; its rounded crown is a trampoline (you stomp it). No body, no face,
  no hands — a *force*.
- **Form:** a roughly teardrop/comma-shaped **vortex** of the sky's undulating bands wound tight
  into a knot — concentric wavering ribbons of burning colour spiralling inward to a **hollow dark
  void at the heart** (the place the scream pours from, pure negative space — emphatically NOT a
  mouth, NOT an eye). Wavering tendrils trail off the lower edge like the tail of a wail. The upper
  curve is smooth and rounded (the clear stompable crown). The whole thing should feel like Munch's
  sky-bands grabbed and twisted into a fist of noise.
- **Style:** the masterpiece creature asset — pure Munch, all writhing strokes, hot and sickening;
  it must read as "anguish" even shrunk to 48 px, purely through coil and colour.
- **Palette:** ribbons `#d6431f`/`#e8662a`/`#f0853a` with `#c0301a` in the depths; the heart-void
  `#0d1a24`; vermilion `#ff2d2d` flushing the leading/lower edge (the lethal tip). Hot throughout —
  it is a piece of the burning sky, never blue.
- **Framing:** centered, the rounded crown up, the trailing tendrils down, generous magenta margin
  (it dives and bobs in-game).
- **Avoid:** ANY face, eyes, mouth, skull, hands, body, or humanoid silhouette (the entire reason
  this asset exists); a tidy spiral (it must writhe); cuteness; text.

## 16. `enemy.jpeg` — Scream-Knot Crawler  *(1:1 · pivot bottom-center · faces RIGHT · transparent · ~448×448)*

- **Concept:** the patrolling grunt — a small, low knot of the same anguish that has settled onto
  the bridge and crawls it. Stomped from above. Cute-sinister, never a creature with a face.
- **Form:** a compact, rounded ground-knot — a small clenched coil of sky-colour bands (a baby
  sibling of the Screamer), low and dome-topped (the dome reads as stompable), with 3–4 stubby
  wavering tendrils beneath it that serve as little legs, scuttling RIGHT. A tiny dark void-pock at
  its front instead of a face (negative space, no eyes).
- **Palette:** body `#e8662a`/`#d6431f` warm coil, tendrils `#6e4a2c` (so they read against the
  timber), a vermilion `#ff2d2d` flush along the front edge (hostile signal); the void-pock
  `#0d1a24`. No fjord-blue.
- **Framing:** three-quarter view scuttling right, tendrils at bottom-center.
- **Avoid:** eyes/face/mouth, raised "arms," spikes on the dome (it must read stompable), text.

## 17. `enemy-flyer.jpeg` — Drifting Wail  *(1:1 · faces RIGHT · transparent · ~512×512 · bobs and drifts in-game)*

- **Concept:** an airborne wisp of the scream drifting across your jump arcs. The player can stomp
  it mid-air, so its top must read as landable.
- **Form:** a long, horizontal, ribbon-like wisp of the sky's bands — a flattened, drifting comma
  of anguish trailing wavering streamers behind it (to the LEFT, since it faces/moves right), with
  a smooth rounded upper edge (the stomp target). Lighter and more diffuse than the Screamer — it
  *floats* rather than plunges. A small dark void-knot at its leading end (no face).
- **Palette:** body `#f0853a`/`#ecc87a` (paler, airborne), trailing streamers fading to `#e8d49a`;
  a `#ff2d2d` flush at the leading edge (hostile); void-knot `#16252e`. Airy, never heavy.
- **Framing:** horizontal drift facing right, streamers trailing left, centered with margin; no
  ground beneath it (it never lands).
- **Avoid:** a bird with wings/beak/eyes (it is a wisp of sound, not an animal), a face, heaviness,
  text.

## 18. `enemy-spiky.jpeg` — Barbed Shriek  *(1:1 · pivot bottom-center · faces RIGHT · transparent · ~448×448)*

- **Concept:** the crawler's untouchable sibling — a knot of anguish bristling with barbs. CANNOT
  be stomped; its whole job is to scream "do NOT land on me."
- **Form:** a round, dark, dense knot of coiled sky-bands **bristling with 12–16 sharp wavering
  barbs** covering its entire top and sides — the upper silhouette must be all points, zero safe
  surface. Barbs longest on the crown, drawn as agitated Munch needles. A dark void at the core
  (no eyes, no face).
- **Palette:** body darker and angrier — `#c0301a` → `#0d1a24` core (a creature half-swallowed by
  the void), barbs `#d6431f` shading to **`#ff2d2d` glowing tips on every point** (many hot points
  = instant "don't touch"); the core void `#0d1a24`. A touch of cold `#243f4e` may creep into the
  base (it's sinking toward the fjord).
- **Framing:** three-quarter view facing right, base at bottom-center.
- **Avoid:** any landable-looking gap in the crown, a face/eyes/mouth, a hedgehog silhouette, soft
  barb tips, text.

# D. Backdrop & atmosphere (the painting's world, no figures)

## 19. `bg-skyband.jpeg` — The Burning Sky  *(wide ~3:1 · transparent · ~1344×448 · the signature backdrop layer, repeated and slowly undulating)*  ★ signature

- **Concept:** the scream itself made sky — the long horizontal bands of blood, fire and sickly
  cream that writhe across the top of the painting. The defining image of the whole level; it sits
  high and pulses as if the heavens are breathing the scream.
- **Form:** a wide horizontal band of 4–6 stacked **undulating ribbons** of colour, each a wavering
  smeared stroke running the full width, the bands rising and dipping like a slow soundwave;
  hottest (most vermilion) toward the top, cooling to cream toward the bottom; the band's top and
  bottom edges ragged so stacked copies chain without a seam, and both ENDS thinning to magenta.
- **Style:** pure Munch — thick hand-dragged horizontal smears, cardboard tooth showing, every line
  wavering. No clouds, no sun disc, no detail — just the writhing colour.
- **Palette:** top bands `#c0301a`/`#d6431f`, middle `#e8662a`/`#f0853a`, lower `#ecc87a`/`#e8d49a`.
  Pure sky-fire — no blue, no timber.
- **Framing:** horizontal band, magenta above/below, both ends tapering to magenta for seamless
  horizontal chaining.
- **Avoid:** a literal sun, recognizable clouds, smooth gradient (it must be visibly *stroked* and
  wavering), figures, text.

## 20. `bg-fjord.jpeg` — The Cold Fjord  *(wide ~3:1 · transparent · ~1344×448 · horizon water band, repeated, gently swaying)*

- **Concept:** the freezing water heaving on the horizon between the bridge and the far shore — the
  sea's cold swirl that the sky-fire reflects in. It sits below the sky-bands and above the playfield
  floor, the death-colour kept to the distance.
- **Form:** a long horizontal band of cold undulating swells — Munch's curving fjord currents, blue-
  black ribbons coiling along their flanks, a couple of **tiny empty boats** as faint dark dashes
  riding a far swell (no rowers, barely there); both ends sinking and dissolving so copies chain
  seamlessly.
- **Palette:** swells `#243f4e`/`#2d4a55` with `#16252e` troughs and `#35514a` muddy eddies; a thin
  `#ecc87a` haze along the far edge where it meets the sky (the sunset on the water). Cold dominates.
- **Framing:** horizontal band, magenta above and below, ends tapering to magenta.
- **Avoid:** the screaming figure, rowers in the boats (empty, full stop), warm timber tones, big
  breaking waves (this is a heaving swell, not surf), text.

## 21. `bg-headland.jpeg` — The Far Shore  *(wide ~2:1 · pivot bottom · transparent · ~1024×512 · low promontory across the water, base fades into haze)*

- **Concept:** the dark landmass across the fjord — the brooding far shore the painting frames in
  the distance, low and quiet, the still backdrop the violence happens in front of.
- **Form:** a long, low promontory — rolling dark land drawn in soft Munch masses, a hint of a
  treeline or a building or two as simple dark lumps (NO figures), the **base dissolving softly into
  haze** (no hard bottom edge, so it floats on the water band in front of it).
- **Palette:** a muted, hazy land — `#35514a` muddy green-blue warmed with `#6e4a2c` earth, the
  upper edge catching a whisper of `#f0853a` sunset; the base fading through `#243f4e` haze. Dark
  and recessive.
- **Framing:** the land low in the frame, base bleeding off the bottom into magenta via the haze;
  magenta elsewhere.
- **Avoid:** tall dramatic mountains (it is LOW and distant), recognizable buildings/people, bright
  colour, a hard bottom edge, text.

## 22. `bg-railing.jpeg` — The Walkway Railing  *(very wide ~3:1 · transparent · ~1344×448 · rakes across the frame in steep perspective, near the camera)*  ★

- **Concept:** the single most recognizable *line* in the painting — the bridge's railing knifing in
  from the lower-left in steep perspective. It rides close to the camera as a foreground depth
  layer, its diagonal raking across the lower frame and selling the painting's vertigo instantly.
- **Form:** a long timber railing seen in **steep diagonal perspective**, running from low-left up
  toward the right, a top rail and a couple of posts, the boards of the walkway suggested beneath it
  — all in Munch's wavering strokes so even the railing seems to bend under the scream. It should
  read as the painting's railing the moment you see it.
- **Palette:** warm timber `#b0814f`/`#6e4a2c`/`#33220f` (it's part of the safe bridge, just far
  foreground), a `#c49a64` lit edge along the top rail.
- **Framing:** the diagonal running lower-left to upper-right, anchored to the bottom-left, the rest
  magenta; both ends arranged so horizontal copies overlap into one continuous receding rail.
- **Avoid:** the screaming figure leaning on it (NONE), fjord-blue, a ruler-straight CAD line (it
  must waver), text.

## 23. `fg-mist.jpeg` — The Anxiety  *(very wide ~3.5:1 · transparent · ~1344×384 · drifts IN FRONT of the level at ~11% opacity)*

- **Concept:** the dread made faintly visible — a thin drifting haze of the sky's colour passing in
  front of the action, the anxiety you can almost see, completing the depth sandwich.
- **Form:** 2–3 long, lazy horizontal drifts of soft wavering smear — ragged, full of open magenta
  gaps, thinning at the edges; one drift curling into a faint comma (a ghost of a scream-knot). Pure
  atmosphere, barely there.
- **Style:** soft smeared Munch strokes, no hard edges, no detail.
- **Palette:** almost monochrome warm — `#f0853a`/`#ecc87a` with the faintest `#e8d49a`; no darks,
  no vermilion (it renders at ~11% alpha and must never tint gameplay or read as a hazard).
- **Framing:** horizontal band, both ends dissolving to magenta so copies chain seamlessly, generous
  magenta above and below.
- **Avoid:** fog/smoke density (sparser is better), recognizable shapes, fjord-blue, figures, text.

---

## (Optional) `player.jpeg`

The player avatar is shared across the Gallery and already has art; you do **not** need to provide
one for Level 4. If you want a level-specific look, drop a `player.jpeg` in the folder and it will
override — otherwise skip it.

---

## Priority order

`enemy-diver` (THE SCREAMER) → `ground` → `crumble` → `pool` → `bg-skyband` → `conveyor` →
`portal` → `feather` → `coin` → `goal` → `spike` → `falling` → `saw` → `platform` → `ground-edge` →
`wall` → `enemy` → `enemy-flyer` → `enemy-spiky` → `bg-fjord` → `bg-headland` → `bg-railing` →
`fg-mist`

(The Screamer IS the level's identity — start there. `ground` + `crumble` + `pool` + `bg-skyband`
transform every screen; the four signature mechanics define the play; the creature family and the
five backdrop layers complete the world.)

## Checklist when the images are ready

1. Save into `Assets/_Game/Resources/Level4/` (create the folder) with the exact base filenames.
2. Stop Play mode → let Unity recompile/import → Play, then enter Level 4 (the 4th painting).
3. Everything swaps in automatically — bridge timber, the fjord, dying/loosed/writhing planks, the
   tear, the feather, the four-voice anguish family, the threshold, and the five-layer backdrop
   (burning sky, cold fjord, far shore, the raking railing, drifting anxiety). Anything you skip
   keeps its procedural fallback.
4. **Sanity pass 1 — the law:** stand anywhere and squint. Warm timber must read *safe*, the blue-
   black fjord must read *deadly*, the orange sky must read *backdrop* — zero ambiguity at a sprint.
5. **Sanity pass 2 — the rule:** scan every asset for any hint of a face, hand, or human figure.
   There must be **none** — least of all on the Screamer. If a creature looks like it's *looking*
   at you, redraw it as pure coil and colour.
6. **Sanity pass 3 — THE BRIDGE:** stand at the act-3 set-piece (the five crumbling spans). The
   dying planks, the two plunging Screamers, the burning sky and the cold fjord below should compose
   the painting's dread in one screen — without a single figure in sight.
