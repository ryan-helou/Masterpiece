# Level 1 Art — FIX PASS (regenerate these 5, same filenames)

In-game verdict: the level reads **overwhelmingly blue** and the terrain repeats like wallpaper.
The painting is not just cobalt — it balances blue against **grass green, chrome yellow and
near-black**. The *sky layers* (swirl, moon, hills, wind) stay blue — they ARE the painting's sky
and they're working. The **playfield** (ground, walls, creatures) is what must carry the green /
yellow / black so the action layer pops off the backdrop instead of dissolving into it.

I've already added code-side mitigation (tiles now mirror and tint-cycle so repeats read less,
deep soil is near-black instead of cobalt) — but the source art is the real fix. Regenerate these
five files, **same exact filenames**, into `Assets/_Game/Resources/Level1/` (overwrite).

## Add this paragraph to the vibe text for every regeneration

> COLOR BALANCE ORDER: the previous batch came out almost entirely cobalt blue and it flattened
> the scene. Rebalance: blues are only ~40% of this object; the rest is **near-black `#0a100d`
> (the painting's deep darks), grass greens `#3a5a2e` → `#6a8f3f`, and warm chrome-yellow /
> cream accents `#ffd23a` `#fff4d0`**. Push VALUE CONTRAST hard — deep blacks against bright
> lit edges; if it would look monochrome in grayscale, it's wrong. This is still *The Starry
> Night*: impasto strokes, dark cobalt edges, moonlit top-left light — but the earth and its
> creatures are dark, green and gold, not blue.

Attach `player.jpeg` + the original painting as style refs, flat magenta `#ff00ff` background
as before (`ground` and `wall` stay full-bleed opaque, no magenta).

---

## 1. `ground.jpeg` — REGENERATE  *(1:1, horizontally seamless, opaque)*
The current tile's swirling blue soil repeats hundreds of times side by side and reads as
patterned wallpaper (see screenshots: obvious horizontal banding).
- **Grass band (top ~30%):** brighter and warmer than before — yellow-green `#6a8f3f` strokes
  over `#3a5a2e`, generous `#ffd23a` flicked tips, crisp `#fff4d0` moonlit line at the very top.
  This band can stay lively — it's the level's green statement.
- **Soil (lower ~70%): QUIET.** Near-black `#0a100d`→`#141a14`, almost texture-free: just 2–3
  faint, widely-spaced dark-green/blue stroke hints and a couple of tiny buried gold specks.
  **No bold swirls, no repeated motif, no visible "design"** — this area tiles 200× and any
  pattern becomes wallpaper. Value variation ≤ 15% across the soil.
- Left/right edges must still match exactly (seamless).

## 2. `wall.jpeg` — REGENERATE  *(1:1, seamless BOTH directions, opaque)*
Current wall is mid-blue with a wavy pattern that grids visibly (see chimney screenshot).
- Near-black compacted night-earth `#0a100d`→`#10160f` with the faintest vertical strata;
  2–3 small moss-green `#2a3f22` patches and one or two dim gold specks per tile.
- **Even quieter than the soil** — contrast within the tile ≤ 10%; absolutely no wave/swirl
  motif (it grids instantly when stacked); all four edges wrap seamlessly.

## 3. `enemy.jpeg` — REGENERATE  *(1:1, faces RIGHT, magenta bg)*
The imp is currently a blue blob that vanishes against blue backgrounds.
- Body: **near-black `#0a100d` living shadow** with dark-green `#2a3f22` swirl accents — NOT
  cobalt. A strong **warm cream `#fff4d0` moonlit rim** along the head/back (top-left light).
- Eyes: bigger, hotter — `#ffd23a` cores with `#fff4d0` centers; this is the focal point and
  must read at 48 px against ANY background.
- Same round stompable silhouette, stubby feet, grumpy frown. Charming-menacing.

## 4. `enemy-flyer.jpeg` — REGENERATE  *(1:1, faces RIGHT, wings mid-beat, magenta bg)*
- Body: near-black like the walker (one family), warm cream rim, hot yellow eyes.
- Wings: keep the sky-swirl idea but invert the balance — **cream/gold leading edges and
  stroke-highlights over dark green-black wing fields**, only hints of blue. The wings should
  glow against the night sky, not dissolve into it.
- Soft rounded top (it's stompable).

## 5. `enemy-spiky.jpeg` — REGENERATE  *(1:1, faces RIGHT, magenta bg)*
- Body: near-black with green swirl accents, cream rim — matching its siblings.
- Crystal spikes: dark green-black `#0c1a10` facets with **white-hot `#fff4d0`→`#ffd23a` tips
  on every point** — the constellation of hot tips against a black body is the "don't touch"
  read. No safe-looking gap on top.

---

**After `ground.jpeg` is regenerated**, also regenerate `ground-edge.jpeg` and `overhang.jpeg`
with the new ground attached as reference so the lips and shelves match the new earth (same
briefs as before, new palette). The backdrop set (moon, stars, swirl, hills, village, cypress,
wind, mountain, mist) and the gameplay objects (coin, ring, spring, spike, pool, saw, goal,
feather) are working — don't touch them in this pass.
