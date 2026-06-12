# Starry Night — NEW Art Briefs for the Redesigned Level 1 (for Gemini image gen)

The Level 1 redesign added beats that still render as procedural placeholder shapes: **four saw
blades** (including the final ring-jump climax), a **flyer** that crosses your jump arc, an
**unstompable spiked patroller**, and the **duck-tunnel ceiling**. It also climbs much higher than
before, exposing an empty mid-sky band and a missing far ridge in the backdrop. The six assets below
fill every one of those gaps.

**Everything is already wired in code.** Save each image with the **exact base filename** below
(`.jpeg` or `.png`, either works) into `Assets/_Game/Resources/Level1/`, stop Play, let Unity
recompile, press Play. No code changes needed. If a file is missing the game just keeps the
procedural fallback, so you can drop these in one at a time.

---

## THE VIBE — paste this whole paragraph at the top of every Gemini prompt

> This artwork is for a 2D speedrun platformer level that takes place *inside* Vincent van Gogh's
> *The Starry Night* — not a game "inspired by" the painting, but the painting itself come alive and
> made playable. The world is a velvet-dark June night over a sleeping village: every surface, object
> and creature in it is built from thick, short, curved **impasto oil brushstrokes** that visibly
> swirl and flow, as if the night wind itself is dragging wet paint across the sky. Nothing is
> airbrushed, nothing is a flat vector fill, nothing has a hard black cartoon outline — shapes are
> separated by **dark cobalt painted edges** and by light, the way Van Gogh did it. The palette is a
> deep ocean of blues — near-black night cobalt `#0a1530`, deep blue `#14264f`, royal blue `#1b3a6b` —
> set on fire by small amounts of radiant warmth: chrome yellow `#ffd23a`, soft gold `#f4d35e`, pale
> glow-cream `#fff4d0`, with cool cerulean `#6fb7ff` for rim-light and highlights. The mood is
> **dreamy but deadly**: this is a beautiful, drowsy, glowing night that will absolutely kill a
> careless traveler — hazards glitter like stars, danger is communicated with sharpness and white-hot
> glow rather than gore or red. The single light source logic across all assets: **cool moonlight
> from the top-left**, plus each object's own self-emitted glow. The game engine applies a heavy
> pixel-downscale filter at runtime, so render gorgeous painterly source art — the retro pixel look
> is added automatically and the impasto texture survives beautifully as shimmering pixel clusters.
> The hero of this world is a small hooded night-spirit carrying a star-lantern; I will attach
> `player.jpeg` as a style reference — **match its exact brushwork density, palette, edge treatment
> and lighting** so every asset looks cut from the same canvas.

**Also attach `player.jpeg`** (from `Resources/Level1/`) to every generation for style consistency.

## Global delivery rules (every asset)

- **Background: flat magenta `#ff00ff` fill — NOT a checkerboard, NOT white.** The loader keys it
  out at runtime. Soft glows may fade toward the magenta; slight fringing is handled.
- **One subject, centered. No scenery, no ground line, no cast shadow, no text, no frame/border.**
- Aspect ratio and orientation matter; exact pixel count doesn't (suggested sizes are for quality).
- Anything with a face or a facing direction faces **RIGHT**.
- Keep silhouettes readable when shrunk to ~48–64 px — these are small in-game.

| swatch | hex | use |
|---|---|---|
| Night Cobalt | `#0a1530` | deepest shadow |
| Deep Blue | `#14264f` | main mid-tone |
| Royal Blue | `#1b3a6b` | lit blue |
| Cerulean | `#6fb7ff` | cool rim-light |
| Pale Glow | `#fff4d0` | white-hot cores |
| Chrome Yellow | `#ffd23a` | stars / lethal glow |
| Soft Gold | `#f4d35e` | warm secondary glow |
| Cypress | `#0c1a10` | near-black green |

---

# A. Gameplay objects (priority — these are on the player's path)

## 1. `saw.jpeg` — "Star-Shear," the Sweeping Blade  *(1:1 square · ~512×512 · spins in-game)*

**Where it lives:** the level's most important new hazard — it appears over safe ground as a warning,
then guards a spike pit, and finally sweeps the landing arc of the climactic ring-jump. It oscillates
up and down on a fixed cycle and the sprite is rotated continuously at high speed by the engine.

- **Concept:** a circular blade forged from the night sky itself — a vicious pinwheel of frozen
  star-shards orbiting a burning core. It must read as *"spinning, lethal, do not touch"* in a
  quarter of a second, while still being beautiful.
- **Form:** a near-perfect **circular silhouette** (it rotates, so any asymmetry will wobble): a ring
  of 8–12 sharp, slightly hooked crystal teeth around a circular hub, like a circular saw blade
  crossed with a compass rose. Teeth angled consistently clockwise so rotation reads. A small
  glowing eye-of-the-storm core at dead center.
- **Brushwork:** the hub painted as a tight Van Gogh **vortex** — concentric curling strokes
  spiraling into the center; the teeth are smooth faceted night-glass (same material language as the
  existing `spike.jpeg` crystals) with faint swirling strokes trapped inside.
- **Palette:** teeth `#1b3a6b` lit facets over `#0a1530` shadow facets with **white-hot `#fff4d0`
  cutting edges and tips** bleeding to `#ffd23a`; hub `#14264f` vortex with `#6fb7ff` stroke
  highlights; core a small radiant `#ffd23a`/`#fff4d0` star.
- **Light:** every tooth tip self-glows hot (the danger language of this world); cool cerulean
  moonlight catches the upper-left facets.
- **Framing:** perfectly centered, perfectly circular footprint, generous transparent (magenta)
  margin all around so the spin never clips.
- **Avoid:** any asymmetric overhang breaking the circle, motion-blur streaks (the engine spins it),
  metal/steel/industrial look, red, gore, text.

## 2. `enemy-flyer.jpeg` — "Night Moth"  *(1:1 · ~512×512 · faces RIGHT · bobs and drifts in-game)*

**Where it lives:** drifts back and forth across the jump line of a spike gap right after the
high-road section — the player either threads past it or **stomps it mid-air** and bounces off its
back to cross fast. It must look hostile but *toppable* — soft and rounded on top.

- **Concept:** a plump nocturnal moth-spirit of living paint that flutters drowsily along the level's
  air currents — the airborne cousin of the existing round walker imp (`enemy.jpeg`); clearly the
  same species of creature, but winged.
- **Form:** a round, slightly squashed fuzzy body (strong circular silhouette — same body language
  as `enemy.jpeg`), **two broad moth wings swept up and back** (one near, one far for a 3/4 view), tiny
  dangling feet, short feathery antennae. The top of the body is a smooth, soft, unarmored dome —
  it must visibly be the safe place to land. Facing RIGHT.
- **Brushwork:** body fur in short curved strokes swirling like a tiny dark vortex; the wings are the
  showpiece — each wing painted as a **swirl of night-sky current**, long curling strokes of cobalt
  with cream and gold star-flecks caught in them, as if the wings are cut from the painting's sky.
- **Palette:** body `#14264f`→`#0a1530` with `#1b3a6b` swirl accents; wings `#1b3a6b` with `#6fb7ff`
  curling stroke-highlights and a few `#ffd23a`/`#fff4d0` star-flecks; two glowing **sour-yellow
  `#ffd23a` eyes** (the focal point), small dark frown; `#6fb7ff` moonlight rim along the head/top.
- **Light:** cool top-left rim, glowing eyes, faint warm under-glow beneath the wings.
- **Framing:** three-quarter side view facing right, wings raised mid-flap (one clear readable pose),
  centered with margin; no ground, it never lands.
- **Avoid:** insect realism/creepiness (no segmented legs, no mandibles), bird feathers, weapons,
  gore, text — charming-menacing, same energy as the walker imp.

## 3. `enemy-spiky.jpeg` — "Star Urchin"  *(1:1 · ~512×512 · faces RIGHT · patrols the ground)*

**Where it lives:** the late-level twist enemy — it patrols a small island between two spike pits
and **cannot be stomped**: touching it anywhere, including the top, kills. Its entire visual job is
to scream *"unlike the others, do NOT land on me."*

- **Concept:** a grumpy little urchin of living shadow-paint that grew a crown of the same lethal
  night-crystals as the level's spike hazards — the walker imp's armored, untouchable sibling.
- **Form:** same round body and stubby feet as `enemy.jpeg` BUT bristling with **8–10 sharp crystal
  spikes covering the entire top and sides** — the upper silhouette must be all points, zero safe
  dome. Spikes longest on the crown, shorter toward the sides. Facing RIGHT.
- **Brushwork:** body a tiny swirling dark vortex like the other enemies; the spikes are smooth
  faceted night-glass, **identical material to `spike.jpeg` and the saw teeth** — that material IS
  this game's "lethal" language, so reuse it exactly.
- **Palette:** body `#14264f`→`#0a1530`; spikes `#1b3a6b` facets with **white-hot `#fff4d0`/`#ffd23a`
  glowing tips on every point**; glowing `#ffd23a` eyes peeking out from under the crystal crown,
  slightly angrier than the walker's; `#6fb7ff` rim-light.
- **Light:** every spike tip self-glows (multiple tiny hot points = unmistakable danger), cool
  moonlit left side.
- **Framing:** three-quarter side view facing right, feet at bottom-center, centered with margin.
- **Avoid:** any smooth landable top surface, cute softness on the silhouette (the BODY can stay
  cute, the OUTLINE must be hostile), red, gore, text.

## 4. `overhang.jpeg` — Duck-Tunnel Shelf  *(wide 3:1 · ~1152×384 · hangs as a low ceiling)*

**Where it lives:** the low rock shelf the player must **hold Down and slide under** early in the
level. Today it reuses the floating-platform art; a dedicated piece makes the "this is a tunnel —
duck!" read instant.

- **Concept:** a heavy slab of night-earth jutting out as a low ceiling — same painted-soil material
  as `ground.jpeg`, but read from BELOW: its drama is on its underside.
- **Form:** a wide, low, horizontal slab. The **top is a flat band of swirling grass** (it's walkable
  terrain seen edge-on). The **underside is the show**: frayed, dripping paint-stalactites — short
  hanging teeth of soil and a few longer trailing wisps, clearly too low to walk under, with a dark
  hollow shadow zone right beneath them.
- **Brushwork:** soil body in dense curved horizontal impasto; the stalactite drips in downward
  comet-strokes that taper and curl, like wet paint surrendering to gravity; grass top in tiny
  upward flicks.
- **Palette:** body `#14264f`→`#0a1530` (darkest at the underside), grass band `#3a5a2e` with
  `#ffd23a` tips, a thin `#fff4d0` moonlit rim along the very top edge, faint `#6fb7ff` cool light
  catching the longest drips.
- **Light:** lit top, deeply shadowed belly — the under-shadow is what tells the player "low
  clearance."
- **Framing:** horizontal, much wider than tall, centered; magenta above and below; the underside
  detail concentrated along the bottom edge.
- **Avoid:** looking like the floating platform (no glowing top-edge rim-light band, no detached
  island feel — this is attached, heavy, oppressive), no icicles/winter read, no text.

# B. Backdrop pieces (fill the sky the level now climbs into)

## 5. `bg-wind.jpeg` — The Rolling Wind-River  *(wide 3:1 · ~1536×512 · drifts in parallax mid-sky)*

**Where it lives:** the painting's most iconic element — the horizontal river of swirling wind across
the middle of the sky — is currently missing from the game. The redesigned level climbs a wall-jump
chimney to a high road, and up there the backdrop is mostly bare gradient. This band drifts at
mid-sky in parallax and gently sways, so the high sections feel like swimming through the painting.

- **Concept:** the great rolling current of night wind from *The Starry Night* — two intertwining
  streams of luminous sky-paint curling over and into each other as they travel horizontally,
  like a slow-motion wave breaking sideways across the heavens.
- **Form:** a long horizontal band. The flow enters from the left, **rolls into one large
  over-curl** (the famous tube-like crest) around the left third, continues as a thick undulating
  stream and tapers off the right edge in trailing wisps. The band's vertical bulk stays in the
  middle ~60% of the frame, with curling tendrils breaking the top and bottom edges.
- **Brushwork:** THE purest Van Gogh statement in the whole set — long, directional, curved impasto
  strokes laid side by side, following the flow like iron filings around a magnet; strokes shorter
  and tighter inside the curl, longer and lazier in the straights; visible thick paint ridges.
- **Palette:** the stream body `#1b3a6b` and `#14264f` strokes interleaved with bright `#6fb7ff`
  and `#fff4d0` stroke-highlights along the flow lines; a handful of `#ffd23a` star-flecks swept up
  and carried inside the current; edges dissolving into separated single strokes against the magenta.
- **Light:** self-luminous — the cream/cerulean strokes glow; brightest along the crest of the curl.
- **Framing:** horizontal band, left-to-right flow, edges (especially right) tapering into loose
  strokes so two copies can overlap side by side without an obvious seam; magenta above/below.
- **Avoid:** clouds (this is wind made of paint, not cumulus), a closed circle (that's `bg-swirl` —
  this one TRAVELS horizontally), hard rectangular edges, text.

## 6. `bg-mountain.jpeg` — The Far Ridge  *(wide 4:1 · ~1536×384 · bottom-anchored, behind the hills)*

**Where it lives:** the deepest terrain layer, behind the existing rolling hills — the dark
mountain range from the painting's right side. Adds one more parallax depth step so the world
feels vast, and keeps the horizon painterly when the camera rides high.

- **Concept:** a distant, sleeping mountain ridge at the very edge of the night — barely more than
  a deep-blue shadow against the sky, the kind of mountains you feel more than see.
- **Form:** a long horizontal silhouette strip of **3–4 overlapping peaks of different heights**,
  rounded-triangular like the painting's hills (soft alpine, nothing jagged), rising from the bottom
  edge of the frame; tallest peak off-center left for asymmetry.
- **Brushwork:** simple and quiet — long diagonal strokes following each slope, slightly visible but
  far less detailed than foreground assets (it must sit BACK); a faint cooler band of atmospheric
  haze along the ridge line.
- **Palette:** atmospheric perspective — slightly LIGHTER and bluer than the existing `bg-hills`,
  so the nearer hills read darker against it: body `#14264f` lifted toward `#1b3a6b`, hazier toward
  the ridge line; crest lines kissed with the faintest `#6fb7ff` moonlit rim.
- **Light:** moonlit crests, no self-glow, no detail in the shadow mass.
- **Framing:** silhouette sits on the **bottom edge** (no gap below), magenta above the ridge line;
  the LEFT and RIGHT ends should taper to similar low heights so repeated copies chain into one
  continuous range without an obvious seam.
- **Avoid:** snow caps, detail/texture that competes with the hills in front of it, stars or sky
  painted in (the sky behind it is rendered separately), text.

---

# C. Professional-finish set (the details that separate "game jam" from "shipped")

## 7. `ground-edge.jpeg` — Painted Cliff Lip  *(tall ~2:3 · ~384×576 · overlays the LEFT end of every ground ledge; mirrored automatically for right ends)*

**Where it lives:** every pit lip and ledge end in the game. Right now the seamless ground tile just
*stops* at a pit — a hard vertical cut, the single most "unfinished tileset" tell in the build. This
piece caps every exposed end automatically.

- **Concept:** the torn end of the painted earth — where the night-soil was ripped away and the
  paint frayed at the wound.
- **Form:** designed for the **LEFT edge** of a ground block: the **right half** of the image is
  grass-and-soil matching `ground.jpeg` (it overlaps the tile and must blend into it); the **left
  half** is the exposed cliff face — a slightly concave, rough-torn vertical edge with a few short
  paint-strands and grass roots curling off it into the air, plus one or two tiny glowing soil
  specks escaping like dust.
- **Brushwork:** the face in short vertical curved strokes (torn paper made of paint); the grass
  overhanging the lip flicks slightly past the edge — the lip should read a touch rounded, never
  razor-straight.
- **Palette:** identical to `ground.jpeg`: grass `#3a5a2e` with `#ffd23a` tips, soil `#14264f`→
  `#0a1530` (darkest at the exposed face), a thin `#fff4d0` moonlit rim that wraps from the top
  edge a little way down the lip's corner.
- **Light:** the top stays lit like normal ground; the cliff face falls into shadow fast.
- **Framing:** vertical rectangle; soil/grass content bleeding off the RIGHT edge (it must merge
  invisibly with the tile it overlaps), magenta on the LEFT beyond the torn face; nothing below
  the bottom edge.
- **Avoid:** a perfectly straight vertical line (it's torn, not cut), any design element that would
  visibly repeat (this appears at dozens of lips), brightness that outshines the grass top, text.

## 8. `wall.jpeg` — Chimney Rock Face  *(1:1 square · ~512×512 · MUST tile seamlessly BOTH directions)*

**Where it lives:** the wall-jump chimney — a marquee moment in the redesigned level — plus every
vertical pit face under a ledge. These currently render as flat featureless dark pillars.

- **Concept:** compressed night-earth seen face-on — the inside of the painting's soil, dense and
  quiet, with the faintest memory of the sky's swirls trapped in the strata.
- **Form:** an all-over texture filling the entire square: gently waving horizontal-ish strata of
  packed soil with a few embedded rounded stones and 2–3 tiny dim star-specks buried in the dark.
  **No borders, no edges, no vignette** — pure repeating material.
- **Brushwork:** subdued, low-contrast curved strokes following the strata; far LESS detailed than
  `ground.jpeg` (it's a supporting surface, it must recede) but clearly hand-painted, never flat.
- **Palette:** narrow value range deep in the blues: `#0a1530` to `#14264f` with rare `#1b3a6b`
  stroke accents and the buried specks at a *dim* `#f4d35e` (low alpha feel, not bright).
- **Light:** essentially unlit — flat ambient darkness; NO directional light (it tiles in any
  position, a light direction would break the illusion).
- **Framing / CRITICAL:** the **left/right AND top/bottom edges must wrap seamlessly** — every
  edge must continue perfectly into its opposite edge. Keep all features away from forming
  noticeable rows or grids.
- **Avoid:** high contrast, recognizable shapes that repeat obviously, bricks/blocks/masonry (this
  is earth, not architecture), moss/grass (that's only on top surfaces), text.

## 9. `fg-mist.jpeg` — Foreground Night-Haze  *(wide ~3.5:1 · ~1344×384 · drifts IN FRONT of the level at 13% opacity)*

**Where it lives:** a new foreground parallax layer hugging the lower part of the frame, drifting
slowly against the camera. The scene currently has five depth layers *behind* the action and zero
in front — this completes the depth sandwich, and at 13% opacity it never costs readability.

- **Concept:** a low ribbon of night-mist rolling along the ground line — the cold air of the
  sleeping valley made visible, a breath of the same paint the sky is made of.
- **Form:** a long, loose horizontal wisp-band: 2–3 overlapping soft mist streams that thicken and
  thin along their length, with completely ragged, broken top and bottom silhouettes; large open
  magenta gaps THROUGH the band in places (it must read as wisps, not a solid bar).
- **Brushwork:** the longest, laziest strokes in the whole set — barely-curved horizontal drags
  with soft feathered ends; a few tiny star-flecks caught inside the mist.
- **Palette:** almost monochrome cool: `#6fb7ff` and `#fff4d0` strokes over hints of `#1b3a6b`;
  no saturated yellows (it would tint the gameplay reading), no darks.
- **Light:** faintly self-luminous, even, no direction.
- **Framing:** horizontal band, both ENDS tapering/dissolving to pure magenta so repeated copies
  chain without a seam; generous magenta above and below.
- **Avoid:** density (when in doubt, thinner — the engine already renders it at 13% alpha),
  cloud-puff shapes, any detail demanding attention, text.

## 10. `logo.jpeg` — "MASTERPIECE" Painted Wordmark  *(wide ~4:1 · ~1600×400 · the title screen)*
**→ this one saves to `Assets/_Game/Resources/UI/logo.jpeg` (note: UI folder, not Level1)**

**Where it lives:** the main menu — currently the word MASTERPIECE in a plain bold system font.
This is the first thing anyone sees; a painted wordmark is the single biggest "this is a real
game" upgrade available.

- **Concept:** the game's title hand-painted by Van Gogh himself — the word **MASTERPIECE** as
  thick impasto letters that ARE the night sky, hanging in the dark.
- **Form:** the single word **MASTERPIECE** in bold painterly capitals, slightly irregular letter
  heights and a gentle hand-painted baseline wobble (confident, not childish); letters built from
  visible directional strokes; the dot-free, ornament-free word only. A few small star-flecks and
  one tiny swirl-current weaving between/behind the letters — decoration kept BEHIND the
  letterforms so legibility stays perfect.
- **Brushwork:** each letter stroked like the wind-river — curved parallel impasto ridges flowing
  through the letterforms; edges soft-painted, no vector crispness, no hard black outline.
- **Palette:** letterforms predominantly `#fff4d0`/`#ffe9b0` glowing cream with `#ffd23a` cores so
  they read bright against the dark menu sky; `#6fb7ff` cool rim on the upper-left of each letter;
  the background flecks/swirl in dim `#1b3a6b`.
- **Light:** the letters self-glow softly (they're made of starlight); faint warm bloom around the
  word.
- **Framing:** the word fills the frame width with a small margin, vertically centered, ONE line;
  magenta everywhere else.
- **Avoid:** any other text (no subtitle, no "presents"), drop shadows, bevels/chrome/3D effects,
  gradients that aren't brushwork, more than one decorative swirl — the word is the art.

---

## Priority order

`saw` → `enemy-flyer` → `enemy-spiky` → `logo` → `ground-edge` → `bg-wind` → `wall` → `overhang` → `fg-mist` → `bg-mountain`

(The saw appears four times in Level 1 including the climax — it's the highest-impact single asset.
The logo is the biggest first-impression win.)

## Checklist when the images are ready

1. Save into `Assets/_Game/Resources/Level1/` with the exact base filenames above — except
   `logo.jpeg`, which goes to `Assets/_Game/Resources/UI/`.
2. Stop Play mode → let Unity recompile/import → Play.
3. Everything swaps in automatically: saws, flyers, spiky urchins, the duck shelf, pit-lip caps,
   chimney walls, the wind-river, far ridge, foreground mist and the menu wordmark. Any file you
   skip keeps its current procedural fallback.

## Beyond art — motion polish I can code on request

These aren't image prompts, but they're the remaining gap between "nice screenshots" and
"feels professional in motion": a 0.3–0.5s glow/sound wind-up before each saw reaches the jump
line (telegraphing), saw cycle phases tuned so a full-speed runner always threads them without
braking (phase-stitching), a slight camera zoom-out during boost/glide flights, coins that only
bank when you next touch safe ground (Celeste-style risk), and a soft vignette + subtle film
grain on the pixel filter.
