# VELOCITY — Improvement Backlog (the HUGE list)

A living brainstorm. Grouped by area, roughly prioritized within each group. Nothing here is
committed — it's the menu we pick from. ★ = high bang-for-buck, ⚠ = needs live playtesting/tuning.

---

## 1. Movement & game feel
- ★ **Corner correction** — nudge the player around ceiling/edge clips so jumps don't "eat" you.
- ★ **Wall slide + wall jump** ⚠ — huge for platformer depth and speedrun routing.
- **Ledge assist / edge grab** — small forgiveness when you just barely miss a ledge.
- **Slope support** — angled ground, slope-launch momentum (currently everything is flat).
- **Variable run states** — walk vs sprint, or a momentum/acceleration meter.
- **8-direction / diagonal dash** ⚠ — up-dashes open new routes.
- **Dash refresh tweaks** — refill on enemy stomp; air-dash count as an upgrade.
- **Crouch / slide** — slide under low gaps, slide-jump for distance.
- **Double jump as an unlockable** (per-world ability gating).
- **Coyote/buffer tuning sliders exposed in a debug panel** for fast iteration.
- **Wavedash / dash-jump tech** — reward skilled inputs with extra speed.

## 2. Enemies & hazards  *(starting now: Walker, Flyer, Spiky + stomp)*
- ★ **More enemy types**: Koopa/shell (stomp → kick shell → it slides and clears others),
  charger (rushes when it sees you), jumper, turret/spitter (fires projectiles), boss per world.
- **Projectiles** — both enemy bullets and a player attack (throw/shoot) option.
- **Enemy variety in behavior** — patrol, chase, ambush, ranged.
- **Bouncy/trampoline enemies**, falling spike blocks (thwomp), fireball bars, rotating spike logs.
- **Hazard polish** — telegraphed attacks, warning flashes, wind-up animations.
- **Destructible blocks** (bump from below like Mario `?` blocks; hide coins/springs).
- **Combo stomping** — chain stomps for rising bounce + score; speedrun routing tech.

## 3. Levels & content
- ★ **30s+ levels** *(doing now)* and a real difficulty curve across worlds.
- **More worlds** with distinct themes & a signature mechanic each.
- **Checkpoints** ⚠ (optional, off in "pure" speedrun mode) for casual play of long levels.
- **Secret areas / alternate routes** — speedrun skips, hidden rooms.
- **Bonus rooms** (collect-all-coins challenge, time-attack vault).
- **Boss levels** at the end of each world.
- **Procedural/endless mode** — daily seed challenge with a shared leaderboard.
- **In-game level editor** — build/share levels (extends the existing LevelAsset system).
- **Tutorialization** — teach each mechanic with a safe intro beat before it's lethal.

## 4. Speedrun features  *(the game's identity)*
- ★ **Live PB/ghost delta on the HUD** — continuous +/- vs your best, not just at the end.
- **Sectioned splits / checkpoints with per-segment gold times** (Celeste-style).
- **Input display overlay** (show the buttons being pressed — great for sharing).
- **Replay system** — save/playback full runs (camera + inputs), export to video.
- **Multiple ghosts at once** (your PB + WR + friend).
- **Run validation / anti-cheat hooks** for an online board.
- **"Reset to last checkpoint" vs "reset run"** bindings.
- **RTA vs in-game-time** distinction; load-removed timing.
- **Practice mode** — save/restore state, slow-mo, hitbox view.

## 5. Progression & meta
- **Unlockables** — skins, trails, palettes earned by medals/coins.
- **Star/medal economy** — gate later worlds behind medal counts.
- **Achievements** (stomp X enemies, no-death clear, sub-X run).
- **Daily/weekly challenges** with rotating modifiers.
- **Stats page** — total playtime, deaths, jumps, distance, stomps.
- **New Game+ / mirror mode / modifiers** (low gravity, no-dash, hardcore).

## 6. UI / UX
- ★ **TextMeshPro upgrade** — crisper text, better fonts (currently legacy UI font).
- **Controller glyphs** — show Xbox/PS/Switch button icons in prompts & rebinding.
- **Pause-menu mini-map / level preview**.
- **Better results screen** — medal animation, coin tally count-up, share button.
- **Tooltips & first-run onboarding**.
- **Resolution dropdown + windowed-borderless toggle** (we have fullscreen/vsync/fps).
- **Remappable gamepad bindings** (currently keyboard-only rebinding).
- **Per-action multiple bindings UI**.
- **Localization** (string table, RTL support).

## 7. Audio
- ★ **Real music** — per-world tracks, dynamic layers that build with intensity.
- **Better SFX** — footsteps, enemy stomps, coin pitch-ladder, ambient beds.
- **Audio mixer** with proper buses, ducking, and a low-pass filter on pause.
- **Music sync** — beat-matched hazards / visual pulses.

## 8. Visuals & art  *(blocked on choosing the vibe)*
- ★ **Art direction pass** — pick a style; replace placeholder shapes with real sprites.
- **Player animation** — run/jump/fall/dash/idle frames (we only squash-stretch now).
- **Tilemap rendering** — nicer, edit-friendly geometry (vs code-built boxes).
- **2D lighting** (URP 2D lights are installed but unused) — glow, normal maps, shadows.
- **Parallax/foreground layers**, weather, particles, post-processing (bloom, vignette, CA).
- **Camera juice** — punch-zoom on stomp/finish, subtle sway, speed-based FOV.
- **Animated UI** (transitions, easing, hover states).

## 9. Accessibility  *(some done: reduce-motion, colorblind themes, shake toggle)*
- **Hold-to-toggle options** (e.g., toggle dash), input assists.
- **Assist mode** — adjustable game speed, invincibility, extra coyote/buffer.
- **Subtitles/visual cues** for audio events.
- **Dyslexia-friendly font option**, text size scaling.
- **Full controller + key remap incl. menus**.

## 10. Online & social
- **Online leaderboards** (per level + full-run; needs a backend).
- **Ghost sharing / friend ghosts**.
- **Spectate / async races**.
- **Discord Rich Presence**, Steam integration (achievements, cloud saves, workshop).
- **Daily challenge with global ranking**.

## 11. Tech & architecture
- ★ **Assembly definitions** — faster compiles, cleaner deps.
- **Unit/play-mode tests** (the test framework is installed) — controller, timer, leaderboard.
- **Save system hardening** — versioned JSON save file instead of scattered PlayerPrefs; cloud sync.
- **Object pooling** for FX/enemies/coins (less GC).
- **Deterministic fixed-step run recording** for frame-perfect ghosts/replays.
- **Config/ScriptableObject tuning** for movement & enemies (designer-editable).
- **Analytics** (opt-in) — where players die, level heatmaps.
- **Build pipeline / CI**, platform builds (Win/Mac/Linux/Web/mobile).

## 12. Game modes
- **Time attack**, **endless**, **boss rush**, **co-op / race** (split-screen or online),
  **hardcore (one life)**, **randomizer**, **custom-modifier playlists**.

## 13. Monetization / platform (if it ships)
- Premium one-time, cosmetic DLC, level packs, soundtrack.
- Demo (World 1 free), wishlists, press kit.

---

### Suggested next 3 (my pick)
1. **Live PB/ghost delta HUD** — cheap, pure speedrun value.
2. **Wall jump + corner correction** ⚠ — the biggest feel upgrade (✅ wall jump now in; corner-correct next).
3. **Art direction decision** — unblocks the whole visual track.

---

# Round 2 — way more ideas (added after wall-jump)

## A. "Figure-it-out" traversal mechanics (puzzle-not-timing)
- **Dash orbs** (Celeste-style) — float in air, refill your dash on touch; chain them to cross huge gaps.
- **Wall-cling** upgrade — hold a button to grip a wall without sliding (think, then act).
- **Momentum gates** — a barrier that only opens if you arrive fast enough (forces a run-up route).
- **Keys & locked doors** — fetch a key tucked in a hazard, backtrack.
- **Switches / levers** — toggle platforms, retract spikes, flip conveyor directions; timing/route puzzles.
- **Crumbling / moving / slippery walls** for wall-jumps — wall-jump before it breaks / off a sliding wall.
- **Gravity-flip & low-gravity zones**, **wind zones** that push you mid-air.
- **Branching routes** — a hard shortcut vs a safe long way (speedrun routing decisions).
- **Auto-scroll rooms** — the screen forces you forward (classic Mario pressure).
- **Ice / mud surfaces** — slide vs slog, change how you approach a jump.

## B. Combat depth (more Mario)
- **Koopa shells** — stomp → kick → the shell slides and clears a whole row / hits a far switch.
- **Ground pound** — stomp straight down to smash breakable blocks & shockwave enemies.
- **Stomp-chains required** — a row of flyers over a pit you *must* bounce across (no floor).
- **Mini-bosses** — pattern enemies that take 3 stomps; a boss flag per world.
- **Turrets / spitters** — telegraphed projectiles to dodge or deflect.
- **`?` blocks** — bump from below for coins, a spring, or a dash refill.

## C. Make the levels themselves smarter
- **Per-level theme + signature gimmick** (one new toy introduced, taught safe, then weaponized).
- **Secret rooms / skips** — reward exploration and route knowledge.
- **Hidden 4th "challenge" coin** behind a genuinely hard optional route (kept your 3 mandatory).
- **Checkpoints** (toggle off for pure speedrun) so 45s killer levels aren't rage-restarts.
- **Set-piece finales** — a saw chase, a collapsing-floor sprint, a wall-jump tower climb.

## D. Speedrun depth (the identity)
- **Live +/- vs your ghost** on the HUD (continuous, not just at the end).
- **Tech to discover**: dash-jump, wall-jump→dash, corner-boost, pogo-stomp height.
- **Style/combo meter** — chain stomps/dashes/wall-jumps without touching ground = bonus + flashy.
- **Per-level rank (S/A/B/C)** from time + coins + deathless.
- **Any% vs 100%** (all coins) categories with separate boards.
- **Bundled WR/dev ghost** to race, plus **input display** + **replay export**.
- **Practice mode** — rewind, slow-mo, hitbox view, save/restore state.

## E. Juice / game feel (cheap, high impact)
- **Hitstop** on stomp & wall-jump, **dash afterimage/trail**, **speed lines** at high velocity.
- **Punch-zoom** on level complete, subtle camera lead on dash.
- **Death**: chromatic-aberration pop + slow-mo crumble of the player.
- **Dynamic music** that layers up with your speed / danger.
- **Coin pitch-ladder** (each coin a higher note), distinct stomp/wall-jump SFX.

## F. Progression & replayability
- **Death counter** per level + a **deathless** badge; **fastest-coin** and **fastest-clear** split records.
- **Cosmetics** (skins/trails/palettes) bought with coins.
- **Daily challenge** — fixed seed, global board, rotating modifier (no-dash, low-gravity, mirror).
- **Modifiers / mutators** for replay value; **mirror mode**; **hardcore one-life**.

## G. Difficulty & accessibility knobs
- **Wall-jump assist** (auto-grip), adjustable coyote/buffer, slow-mo assist mode.
- **Adjustable game speed** (50–100%) for learning, off for ranked.
- Difficulty tiers per world (gate later worlds behind medal counts).

## H. Tech that unlocks the above
- **Object pooling** (FX/enemies/coins), **deterministic fixed-step recording** (frame-perfect ghosts).
- **ScriptableObject tuning** for movement/enemies so design is editable without code.
- **Assembly definitions**, **play-mode tests**, **versioned save file + cloud**.
