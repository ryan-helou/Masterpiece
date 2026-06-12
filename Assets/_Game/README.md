# VELOCITY — a speedrun platformer (template)

A complete, fully-playable 2D speedrun platformer skeleton built for Unity 6 (URP, new Input
System). Tight Mario/Cuphead-style movement, a millisecond speedrun timer, a local best-times
leaderboard, short levels, a full menu flow, and a settings screen with **remappable controls**.

Everything is generated from code at runtime (geometry, sprites, audio, UI) so there is **nothing to
wire up in the editor**.

## 🖼 The Gallery — eight paintings, eight levels

The game is one world, **"The Gallery,"** of eight levels — each themed around a famous painting. The
real (public-domain) artwork hangs behind the level as a dimmed backdrop, the whole palette is
sampled from it, and the **mechanics are matched to the art**:

| # | Painting | Theme |
|---|----------|-------|
| 1 | Starry Night (van Gogh) | low-gravity night — swirl-rings + glide |
| 2 | The Creation of Adam (Michelangelo) | "the reach" — gaps + spark boost-pads |
| 3 | The Swing (Fragonard) | momentum — springs, note-blocks, swinging saws |
| 4 | The Scream (Munch) | disorientation — collapsing bridges |
| 5 | Napoleon Crossing the Alps (David) | the charge — boost-pad speed run |
| 6 | Wanderer above the Sea of Fog (Friedrich) | the ascent — required wall-climbs |
| 7 | The Night Watch (Rembrandt) | light & shadow — enemy gauntlet |
| 8 | The Great Wave off Kanagawa (Hokusai) | ride the wave — float + bounce finale |

The level-select is a **museum walk** (New-Super-Mario-Bros style): stroll an avatar down a gallery
hall of framed paintings on plinths; stand in front of one to see its name, your PB and coins on the
plaque, and press **Jump** to step into the painting. The pedestal at the end starts a full run.

Paintings live in `Assets/_Game/Resources/Paintings/`. To swap one: drop a new image there and edit
its row (name / painter / `ResourceKey` / colours) in **`Util/PaintingThemes.cs`** — the table order
is the level order. Everything else (backdrop, palette, plaque, overworld frame) follows.

**Also included (polish pass):** ghost replay of your best run, generative ambient music, screen
shake + hit-pause + particle bursts (death/finish/jump/land/dash), a fixed-width timer that never
jiggles, a parallax starfield backdrop, animated level-intro cards, gamepad/keyboard menu
navigation, and extra settings (music volume, ghost on/off, screen-shake, "reset timer on death").

**Plus (systems pass):**
- **Two worlds, 8 levels.** World 2 ("Kinetic") adds bounce pads, conveyor belts, crumbling
  platforms and moving saw blades.
- **Full-run mode** — race a whole world back-to-back on one cumulative timer with per-level
  splits and a saved best-run time. Hit **FULL RUN** on the level-select screen.
- **Optional coins** per level (collected count saved + shown on results).
- **Display & accessibility settings** — fullscreen, vsync, frame cap, reduce-motion, color
  themes (incl. a colorblind-friendly palette), and an optional 3-2-1 countdown start.
- **Editable levels** — make a `LevelAsset` via right-click → Create → Velocity → Level, drop it in
  a `Resources/Levels` folder, and it shows up in-game as a "Custom" world.

---

## ▶ How to run it

1. Open the project in **Unity 6000.4.10f1** (or newer 6.x).
2. Open the scene **`Assets/_Game/Scenes/Game.unity`**.
3. Press **Play**.

That's it. The scene contains a single `[Bootstrap]` object that builds the whole game.

> If you press Play from a different scene, it won't run — open `Game.unity`. It's already set as
> the first scene in Build Settings, so builds will start there automatically.

---

## 🎮 Default controls

| Action   | Keyboard            | Gamepad        |
|----------|---------------------|----------------|
| Move     | WASD / Arrow keys   | Left stick / D-pad |
| Jump     | Space               | A / Cross      |
| Dash     | Left Shift          | X / Square     |
| Restart  | R                   | Y / Triangle   |
| Pause    | Esc                 | Start          |

All keyboard bindings are **remappable** in **Settings → Controls** (saved automatically).

### Movement feel (already tuned)
- **Variable jump height** — tap for a short hop, hold for a full jump.
- **Coyote time** — you can still jump for a few frames after leaving a ledge.
- **Jump buffering** — press jump slightly before landing and it still fires.
- **Apex hang** + **fast-fall** for a snappy, weighty arc.
- **Air dash** (refills on landing) — Cuphead-style horizontal burst. Dash-jump is possible.

---

## ⏱ Speedrun rules

- The timer **arms** on level load and **starts on your first input**, so you can settle first.
- It **stops** the instant you touch the green goal.
- **Death = full restart** of the attempt (timer resets). Hit **R** any time to restart.
- Each level keeps your **top 10 times** locally; the fastest is your PB. Beat it for a NEW RECORD.
- Each level has **Gold / Silver** target times for medals (cosmetic goals).

The "leaderboard" is your **local** personal-best ladder (stored in PlayerPrefs). To make it online,
swap `Leaderboard.cs` for a backend call — the rest of the game already routes through it.

---

## 🗂 Project layout

```
Assets/_Game/
  Scenes/Game.unity            ← the only scene (one [Bootstrap] object)
  Scripts/
    Core/
      GameBootstrap.cs         ← builds camera, EventSystem, input, UI, GameManager
      GameManager.cs           ← state machine, timer, level load/unload, navigation
      Settings.cs              ← audio/display prefs (PlayerPrefs)
      Leaderboard.cs           ← per-level best times (PlayerPrefs)
      SpeedrunTimer.cs         ← high-precision run timer + M:SS.mmm formatting
      GameLayers.cs            ← physics layer constants
    Input/GameInput.cs         ← code-defined, rebindable controls + persistence
    Player/PlayerController.cs ← the movement (run/jump/dash, game feel)
    Player/Ghost.cs            ← record best run + replay a translucent racer
    Camera/CameraController.cs ← smooth follow + look-ahead + bounds + screenshake
    Camera/Backdrop.cs         ← parallax starfield
    Level/
      LevelDefinition.cs       ← level data (ASCII grid + medal times)
      LevelLibrary.cs          ← World 1 levels + a small drawing helper
      LevelBuilder.cs          ← turns ASCII into geometry/hazards/goal
      Goal.cs / Hazard.cs / MovingPlatform.cs
    UI/UIFactory.cs            ← code-only UGUI builder (uses built-in font, no TMP)
    UI/UIManager.cs            ← every screen (menu, HUD, pause, results, settings, leaderboard)
    UI/MonoText.cs             ← fixed-width timer (per-glyph cells, no jiggle)
    Util/
      Palette.cs               ← all colors (reskin here)
      TextureFactory.cs        ← runtime sprite generation
      Sfx.cs                   ← runtime procedural sound effects
      Music.cs                 ← generative ambient music loop
      FX.cs                    ← particle bursts (death/finish/jump/land/dash)
```

---

## ✏️ Making / editing levels

Levels live in `LevelLibrary.cs`. Each is drawn with a tiny grid helper (bottom-left origin, y up):

```csharp
var d = new LevelDraw(48, 14);   // width, height in tiles (1 tile = 1 world unit)
d.Floor(0, 47, 2);               // solid floor, 2 tiles thick
d.Pit(14, 16);                   // carve a spike pit through the floor
d.Block(20, 3, 4, 1);            // a 4x1 solid platform at (20,3)
d.OneWay(31, 34, 5);             // one-way platform you can jump up through
d.Set(16, 11, 'V');              // a vertical moving lift
d.Spawn(3, 2);                   // player start
d.Goal(45, 2);                   // finish
return new LevelDefinition("1-1", "First Steps", "hint text", gold:5.5, silver:8.0, d.Rows());
```

You can also hand-author the raw ASCII (`string[] Rows`) directly. Tile legend:

```
#  solid      ^  spike (kills)     -  one-way platform     P  spawn
x  hazard     M  moving (horiz)    V  moving (vert)        G  goal
o  coin       b  bounce pad        c  crumbling platform   s  saw (horiz)
>  conveyor right    <  conveyor left                      S  saw (vert)
g  walker (goomba — stompable)     f  flyer (stompable)    y  spiky (NOT stompable)
d  dash orb (refills your air-dash mid-flight)
p q r  warp portals (each letter is a linked pair)         j  jump ring (GD: tap jump inside it)
k  boost pad (fling right)         K  boost pad (fling up)  n  note/bounce block
l  falling platform                w  glide feather (low-gravity float)
```

See **`MARIO_LEVEL_IDEAS.md`** for the design playbook these levels are built from. `LevelComposer`
chunks for the new toys: `Warp()`, `RingGap()`, `BoostGap()`, `NoteBounce()`, `FallBridge()`,
`FeatherFloat()`.

`LevelComposer` also has `WallClimb()` (a required wall-jump chimney) and `DashGap()` (a pit you
cross by grabbing a dash orb mid-air). The floor can rise/fall via `WallClimb()`/`Drop()`.

Long levels are built with **`LevelComposer`** (see `LevelComposer.cs`) — chunk methods like
`Flat()`, `Pit()`, `Walker()`, `SpringUp()`, `Crumbles()`, `SawLane()`, `End()` chained
left-to-right. That's the easy way to author big 30s+ levels.

### Enemies
- **Stomp** an enemy from above (while falling) to kill it and bounce — chaining stomps refreshes
  your air-dash. Touch one from the side/below and you die.
- **Spiky** enemies can't be stomped — jump over or dash past them.

Add a level by appending another `LevelN()` to `LevelLibrary.Build()`. The level-select screen,
leaderboard, and world-total update automatically.

---

## 🎨 Reskinning

- **Colors:** everything reads from `Palette.cs` — change those and the whole game retints.
- **Shapes:** `TextureFactory.cs` generates the player/ground/spike/goal sprites. Swap in real
  sprites by assigning to the `SpriteRenderer`s the builders create (search `AddComponent<SpriteRenderer>`).
- **Sound:** `Sfx.cs` synthesizes blips. Replace `AudioClip`s with imported clips when you have them.
- **Tuning:** all movement numbers are public fields on `PlayerController` (also editable live in the
  Inspector once the player exists in Play mode).

---

## 🔧 Notes / next steps

- Built against the **new Input System** (project is set to "Input System Package" only).
- Uses **legacy UI Text** (built-in font) so nothing needs importing. Upgrade to TextMeshPro later
  for crisper text if you want.
- Ideas to extend: ghost replays, online leaderboard, more worlds, checkpoints for longer levels,
  wall-jump, particle juice, and a proper art pass.

Built with Claude Code. Have fun, boss. 🏁
