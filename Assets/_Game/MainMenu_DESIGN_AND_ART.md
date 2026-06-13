# MASTERPIECE — Welcome Screen Design Plan & Art Brief

> A complete design + art-generation brief for the game's **welcome / main-menu screen** and its
> **buttons**. Read §0 for the one-screen checklist of what to generate. §1–§6 are the design.
> §7–§9 are the art manifest + the big Gemini prompts. §10 is how I'll wire it into the engine
> once the PNGs land.

---

## 0. TL;DR — what to generate in Gemini

Drop every file into **`Assets/_Game/Resources/UI/`** (create the folder). Use **PNG**. Keyed art
(anything with transparency) must be painted on a **pure magenta `#FF00FF`** background — the engine's
`ArtKey` auto-detects magenta and cuts it to transparent, decontaminating the pink fringe. Backgrounds
are **opaque, full-bleed, no magenta**.

| # | File (`Resources/UI/…`) | What | Size (px) | Background | Priority |
|---|---|---|---|---|---|
| 1 | `menu-wall.png` | Far gallery wall (parallax back layer) | 2560×1440 | opaque | ★ essential |
| 2 | `menu-frames.png` | The 4 masterpieces in gilded frames (parallax mid layer) | 2560×1440 | **magenta** | ★ essential |
| 3 | `menu-floor.png` | Polished floor + vignette (parallax front layer) | 2560×1440 | **magenta** | ★ essential |
| 4 | `logo.png` | "MASTERPIECE" gilded wordmark **or** blank cartouche | 2200×640 | **magenta** | ★ essential |
| 5 | `plaque.png` | Brass button plate (9-slice) — normal buttons | 640×200 | **magenta** | ★ essential |
| 6 | `plaque-gold.png` | Gilded button plate (9-slice) — the PLAY button | 700×220 | **magenta** | ★ essential |
| 7 | `frame-ornament.png` | Gilded corner/border flourish for the title | 1400×500 | **magenta** | ◇ high-value |
| 8 | `light-shaft.png` | Soft museum spotlight cone | 1024×1536 | **magenta** | ◇ high-value |
| 9 | `visitor.png` | The cloaked lantern avatar, gazing up | 800×1200 | **magenta** | ○ optional |
| 10 | `stanchion.png` | Brass velvet-rope post | 512×900 | **magenta** | ○ optional |
| 11 | `nameplate.png` | Small brass subtitle plate | 1200×260 | **magenta** | ○ optional |

**If you only do three:** #1, #2, #5. That alone transforms the screen.

**The title text:** AI image models render exact lettering unreliably. The robust path is **#4 as a
blank gilded cartouche** and let the engine stamp "MASTERPIECE" in Georgia over it with a gold emboss
(we already render this text today). Prompt #4 gives you *both* a with-text and a blank-cartouche
version — generate the blank one first; only chase the painted wordmark if you want it.

---

## 1. Context — the game and where the menu sits

**Masterpiece** is a 2D precision **speedrun platformer where every level is a famous painting brought
to life.** You don't pick levels from a list — you walk an **art museum**; each masterpiece hangs in a
gilded frame, and you *jump into the canvas*. The four canvases:

| # | Painting | Artist | Year | Signature mechanic |
|---|---|---|---|---|
| 1 | Starry Night | Van Gogh | 1889 | low-gravity glide, swirl-ring boosts |
| 2 | The Great Wave off Kanagawa | Hokusai | 1831 | wave-pad flings, spray-pearl dashes |
| 3 | The Swing | Fragonard | 1767 | ridden swing-seats, fountain-jet lifts |
| 4 | The Scream | Munch | 1893 | collapsing bridges over the fjord |

Identity pillars to honor on the welcome screen:
- **Reverent but playful** — a love letter to art history, not a stuffy textbook.
- **"After hours in the museum"** — moody, lamplit, hushed, a little magical.
- **Gilded & painterly** — serif type (Georgia), gold leaf, canvas grain, warm pooled light.
- **Speed underneath the elegance** — the calm gallery hides a stopwatch.

### What the menu is today (the starting point)

Built entirely in code in `UIManager.BuildMainMenu()`:
- Flat `Palette.Background` (#11131c) panel + a 5px accent rule across the very top.
- Title: optional `Resources/UI/logo` art (the slot already exists!) **or** the text "MASTERPIECE"
  at 130 pt Georgia. Subtitle "a speedrun platformer" 34 pt italic.
- Four stacked buttons — **PLAY** (hero, dark-green `UiAccentDim`), **LEADERBOARDS**, **SETTINGS**,
  **QUIT** — each a flat rounded rectangle (`UIFactory.Button`) that lightens on hover, darkens on
  press. No texture, no icon, no frame.
- Footer: "v0.1 • built with Claude Code".

It is functional and clean but **visually generic** — nothing about it says *art museum*. This brief
turns it into a place.

### Palette (from `Palette.cs`, exact hex)

| Token | Hex | Role on the menu |
|---|---|---|
| `Background` | `#11131c` | museum-wall near-black navy |
| `BackdropFar` / `BackdropNear` | `#171a27` / `#1d2233` | wall depth gradient |
| `UiPanel` / `UiPanelSoft` | `#161a26` / `#1f2536` | plate shadow / plate base |
| `UiText` | `#e7ecf5` | cream label text |
| `UiTextDim` / `UiTextFaint` | `#8893a8` / `#5a647a` | subtitle / footer |
| `UiGold` | `#ffd166` | **the hero accent — gilding** |
| `UiAccent` | `#48e0a0` | gameplay green (use sparingly here) |

**Design decision:** the menu leans **gold/brass**, not the gameplay green. Reserve green for a single
"you are here / selected" tick. Suggested extended gilt ramp for the art prompts: deep antique brass
`#9a7b1f` → gilt `#c9a227` → bright gold `#ffd166` → leaf highlight `#fff1c0`.

### Typeface

OS serif resolved at runtime — **Georgia** (Times/Palatino fallback). Everything stays in this family.
Title wants **wide letter-spacing** (museum signage), small-caps energy, and a soft gold emboss.

---

## 2. Design vision — "After Hours in the Gallery"

> The lights are low. A single guard's lamp is gone. Four masterpieces hang on a deep navy wall,
> each lit by its own warm spotlight, each faintly *alive* — Van Gogh's sky still swirls, the Great
> Wave still curls. Dust drifts in the light. A small cloaked figure with a lantern stands on the
> polished floor, looking up, about to step into a painting. The game's name is cut in gold leaf
> above. It feels like a secret — the museum is yours tonight.

That's the whole pitch. The welcome screen is **the museum entrance hall**, continuous in spirit with
the in-game gallery overworld (same plaques, same gilded frames), so booting the game already places
you *inside the fiction*.

**Three feelings, in order:** *hushed reverence* → *warm invitation* → *a coiled urge to run.*

---

## 3. Layout & composition

16:9, designed on the 1920×1080 reference canvas. Rule of thirds: title in the **upper third**,
button column on the **center-left vertical**, the avatar + footer anchoring the **lower band**, the
framed paintings as a **mid-depth chorus** behind it all.

```
┌─────────────────────────────────────────────────────────────────────┐ ← 5px gilded top rule
│  · dust ·        ╲   warm spotlight pools   ╱         · dust ·         │
│                   ╲                        ╱                          │
│            ┌────────  gilded cartouche / flourish  ────────┐          │
│                      M A S T E R P I E C E                            │  gold-embossed, wide-tracked
│            └──────────────────────────────────────────────┘          │
│                 ·  a speedrun through the great paintings  ·          │  brass nameplate
│                                                                       │
│   ┌▣▣▣┐      ┌▣▣▣┐        ┌▣▣▣┐        ┌▣▣▣┐     ← 4 framed masterpieces│
│   │Star│     │Wave│       │Swing│      │Scream│      (dim, parallax)   │
│   └───┘      └───┘        └───┘        └────┘                         │
│                                                                       │
│              ╔═══════════════════════════════╗                       │
│              ║            P L A Y            ║  ← hero gilt plaque, glows│
│              ╚═══════════════════════════════╝                       │
│                [      LEADERBOARDS      ]       ← brass plaques        │
│                [        SETTINGS        ]                             │
│                [          QUIT          ]                             │
│                                                                       │
│   🜍 visitor w/ lantern, gazing up                                     │
│   v0.1 · built with Claude Code                      · floor sheen ·   │
└─────────────────────────────────────────────────────────────────────┘
```

**Depth stack (back → front), each a parallax layer:**
1. `menu-wall` — the gallery wall + architecture + ambient spotlight glow (slowest).
2. `menu-frames` — the four gilded frames with living paintings (mid).
3. **UI layer** — title cartouche, nameplate, button column (locked to camera).
4. `menu-floor` — polished foreground floor, reflections, heavy vignette, a stanchion (fastest, drawn
   over the bottom edge so the buttons sit *in* the room).
5. `visitor` + drifting **dust motes** (front-most, subtle).

**Spacing:** generous. Negative space is the point — a gallery is mostly air. Button column ~420 px
wide, centered or slightly left; PLAY is ~10% larger than the rest with a 24 px breath beneath before
the secondary stack.

---

## 4. Visual language

- **Lighting:** warm tungsten spotlights pooling on the wall and floor; everything else falls to
  museum-navy shadow. A soft global **vignette** pulls the eye to center. One cool moonlight sliver
  from an unseen high window keeps it from going monochrome-gold.
- **Material:** **gold leaf** (frames, title, PLAY) — slightly worn, not chrome. **Antique brass**
  (secondary buttons, nameplate, stanchion). **Aged plaster / deep velvet wall**. A faint **canvas
  weave grain** over the whole screen ties it to "everything here is a painting."
- **Color discipline:** ~80% deep navy + warm shadow, ~15% gold/brass, ~5% the living paintings'
  colors leaking from their frames (a teal swirl, a foam white, a rose, a scream-orange). Green only
  as the selection tick.
- **Edges:** ornament is *carved and soft*, never sharp vector. Think gallery, not sci-fi.
- **Mood words:** hushed, gilded, lamplit, reverent, expectant, nocturnal.

---

## 5. The buttons

The buttons are **museum plaques** — the same brass label-plates you'd read beside a painting. This
makes them feel *part of the world*, not a UI overlay.

### Hierarchy
- **PLAY** — the hero. Larger (≈460×88), **gilded** (`plaque-gold`), a soft outer **gold glow**, and
  the engraved word "PLAY" in cream/gold. It should look like the one warm thing in the room.
- **LEADERBOARDS / SETTINGS / QUIT** — secondary. Smaller (≈420×70), **antique brass** (`plaque`),
  quieter, engraved label.

### States (engine-driven tints over the plate sprite)
| State | Treatment |
|---|---|
| Normal | plate at rest; label cream `#e7ecf5` |
| Hover/Focus | plate brightens ~12%; a thin gold underline sweeps in; faint spotlight grows behind it; a soft "brass tap" cue plays (`PlayClick`) |
| Pressed | plate darkens ~12%, sinks 2 px |
| Selected (gamepad) | a small green `#48e0a0` tick / corner notch — the only green on screen |

### Geometry
The plate sprites are **9-slice**: ornate fixed corners + a tileable middle, so one `plaque.png`
stretches to any label width without distorting the engraving. (I'll set `Image.type = Sliced` and
borders in code.)

---

## 6. Motion & juice (runtime — I implement, no art needed)

The screen should breathe. All of this reuses systems we already have (`Drift`, `Sway`, `Twinkle`,
`Motes`, `Sfx`, the parallax `PaintingBackdrop`, the "Gallery" music theme that already plays in menus):

1. **Parallax drift** — the three background layers ease on a slow autonomous sine (and a few px
   toward the cursor / left-stick), so the room has depth and never sits dead-still.
2. **Living frames** — the four framed paintings get tiny ambient motion: Starry Night's swirl
   slowly rotates (`Spinner`), the Wave sways (`Sway`), a rose-petal drifts on The Swing, the Scream
   sky pulses. Reuse the level backdrops at low alpha, masked to each frame.
3. **Title gleam** — a slow gold specular highlight sweeps left→right across the wordmark every ~6 s.
4. **Dust motes** — `Motes(...)` drifting through the spotlight cones with `Drift`.
5. **Button focus** — spotlight-grow + gold underline wipe + brass tap on focus; a 2 px sink + click
   on press. Gamepad/keyboard navigation already drives `EventSystem` selection.
6. **Entrance** — on first show, layers fade/parallax in from a hair of offset, the title gleam fires
   once, the PLAY plaque settles last with a single soft chime. ≤0.6 s, skippable.
7. **Idle invitation** — after ~8 s untouched, the visitor avatar shifts weight / the lantern flickers,
   nudging "press PLAY."

All of it respects `Settings.ReduceMotion` (freeze parallax/gleam/dust, keep static composition).

---

## 7. Art asset manifest (detail)

> Folder: `Assets/_Game/Resources/UI/`. Loader: `Resources.Load` via `ArtKey.Load("UI/<name>", Auto)`.
> `Auto` mode detects magenta → alpha. Backgrounds use `Opaque` (no key). PNG throughout (JPEG fringes
> on magenta edges; PNG stays clean).

| File | Layer | Keying | Placed at (ref px) | Notes |
|---|---|---|---|---|
| `menu-wall.png` | bg far | opaque | full-bleed | architecture, ambient glow, **no** frames/text |
| `menu-frames.png` | bg mid | magenta | full-bleed | only the 4 gilded frames + their paintings, rest magenta |
| `menu-floor.png` | bg front | magenta | bottom-anchored | floor + vignette ring, top half magenta |
| `logo.png` | UI | magenta | 1100×250 @ top-center | blank cartouche (recommended) or painted wordmark |
| `plaque.png` | UI | magenta | 9-slice → 420×70 | brass, tileable middle, ornate ends |
| `plaque-gold.png` | UI | magenta | 9-slice → 460×88 | gilded PLAY plate, brighter |
| `frame-ornament.png` | UI | magenta | ~1300×460 behind title | optional gilt flourish framing the title |
| `light-shaft.png` | UI/bg | magenta | scaled, additive | soft cone, several placed/tinted |
| `visitor.png` | front | magenta | ~260×380 lower-left | cloaked lantern avatar |
| `stanchion.png` | front | magenta | ~120×360 | brass velvet-rope post |
| `nameplate.png` | UI | magenta | ~520×90 under title | brass subtitle plate (optional) |

---

## 8. The art prompts

Each prompt is self-contained. Paste into Gemini / Imagen. Structure is **Subject → Style → Palette →
Lighting → Composition → Format & keying → Negatives**. Keep the *style/palette* lines identical across
assets so the set feels like one hand. **Cohesion lines** (copy verbatim into every prompt):

> *Style: a moody, painterly digital illustration in the spirit of a dim fine-art museum at night —
> oil-painted texture, soft gallery lighting, gilded gold leaf, deep navy walls, warm tungsten glow,
> a faint canvas-weave grain over everything; elegant, reverent, slightly magical; NOT flat vector,
> NOT cartoonish, NOT 3D-rendered, NOT photographic.*
> *Palette: deep museum-navy `#11131c`, warm shadow, antique brass and bright gold-leaf
> (`#9a7b1f`→`#c9a227`→`#ffd166`→`#fff1c0`), cream `#e7ecf5` highlights; tiny accents of teal,
> foam-white, rose, and scream-orange from the paintings.*

---

### 8.1 — `menu-wall.png` (background, far layer, OPAQUE)

> **Subject:** The far wall of a grand but intimate art museum gallery at night, viewed head-on — a
> deep navy plaster wall with subtle classical architecture: a faint cornice line near the top, a
> shallow arched alcove or two, the suggestion of a darkened doorway leading deeper into the museum.
> The wall is **empty of paintings** (they live on a separate layer) but carries soft warm pools of
> spotlight glow where frames will hang, and gentle falloff into shadow at the edges. A whisper of a
> polished marble floor begins at the very bottom.
>
> **Style:** [cohesion style line]
> **Palette:** [cohesion palette line] — overall very dark, navy-dominant, gold only as the warm glow.
> **Lighting:** four soft warm spotlight pools across the upper-middle wall, falling to deep shadow
> at the left/right edges and the ceiling; one faint cool moonlight sliver from upper-right.
> **Composition:** symmetrical, calm, head-on, lots of negative space; horizon/floor line low; nothing
> in the dead center (the title and buttons go there). Full-bleed, edge to edge.
> **Format & keying:** 2560×1440, 16:9, fully **opaque**, no transparency, no magenta.
> **Negatives:** no text, no letters, no framed pictures, no people, no furniture clutter, no UI,
> no watermark, no harsh vector edges, no bright saturated colors, no daylight.

---

### 8.2 — `menu-frames.png` (background, mid layer, MAGENTA key)

> **Subject:** A horizontal row of **four ornate gilded picture frames** hanging on an unseen wall,
> evenly spaced, each containing a small evocative masterpiece, painted to read at a glance:
> (1) a **swirling starry night sky** with a glowing moon and cypress — deep blues and gold stars;
> (2) a **great cresting wave** with foam claws and a distant pale peak — Prussian blue and cream;
> (3) a **rococo garden swing** — lush greens, a rose-pink blush, dappled gold light;
> (4) a **screaming orange-and-blood sky over a dark fjord** with a swirling sky and a bridge rail.
> The frames are **carved gold leaf**, slightly worn, each with a tiny brass nameplate at its base.
> Everything **except the frames is pure magenta** so it keys to transparent.
>
> **Style:** [cohesion style line] — the four inset paintings are loose, impressionistic, unmistakable
> by mood not detail.
> **Palette:** [cohesion palette line]; let each painting leak its signature color.
> **Lighting:** each frame lit by its own warm spotlight from above, gold leaf catching the light,
> subtle drop shadow behind each frame onto (implied) wall.
> **Composition:** four frames in a single horizontal row, evenly spaced across the width, vertically
> centered in the upper-middle band, sized so the center of the image stays clear. Frames roughly
> equal size; slight variation in height for life.
> **Format & keying:** 2560×1440, the **background a single flat pure-magenta `#FF00FF`**, only the
> frames + paintings painted; PNG.
> **Negatives:** no wall texture (magenta only outside frames), no human figures inside the paintings
> (keep them to skies/waves/garden/fjord), no real-world text/letters on the nameplates (illegible
> brass squiggle is fine), no UI, no watermark, no magenta spill onto the gold.

---

### 8.3 — `menu-floor.png` (background, front layer, MAGENTA key)

> **Subject:** The **foreground floor of the gallery** filling only the bottom third of the frame —
> a polished dark marble or herringbone parquet floor catching warm reflections of the (unseen)
> spotlights and gilded frames above, receding to the wall. Above the floor line, **pure magenta**.
> Wrapping the whole edge of the image, a soft dark **vignette** ring (also painted, fading from the
> magenta) that will sit over everything to focus the center.
>
> **Style:** [cohesion style line]
> **Palette:** [cohesion palette line]; floor very dark with long warm gold reflection streaks.
> **Lighting:** warm reflected pools on the polished floor directly below where frames hang; darker
> toward the bottom edge.
> **Composition:** floor occupies bottom ~35%, perspective gently receding to a low horizon; the
> vignette darkens the four corners and top edge. Center-bottom kept relatively clear.
> **Format & keying:** 2560×1440; everything above the floor + outside the vignette is **pure magenta
> `#FF00FF`**; PNG. The vignette itself should be painted dark navy fading to magenta so it keys to a
> soft translucent shadow.
> **Negatives:** no people, no furniture, no text, no frames, no bright colors, no hard line where
> floor meets magenta (feather it).

---

### 8.4 — `logo.png` (title, MAGENTA key) — generate the BLANK version first

**4a — blank gilded cartouche (RECOMMENDED, engine stamps the word):**
> **Subject:** An ornate horizontal **gilded cartouche / decorative plaque** — a carved gold-leaf
> banner with scrollwork ends and a smooth empty center panel of dark aged bronze, the kind a museum
> would engrave a gallery name onto. The center is **intentionally blank** (the game stamps text over
> it). Worn, elegant, baroque-but-restrained.
>
> **Style:** [cohesion style line] **Palette:** [cohesion palette line] — gold leaf on dark bronze.
> **Lighting:** soft top-down spotlight, gold highlights along the upper rim, shadow in the recessed
> center so overlaid cream text will pop.
> **Composition:** wide horizontal cartouche centered on a **pure magenta** field, generous magenta
> margin all around; symmetrical scrollwork; center panel ~60% of the width left clean.
> **Format & keying:** 2200×640, background **pure magenta `#FF00FF`**, PNG.
> **Negatives:** **no text, no letters, no words**, no people, no watermark, no flat vector, no second
> cartouche.

**4b — painted wordmark (OPTIONAL, expect retries — models misspell):**
> Same as 4a, but the center reads the single word **"MASTERPIECE"** in elegant wide-tracked gold
> **serif** capitals (Trajan/Georgia feel), engraved into the bronze with a gold-leaf bevel and soft
> inner shadow. Spell it exactly: M-A-S-T-E-R-P-I-E-C-E. Pure magenta background, 2200×640, PNG.
> *If the spelling comes out wrong after a few tries, fall back to 4a.*

---

### 8.5 — `plaque.png` (secondary button, MAGENTA key, 9-slice)

> **Subject:** A single **antique brass museum label-plate** — a horizontal rounded-rectangle plaque
> of aged brass with a thin engraved inner border and four small corner rivets, the kind screwed to a
> gallery wall beside a painting. Smooth empty center (text is engine-stamped). Worn, warm, tactile.
>
> **Style:** [cohesion style line] **Palette:** brass `#9a7b1f`→`#c9a227`, no bright gilt (that's for
> the PLAY plate); dark engraved border.
> **Lighting:** soft even top light, a gentle highlight along the top edge, faint shadow along the
> bottom so it reads as raised metal.
> **Composition:** the plaque **fills the frame** with a few px of magenta margin; **left/right thirds
> are ornate (corners+rivets), the center third is a plain uniform brass band** so it can be
> horizontally 9-sliced/stretched without distorting the ends. Perfectly horizontal, symmetrical.
> **Format & keying:** 640×200, background **pure magenta `#FF00FF`**, PNG.
> **Negatives:** no text/letters, no people, no glow, no bright gold, no drop shadow outside the plate,
> no perspective tilt (must tile cleanly).

---

### 8.6 — `plaque-gold.png` (PLAY button, MAGENTA key, 9-slice)

> **Subject:** The hero **gilded label-plate** — same shape and 9-slice construction as the brass
> plaque (#8.5) but in **bright worn gold leaf** with a richer carved border and a soft warm **outer
> glow** bleeding into the magenta, so it reads as the one luminous, inviting button on the screen.
> Smooth empty center.
>
> **Style:** [cohesion style line] **Palette:** bright gold-leaf `#c9a227`→`#ffd166`→`#fff1c0`
> highlights; warm amber glow.
> **Lighting:** strong warm top-down spotlight, crisp gold highlight on the upper bevel, gentle inner
> shadow in the center panel; a soft radial amber glow around the whole plate.
> **Composition:** identical layout discipline to #8.5 — ornate ends, **plain uniform center band for
> 9-slicing**, horizontal, symmetrical, centered with magenta margin (leave room for the glow).
> **Format & keying:** 700×220, background **pure magenta `#FF00FF`**, PNG.
> **Negatives:** no text, no people, no green, no hard vector edges, no perspective tilt.

---

### 8.7 — `frame-ornament.png` (title flourish, MAGENTA key) — optional

> **Subject:** A **gilded decorative flourish** to crown the title — carved gold-leaf scrollwork
> forming an open top-and-sides border (like the top of an ornate picture frame) with a small central
> crest, **open in the middle** so the wordmark shows through. Symmetrical, baroque, worn gold.
>
> **Style/Palette/Lighting:** [cohesion lines]; gold leaf on magenta, soft top spotlight.
> **Composition:** wide, horizontally symmetrical, hollow center; centered on pure magenta with margin.
> **Format & keying:** 1400×500, **pure magenta `#FF00FF`**, PNG.
> **Negatives:** no text, no people, no solid center (must be open), no watermark.

---

### 8.8 — `light-shaft.png` (spotlight cone, MAGENTA key) — optional

> **Subject:** A single soft **volumetric spotlight cone** — a beam of warm tungsten light angling
> down, brightest at the narrow top, fanning and fading to nothing at the bottom, with faint dust
> catching in it. Pure magenta everywhere else.
>
> **Style/Palette/Lighting:** [cohesion lines]; warm amber-white light, very soft, additive-friendly.
> **Composition:** vertical cone, narrow top, wide soft base; centered on pure magenta.
> **Format & keying:** 1024×1536, **pure magenta `#FF00FF`**, PNG. (Will be placed several times,
> tinted, at low alpha, additive.)
> **Negatives:** no hard edges, no lens flare, no text, no objects, no people.

---

### 8.9 — `visitor.png` (avatar, MAGENTA key) — optional

> **Subject:** A small **hooded/cloaked figure holding a softly glowing lantern**, seen from behind or
> 3/4 back, standing on a gallery floor and **gazing up** at the (unseen) paintings — the quiet
> museum visitor about to step into a canvas. Matches the in-game character (deep blue hooded cloak,
> a warm star-shaped lantern). Calm, contemplative silhouette.
>
> **Style/Palette/Lighting:** [cohesion lines]; the figure mostly in shadow (a near-silhouette) with a
> warm lantern glow lighting the cloak edge and the floor at the feet.
> **Composition:** full figure, standing, lower portion of frame, facing away/up; centered on pure
> magenta with margin; feet near the bottom edge.
> **Format & keying:** 800×1200, **pure magenta `#FF00FF`**, PNG.
> **Negatives:** no visible face/realistic human features (keep it a shadowed silhouette), no text,
> no second figure, no bright colors except the lantern, no magenta spill on the cloak.

---

### 8.10 — `stanchion.png` (velvet rope post, MAGENTA key) — optional

> **Subject:** A single **brass museum stanchion** (the posts that hold a velvet rope) with a deep-red
> velvet rope curving off one side, classic rounded brass top, weighted base. Foreground prop.
> **Style/Palette/Lighting:** [cohesion lines]; brass with a warm highlight, deep crimson rope.
> **Composition:** single vertical post slightly left of center on pure magenta, rope exiting to one
> side; base near the bottom.
> **Format & keying:** 512×900, **pure magenta `#FF00FF`**, PNG.
> **Negatives:** no text, no people, no second post, no floor.

---

### 8.11 — `nameplate.png` (subtitle plate, MAGENTA key) — optional

> **Subject:** A small horizontal **brass nameplate** (smaller, plainer than the button plaque) to sit
> under the title and carry the tagline; smooth empty center for engine text.
> **Style/Palette/Lighting:** [cohesion lines]; thin elegant brass, subtle engraved border.
> **Composition:** wide thin plate centered on pure magenta with margin; plain center band.
> **Format & keying:** 1200×260, **pure magenta `#FF00FF`**, PNG.
> **Negatives:** no text, no rivets-heavy ornament, no people.

---

## 9. Pipeline & format rules (read before generating)

- **Folder:** create `Assets/_Game/Resources/UI/` and drop every PNG there. The engine loads by name
  (`UI/menu-wall`, `UI/plaque`, …) — **exact filenames from §0** matter.
- **Magenta key:** anything with transparency is painted on **pure `#FF00FF`**. The `ArtKey` importer
  auto-detects it, cuts it to alpha, and *decontaminates* the pink fringe — but help it by keeping a
  **clean, even magenta** (don't antialias the subject into a dark halo; a crisp edge keys best).
- **PNG, not JPEG**, for all UI/keyed art — JPEG's blocky compression bleeds magenta into edges.
- **Backgrounds** (`menu-wall`) are **opaque** — no magenta, full-bleed.
- **Aspect:** backgrounds 16:9 (2560×1440). Keyed props: generous magenta margin so nothing clips.
- **No real text** in any generated art except optional `logo.png` 4b — we render type in-engine in
  Georgia for crispness and correctness.
- **Consistency:** generate the whole set in one session with the cohesion lines pasted into each
  prompt; if one looks off-palette, regenerate it rather than letting it drift.

---

## 10. Implementation plan (I wire this up once the PNGs exist)

All in `UIManager` + `UIFactory`; no new dependencies. In rough order:

1. **Background loader** — in `BuildMainMenu` (and a shared `MenuRoot` option), load `UI/menu-wall`
   (opaque), `UI/menu-frames`, `UI/menu-floor` (keyed) as three stacked full-screen `Image`s, back to
   front, with a lightweight `MenuParallax` component easing each on a slow sine + cursor offset.
   Fallback: current flat `Palette.Background` if a file is missing.
2. **Living frames** — overlay the existing level backdrops (Starry/Wave/Swing/Scream) at low alpha,
   rect-masked to each frame's screen position, with `Spinner`/`Sway`/`Twinkle` so they move. (Cheap;
   reuses `PaintingBackdrop` pieces.) Optional polish pass.
3. **Title** — place `UI/logo`. If it's the blank cartouche (4a), stamp "MASTERPIECE" over it in
   Georgia with a gold gradient + soft shadow + wide letter-spacing, plus a periodic gleam coroutine.
   If it's the painted wordmark (4b), just show the sprite. Keep the text fallback.
4. **Buttons → plaques** — extend `UIFactory.Button` to accept an optional 9-slice sprite
   (`Image.type = Sliced`, borders set from the plate). PLAY uses `plaque-gold` (+ a glow child +
   slightly larger), the rest use `plaque`. Hover = brighten + gold underline wipe + spotlight-grow +
   `PlayClick`; press = darken + 2 px sink; selected = green corner tick.
5. **Foreground** — `UI/menu-floor` over the bottom, `UI/visitor` lower-left, optional `UI/stanchion`,
   and `Motes`/`Drift` dust in the light shafts.
6. **Entrance + idle** — a short fade/parallax-in coroutine on `ShowMainMenu`; an idle nudge after ~8 s.
7. **ReduceMotion** — gate parallax/gleam/dust/idle; static composition still reads.
8. **Subtitle** — change "a speedrun platformer" → **"a speedrun through the great paintings"** on the
   `nameplate`, in italic Georgia.

**Fallbacks (ship-safe):** every art load is null-checked. With **zero** new art the menu looks exactly
as it does today; each PNG you add upgrades one layer independently, so you can generate and drop them
in any order and see incremental improvement.

---

## 11. Open choices for you

- **Menu accent:** I'm steering **gold/brass** (museum) over the gameplay green. Say if you want green
  to stay prominent.
- **Wordmark:** engine-stamped over a cartouche (robust, recommended) vs fully-painted (riskier). 
- **Avatar on the menu:** in or out? It's lovely but optional.
- **Tagline:** "a speedrun through the great paintings" vs the current "a speedrun platformer".

Once the PNGs are in `Resources/UI/`, ping me and I'll implement §10 end to end.
