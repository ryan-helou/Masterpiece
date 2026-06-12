# Masterpiece

**A 2D precision speedrun platformer where every level is a famous painting brought to life.**

You don't pick levels from a menu — you walk through an art museum. Each masterpiece hangs in a
gilded frame above its plinth; step up to one, read the plaque, and *jump into the canvas*. The
painting becomes a playable world: its colors, its sky, its mood — and a platforming gauntlet built
to be run as fast as humanly possible.

---

## The concept

Two ideas pressed together:

1. **A love letter to famous art.** Each stage is themed on a real masterpiece. The palette is
   sampled straight from the canvas, and the scenery behind you is hand-built to evoke the *vibe* of
   the painting rather than photograph it — Van Gogh's swirling night sky, Munch's burning fjord.
   The actual artwork only ever appears small, framed on the museum wall you walked past to get here.

2. **A speedrunner's playground.** Movement is the whole game. The controller is tuned for flow and
   mastery, the clock is always watching, and the real opponent is your own past self.

The fantasy: sprint through the great paintings of history, shaving milliseconds off each canvas.

---

## How it plays

- **The museum is the level select.** A side-on gallery hall (New-Super-Mario-Bros–style world map).
  Walk your avatar past the frames; stand in front of a painting to see its title, your personal
  best, and coins collected. Press **jump** to enter. A final pedestal starts a **full-gallery run**
  — every painting back-to-back, with splits.

- **Run for time.** A high-precision timer arms when the level loads and starts the instant you
  move. Finish at the goal and your time is frozen, scored, and saved to the leaderboard.

- **Race your ghost.** Your best run on each level is recorded and replayed as a translucent ghost,
  so you're always chasing your own perfect line.

- **The movement is the point.** A custom-gravity platformer controller built for *feel*: variable
  jump height, coyote time, jump buffering, a light apex hang, fast-fall, a quick dash, wall jumps,
  and corner-correction so you slip past clipped ledges instead of bonking them.

---

## The paintings

Each canvas isn't just a skin — it sets the level's signature mechanic:

| # | Painting | Artist | The twist |
|---|----------|--------|-----------|
| 1 | **Starry Night** | Vincent van Gogh, 1889 | Dreamy low gravity — tap jump inside a swirl-ring for a second boost, then **glide** the long gaps. |
| 2 | **The Creation of Adam** | Michelangelo, 1512 | "The reach" — precise gaps and **spark boost-pads** that fling you across. Almost touching is never enough. |
| 3 | **The Swing** | Jean-Honoré Fragonard, 1767 | Momentum & pendulum — **springs and note-blocks**; carry your speed through the garden. |
| 4 | **The Scream** | Edvard Munch, 1893 | Disorientation — **collapsing bridges** over the fjord. The boards drop the instant you touch them, so don't stop. |

---

## Under the hood

A Unity 2D project with a deliberately code-first design. The entire game — camera, input, UI, state
machine, and every level — is constructed in C# at runtime: drop a single `GameBootstrap` component
on one GameObject, press Play, and the game builds itself. Levels are authored as char grids in code
rather than hand-placed scenes, physics runs at a crisp 120 Hz, and a pixelation pass gives the whole
thing a retro look.

### Running it

1. Open the project in Unity (the version in `ProjectSettings/ProjectVersion.txt`).
2. Open the main scene and press **Play** — or build for your platform.

---

*Sprint through history, one masterpiece at a time.*
