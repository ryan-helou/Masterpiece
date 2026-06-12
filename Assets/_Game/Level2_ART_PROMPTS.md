# The Great Wave off Kanagawa — Complete Art Briefs for Level 2 (for Gemini image gen)

Level 2 is now Hokusai's *Great Wave* (the Creation of Adam version is fully retired). The level
has **no art folder yet** — every object renders as a procedural placeholder until you fill one.
This document is the complete set: **20 assets** covering terrain, hazards, the wave-pad and
Spray Pearl mechanics, all three creatures, the goal, and a six-layer ukiyo-e backdrop.
**Everything is already wired in code**: save each image with the **exact base filename** into
`Assets/_Game/Resources/Level2/` (create the folder; `.jpeg` or `.png`), stop Play, let Unity
recompile, press Play. Any file you skip keeps its procedural fallback, so generate in any order.

---

## THE LEVEL — what the art has to serve

The redesigned Level 2 is built on the classic teach → develop → twist → climax arc:

1. **Act 1 — teach the surge.** A friendly gap, plank-raft hops, a crab stomp lesson, then the
   **wave-pad** flings you full-length over safe planking (the toy is joy before it's danger),
   and immediately after, over open sea.
2. **Act 2 — the rising swell.** Patrolled islands in rhythm, a whirlpool sweeping a jump arc,
   a wall-jump chimney between two stacked hulls, spray-top hops with a high coin, a gull
   crossing the arc, a longer fling, a committed drop into the trough.
3. **Act 3 — the pearl.** The **Spray Pearl** (mid-air dash refill) is introduced over a modest
   gap, an urchin denies a stomp, a capsized hull forces feathered hops, then a wider pearl gap
   rehearses the climax skill.
4. **Climax — THE CLAW.** The painting itself, made playable: two thin foam crest-lips reach
   toward each other across a sea no single fling can cross. Sprint out the near lip, boost off
   its curling tip, catch the first pearl on the fling line, dash, catch the second hanging lower
   on the descent, dash again to the far crest. The coin glints between the pearls.
5. **Victory lap** at full tilt to the banner.

**The one visual law of the level: warm timber = safe; Prussian blue + foam = the sea = death.**
Every solid surface is shipwreck wood. Everything blue or foam-white is trying to kill you.
The player must be able to make that read at full sprint, pixelated, in a sixteenth of a second.

---

## THE VIBE — paste this whole paragraph at the top of every Gemini prompt

> This artwork is for a 2D speedrun platformer level set *inside* Katsushika Hokusai's *The
> Great Wave off Kanagawa* — the ukiyo-e woodblock print brought to life as a playable world.
> The visual language is **Edo-period Japanese woodblock print (nishiki-e)**: flat, confident
> fields of mineral pigment bounded by bold dark keylines, with **bokashi** (the hand-wiped
> soft gradient of woodblock printing) used for skies and water depth — NOT Western painterly
> rendering, NOT impasto brushstrokes, NOT anime, NOT photorealism. Shapes are stylized,
> rhythmic and graphic the way Hokusai drew water: curves that coil like claws, foam that
> breaks into clean rounded fingers and flecks. Surfaces carry the faint tooth of **rice
> paper** (washi grain) and the gentle imperfection of hand-printing — slightly dry pigment
> at the edges of color fields, a whisper of registration softness — the print is aged but
> proud. The palette is the print's own: **Prussian blue** `#0f2a43` `#1b4060` `#2d5a80`
> with pale water streaks `#7fa8c4`, foam and spray in cream `#f4eedd`, the sky in rice-paper
> tones `#e8dcc0` `#cfc3a4` with a storm-grey band `#9a9482`, and the boats' warm timber
> `#c89b62` `#7a5a34` `#4a3520` — in this game **warm wood means safe footing and blue water
> means death**, so never mix the two languages on one object. Gold accents `#e8c66a`, the
> goal's crimson `#d24a37`, and lethal danger always spoken in pure vermilion `#ff3b30` (shu,
> a true ukiyo-e pigment). **Crucially, the print is NOT a blue picture**: it is a pale
> paper-toned world of beige, cream and white that the blue wave *invades* — keep Prussian
> blue to roughly a fifth of any composition, reserved strictly for water and danger, and let
> rice-paper, cream and foam-white tones carry everything else; when in doubt, go paler and
> let foam and haze eat into the blue. Light is the flat, even light of a print — no cast shadows, no
> volumetric glow; depth comes from keyline weight, overlapping shapes and bokashi. The soul
> of the level is *the looming claw*: the held breath of a wave that will never finish
> breaking, hanging over something small and steadfast. **ABSOLUTE RULE: depict NO human
> figures, NO bodies, NO faces, NO hands — the boats are EMPTY, adrift, oars shipped; the sea
> did its work before the level began.** Animals (gull, crab, urchin) are allowed and welcome.
> The mood is vast, fatalistic and beautiful — serene Fuji watching a sea that takes whatever
> it wants. The game engine applies a pixel-downscale filter at runtime, so render crisp
> graphic source art; bold flat shapes survive pixelation better than anything. I will attach
> the original print as the style reference — **match its exact pigment palette, keyline
> weight, foam vocabulary and bokashi gradients** so every asset looks pulled from the same
> woodblock.

**Attach `Assets/_Game/Resources/Paintings/great-wave.jpg`** to every generation as the style
reference (it's the actual print). As your generated set grows, also attach 1–2 finished assets
(e.g. `ground.jpeg`) so the set stays self-consistent.

## Global delivery rules (every asset)

- **Background: flat magenta `#ff00ff` fill — NOT a checkerboard, NOT white** (the loader keys it
  out). Exception: `ground` and `wall` are full-bleed opaque tiles with no background at all.
- **One subject, centered. No scenery, no ground line, no cast shadow, no text, no frame/border,
  no signature cartouche** (the red box in the print is Hokusai's signature — never reproduce it).
- Aspect ratio and orientation matter; exact pixel counts are quality suggestions only.
- Anything with a face or facing direction faces **RIGHT** (the direction of travel).
- Silhouettes must stay readable shrunk to ~48–64 px.
- **No humans, no body parts, no figures — ever. Boats are always empty.** (Worth repeating
  inside every prompt.)

## Palette

| swatch | hex | use |
|---|---|---|
| Prussian Deep | `#0f2a43` | sea depths, darkest keylines |
| Prussian Blue | `#1b4060` | wave bodies, water mass |
| Indigo Mid | `#2d5a80` | lit water, secondary blues |
| Pale Wave | `#7fa8c4` | water streaks, highlights on blue |
| Foam Cream | `#f4eedd` | foam, spray, Fuji's snow |
| Rice Paper | `#e8dcc0` | sky light, paper tone |
| Putty Sky | `#cfc3a4` | sky band |
| Storm Grey | `#9a9482` | horizon shadow band |
| Timber Ochre | `#c89b62` | lit deck wood — SAFE footing |
| Timber Brown | `#7a5a34` | wood body |
| Timber Deep | `#4a3520` | wood shadow, keylines on wood |
| Koban Gold | `#e8c66a` | coins, gilt fittings |
| Crimson Lake | `#d24a37` | the goal banner |
| Vermilion | `#ff3b30` | lethal danger only |

**Color budget across the whole level: ~60% paper/beige/cream/white, ~25% warm timber, ~15%
Prussian blue.** Blue belongs to the sea and its weapons only — sky, haze, clouds, Fuji's
distance and all atmosphere lean beige and white, never blue-washed.

---

# THE CLAW — the level's signature set-piece (context for several assets)

The climax is the print's frozen instant, playable: two **thin foam-lipped timber crests stretch
toward each other** across a sea too wide for any single fling. The player sprints out the near
crest, hits the **wave-pad** on its very tip, flies, catches the first **Spray Pearl**
(`dash-orb`) hanging on the fling line, dashes, catches the second pearl hanging lower in the
claw's shadow, and dashes again to the far crest. Three assets build this moment: `platform`
(the tapered crest ledges), `boost-pad` (the wave-pad on the tip), and `dash-orb` (the pearl —
the single most important asset in this set). The great claw itself looms behind it all as
`bg-claw`. None of them depict the player's rescue — the *composition* is the held breath.

---

# A. Terrain & hazards

## 1. `ground.jpeg` — Lashed Deck Tile  *(1:1 SQUARE · horizontally SEAMLESS · SOLID/opaque · ~512×512)*

- **Concept:** the walkable world — salvaged ship planking lashed into a deck, the warm timber
  of the oshiokuri boats become the level's earth. Everything safe in this level is this wood.
- **Form:** fills the whole square, no background. Top ~25% is a band of sun-bleached deck
  boards running **horizontally** (the walkable surface) with a crisp pale lit line along the
  very top edge; the lower 75% is the hull's stacked strakes — long horizontal timber planks,
  darkening downward, with woodblock-style grain lines (clean, rhythmic, calligraphic — not
  photographic wood texture) and an occasional dark rope lashing or iron nail head.
- **Style:** flat color fields per plank with bold `#4a3520` keylines between boards; washi
  grain across everything; zero gloss.
- **Palette:** deck band `#c89b62` lit to `#e8dcc0` at the top line; planks below `#7a5a34` →
  `#4a3520` → near `#0f2a43`-dark at the bottom.
- **CRITICAL:** the **left and right edges must match exactly** for seamless horizontal tiling;
  keep plank lines horizontally continuous, stagger the vertical board joints, no centered
  focal detail, no obvious repeating element.
- **Avoid:** any blue or foam on this tile (wood = safe is the level's law), photographic wood
  grain, moss/barnacles in green, perspective, text.

## 2. `ground-edge.jpeg` — Splintered Plank End  *(tall ~2:3 · ~384×576 · overlays the LEFT end of every ledge; auto-mirrored for right ends)*

- **Concept:** where the deck breaks off into the sea — a snapped, splintered timber end, the
  wound where the wave bit through.
- **Form:** designed for the **LEFT edge** of a ground block: the right half matches
  `ground.jpeg` exactly — deck band over hull strakes (it overlaps the tile and must blend in
  invisibly); the left half is the break — planks snapped at staggered lengths, clean graphic
  splinter spikes pointing left, one or two splinters separated mid-air near the lip, keylines
  dense at the fracture.
- **Palette:** identical to `ground.jpeg`; broken end-grain faces fall to `#4a3520`; a thin
  `#e8dcc0` lit edge wraps the deck's corner.
- **Framing:** vertical; content bleeds off the RIGHT edge; magenta to the LEFT beyond the
  splinters; nothing below the bottom edge.
- **Avoid:** a ruler-straight break, identical splinter lengths (this appears at dozens of
  ledge ends), any foam or water on the wood, brightness exceeding the deck band, text.

## 3. `wall.jpeg` — Hull Strakes  *(1:1 square · ~512×512 · MUST tile seamlessly BOTH directions · opaque)*

- **Concept:** the side of a great hull seen face-on — quiet stacked timber, the texture for
  the wall-jump chimney and every vertical face. It must recede behind gameplay.
- **Form:** all-over material filling the square: horizontal strakes (hull planks) of subtly
  varying height, separated by slim dark keyline seams; sparse quiet detail — a faint salt
  bloom, a small iron nail head or two, one plank slightly warmer than its neighbors; washi
  grain over everything.
- **Palette:** narrow, recessive range: `#7a5a34` to `#4a3520` with rare `#c89b62` warmth;
  seams `#0f2a43`-dark.
- **Light:** flat, even — NO light direction (it tiles anywhere, including overhangs).
- **CRITICAL:** all four edges must wrap seamlessly into their opposite edges; avoid features
  that form visible rows/grids when repeated; keep contrast LOW.
- **Avoid:** blue, foam, portholes, ropes dangling (they'd repeat), brightness, text.

## 4. `platform.jpeg` — Drifting Crest Raft  *(wide 3:1 · pivot center · transparent · ~768×256)*

- **Concept:** a floating fragment of deck riding its own sliver of wave — the level's every
  airborne ledge. It must read wood-on-top (safe) at a glance, with the sea's curl carrying it.
- **Form:** a wide, low horizontal shard: a sun-bleached plank deck band on top (walkable, flat
  and clean), and below it the timber tapers and breaks; hugging the underside, a slim coil of
  Prussian-blue water curls around the wood like the small wave in front of Fuji — its foam
  edge breaking into 3–4 small rounded fingers and a few flecks beneath the raft; **both ends
  taper**, the right end stretching to a finer, slightly upturned point (the reaching tip of
  the Claw), the left end blunter and splintered.
- **Style:** bold keylines, flat fields; the water coil is clearly UNDER the walk surface —
  the top edge of the platform is 100% wood.
- **Palette:** deck `#c89b62`→`#e8dcc0` lit line; timber `#7a5a34`→`#4a3520`; water coil
  `#1b4060`/`#2d5a80` with `#f4eedd` foam fingers.
- **Framing:** horizontal, much wider than tall, centered, magenta above and below.
- **Avoid:** foam or blue touching the TOP surface (must read safe), hand/claw anatomy, a
  symmetric slab look, text.

## 5. `spike.jpeg` — Foam Claw  *(~1:1.3 tall · pivot bottom-center · transparent · ~400×512)*

- **Concept:** the level's lethal hazard — one of the wave's clawed foam fingertips, risen
  from the sea and frozen mid-grab. The print's most famous shape, weaponized.
- **Form:** a single curling talon of water pointing up and slightly forward: a Prussian-blue
  body rising from a wide base, its crest breaking into a hooked foam tip with 2–3 small
  rounded foam fingers curling off the main hook (exactly Hokusai's foam vocabulary); a few
  spray flecks frozen beside the tip; bold `#0f2a43` keylines.
- **Palette:** foam-forward — the entire upper half is creamy `#f4eedd` foam, the blue body
  (`#2d5a80` with `#7fa8c4` streak lines, `#1b4060` only at the base) confined to the lower
  half; and a burning **vermilion `#ff3b30` flush concentrated at the very tip of the
  hook** — the game's universal "lethal" signal — bleeding a short way down the foam.
- **Framing:** base flat at bottom-center, hook at top, vertical, centered. The silhouette
  must read as a claw at 48 px.
- **Avoid:** wood anywhere on it (blue = death is the law), icicle/crystal look, more than one
  talon, symmetric drips, text.

## 6. `pool.jpeg` — The Open Sea  *(wide ~2:1 · horizontally SEAMLESS · ~1024×512)*

- **Concept:** the death-liquid that fills every pit — the Great Wave's own sea, gorgeous,
  patient and absolutely fatal. (It replaces spike rows in deep pits, so it shares their
  danger language.)
- **Form:** the sea fills the whole tile below a thin strip of flat magenta at the very top
  (the air above the surface). The **surface line** is alive in Hokusai's vocabulary: low
  rolling crests whose lips break into small foam fingers and flecks of `#f4eedd`, hooked
  consistently in one direction; below the surface, layered ribbons of Prussian blue —
  `#2d5a80` streaks over `#1b4060` over depths sinking to `#0f2a43` near-black at the bottom —
  drawn as clean calligraphic current lines, not realistic murk.
- **CRITICAL:** LEFT and RIGHT edges must match exactly (it tiles into one continuous sea).
  Keep the foam pattern irregular enough that the repeat isn't obvious.
- **Palette discipline:** a faint cold `#ff3b30` shimmer may live in the foam line sparingly
  (danger whisper), but the body stays pure blue.
- **Avoid:** boats, creatures, sky reflections, photographic water, splashes above the surface
  strip, text.

## 7. `saw.jpeg` — Whirlpool Blade  *(1:1 square · ~512×512 · spins at high speed in-game)*

- **Concept:** the sweeping hazard — a tight coil of the sea torn loose, a spinning vortex of
  water and foam that patrols arcs through the level. Naruto's whirlpools by way of Hokusai.
- **Form:** a near-perfect **circular silhouette** (it rotates — asymmetry will wobble): a
  spiral of 3–4 nested Prussian-blue water ribbons coiling into the center, their outer rim
  breaking into a ring of 8–12 small hooked foam blades, every hook curled consistently
  clockwise; a small glowing **vermilion `#ff3b30` eye** at the spiral's center.
- **Style:** bold keylines between ribbons; flat fields; the foam hooks crisp and even enough
  to read as teeth when spinning. Make it **foam-forward**: the white outer tooth-ring and
  pale streaks should dominate, blue confined to the inner coils.
- **Palette:** outer tooth-ring and broad streaks `#f4eedd`/`#7fa8c4`; inner ribbons
  `#2d5a80` with `#1b4060` only near the center; center eye `#ff3b30` ringed in `#0f2a43`.
- **Framing:** perfectly centered and circular, generous magenta margin (it spins).
- **Avoid:** industrial sawblade look, motion blur (the engine spins it), breaking the
  circular outline, wood debris, text.

# B. Interactive objects

## 8. `boost-pad.jpeg` — The Wave-Pad  *(1:1 · pivot center · faces RIGHT · transparent · ~448×448)*

- **Concept:** a small charged curl of the sea's power lashed to a timber base — step on it and
  the surge hurls you toward Fuji. Sits on decks and crest-tips; the engine rotates the same
  art 90° for vertical launchers, so it must read in any orientation.
- **Form:** a compact low timber dais (two or three lashed planks, clearly SAFE wood) holding
  a horizontal **breaking curl of water**: a tight coiled wave form with its foam lip streaming
  unmistakably to the RIGHT in 2–3 trailing foam fingers and flecks — the direction language
  must be instant. The curl hovers a hair above the planks.
- **Style:** the curl is pure Hokusai — coil, lip, fingers — rendered crisp and energetic; the
  dais is plain quiet timber so the curl owns the eye.
- **Palette:** dais `#c89b62`/`#7a5a34` with `#4a3520` keylines; curl `#2d5a80` over `#1b4060`
  with `#7fa8c4` speed streaks; foam lip and trail `#f4eedd`.
- **Framing:** centered, the directional curl dominant, magenta around.
- **Avoid:** arrows/chevrons/UI symbols (the curl's stream IS the arrow), vermilion anywhere
  (this is friendly water — the ONE blue thing that helps you; keep it cool and clean), text.

## 9. `dash-orb.jpeg` — The Spray Pearl  *(1:1 · pivot center · transparent · ~448×448)*

**The most important asset in this set.** This is what hangs in the void of the Claw — the sea's
one gift, suspended where the wave's grip should close. Catching it mid-flight refills your dash.

- **Concept:** a single perfect droplet of the wave's spray, grown to a pearl — luminous,
  round, impossibly still in the chaos. Pure potential given just enough body to be caught.
- **Form:** a small intensely bright pearl-core wrapped in **two or three slim interlocking
  arcs of water** (orbit-like, suggesting a coiled current about to release), trailed by 5–7
  fine spray flecks — *flung*, not symmetric: let the flecks stream slightly longer on the
  horizontal axis, echoing the dash it grants. A soft warm rim fading to magenta.
- **Style:** the one asset allowed a self-glow — render it like moonlight through seafoam, but
  keep the print's flat-field discipline in the arcs (keylined ribbons, not lens flares).
- **Palette:** core `#f4eedd` white-hot with a `#e8dcc0` halo; arcs `#7fa8c4` over `#2d5a80`;
  flecks `#f4eedd`; the faintest outer breath of `#e8c66a` gold.
- **Framing:** centered, generous magenta margin (it pulses in-game).
- **Avoid:** hand/finger imagery, sci-fi energy-ball look, electric blue-white zaps, a literal
  pearl-in-oyster, text.

## 10. `coin.jpeg` — Gilt Wave Medallion  *(1:1 · pivot center · transparent · ~384×384)*

- **Concept:** the collectible — a round gold medallion stamped with the sea, like a temple
  offering coin lost overboard. Exactly three exist per level, always in dangerous places: it
  must look *worth it*.
- **Form:** a round, slightly thick gold coin: a raised rim ring of **seigaiha** (the classic
  Japanese overlapping wave-scallop pattern) around a smooth burnished empty center boss — no
  lettering, no square hole, no portrait, the center pure polished gold; rim minutely worn.
- **Palette:** body `#e8c66a`, center highlight and top glint `#f4eedd`, recesses `#c89b62` →
  `#7a5a34`; a soft warm halo bloom around it.
- **Light:** front-lit, glowing, one crisp specular at upper-right.
- **Framing:** perfectly front-facing, centered, generous margin (it spins in-game).
- **Avoid:** currency symbols or kanji (no text of any kind), a square center hole (reads as
  ancient Chinese cash coin — keep the boss solid), portraits, text.

## 11. `goal.jpeg` — The Crimson Nobori  *(1:2 tall · pivot bottom-center of the pole · transparent · ~512×1024)*

- **Concept:** the finish line — a tall harbor banner (nobori) on a bamboo pole, planted on the
  far shore, snapping in the storm wind. The one big crimson statement in the level: land is
  announced in the warmest color the world has.
- **Form:** a slender bamboo pole rising from the bottom (visible nodes, a simple dark iron
  finial); from a short cross-arm at its top hangs a long vertical **crimson silk banner**
  streaming and rippling to the RIGHT — deep graphic folds in the woodblock manner (flat fold
  fields separated by keylines, not soft Western shading); the banner carries a single large
  **white wave-circle crest** (a stylized circular wave roundel, foam fingers curling inside a
  ring) — a crest, NOT lettering; two small white tassel-ties flutter from the cross-arm.
- **Palette:** banner `#d24a37` with folds to a deeper `#9a3328`-adjacent dark and `#e8dcc0`-
  kissed lit edges; crest `#f4eedd`; pole `#c89b62`/`#7a5a34` with `#4a3520` nodes.
- **Framing:** vertical, pole base flat at bottom-center, banner streaming right, centered.
- **Avoid:** kanji or ANY text on the cloth (the roundel only), checkered/racing flags, a
  koinobori carp (this is a flat banner), figures hidden in the folds.

# C. Creatures (hostile, charming, and emphatically not human)

## 12. `enemy.jpeg` — Heike Crab (walker)  *(1:1 · pivot bottom-center · faces RIGHT · transparent · ~448×448)*

- **Concept:** the patrolling grunt — a stout little beach crab that marches the decks like it
  pays rent. Cute but clearly hostile; stomped from above.
- **Form:** a round, weighty crab seen in three-quarter side view facing RIGHT: a smooth
  dome-topped shell (it must look stompable — the dome unarmored and clean), four stubby
  walking legs in a busy row, two small folded claws tucked low at the front (folded, never
  raised — no grabbing gesture), drawn with bold keylines and flat woodblock color fields.
- **Face:** two small glowing **vermilion `#ff3b30` ember eyes** on short stalks and a grumpy
  little frown-line on the shell front — hostile, never scary, zero human features.
- **Palette:** shell `#d24a37` softened toward `#c89b62` at the dome's lit top (a warm crab —
  it lives on the wood and it reads as stompable), legs `#7a5a34`, keylines `#4a3520`, eyes
  `#ff3b30`.
- **Framing:** three-quarter side view facing right, legs at bottom-center.
- **Avoid:** raised pincers (reads as un-stompable), spikes on the shell, realism, gore, text.

## 13. `enemy-flyer.jpeg` — Storm Gull  *(1:1 · faces RIGHT · transparent · ~512×512 · bobs and drifts in-game)*

- **Concept:** a seabird sculpted in woodblock lines — the storm's scavenger, drifting across
  jump arcs. The player can stomp it mid-air, so its back must read as landable.
- **Form:** a plump, rounded gull in three-quarter side view facing RIGHT, wings raised
  mid-beat: broad simple wing shapes with feathers drawn as clean rhythmic keyline rows (the
  print's striped economy, not individual barbs), a smooth rounded back between the wings (the
  stomp target), short fanned tail, small head with a slim ochre beak and one **vermilion
  `#ff3b30` ember eye** under a lowered brow-line — set the brow; this bird is hostile.
- **Palette:** body `#f4eedd`/`#e8dcc0`; wingtips and feather rows `#2d5a80`→`#1b4060` (storm
  grey-blue, tying it to the sea); beak `#c89b62`; keylines `#0f2a43`; eye `#ff3b30`.
- **Framing:** three-quarter side view facing right, wings raised mid-beat, centered with
  margin; no ground, no water (it never lands).
- **Avoid:** cuteness without menace, photorealistic plumage, fish in the beak, a crane or
  sparrow silhouette (unmistakably a gull), text.

## 14. `enemy-spiky.jpeg` — Sea Urchin  *(1:1 · pivot bottom-center · faces RIGHT · transparent · ~448×448)*

- **Concept:** the crab's untouchable sibling — a fat sea urchin bristling with needle spines.
  CANNOT be stomped: its whole job is to scream *"unlike the others, do NOT land on me."*
- **Form:** a round dark body on two comically stubby tube-feet, bristling with **12–16 long
  needle spines covering the entire top and sides** — the upper silhouette must be all points,
  zero safe surface; spines longest on the crown, drawn as crisp keyline needles with flat
  fills; between the spine roots, a hint of the urchin's radial shell pattern. Facing RIGHT.
- **Face:** two small **vermilion ember eyes** glowing from the dark between spines, and
  nothing else — a grumpy pincushion.
- **Palette:** body `#0f2a43`→`#1b4060` (a creature OF the sea — death-blue, reinforcing the
  law); spines `#2d5a80` shading to **`#ff3b30` glowing tips on every point** — many small hot
  points = instant "don't touch"; feet `#7a5a34`.
- **Framing:** three-quarter side view facing right, feet at bottom-center.
- **Avoid:** any landable-looking gap in the crown, hedgehog/Sonic silhouette, soft rounded
  spine tips, text.

# D. Backdrop & atmosphere (the print's world, no figures)

## 15. `bg-fuji.jpeg` — The Witness  *(~3:2 · pivot bottom · transparent · ~768×512 · the still point of every screen)*

- **Concept:** Mount Fuji exactly as the print frames it — small, distant, serene, snow-capped,
  utterly indifferent to the violence in the foreground. The level's emotional anchor: the calm
  the player is running toward.
- **Form:** Fuji's long, low, perfect cone seen from the sea: dark blue-grey slopes, the famous
  asymmetric **snow cap** with its fingered, dripping snow-line (echoing the foam — Hokusai's
  visual rhyme: keep that rhyme), rendered in 3–4 flat tones with a **bokashi gradient** at the
  mountain's base dissolving it into haze — the base must fade out softly, no hard bottom edge,
  so it floats on the horizon.
- **Palette:** a PALE mountain — distance is beige, not blue: slopes a hazy grey-beige
  `#9a9482` warmed with `#cfc3a4`, only the upper shadow side carrying a whisper of cool
  `#7fa8c4`; snow cap a generous `#f4f6f2`-white over `#f4eedd`; the haze at the base
  dissolving through `#e8dcc0`; thin soft keyline only on the upper silhouette. White and
  beige should own at least two-thirds of the mountain.
- **Framing:** the cone centered, summit in the upper half, base bleeding off the bottom edge
  into magenta via the haze; magenta elsewhere.
- **Avoid:** a tall steep Hollywood volcano (Fuji is LOW and WIDE from this view), trees,
  villages, birds, snow covering the whole mountain, text.

## 16. `bg-clouds.jpeg` — Print Sky Band  *(wide ~3:1 · transparent · ~1344×448 · high parallax band, repeated)*

- **Concept:** the print's quiet sky — flat, stylized Edo cloud bands drifting high above the
  storm, drawn the way ukiyo-e handles weather: as calm horizontal ribbons.
- **Form:** a long horizontal band of 2–3 overlapping flat cloud forms with gently lobed
  edges, each cloud a single field of tone with one bokashi gradient across it and a slim
  keyline on its underside only; ends of the band thinning and dissolving so repeated copies
  chain without a seam.
- **Palette:** cloud bodies `#e8dcc0` and `#cfc3a4` with `#9a9482` undersides; no pure white
  (the sky must never compete with foam — foam owns white).
- **Framing:** horizontal band, magenta above/below, both ends tapering to magenta.
- **Avoid:** fluffy volumetric cumulus, storm drama (the sky in this print is eerily calm),
  birds, sun disc, text.

## 17. `bg-swell.jpeg` — Far Rollers  *(wide ~3:1 · transparent · ~1344×448 · horizon band, repeated and gently swaying)*

- **Concept:** the deep-water swell on the horizon — long heaving rollers between the level and
  Fuji, the sea's slow breathing behind the foreground violence.
- **Form:** a long horizontal band of 3–4 overlapping low swells: smooth-backed rolling forms
  drawn with calligraphic current streaks along their flanks, only the nearest one breaking a
  modest foam lip with a few small fingers (save the big claws for `bg-claw`); both ends of
  the band sink and dissolve so copies chain seamlessly.
- **Palette:** haze-washed and PALE — these are distant: swell backs `#7fa8c4` washed toward
  `#cfc3a4` beige at their crowns, with `#2d5a80` only in the troughs between them (no
  `#1b4060` deeps out here); generous foam and haze in `#f4eedd`/`#e8dcc0`; soft muted
  keylines. Cream and beige should outweigh blue across the band.
- **Framing:** horizontal band, magenta above and below, ends tapering to magenta.
- **Avoid:** big breaking claws (that's the next asset's job), boats (also a separate asset),
  Fuji, repeating identical swell shapes, text.

## 18. `bg-claw.jpeg` — The Great Claw  *(tall ~5:6 · pivot bottom · transparent · ~640×768 · looms behind the action, slowly swaying)*

- **Concept:** the wave itself — a single monumental claw of the Great Wave rising and curling
  over, frozen at the top of its arc, repeated at intervals behind the level like a range of
  hungry mountains. The level's dread made visible.
- **Form:** one great wave form rising from a wide base at the bottom edge: the towering
  Prussian-blue body climbing in layered calligraphic streaks, leaning RIGHT and curling over
  at the top into the print's iconic clawed crest — the lip shattering into a row of hooked
  foam fingers and a scatter of flecks reaching down and forward; the base runs flush off the
  bottom edge (it rises from behind the level's floor); overall silhouette tall, top-heavy,
  mid-pounce.
- **Style:** this is the masterpiece asset of the backdrop — pure Hokusai: bold keylines, flat
  layered blues, foam vocabulary exact to the reference. It is the ONE place deep Prussian
  blue is allowed to be big — and even here, **foam white should claim close to half the
  silhouette**: a thick creamy crest, fingers, and broad foam streaks raking down the face.
- **Palette:** body `#1b4060` with depths of `#0f2a43` kept small, flank streaks
  `#2d5a80`/`#7fa8c4`, generous foam crest, fingers and face-streaks in `#f4eedd`.
- **Framing:** vertical-ish, the curl reaching into the upper-right of the frame, base flush
  on the bottom edge, magenta everywhere else.
- **Avoid:** symmetry, a surfer-barrel tube (the claw hooks DOWN, it doesn't tunnel), boats or
  Fuji inside it, vermilion (background danger stays silent), text.

## 19. `bg-boat.jpeg` — Empty Oshiokuri  *(wide ~2:1 · pivot center · faces RIGHT · transparent · ~768×384 · pitches on the far swell)*

- **Concept:** one of the print's slim cargo boats — adrift and **empty**. The most loaded
  storytelling asset in the set: oars shipped, nobody aboard, riding a sea that already won.
  The player is the only living thing left out there.
- **Form:** a long, low, slender open boat in side view, bow RIGHT: the oshiokuri-bune's
  shallow curved hull in lapped timber strakes with keyline seams, a slightly upswept bow, the
  thin oars **pulled in and lying along the gunwale** (visible but clearly unmanned), the open
  hull interior a dark empty hollow; the hull pitched a few degrees as if cresting a swell
  (the engine adds the rocking).
- **Palette:** hull `#c89b62`/`#7a5a34` with `#4a3520` seams and interior shadow falling to
  `#0f2a43`-dark; a whisper of `#7fa8c4` waterline stain along the hull's bottom edge.
- **Framing:** horizontal, centered, bow to the RIGHT, generous magenta margin (it sways from
  its center); NO water under it (the backdrop swell provides the sea).
- **Avoid:** ANY rowers, oarsmen, silhouettes or suggestion of crew (the fail state of this
  asset — the boat is empty, full stop), sails or masts (oshiokuri boats have none), cargo
  details that read as huddled figures, text.

## 20. `fg-mist.jpeg` — The Spray  *(wide ~3.5:1 · transparent · ~1344×384 · drifts IN FRONT of the level at 12% opacity)*

- **Concept:** the print's famous airborne spray — the blizzard of foam flecks the claw flings
  over Fuji — drifting as a thin band in front of the action, completing the depth sandwich.
- **Form:** 2–3 long, lazy horizontal drifts made of **scattered round and teardrop foam
  flecks** of varying size (Hokusai's snow-like spray dots, exactly as the reference), with
  completely ragged silhouettes and big open magenta gaps through the band; one drift curls
  into the ghost of a tiny foam hook; flecks sparser at the edges.
- **Style:** flat cream dots with the faintest keyline on only the largest flecks — graphic,
  not misty.
- **Palette:** almost monochrome: `#f4eedd` and `#e8dcc0` flecks, a rare `#7fa8c4` fleck; no
  darks, no saturated color (it renders at 12% alpha and must never tint gameplay).
- **Framing:** horizontal band, both ends dissolving to magenta so copies chain seamlessly,
  generous magenta above and below.
- **Avoid:** fog/smoke/haze (this is DROPLETS, discrete and countable), density (sparser is
  better), recognizable shapes in the scatter, text.

---

## Priority order

`dash-orb` → `boost-pad` → `platform` → `ground` → `spike` → `pool` → `saw` → `coin` → `goal` →
`enemy` → `enemy-flyer` → `enemy-spiky` → `ground-edge` → `bg-claw` → `bg-fuji` → `bg-swell` →
`wall` → `bg-boat` → `bg-clouds` → `fg-mist`

(The Spray Pearl and wave-pad ARE the level's identity and its climax — start there. The first
six transform every screen of the level; `bg-claw` + `bg-fuji` transform the mood.)

## Checklist when the images are ready

1. Save into `Assets/_Game/Resources/Level2/` (create the folder) with the exact base filenames.
2. Stop Play mode → let Unity recompile/import → Play, then enter Level 2.
3. Everything swaps in automatically — terrain, hazards, pads, the Pearl, creatures, the crimson
   nobori, and the six-layer backdrop (Fuji on the horizon, print-sky clouds, far rollers, the
   looming claws, empty boats, spray drifting in front). Anything you skip keeps its procedural
   fallback.
4. Sanity pass 1 — **the law**: stand anywhere and squint. Wood must read warm/safe, sea must
   read blue/deadly, with zero ambiguity at speed.
5. Sanity pass 2 — **the Claw**: stand at the climax (the two crest-lips near the end). The
   tapered tips, wave-pad, both pearls and the looming `bg-claw` behind should compose the
   print's held-breath moment in one screen.
