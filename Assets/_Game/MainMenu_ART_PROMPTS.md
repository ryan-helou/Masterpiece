# Main Menu — Gemini Art Prompts

## About this set (keep in mind for every image)

**Masterpiece** is a 2D speedrun platformer where every level is a famous painting brought to life
(Van Gogh's *Starry Night*, Hokusai's *Great Wave*, Fragonard's *The Swing*, Munch's *The Scream*).
These 10 PNGs are not separate pictures — they are **stacked layers of one screen**: the game's
**main menu, imagined as an art museum after hours**. Lights low, four masterpieces glowing on a deep
navy wall, a hushed and slightly magical mood. So every asset must feel like **one hand, one room, one
light**: the warm tungsten spotlights always come from **above**, shadows fall to **museum-navy**, and
gold leaf is the only bright thing.

**Shared look (already baked into each prompt — don't fight it):**
- *Style:* moody painterly oil illustration, soft gallery light, gilded gold leaf, faint canvas-weave
  grain over everything; elegant, reverent, nocturnal. NOT flat vector / cartoon / 3D / photographic.
- *Palette:* deep museum-navy `#11131c`, warm shadow, antique brass → gold-leaf
  (`#9a7b1f`→`#c9a227`→`#ffd166`→`#fff1c0`), cream `#e7ecf5` highlights; tiny accents of teal,
  foam-white, rose, and scream-orange leaking from the paintings.

**How the layers stack on screen (back → front):** `menu-wall` (opaque room) → `menu-frames` (the 4
paintings) → title `logo` + `plaque`/`plaque-gold` buttons → `menu-floor` (foreground floor + vignette)
→ `visitor` + `stanchion` props. `frame-ornament` crowns the title, `light-shaft` is dropped in a few
times as spotlight glow. Each piece keeps the **center clear** so the title and buttons have room.

**Pipeline:** drop every PNG into `Assets/_Game/Resources/UI/` using the **exact filename** in each
heading. PNG only. Keyed art = pure magenta `#FF00FF` background (the engine cuts it to alpha, so keep
the magenta clean and the subject edge crisp). Backgrounds = opaque, no magenta. Generate the whole set
in one session so they match; if one drifts off-palette, regenerate it.

**Generate order:** **1 → 2 → 5 → 4 → 6 → 3 → 7 → 8 → 9 → 10**.

---

## 1 — `menu-wall.png` · 2560×1440 · OPAQUE

```
A moody painterly oil illustration of the far wall of a grand but intimate art museum gallery at night, viewed head-on: a deep museum-navy (#11131c) plaster wall with subtle classical architecture — a faint cornice line near the top, a shallow arched alcove or two, the suggestion of a darkened doorway leading deeper into the museum. The wall is EMPTY of paintings but carries soft warm tungsten spotlight pools where frames will hang, falling to deep shadow at the left/right edges and ceiling; one faint cool moonlight sliver from the upper-right keeps it from going monochrome. A whisper of polished marble floor begins at the very bottom. Soft gallery lighting, gilded warmth, a faint canvas-weave grain over everything; elegant, reverent, slightly magical. Palette: deep navy, warm shadow, antique brass and gold-leaf glow, cream highlights. Symmetrical, calm, lots of negative space, nothing in the dead center (UI goes there). Full-bleed edge to edge, fully opaque. NOT flat vector, NOT cartoonish, NOT 3D, NOT photographic. Negatives: no text, no letters, no framed pictures, no people, no furniture clutter, no UI, no watermark, no harsh edges, no bright saturated colors, no daylight.
```

---

## 2 — `menu-frames.png` · 2560×1440 · MAGENTA key

```
A horizontal row of FOUR ornate carved gold-leaf picture frames hanging on an unseen wall, evenly spaced, vertically centered in the upper-middle band, the center of the image kept clear. Each frame holds a small loose impressionistic masterpiece, unmistakable by mood not detail: (1) a swirling starry night sky with a glowing moon and a dark cypress — deep blues and gold stars; (2) a great cresting wave with foam claws and a distant pale peak — Prussian blue and cream; (3) a rococo garden swing — lush greens, a rose-pink blush, dappled gold light; (4) a screaming orange-and-blood sky over a dark fjord with a swirling sky and a bridge rail. Frames are worn carved gold leaf, each lit by its own warm spotlight from above, gold catching the light, a subtle drop shadow behind each. Moody painterly oil texture, faint canvas grain, reverent and slightly magical. EVERYTHING except the frames and their paintings is pure flat magenta #FF00FF so it keys to transparent. 2560×1440, PNG. Negatives: no wall texture (magenta only outside frames), no human figures inside the paintings (skies/waves/garden/fjord only), no real letters on the nameplates, no UI, no watermark, no magenta spill onto the gold.
```

---

## 3 — `menu-floor.png` · 2560×1440 · MAGENTA key

```
The foreground floor of a night museum gallery filling only the bottom ~35% of the frame: a polished dark marble or herringbone parquet floor catching warm reflected pools of the unseen spotlights and gilded frames above, perspective gently receding to a low horizon. Long warm gold reflection streaks on the dark floor; darker toward the bottom edge. Wrapping the whole edge of the image, a soft dark navy VIGNETTE ring that fades into the magenta (so it keys to a soft translucent shadow over the screen). Above the floor line and outside the vignette: pure flat magenta #FF00FF. Feather the line where floor meets magenta — no hard edge. Moody painterly oil texture, faint canvas grain. Center-bottom kept relatively clear. 2560×1440, PNG. Negatives: no people, no furniture, no text, no frames, no bright colors, no hard magenta seam.
```

---

## 4 — `logo.png` · 2200×640 · MAGENTA key · painted wordmark

```
An ornate horizontal gilded museum cartouche — a carved gold-leaf banner with baroque scrollwork ends and a recessed dark aged-bronze center panel — engraved with the single word "MASTERPIECE" in elegant wide-tracked gold SERIF capitals (Trajan / Georgia feel), the letters cut into the bronze with a gold-leaf bevel and soft inner shadow so they catch a top-down spotlight. Spell it EXACTLY: M-A-S-T-E-R-P-I-E-C-E — one word, eleven letters, no other text. Worn, elegant, restrained baroque; gold leaf on dark bronze, cream highlights along the upper rim. Moody painterly oil texture, faint canvas grain. Centered on a pure flat magenta #FF00FF field with generous magenta margin all around; symmetrical. 2200×640, PNG. Negatives: no second word, no subtitle, no misspelling, no extra letters, no people, no watermark, no flat vector look.
```

> ⚠️ Verify the letters read MASTERPIECE exactly before keeping one. If it won't spell after a few tries, regenerate with the center panel **blank/empty (no text)** — the engine can stamp the word over it.

---

## 5 — `plaque.png` · 640×200 · MAGENTA key · 9-slice

```
A single antique brass museum label-plate: a horizontal rounded-rectangle plaque of aged brass with a thin engraved inner border and four small corner rivets, the kind screwed to a gallery wall beside a painting. Smooth EMPTY center (text is added later by the engine). Worn, warm, tactile metal. Palette: brass #9a7b1f→#c9a227, dark engraved border, NO bright gilt. Soft even top light, a gentle highlight along the top edge, faint shadow along the bottom so it reads as raised metal. CRITICAL for 9-slicing: the left and right thirds are ornate (corners + rivets), the CENTER THIRD is a plain uniform brass band with no detail so it can stretch without distortion. Perfectly horizontal, symmetrical, fills the frame with a few px of magenta margin. Background pure flat magenta #FF00FF. 640×200, PNG. Negatives: no text/letters, no people, no glow, no bright gold, no drop shadow outside the plate, no perspective tilt.
```

---

## 6 — `plaque-gold.png` · 700×220 · MAGENTA key · 9-slice

```
The hero gilded museum label-plate — same shape and 9-slice construction as a brass plaque but in BRIGHT worn gold leaf with a richer carved border, the one luminous inviting button on the screen. Smooth EMPTY center. Palette: bright gold-leaf #c9a227→#ffd166→#fff1c0 highlights; a soft warm amber glow bleeding outward into the magenta. Strong warm top-down spotlight, crisp gold highlight on the upper bevel, gentle inner shadow in the center panel, a soft radial amber glow around the whole plate. CRITICAL for 9-slicing: ornate left/right ends, a plain uniform gold center band that stretches cleanly. Horizontal, symmetrical, centered with magenta margin (leave room for the glow). Background pure flat magenta #FF00FF. 700×220, PNG. Negatives: no text, no people, no green, no hard vector edges, no perspective tilt.
```

---

## 7 — `visitor.png` · 800×1200 · MAGENTA key

```
A small hooded/cloaked figure holding a softly glowing star-shaped lantern, seen from behind or 3/4-back, standing on a gallery floor and gazing UP at the unseen paintings — the quiet museum visitor about to step into a canvas. Deep-blue hooded cloak, mostly a near-silhouette in shadow, with a warm lantern glow lighting the cloak edge and the floor at the feet. Calm, contemplative. Moody painterly oil texture, faint canvas grain. Full figure, lower portion of frame, feet near the bottom edge, centered with magenta margin. Background pure flat magenta #FF00FF. 800×1200, PNG. Negatives: no visible face or realistic human features (keep it a shadowed silhouette), no text, no second figure, no bright colors except the lantern, no magenta spill on the cloak.
```

---

## 8 — `stanchion.png` · 512×900 · MAGENTA key

```
A single brass museum stanchion (the posts that hold a velvet rope): a classic rounded brass top on a slim polished post with a weighted base, a deep-red velvet rope curving off one side and exiting the frame. Foreground prop. Brass with a warm highlight, deep crimson rope. Moody painterly oil texture, faint canvas grain. Single vertical post slightly left of center, base near the bottom, centered on pure flat magenta #FF00FF with margin. 512×900, PNG. Negatives: no text, no people, no second post, no floor, no magenta spill.
```

---

## 9 — `frame-ornament.png` · 1400×500 · MAGENTA key

```
A gilded decorative flourish to crown a title: carved gold-leaf scrollwork forming an OPEN top-and-sides border (like the top of an ornate picture frame) with a small central crest, hollow/open in the middle so a wordmark shows through. Symmetrical, baroque, worn gold leaf, soft top spotlight, faint canvas grain. Wide, horizontally symmetrical, hollow center, centered on pure flat magenta #FF00FF with margin. 1400×500, PNG. Negatives: no text, no people, no solid center (must be open), no watermark.
```

---

## 10 — `light-shaft.png` · 1024×1536 · MAGENTA key

```
A single soft volumetric spotlight cone: a beam of warm tungsten light angling down, brightest at the narrow top, fanning and fading to nothing at the bottom, faint dust motes catching in it. Very soft, additive-friendly, no hard edges. Warm amber-white light. Vertical cone, narrow top, wide soft base, centered on pure flat magenta #FF00FF everywhere else. 1024×1536, PNG. Negatives: no hard edges, no lens flare, no text, no objects, no people.
```
