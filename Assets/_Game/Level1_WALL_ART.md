# Level 1 — `wall.jpeg` regeneration (the wall-jump cliffs)

The tall vertical columns you wall-jump between currently use a generic swirly tile that reads as
a flat blue/black bar. I've improved the in-engine rendering (brighter night-stone tint + a
moonlit left edge and shadowed right edge so each column reads as a rounded 3-D pillar), but the
**real** fix is a dedicated vertical cliff texture. Regenerate `wall.jpeg` with the brief below
and overwrite `Assets/_Game/Resources/Level1/wall.jpeg`. No code changes needed — it's already
wired as a seamless opaque tile.

## Why this one is special
This texture **tiles both horizontally and vertically** and is stacked into TALL pillars (the
chimney walls). So it must:
- have **NO background** (full-bleed, opaque — like `ground.jpeg`, not magenta-keyed),
- **seam perfectly on all four edges** (top↔bottom especially — it stacks vertically many times),
- read as **rock/earth seen face-on**, not soil-from-above and not the swirly sky.

## The prompt

> [paste the standard Starry-Night vibe paragraph]
>
> Generate a **`wall.jpeg`**: a seamless, tileable texture of a **vertical cliff face of
> night-earth** — the painted stone wall of Van Gogh's world seen straight-on, the kind of
> sheer rock face a climber scales. Full-bleed, edge to edge, **no background, opaque**.
>
> - **Material:** packed dark earth and stone in vertical strata — long, mostly-vertical impasto
>   strokes flowing top to bottom (gravity-pulled), like a quarried cliff. A few embedded rounded
>   stones and 2–3 tiny buried gold star-specks catching light.
> - **Value:** this reads in moonlight, so keep it **dim** — near-black `#0a100d` and deep earth
>   `#14241c` in the recesses — but with enough **mid-value stone `#2c4030` ridges** that the
>   surface has visible relief and texture (NOT a flat black field; NOT bright). Think "dark rock
>   you can just make out by moonlight," with quiet green-blue undertones, never cobalt-blue.
> - **Light:** even and flat (this tiles in any position, so it must have NO baked-in light
>   direction and no single hotspot) — the game adds the moonlit edge highlight itself.
> - **Brushwork:** Van Gogh impasto, but **calmer and more vertical** than the sky — no big swirls
>   or vortices (they grid badly when stacked); think flowing vertical rock grain.
> - **CRITICAL — seamless on ALL FOUR edges:** the left edge must continue into the right edge AND
>   the top edge into the bottom edge, so copies tile in a grid with zero visible seam. Keep all
>   features away from forming obvious repeating rows or a center motif.
> - **Avoid:** grass (that's only on top surfaces), bricks/masonry/man-made joints, big swirls,
>   bright highlights, cobalt blue, any background, any text.

## Optional companion: `wall-cap.png` (a nice-to-have, not required)
If you want the *top* of each pillar to look carved rather than grass-hatted, generate a short
wide **`wall-cap.png`** (≈3:1, magenta background): a weathered stone ledge-top — a band of lit
moonlit rock with a frayed/eroded lower edge, same palette as `wall.jpeg`. Tell me and I'll wire
it as the cap on column tops. Skip it if the grass top looks fine to you.
