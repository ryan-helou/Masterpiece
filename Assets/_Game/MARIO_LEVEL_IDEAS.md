# The Mario Level Playbook

A study of how the Super Mario series (SMB1, Lost Levels, SMB2/USA, SMB3, World, Yoshi's Island,
NSMB 1/Wii/2/U, 3D Land/World's 2D-style stages) makes levels *interesting* — distilled into
reusable devices, plus the design philosophy underneath, plus what we're stealing from Geometry
Dash. This is the source list we rebuild our 8 levels from.

---

## The core principle: kishōtenketsu (introduce → develop → twist → conclude)
Nintendo's famous 4-beat structure — the real reason Mario levels feel "designed," not random:
1. **Introduce** a mechanic in a *safe* spot (no death if you fumble it).
2. **Develop** it — same idea, now with stakes (a pit, an enemy).
3. **Twist** it — combine with another mechanic, or invert the rules.
4. **Conclude** — a satisfying final expression / set-piece before the flag.

Every great level is a *single idea explored*, not a grab-bag. **That's the thing our levels were
missing.** Each of our 8 levels should now be built around ONE signature gimmick, taught → developed
→ twisted → concluded.

Other guiding rules Nintendo follows:
- **One new thing per level.** Teach it wordlessly through level geometry (the "first goomba" is
  placed so you *must* learn to jump).
- **Telegraph danger.** The player should always feel a death was their fault.
- **Reward curiosity** (hidden blocks, secret exits, off-path coins) but never *require* it.
- **Rising tension, then a breather, then a peak.** Pace it like music.
- **Multiple solutions** (run vs. stomp vs. skip) → speedrun routing.

---

## A. Terrain & traversal devices
- **Warp pipes** — enter to teleport (sideways or down), bonus rooms, secret exits, shortcuts.
- **Pits** of escalating width; **pits with a single mid platform**; **pits under a low ceiling**.
- **Staircases** up to the flag; **descending stairs**; **zig-zag towers**.
- **Donut/falling platforms** — stand and they shake, then drop.
- **Note/music blocks** — bounce you sky-high; bounce-block ladders.
- **Moving platforms** — horizontal, vertical, on tracks, **circular**, **see-saw/balance lifts**,
  **platforms on a falling chain**, **conveyor belts**.
- **Rotating/somersault platforms**, **spinning log over lava**.
- **One-way / cloud platforms**, **semi-solid layers** you jump up through.
- **Slopes** — slide down to build speed, slope-jump for distance, butt-slide through enemies.
- **Ice** (slippery) and **muncher/quicksand** (slow/sink) floors.
- **Vine / beanstalk climbs** to sky bonus areas; **ropes & fences** you climb and flip around.
- **Auto-scroll** sections — the screen forces forward pace; **vertical auto-scroll** climbs.
- **Forced-water / swimming** (floaty gravity, currents).
- **Wind** gusts that push you; **conveyor wind tunnels**.
- **Dark caves** lit by a moving light / on-off.

## B. Hazards
- **Spikes / munchers / lava / spike pits**; **spike ceilings** you duck under.
- **Thwomps** (slam down when you pass), **spiked rollers/pillars**.
- **Fire bars** (rotating flame), **podoboos/lava bubbles** that leap from below.
- **Bullet Bill / Banzai Bill cannons** — homing-ish horizontal shots.
- **Burner/flame jets** on timers; **crushing walls / pistons**.
- **Falling spike blocks**, **rolling boulders**, **buzzsaws on rails**.
- **Collapsing floors / timed-disappear blocks** (on/off via beat or P-switch).

## C. Switches, blocks & state changes
- **? blocks** — coins, power-ups, springs, vines; **hidden invisible blocks**.
- **Brick blocks** you break from below; **multi-coin blocks**.
- **P-switch** — turns bricks↔coins, reveals timed paths, opens routes (a *timed sprint*).
- **! / ON-OFF switch blocks** — toggle which platforms are solid (Geometry-Dash-y rhythm).
- **Note-block / trampoline / springboard** you carry and place.
- **Key & locked door**; **POW block** (clears screen).
- **Color-coded gates** opened by hitting the matching block.

## D. Enemies as level design (not just filler)
- **Goomba lines** you chain-stomp across a pit (the floor *is* the enemies).
- **Koopa shells** — stomp → kick → it clears a row / ricochets / hits a far switch.
- **Paratroopas** as **bounce pads** (stomp to gain height across a gap).
- **Spinies / spikes-tops** — unstompable, force you around.
- **Piranha plants** in pipes (timed emergence), **fire piranhas**.
- **Hammer bros / boomerang bros** — projectile arcs to weave through.
- **Bob-ombs** — walk, then explode (break walls / chain reactions).
- **Chain chomps** lunging on a leash; **Lakitu** dropping spinies from above.
- **Bosses / mini-bosses** — pattern, jump-on-head ×3, or use the environment to beat them.

## E. Verticality & "rooms"
- **Sub-areas via pipes/doors** — a level is several connected rooms, not one strip.
- **Climb shafts** (wall-jump chimneys, vine, note-block ladder).
- **Drop shafts** — fall through a hazard gauntlet, threading platforms.
- **Loops/backtracks** — fetch a key, flip a switch, return.
- **Hidden upper route** (faster, riskier) vs. safe lower route.

## F. Pacing & set-pieces (the "twist" and "conclusion")
- **The chase** — a wall of saws / rising lava / Boo train pushes you forward.
- **The collapse** — floor falls away behind you as you sprint.
- **The gauntlet** — every mechanic of the level, back-to-back, no breather.
- **The puzzle gate** — you must combine two mechanics to pass (figure-it-out, not timing).
- **The victory lap** — a fast, flashy, low-stakes run of springs/boosts to the flag.

## G. Stealing from Geometry Dash (rhythm-platformer juice)
- **Jump rings / pads** — pass through a ring and tap jump for a *boosted air-jump* (re-jump in
  mid-air at fixed points → vertical puzzles). ← the user specifically wants this.
- **Boost / dash pads** — fling you fast in a direction (speed sections).
- **Gravity-flip portals** — invert gravity; walk the ceiling.
- **Speed portals** — change your run speed for a section.
- **Mode portals** — temporary ability changes.
- **Rhythm** — hazards/jumps timed to the music's beat.
- **Pulse/flash on the beat**, **camera shake on drops**.

---

## What we're implementing now (round 1 of "make it crazy")
Tile-friendly, robust, high-creativity:
- **Warp portals / pipes** (`p`,`q`,`r`) — paired teleporters; warp transitions & required gates.
- **Jump rings** (`j`) — GD-style: tap jump inside the ring for a boosted air-jump.
- **Boost pads** (`k`) — GD dash panels that fling you forward fast.
- **Note/bounce blocks** (`n`) — land on them and rocket upward (bounce ladders).
- **Falling platforms** (`l`) — stand and they drop; cross fast.
- **Glide feathers** (`w`) — grab for a few seconds of floaty low-gravity (long hang-time jumps).

Plus the existing kit (spikes, one-ways, moving platforms, conveyors, saws, crumbles, springs,
wall-jump chimneys, dash orbs, stompable/spiky/flyer enemies, coins).

### The 8 levels, re-themed (one signature idea each, taught→developed→twisted→concluded)
1. **1-1 Warp Yard** — warp pipes/portals + note blocks. Intro to teleporting.
2. **1-2 Up We Go** — jump-ring ascension + wall-climbs (vertical).
3. **1-3 Fast Lane** — boost-pad speed run; dash panels over death.
4. **1-4 The Maze** — portal-connected rooms + enemy gauntlet; figure out the route.
5. **2-1 Featherfall** — glide feathers over huge floaty gaps; flyers.
6. **2-2 The Factory** — conveyors + boost pads + saws (industrial rhythm).
7. **2-3 Collapse** — falling platforms + crumbling bridges; the floor chases you.
8. **2-4 Grand Finale** — every mechanic, portal-stitched set-pieces, the gauntlet.

### Next swings (round 2)
Gravity-flip portals (walk the ceiling), on/off rhythm blocks, P-switch timed sprints, koopa
shells, checkpoints, boss set-pieces, beat-synced hazards.
