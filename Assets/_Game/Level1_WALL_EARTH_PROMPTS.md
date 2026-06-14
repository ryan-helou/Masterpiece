# Starry Night — Wall + Deep-Earth art briefs (for Gemini image gen)

Two textures to fix the wall-jump columns and the dead-black space under platforms. Both are
**already wired** — save with the exact filename into `Assets/_Game/Resources/Level1/`, stop Play,
let Unity recompile, press Play. Drop them in one at a time; missing files keep the current look.

- `wall_new.jpeg` — **replaces** the existing muddy bark column (the wall-jump pillars).
- `deep-earth.jpeg` — **new, optional**: the terrain body shown under platforms. If absent, the code
  tiles the cliff texture instead, so this one is a polish upgrade, not a requirement.

> ⚠️ Both are **OPAQUE, full-bleed tiles** — NOT magenta-keyed cut-outs. Fill the whole frame
> edge-to-edge with paint (no background to remove) and make them **seamlessly tileable, especially
> top↔bottom**, because the engine stacks them vertically down a column. A visible repeat seam is the
> #1 thing to avoid.

---

## THE VIBE — paste this paragraph at the top of BOTH prompts

> This artwork is for a 2D speedrun platformer that takes place *inside* Vincent van Gogh's *The
> Starry Night* — not "inspired by," the painting itself come alive and made playable. Every surface
> is built from thick, short, curved **impasto oil brushstrokes** that visibly swirl and flow.
> Nothing is airbrushed, nothing is a flat vector fill, nothing has a hard black cartoon outline —
> shapes are separated by **dark cobalt painted edges and by light**, the way Van Gogh did it. The
> single light source across the whole world is **cool moonlight from the top-left**. The engine
> applies a heavy pixel-downscale filter at runtime, so render gorgeous painterly source art — the
> impasto texture survives beautifully as shimmering pixel clusters. I will attach the current
> `wall_new.jpeg` and `player.jpeg` from `Resources/Level1/` — **match that exact brushwork density,
> palette and edge treatment** so it looks cut from the same canvas.

Palette anchors: Night Cobalt `#0a1530`, Deep Blue `#14264f`, Cypress near-black green `#0c1a10`,
warm timber browns `#3a2c1c`/`#5a4327`, mossy olive `#5b6b3a`, Cerulean moonlight `#6fb7ff`, pale
glow `#fff4d0`, chrome-yellow star fleck `#ffd23a`.

---

## 1. `wall_new.jpeg` — the moonlit timber cliff-column  *(portrait ~768×1024 · OPAQUE · tiles top↔bottom)*

**What it is in-game:** the tall vertical pillars you wall-slide and wall-jump between. The current
version reads as a flat near-black muddy bar — this replaces it. It is a **safe surface**, so it must
read as **wood / timber**, never blue (blue = death in this world).

- **Concept:** a weathered vertical timber cliff — a wall of old van-Gogh cypress-wood and bark,
  standing like a stockade of fused tree-trunks. It should read instantly as a solid, grippable,
  climbable wooden face.
- **Form & flow:** long **vertical impasto grooves** running top-to-bottom (the grain of the wood),
  with a few rounded trunk-lobes bulging out so the surface has 3-D relief, not a flat plank. Deep
  warm-brown crevices between the lobes.
- **Lighting (critical):** **cool cerulean `#6fb7ff` moonlight rakes the LEFT edge and the top of
  each ridge**; the right side and the crevices fall into warm shadow `#0c1a10`. This top-left key is
  what makes it read as rounded form instead of a bar.
- **Brightness (the main fix):** **lift the mid-tones well out of the mud** — the lit ridges should
  sit around mossy-olive/timber-brown `#5b6b3a`/`#5a4327`, clearly readable, not near-black. Only the
  deepest crevices go to `#0c1a10`. Overall this should be noticeably *brighter and more legible* than
  the current wall_new.
- **Painterly accents:** a couple of tiny chrome-yellow `#ffd23a` star-flecks and one or two pale
  pebbles caught in the bark, sparingly — they tie it to the sky without becoming busy.
- **Tiling:** **seamless top↔bottom** (a column stacks several copies) and tolerant left↔right
  (columns are mirrored at random). No single hero feature that will obviously repeat — keep the
  grain even and rhythmic.

---

## 2. `deep-earth.jpeg` — the packed-earth terrain body  *(square ~768×768 · OPAQUE · tiles all edges)*

**What it is in-game:** the cross-section *under* platforms and grass — the dirt the level is carved
out of. Right now that space is one flat dark colour; this gives it painted body. The code tints it
brighter near the surface and darker with depth, so render it **mid-toned**, not already black.

- **Concept:** packed nocturnal earth and roots seen in cross-section — van-Gogh soil, swirling
  horizontally like sediment strata dragged by a brush.
- **Form & flow:** mostly **horizontal / gently-diagonal swirling strokes** (sediment layers), with a
  few scattered rounded pebbles and pale root-threads. Distinct from the *vertical* grain of the wall
  so the two never look identical.
- **Palette:** warm earthy browns `#3a2c1c`/`#5a4327`, cypress-green-black `#0c1a10`, deep cobalt
  only in the shadow seams. **Earth tones, NOT blue** (blue = death; keep it clearly soil).
- **Accents:** very sparse — a faint chrome-yellow `#ffd23a` glimmer or a single pale pebble here and
  there, as if a buried star or stone catches the light. Keep it subtle; this sits behind gameplay.
- **Brightness:** **mid-toned** (the engine darkens it downward at runtime). Think "lit dirt just
  under the grass," not "bottom of a cave."
- **Tiling:** **seamless on all four edges** — it tiles both across and down. Even, no hero feature.
