# The Creation of Adam — Complete Art Briefs for Level 2 (for Gemini image gen)

Level 2 currently has **no art folder at all** — every object renders as a procedural placeholder.
This document is the complete set: 19 assets covering terrain, hazards, the spark-pad and Divine
Spark mechanics, all three creatures, the goal, and a five-layer fresco backdrop. **Everything is
already wired in code**: save each image with the **exact base filename** into
`Assets/_Game/Resources/Level2/` (`.jpeg` or `.png`), stop Play, let Unity recompile, press Play.
Any file you skip keeps its procedural fallback, so generate in any order.

---

## THE VIBE — paste this whole paragraph at the top of every Gemini prompt

> This artwork is for a 2D speedrun platformer level set *inside* Michelangelo's *The Creation of
> Adam* — the Sistine Chapel fresco brought to life as a playable world. The visual language is
> **High Renaissance buon fresco realism**: smooth, sculptural, volumetrically modeled forms with
> soft blended chiaroscuro — Michelangelo was a sculptor who painted, so every shape must feel
> carved, weighty and anatomically *believable in its physics* (folds of cloth, masses of stone,
> banks of cloud) — rendered in matte mineral pigments on aged intonaco plaster. This is NOT
> impressionism, NOT visible brushstrokes, NOT Van Gogh impasto (that's a different level of this
> game) — surfaces are smooth and blended, with the realism and finish of a master fresco, then
> aged five hundred years: a fine net of **craquelure** (hairline plaster cracks), gentle patina,
> and edges softened by time. The palette is warm and mineral — plaster-shadow umbers `#241a14`
> `#3a2a1e` `#50392a`, raw umber earth `#6e4d35`, sun-baked ochre `#9a6f48`, pale flesh-cream
> plaster `#ecc0a0` — lit by the divine: cream god-light `#fff0d2`, gold leaf `#f0c674`, and the
> famous **crimson drape** `#d24a37`, with lethal danger always spoken in pure vermilion `#ff3b30`.
> The light is warm, soft and diffuse, falling from the **top-right** of every image — in this
> level you run toward the light. The soul of the level is *the almost-touching gap*: the charged
> empty space between two reaching fingertips, the spark that jumps the final inch. **ABSOLUTE
> RULE: depict NO human figures, NO bodies, NO faces, NO hands, NO God, NO angels, NO cherubs —
> not even silhouettes.** The divine presence is expressed *only* through light, energy, floating
> cloth, clouds, stone and architecture. The mood is solemn grandeur with electric tension —
> a cathedral holding its breath. The game engine applies a pixel-downscale filter at runtime, so
> render gorgeous realistic source art; the detail survives as rich pixel clusters. I will attach
> the original painting as the style reference — **match its exact pigment palette, soft fresco
> modeling, aged plaster surface and lighting temperature** so every asset looks lifted from the
> same ceiling.

**Attach `Assets/_Game/Resources/Paintings/creation-of-adam.jpg`** to every generation as the
style reference (it's the actual painting). As your generated set grows, also attach 1–2 finished
assets (e.g. `ground.jpeg`) so the set stays self-consistent.

## Global delivery rules (every asset)

- **Background: flat magenta `#ff00ff` fill — NOT a checkerboard, NOT white** (the loader keys it
  out). Exception: `ground` and `wall` are full-bleed opaque tiles with no background at all.
- **One subject, centered. No scenery, no ground line, no cast shadow, no text, no frame/border.**
- Aspect ratio and orientation matter; exact pixel counts are quality suggestions only.
- Anything with a face or facing direction faces **RIGHT**. Light from the **top-right**.
- Silhouettes must stay readable shrunk to ~48–64 px.
- **No humans, no body parts, no figures — ever.** (Worth repeating inside every prompt.)

## Palette

| swatch | hex | use |
|---|---|---|
| Plaster Shadow | `#241a14` | deepest darks |
| Umber Dark | `#3a2a1e` | shadow mid-tone |
| Warm Sienna | `#50392a` | lit shadow |
| Raw Umber | `#6e4d35` | earth / soil body |
| Baked Ochre | `#9a6f48` | lit earth, plaster crests |
| Deep Earth | `#3a2718` | soil depths |
| Flesh Cream | `#ecc0a0` | pale plaster highlights |
| Divine Cream | `#fff0d2` | god-light, spark cores |
| Gold Leaf | `#f0c674` | coins, gilt edges |
| Terracotta | `#d98c5f` | warm accents |
| Crimson Lake | `#d24a37` | the drape, the goal |
| Vermilion | `#ff3b30` | lethal danger only |
| Chestnut | `#7a5238` | distant silhouettes |

---

# THE REACH — the level's signature set-piece (context for several assets)

The climax of this level is a literal playable Creation: two **thin stone promontories stretch
toward each other** across a void too wide for any single jump — fingertips of rock, almost
touching, the gap charged with meaning. The player sprints out the near promontory, hits the
spark-pad on its very tip, flies, catches the **Divine Spark** (`dash-orb`) hanging in the middle
of the void, and dashes the last almost-touching inch to the far fingertip. Three assets build
this moment: `platform` (the tapered promontory ledges), `boost-pad` (the spark-pad on the tip),
and `dash-orb` (the spark itself — the single most important asset in this set). None of them
depict a hand — the *composition* is the reach.

---

# A. Terrain & hazards

## 1. `ground.jpeg` — Fresco Earth Tile  *(1:1 SQUARE · horizontally SEAMLESS · SOLID/opaque · ~512×512)*

- **Concept:** the painted earth of the fresco world — a repeating terrain block of aged plaster
  ground, the warm umber land Adam reclines on, minus Adam.
- **Form:** fills the whole square, no background. Top ~25% is a crest band of sun-baked ochre
  plaster (the walkable surface) with a crisp pale lit line along the very top edge; the lower 75%
  is dense packed umber earth, darkening downward.
- **Surface / realism:** smooth blended fresco modeling — realistic compacted soil and plaster
  strata with subtle pebble masses, crossed by a fine, faint **craquelure** network (hairline
  cracks, low contrast); one or two small patches where the top pigment layer has flaked,
  revealing paler plaster beneath.
- **Palette:** crest `#9a6f48` lit to `#ecc0a0` at the top line; body `#6e4d35` → `#3a2718` →
  `#241a14` at the bottom.
- **Light:** top-lit from upper-right, soft and warm.
- **CRITICAL:** the **left and right edges must match exactly** for seamless horizontal tiling;
  keep features horizontally continuous, no centered focal detail, no obvious repeating element.
- **Avoid:** grass or vegetation (this is bare fresco earth), visible brushstrokes, cool colors,
  transparency, text.

## 2. `ground-edge.jpeg` — Broken Plaster Lip  *(tall ~2:3 · ~384×576 · overlays the LEFT end of every ledge; auto-mirrored for right ends)*

- **Concept:** where the painted earth breaks off into the void — a fractured fresco edge, as if
  this piece of the ceiling cracked away and the rest fell.
- **Form:** designed for the **LEFT edge** of a ground block: the right half is ochre-crest-over-
  umber earth matching `ground.jpeg` exactly (it overlaps the tile and must blend in invisibly);
  the left half is the exposed broken face — a slightly concave fracture with realistic layered
  strata (thin pale plaster skin, then earth), a few crumbs breaking away mid-air near the lip,
  craquelure densest near the break.
- **Palette:** identical to `ground.jpeg`; the fracture face falls to `#3a2718`/`#241a14`; a thin
  `#ecc0a0` lit edge wraps the lip's corner.
- **Light:** top-right light catches the lip; the broken face falls into shadow.
- **Framing:** vertical; content bleeds off the RIGHT edge; magenta to the LEFT beyond the
  fracture; nothing below the bottom edge.
- **Avoid:** a ruler-straight break line, repeated recognizable shapes (this appears at dozens of
  lips), brightness exceeding the crest, text.

## 3. `wall.jpeg` — Chapel Plaster Face  *(1:1 square · ~512×512 · MUST tile seamlessly BOTH directions · opaque)*

- **Concept:** the body of the fresco wall seen face-on — quiet aged intonaco, the plaster flesh
  of the chapel itself. Texture for the wall-jump chimney and every vertical pit face.
- **Form:** all-over material filling the square: subtly waving plaster with faint trowel-sweep
  undulations, a sparse craquelure net, one or two dim water-stain blooms, and the faintest ghost
  of *giornata* seams (the day-joints of true fresco — barely visible soft lines).
- **Palette:** narrow, recessive range: `#3a2a1e` to `#50392a` with rare `#6e4d35` warmth; stains
  toward `#241a14`.
- **Light:** essentially unlit, flat ambient — NO light direction (it tiles anywhere).
- **CRITICAL:** all four edges must wrap seamlessly into their opposite edges; avoid features that
  form visible rows/grids when repeated; keep contrast LOW so it recedes behind gameplay.
- **Avoid:** bricks/blocks/masonry joints, bright details, brushstrokes, text.

## 4. `platform.jpeg` — Reaching Promontory  *(wide 3:1 · pivot center · transparent · ~768×256)*

- **Concept:** a floating shard of the fresco's earth — an elongated stone promontory that tapers
  toward a point, the "fingertip" ledge of the Reach. Every floating ledge in the level uses this
  piece, so it must read as terrain first, gesture second.
- **Form:** a wide, low, horizontal shard: ochre crest band on top (walkable), realistic layered
  umber stone body below; **both ends taper** — the right end stretching to a finer, slightly
  *upturned* point (the reaching tip), the left end blunter and broken; the underside frays into
  hanging fractured strata and a few suspended crumbs, as if freshly torn from the ceiling.
- **Realism:** carved, weighty rock masses with believable fracture planes and soft fresco
  modeling; fine craquelure on the crest.
- **Palette:** crest `#9a6f48`→`#ecc0a0` lit line; body `#6e4d35`→`#3a2718`; a warm `#f0c674`
  gilt kiss along the topmost lit edge (these ledges glow faintly with purpose).
- **Light:** top-right; the tapered tip catches the most light — the eye should run along the
  ledge toward its point.
- **Framing:** horizontal, much wider than tall, centered, magenta above and below.
- **Avoid:** ANY resemblance to an actual hand or finger anatomy — this is geology that *gestures*,
  not a body part; no symmetric slab look, no vegetation, no text.

## 5. `spike.jpeg` — Gilt-Frame Splinter  *(~1:1.3 tall · pivot bottom-center · transparent · ~400×512)*

- **Concept:** the fresco's lethal hazard — a long splinter of shattered gilded picture-frame,
  fallen point-up. Beautiful, ornate, and unmistakably deadly in half a second.
- **Form:** a single sharp shard pointing straight up: wide broken base narrowing to a needle
  point; faceted like split hardwood with gold-leaf skin on one face — the gilt face catches
  light, the raw wood face stays dark; ornament fragments (a hint of carved egg-and-dart molding)
  visible near the base only.
- **Palette:** gilt face `#f0c674` with `#fff0d2` glints; raw face `#3a2a1e`/`#241a14`; and a
  burning **vermilion `#ff3b30` glow concentrated at the very tip** — the game's universal
  "lethal" signal — bleeding a short way down the point.
- **Light:** top-right on the gilt facet; self-emissive vermilion tip.
- **Framing:** base flat at bottom-center, point at top, vertical, centered.
- **Avoid:** blood, rust, cartoon zigzags, more than one shard, text.

## 6. `pool.jpeg` — Molten Gilding  *(wide ~2:1 · horizontally SEAMLESS · ~1024×512)*

- **Concept:** the death-liquid that fills every pit — a slow river of molten gold leaf and
  vermilion pigment, gorgeous and absolutely fatal. (It visually replaces the spike rows where
  pits are deep, so it shares their danger language.)
- **Form:** the pool fills the whole tile below a thin strip of flat magenta at the very top (the
  air above the surface). A bright turbulent **surface line** of `#fff0d2` over `#f0c674` along
  the top of the liquid; below it, slow realistic swirls of molten gold streaked with veins of
  `#ff3b30` vermilion and sinking darks of `#3a2a1e`, dimming to near-black `#241a14` at the
  bottom. A few small rising bubbles frozen mid-pop at the surface.
- **Realism:** honey-thick liquid metal physics — smooth meniscus swirls, not stormy waves.
- **CRITICAL:** LEFT and RIGHT edges must match exactly (it tiles into one continuous pool).
- **Light:** the surface line glows; everything below is lit by it.
- **Avoid:** lava clichés (no orange-red body — gold first, vermilion as veins), creatures,
  splashes above the surface line, text.

## 7. `saw.jpeg` — Gilt Rosette Blade  *(1:1 square · ~512×512 · spins at high speed in-game)*

- **Concept:** a ceiling rosette ornament gone murderous — a circular carved-and-gilded medallion
  whose petals have been honed into blades, sweeping up and down through the level.
- **Form:** a near-perfect **circular silhouette** (it rotates — asymmetry will wobble): a ring of
  8–12 sharp gilt acanthus-petal blades around a carved circular boss, each petal-blade hooked
  consistently clockwise; a small vermilion-glowing center medallion.
- **Realism:** crisp carved-wood-and-gold-leaf rendering, worn gilt with dark recesses between
  petals.
- **Palette:** blades `#f0c674` gilt faces over `#50392a` carved shadow, **`#fff0d2` honed
  cutting edges**, center boss `#d24a37` deepening to a `#ff3b30` glowing core.
- **Light:** top-right glints on the upper petals; self-lit core.
- **Framing:** perfectly centered and circular, generous magenta margin (it spins).
- **Avoid:** industrial sawblade look, motion blur (the engine spins it), breaking the circular
  outline, text.

# B. Interactive objects

## 8. `boost-pad.jpeg` — The Spark-Pad  *(1:1 · pivot center · faces RIGHT · transparent · ~448×448)*

- **Concept:** a small charged altar of the divine current — step on it and it hurls you toward
  the light. Sits on fingertips and ledges; the engine rotates the same art 90° for vertical
  launchers, so it must read in any orientation.
- **Form:** a compact rounded plaster dais holding a horizontal **streak of living energy**: a
  tight comet of divine light with 2–3 trailing arcs sweeping unmistakably to the RIGHT —
  the direction language must be instant. The energy hovers a hair above the dais.
- **Realism:** the dais is realistic carved plaster with gilt trim; the energy is rendered like
  slow-exposure light — smooth, luminous, physical.
- **Palette:** dais `#9a6f48`/`#6e4d35` with `#f0c674` trim; energy core `#fff0d2` wrapped in
  `#f0c674` with a faint `#d98c5f` warm halo.
- **Light:** the energy is the light source; it under-lights the dais rim.
- **Framing:** centered, the directional streak dominant, magenta around.
- **Avoid:** arrows/chevrons/UI symbols (the energy's sweep IS the arrow), lightning-bolt
  clichés, cool blues, text.

## 9. `dash-orb.jpeg` — The Divine Spark  *(1:1 · pivot center · transparent · ~448×448)*

**The most important asset in this set.** This is the spark that hangs in the exact center of the
Reach — the touch that never happened, made collectible. Catching it mid-flight refills your dash.

- **Concept:** a single point of creation-energy suspended in the void — the charge that jumps
  between fingertips. Pure light given just enough body to be caught.
- **Form:** a small intensely bright core wrapped in **two or three slim interlocking arcs of
  light** (orbit-like, suggesting contained energy about to leap), with 4–6 fine radiant filaments
  reaching outward — *reaching*, not symmetric: let two filaments stretch slightly longer
  horizontally, echoing the gap it bridges. Soft warm bloom fading to magenta.
- **Realism:** rendered like a physical phenomenon — lens-soft luminous plasma, believable light
  falloff, no flat vector glow.
- **Palette:** core `#fff0d2` white-hot; arcs `#f0c674`; filaments fading through `#d98c5f`; the
  faintest outer breath of `#d24a37`.
- **Light:** fully self-emissive — the brightest object in the level after the goal.
- **Framing:** centered, generous magenta margin (it pulses in-game).
- **Avoid:** any hand/finger imagery, sun/star clichés with triangular points, electric-blue
  sci-fi energy, text.

## 10. `coin.jpeg` — Gold-Leaf Medallion  *(1:1 · pivot center · transparent · ~384×384)*

- **Concept:** the collectible — a small gilded ceiling medallion, pried loose and spinning in
  the air. Exactly three exist per level, always in dangerous places: it must look *worth it*.
- **Form:** a round, slightly thick medallion: a raised laurel-wreath ring around an empty
  polished center boss (no figures, no profiles, no lettering — the center is pure burnished
  gold), rim minutely chipped to show age.
- **Palette:** `#f0c674` body, `#fff0d2` center highlight and top-right glint, recesses `#9a6f48`
  → `#50392a`; a soft warm halo bloom around it.
- **Light:** front-lit, glowing, crisp top-right specular.
- **Framing:** perfectly front-facing, centered, generous margin (it spins in-game).
- **Avoid:** currency symbols, portraits/heads (classic coin trap — keep the center EMPTY),
  lettering, text.

## 11. `goal.jpeg` — The Crimson Drape  *(1:2 tall · pivot bottom-center of the pole · transparent · ~512×1024)*

- **Concept:** the finish line — the famous billowing crimson cloth from the painting, hung as a
  triumphant standard from a gilded pole. The one big crimson statement in the level: you ran
  toward the light, and the light flies a flag.
- **Form:** a slender dark bronze-and-gilt pole rising from the bottom; from its top, a large
  **crimson silk banner** streams to the RIGHT in deep, realistic Renaissance folds — wind-filled,
  weighty, alive (cloth is allowed and encouraged; it must contain or suggest NO figures); the
  pole crowned with a small gilt sphere finial wrapped in a soft divine glow.
- **Realism:** masterful cloth physics — deep shadowed folds, glowing lit ridges, the silk's
  weight believable; this asset carries the painting's realism torch.
- **Palette:** banner `#d24a37` with folds to `#7a3b2a`-ish darks and `#ecc0a0`-kissed lit ridges;
  pole `#3a2a1e` with `#f0c674` bands; finial glow `#fff0d2`.
- **Light:** top-right; the banner's right edge catches the most light; finial self-glows.
- **Framing:** vertical, pole base flat at bottom-center, banner streaming right, centered.
- **Avoid:** checkered/racing flags, heraldic symbols or emblems on the cloth (plain crimson
  silk), figures hidden in the folds, text.

# C. Creatures (hostile, charming, and emphatically not human)

## 12. `enemy.jpeg` — Intonaco Imp (walker)  *(1:1 · pivot bottom-center · faces RIGHT · transparent · ~448×448)*

- **Concept:** the patrolling grunt — a lump of fresco plaster that crumbled off the ceiling and
  decided to be alive. Cute but clearly hostile; stomped from above.
- **Form:** a round, weighty blob of cracked plaster with two stubby earthen feet, no arms; its
  body shows realistic fresco strata (pale plaster skin over umber filling) with craquelure that
  *deepens around its scowl*; a strong round silhouette, smooth unarmored top (it must look
  stompable). Facing RIGHT.
- **Face:** two small glowing **vermilion `#ff3b30` ember eyes** set in dark cracks and a grumpy
  crack of a frown — hostile, never scary. NO human features beyond abstract eyes/frown.
- **Palette:** body `#9a6f48` plaster skin over `#6e4d35`/`#3a2718` core; `#ecc0a0` top-right rim
  light.
- **Light:** top-right rim; ember eyes as focal point.
- **Framing:** three-quarter side view facing right, feet at bottom-center.
- **Avoid:** humanoid proportions, arms, weapons, gore, text.

## 13. `enemy-flyer.jpeg` — Plaster Dove  *(1:1 · faces RIGHT · transparent · ~512×512 · bobs and drifts in-game)*

- **Concept:** a dove sculpted from animated plaster and gilt — the chapel's bird, gone feral.
  It drifts across jump arcs; the player can stomp it mid-air, so its back must read as landable.
- **Form:** a plump, rounded dove silhouette (a BIRD — animals are allowed; absolutely no cherub,
  no putto, no human element): broad arched wings mid-beat rendered as realistic layered plaster
  feathers with gilt-leaf edging on the flight feathers, a smooth rounded back (the stomp
  target), small head with a vermilion ember eye and a short gilt beak. Facing RIGHT.
- **Realism:** carved-marble-meets-living-bird — believable feather layering and wing anatomy,
  matte plaster surface, fine cracks at the wing joints.
- **Palette:** body `#ecc0a0`/`#9a6f48` plaster; feather edges `#f0c674`; shadows `#50392a`;
  eye `#ff3b30`.
- **Light:** top-right; soft warm under-glow reflected from below.
- **Framing:** three-quarter side view facing right, wings raised mid-beat, centered with margin;
  no ground (it never lands).
- **Avoid:** cherubs/putti or any human hybrid, peace-dove softness (this one is hostile — set
  the brow), olive branches, text.

## 14. `enemy-spiky.jpeg` — Splinter Urchin  *(1:1 · pivot bottom-center · faces RIGHT · transparent · ~448×448)*

- **Concept:** the Intonaco Imp's untouchable sibling — it armored itself in shards of the same
  shattered gilt frame as the spikes. CANNOT be stomped: its whole job is to scream
  *"unlike the others, do NOT land on me."*
- **Form:** the same round plaster body and stubby feet as `enemy.jpeg`, but bristling with
  **8–10 gilt-frame splinters covering the entire top and sides** — the upper silhouette must be
  all points, zero safe surface; splinters longest on the crown; each one a miniature of
  `spike.jpeg` with the same glowing vermilion tip. Facing RIGHT.
- **Palette:** body as `enemy.jpeg`; splinters `#f0c674` gilt over `#3a2a1e` with **`#ff3b30`
  glowing tips on every point** — many small hot points = instant "don't touch."
- **Light:** top-right; the constellation of vermilion tips is the focal point.
- **Framing:** three-quarter side view facing right, feet at bottom-center.
- **Avoid:** any landable-looking gap in the crown, hedgehog cuteness in the silhouette (body can
  stay charming, the OUTLINE must be hostile), text.

# D. Backdrop & atmosphere (the fresco sky, no figures)

## 15. `bg-rays.jpeg` — Divine Light  *(~1:1 · transparent · ~768×768 · locked to the top-right of every screen)*

- **Concept:** the presence the level runs toward — a great soft fan of god-rays breaking through
  the fresco sky. This is "God" in this game: pure light, nothing else.
- **Form:** 5–7 broad, soft-edged volumetric light shafts radiating downward-left from a bright
  unseen source at the top-right corner of the image; shafts of unequal width and intensity,
  fading and dissolving before they leave the frame; a gentle bloom mass at the source corner;
  a few tiny drifting dust sparks caught in the beams.
- **Realism:** true volumetric light through hazy air — smooth gradients, soft edges, believable
  atmospheric falloff (no hard vector rays).
- **Palette:** source bloom `#fff0d2`, shafts `#f0c674` fading through `#d98c5f` to nothing;
  everything warm.
- **Light:** it IS the light.
- **Framing:** source anchored at the top-right corner area, rays sweeping down-left, all edges
  fading to magenta.
- **Avoid:** a visible sun disc, lens flares, any figure or silhouette inside the light, hard ray
  edges, text.

## 16. `bg-clouds.jpeg` — Fresco Cloud Bank  *(wide ~3:1 · transparent · ~1344×448 · parallax band, repeated)*

- **Concept:** the sculptural cloud masses of the Sistine sky — clouds painted like carved stone,
  heavy and noble, drifting slowly behind the level.
- **Form:** a long horizontal bank of 2–3 overlapping cumulus masses with strong sculptural
  modeling — rounded carved-looking lobes, deep shadowed undersides, lit crowns; ends of the band
  thinning and dissolving so repeated copies chain without a seam.
- **Realism:** Renaissance cloud rendering — volumetric, weighty, smooth-blended; NOT photographic
  wispiness, NOT cartoon puffs.
- **Palette:** lit crowns `#ecc0a0` warmed by `#fff0d2`; bodies `#9a6f48`/`#d98c5f`; shadowed
  undersides `#50392a`→`#3a2a1e`.
- **Light:** top-right, consistent with everything else.
- **Framing:** horizontal band, magenta above/below, both ends tapering to magenta.
- **Avoid:** figures or faces emerging from clouds (classic fresco trope — explicitly none),
  blue-sky cheerfulness, storm drama, text.

## 17. `bg-drape.jpeg` — The Floating Silk  *(tall ~1:2 · transparent · ~512×1024 · sways gently in-game)*

- **Concept:** the painting's great rose-and-crimson mantle, emptied of its passengers — a huge
  ribbon of silk adrift in the sky, holding the *shape* of meaning with nobody inside it. The
  most poetic asset in the set: cloth as the ghost of the divine.
- **Form:** one long vertical ribbon of silk caught mid-billow: an S-curve of fabric with one
  open wind-filled belly, deep realistic folds, a few trailing ends curling off; it must read as
  **empty cloth** — billowed by wind alone, with NO suggestion of a body, shoulder, or limb
  beneath the folds (keep the volumes airy and hollow, fold logic driven by wind, not anatomy).
- **Realism:** the cloth-physics showpiece — weighty silk with believable tension lines, lit
  ridges and shadowed valleys, soft fresco finish, fine craquelure across the paint surface.
- **Palette:** `#d24a37` body, folds to deep `#7a5238`-adjacent darks, lit ridges kissed with
  `#ecc0a0`; a faint `#f0c674` edge where the light wraps.
- **Light:** top-right.
- **Framing:** vertical, centered, generous magenta all around (it sways from its center).
- **Avoid:** ANY body-shaped volume under the cloth (the fail state of this asset), flags,
  banners with emblems, tassels/cords, text.

## 18. `bg-arch.jpeg` — Chapel Bones  *(wide 4:1 · sits on the bottom edge · transparent above · ~1536×384 · far horizon band)*

- **Concept:** the deepest layer — the faded architecture of the chapel itself on the horizon:
  a run of pale pilasters, arches and entablature, like the painted stone framework of the
  Sistine ceiling seen through haze. The world's far edge is the building it lives in.
- **Form:** a long horizontal strip of flat-on classical architecture: a rhythm of round arches
  on pilasters with a continuous entablature line above them, plus one taller arched bay
  off-center-left for asymmetry; heavily faded and atmospheric — silhouette-plus-hints, not
  detailed rendering; the base sits flush on the bottom edge.
- **Realism:** correct classical proportions (this is Michelangelo's world — the architecture
  must be *right*), then dissolved by distance haze.
- **Palette:** hazy `#50392a` lifted toward `#7a5238` (lighter than foreground terrain so it
  reads as far), arch openings dropping to `#3a2a1e`; faint `#9a6f48` light on the entablature.
- **Light:** soft top-right, mostly flattened by haze.
- **Framing:** silhouette strip on the bottom edge, magenta above; the LEFT and RIGHT ends at
  matching heights so repeated copies chain into one continuous colonnade.
- **Avoid:** statues or figures in niches (empty niches are fine and evocative), sharp detail
  that competes with gameplay, perspective depth (keep it flat-on), text.

## 19. `fg-mist.jpeg` — Incense Haze  *(wide ~3.5:1 · transparent · ~1344×384 · drifts IN FRONT of the level at 12% opacity)*

- **Concept:** the chapel's breath — a thin ribbon of incense smoke and plaster dust drifting
  across the foreground, completing the depth sandwich in front of the action.
- **Form:** 2–3 long, lazy overlapping smoke wisps with completely ragged silhouettes and big
  open magenta gaps through the band; one wisp curls into a single soft volute; a few dust motes
  caught inside.
- **Realism:** true thin smoke physics — laminar drifts barely beginning to curl, not billowing
  clouds.
- **Palette:** almost monochrome warm: `#fff0d2` and `#ecc0a0` wisps with hints of `#d98c5f`;
  no darks, no saturated color (it renders at 12% alpha and must never tint gameplay).
- **Light:** faintly self-luminous, even.
- **Framing:** horizontal band, both ends dissolving to magenta so copies chain seamlessly,
  generous magenta above and below.
- **Avoid:** density (thinner is better), recognizable shapes in the smoke, cool tones, text.

---

## Priority order

`dash-orb` → `boost-pad` → `platform` → `ground` → `spike` → `saw` → `coin` → `goal` → `enemy` →
`enemy-flyer` → `enemy-spiky` → `ground-edge` → `bg-rays` → `pool` → `bg-clouds` → `bg-drape` →
`wall` → `bg-arch` → `fg-mist`

(The Divine Spark and spark-pad ARE the level's identity and its climax — start there. The first
six transform every screen of the level.)

## Checklist when the images are ready

1. Save into `Assets/_Game/Resources/Level2/` (create the folder) with the exact base filenames.
2. Stop Play mode → let Unity recompile/import → Play, then enter Level 2.
3. Everything swaps in automatically — terrain, hazards, pads, the Spark, creatures, the crimson
   goal, and the five-layer fresco backdrop (divine rays upper-right, clouds, floating silk,
   chapel horizon, incense foreground). Anything you skip keeps its procedural fallback.
4. Sanity pass: stand at the Reach (the two fingertip ledges near the end) — the promontory tips,
   spark-pad and Divine Spark should compose the almost-touching moment in one screen.
